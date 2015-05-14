#region Usings

using Logictracker.Culture;
using Logictracker.DAL.Factories;

#endregion

namespace Logictracker.Web.Controls.DataBinders
{
    public abstract class DataBinderBase: IDataBinder
    {
        private DAOFactory _daof;

        protected DAOFactory DaoFactory { get { return _daof ?? (_daof = new DAOFactory()); } }

        public abstract void DataBind(IBindableListControl control);

        public virtual int ItemAllValue { get { return -1; } }

        public virtual int ItemNoneValue { get { return -2; } }

        public virtual string ItemAllName { get { return CultureManager.GetControl("DDL_ALL_ITEMS"); } }

        public virtual string ItemNoneName { get { return CultureManager.GetControl("DDL_NONE"); } }
    }
}
