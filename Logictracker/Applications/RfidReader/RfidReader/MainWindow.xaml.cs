using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Application = System.Windows.Application;
using TextBox = System.Windows.Controls.TextBox;

namespace RfidReader
{
	public partial class MainWindow
	{
		private static SerialPort port;
		private static TextBox textBox_;
		private static TextBox textBox2_;
		private static TextBox textBox3_;
		private static readonly Object uilocker = new Object();

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			textBox_ = textBox1;
			textBox2_ = textBox2;
			textBox3_ = textBox3;
			comboBox1.ItemsSource = SerialPort.GetPortNames();
			if (comboBox1.Items.Count > 0)
			{
				comboBox1.SelectedIndex = 0;
			}
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if (port != null && port.IsOpen)
			{
				port.Close();
			}
		}

		private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (comboBox1.SelectedValue == null) return;

			if (port != null && port.IsOpen)
			{
				port.Close();
			}

			port = new SerialPort
			{
				PortName = (String)comboBox1.SelectedValue,
				BaudRate = Convert.ToInt32(comboBox2.SelectedIndex == 0 ? 115200 : 9600),
				DataBits = 8,
				Parity = Parity.None,
				StopBits = StopBits.One,
				Handshake = Handshake.None,
				ReadTimeout = 1000,
				WriteTimeout = 1000,
			};
			port.DataReceived += ReadPort;
			port.Open();
		}

		private static void ReadPort(Object sender, SerialDataReceivedEventArgs e)
		{
			try
			{
				if (!port.IsOpen)
				{
					return;
				}

				var taux = port.ReadExisting();
				input.Append(taux);

				Application.Current.Dispatcher.Invoke((MethodInvoker) (() =>
					{
						lock (uilocker)
						{
							var text = input.ToString();
							var detected = false;
							while (text.Contains("<"))
							{
								var text2 = text.Substring(0, text.IndexOf("<") + 1);
								text = text.Substring(text2.Length);
								input.Remove(0, text2.Length);

								if (text2.Contains(">"))
								{
									text2 = text2.Substring(text2.LastIndexOf(">")); //remuevo lo que halla llegado incompleto y quedo asi

									if (text2.Contains(">STX09, "))
									{
										var rfid = text2.Substring(text2.IndexOf(">STX09, ") + 8); //, 8);
										rfid = rfid.Substring(0, rfid.IndexOf("<"));
										if (rfid.Length == 8)
										{
											detected = true;
											var cabezadetacho = RfidHexaFromBase36(rfid);
											textBox_.AppendText(String.Format("Rfid: {0} ({1}){2}", cabezadetacho, rfid, Environment.NewLine));
											textBox3_.Text = cabezadetacho;
										}
									}
								}
							}
							textBox2_.AppendText(taux + (detected ? " ._( Rfid detected )_." : "") + Environment.NewLine);
							if (detected && !textBox_.IsFocused) textBox_.ScrollToLine(textBox_.LineCount - 1);
							if (!textBox2_.IsFocused) textBox2_.ScrollToLine(textBox2_.LineCount - 1);
						}
					}));
			}
			catch (TimeoutException) { }
		}
		private static readonly StringBuilder input = new StringBuilder();

		private static string RfidHexaFromBase36(String entrada)
		{
			if ((entrada == null) || (entrada.Trim() == String.Empty))
				return "0000000000";

			const string clist = "0123456789abcdefghijklmnopqrstuvwxyz";
			long result = 0;
			var pos = 0;
			for (var i = entrada.Length - 1; i > -1; i--)
			{
				var c = Char.ToLower(entrada[i]);
				result += clist.IndexOf(c) * (long)Math.Pow(36, pos);
				pos++;
			}
			return result.ToString("X10");
		}

		private void checkBox1_Click(object sender, RoutedEventArgs e)
		{
			textBox1.Visibility = checkBox1.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
			textBox2.Visibility = checkBox1.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;

		}
	}
}
