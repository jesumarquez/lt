using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Urbetrack.FleetManagment.ViewModel;
namespace Urbetrack.FleetManagment.View
{
    /// <summary>
    /// Interaction logic for MobileView.xaml
    /// </summary>
    public partial class MobileView : UserControl
    {
        public MobileView()
        {
            InitializeComponent();
        }

        private void button1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MobileViewModel mvm = (MobileViewModel)((Button)sender).DataContext;
            if (String.IsNullOrEmpty(deviceCommands.Text))
            {
                MessageBox.Show("No hay comandos a enviar.");
                return;
            }
            String[] commands = deviceCommands.Text.Split('\n');
            mvm.SendCommands(commands);
            MessageBox.Show("Se enviaron los comandos indicados");
            deviceCommands.Text = "";
        }
    }
}
