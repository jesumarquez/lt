using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using TraxEmu.Annotations;
using TraxEmu.Properties;

namespace TraxEmu
{

    public class EmuVM : INotifyPropertyChanged
    {
        private int _port;
        private string _ip;
        private string _cmdToSend;
        private UdpClient _udp;
        private int _deviceId;

        public int Port
        {
            get { return _port; }
            set
            {
                if (value == _port) return;
                _port = value;
                OnPropertyChanged();
            }
        }

        public string CmdToSend
        {
            get { return _cmdToSend; }
            set
            {
                if (value == _cmdToSend) return;
                _cmdToSend = value;
                OnPropertyChanged();
            }
        }

        public string IP
        {
            get { return _ip; }
            set
            {
                if (value == _ip) return;
                _ip = value;
                OnPropertyChanged();
            }
        }

        public int DeviceId
        {
            get { return _deviceId; }
            set
            {
                if (value == _deviceId) return;
                _deviceId = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void Enviar()
        {
            string toSend = Utils.Factory(DeviceId, CmdToSend, null);
            var buffer = Encoding.ASCII.GetBytes(toSend);
            _udp.Send(buffer, buffer.Length, IP, Port);
            
        }

        public EmuVM()
        {
            Port = Convert.ToInt32(Settings.Default.Port);
            IP = Settings.Default.IP;
            _udp = new UdpClient();
        }
    }
}
