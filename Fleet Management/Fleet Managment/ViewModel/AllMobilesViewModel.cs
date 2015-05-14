using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Urbetrack.FleetManagment.DataAccess;
using Urbetrack.FleetManagment.Properties;

namespace Urbetrack.FleetManagment.ViewModel
{
    public class AllMobilesViewModel : WorkspaceViewModel
    {
        readonly MobilesRepository _mobilesRepository;
        readonly MainWindowViewModel _mainWindowViewModel;

        public AllMobilesViewModel(MobilesRepository mobilesRepository, MainWindowViewModel mainWindowViewModel)
        {
            if (mobilesRepository == null)
                throw new ArgumentNullException("mobilesRepository");
            if (mainWindowViewModel == null)
                throw new ArgumentNullException("mainWindowViewModel");

            base.DisplayName = Strings.AllMobilesViewModel_DisplayName;

            _mobilesRepository = mobilesRepository;
            _mainWindowViewModel = mainWindowViewModel;
            CreateAllMobiles();
        }

        private void CreateAllMobiles()
        {
            List<MobileViewModel> all =
               (from mobile in _mobilesRepository.GetMobiles()
                select new MobileViewModel(mobile, _mainWindowViewModel)).ToList();

            AllMobiles = new ObservableCollection<MobileViewModel>(all);
        }


        #region Public Interface

        /// <summary>
        /// Returns a collection of all the CustomerViewModel objects.
        /// </summary>
        public ObservableCollection<MobileViewModel> AllMobiles { get; private set; }

        #endregion
    }
}
