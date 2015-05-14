#region Usings

using Logictracker.Web.Controls.BindableListControl;

#endregion

namespace Logictracker.Web.Controls.DataBinders
{
    public class LineaDataBinder : DataBinderBase
    {
        #region Implementation of IDataBinder

        public override void DataBind(IBindableListControl control)
        {
            var empresas = control.GetParentSelected(BindEntities.Empresa);
            var lineas = DaoFactory.LineaDAO.GetList(empresas);

            foreach (var linea in lineas) control.AddItem(linea.Descripcion, linea.Id);
        }

        #endregion
    }
}
