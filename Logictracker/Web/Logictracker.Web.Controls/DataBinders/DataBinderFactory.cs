#region Usings

using Logictracker.Web.Controls.BindableListControl;

#endregion

namespace Logictracker.Web.Controls.DataBinders
{
    public static class DataBinderFactory
    {
        public static IDataBinder GetDataBinder(BindEntities entity)
        {
            switch(entity)
            {
                case BindEntities.Empresa: return new EmpresaDataBinder();
                case BindEntities.Linea: return new LineaDataBinder();
                default: return null;
            }
        }
    }
}
