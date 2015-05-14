using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.Security;

namespace Logictracker.Web.Controls
{
    public static class AutoBindingManager
    {
        private static DAOFactory _daoFactory;
        public static DAOFactory DaoFactory
        {
            get { return _daoFactory ?? (_daoFactory = new DAOFactory()); }
        }
        public static void Bind(IAutoBindeable3 combo)
        {
            combo.Clear();

            if (!WebSecurity.Authenticated) return;

            var items = GetItems(combo);

            if (items == null)
            {
                return;
            }

            if (combo.AddAllItem)
            {
                combo.AddItem(new ComboBoxItem(ComboBox.AllItemsValue, ComboBox.AllItemsName));
            }

            if (combo.AddNoneItem)
            {
                combo.AddItem(new ComboBoxItem(ComboBox.NoneItemsValue, ComboBox.NoneItemsName));
            }
            
            foreach (var item in items)
            {
                combo.AddItem(item);
            }
        }

        private static IEnumerable<ComboBoxItem> GetItems(IAutoBindeable3 combo)
        {
            switch (combo.AutoBindingMode)
            {
                case AutoBindingMode.Empresa:
                    return GetItems(DaoFactory.EmpresaDAO.GetList(), "Id", "RazonSocial");
                case AutoBindingMode.Linea:
                    return GetItems(DaoFactory.LineaDAO.GetList(combo.ParentSelectedValues(AutoBindingMode.Empresa)), "Id", "Descripcion");
            }
            return null;
        }
        private static IEnumerable<ComboBoxItem> GetItems<T>(IEnumerable<T> elements, string valueField, string textField)
        {
            var type = typeof (T);
            elements = elements.OrderBy(el => type.GetProperty(textField).GetValue(el, null).ToString());
            foreach(var el in elements)
            {
                var value = (int)type.GetProperty(valueField).GetValue(el, null);
                var text = type.GetProperty(textField).GetValue(el, null).ToString();
                yield return new ComboBoxItem(value, text);
            }
        }
    }
}
