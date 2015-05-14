using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Urbetrack.FleetManagment.Model;
using Urbetrack.FleetManagment.MQ;

namespace Urbetrack.FleetManagment.ViewModel
{
    public class MobileViewModel : WorkspaceViewModel
    {
        public const string SubmitTextMessage = "SubmitTextMessage";

        readonly Mobile _mobile;
        private bool _isSelected;
        readonly MainWindowViewModel _mainWindowViewModel;

        public MobileViewModel(Mobile mobile, MainWindowViewModel mainWindowViewModel)
        {
            if (mobile == null)
                throw new ArgumentNullException("mobile");
            if (mainWindowViewModel == null)
                throw new ArgumentNullException("mainWindowViewModel");

            _mobile = mobile;
            _mainWindowViewModel = mainWindowViewModel;
        }

        #region Mobile properties
        public int VehicleId { get { return _mobile.VehicleId; } }
        public int DeviceId { get { return _mobile.DeviceId; } }
        public String Device { get { return _mobile.Device; } }
        public String Vehicle { get { return _mobile.Vehicle; } }
        public String IMEI { get { return _mobile.IMEI; } }
        public String District { get { return _mobile.District; } }
        public String Base { get { return _mobile.Base; } }
        public String Queue { get { return _mobile.Queue; } }
        #endregion

        /// <summary>
        /// Gets/sets whether this customer is selected in the UI.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected)
                    return;

                _isSelected = value;

                base.OnPropertyChanged("IsSelected");
            }
        }

        public override string DisplayName
        {
            get
            {
                return String.Format("Vehicle: {0} Device: {1}", _mobile.Vehicle, _mobile.Device);
            }
        }

        public string DisplayDistrict
        {
            get
            {
                return String.Format("Distrito: {0} ", _mobile.District);
            }
        }

        public void ShowMobileView()
        {
            _mainWindowViewModel.ShowMobile(this);
        }

        public void SendCommands(String[] commands)
        {
            foreach (var cmd in commands)
            {
                var limpio = cmd.Replace("\r", "");
                SendCommands(SubmitTextMessage, limpio);
            }
        }
       
        private bool SendCommands(string _command, string cmd)
        {
            Dictionary<string, string> _parameters = new Dictionary<string, string>();
            _parameters.Add("msgText", cmd);
            var parameters = string.Join("", _parameters.Select(p => string.Format("&{0}={1}", p.Key, p.Value)).ToArray());
            var commandText = string.Format("/Device.{0}?devId={1}{2}", _command, DeviceId, parameters);
            
            try
            {
                var cola = new MQueue(_mobile.Queue);
                cola.Send(commandText);
                return true;
            }
            catch { return false; }
       }
    }
}
