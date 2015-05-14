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
    /// Interaction logic for AllMobilesView.xaml
    /// </summary>
    public partial class AllMobilesView : UserControl
    {
        public AllMobilesView()
        {
            InitializeComponent();
        }

        public void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)

        {
            MobileViewModel mvm = (MobileViewModel)((ListViewItem) sender).DataContext;
            mvm.ShowMobileView();
        }

        private void ListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            movilList.SelectedItems.Cast<MobileViewModel>().ToList().ForEach(mvm => mvm.ShowMobileView());


        }
    }

   
}
