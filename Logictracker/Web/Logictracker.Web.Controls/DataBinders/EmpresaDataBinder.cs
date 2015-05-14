namespace Logictracker.Web.Controls.DataBinders
{
    public class EmpresaDataBinder : DataBinderBase
    {
        #region Implementation of IDataBinder

        public override void DataBind(IBindableListControl control)
        {
            var empresas = DaoFactory.EmpresaDAO.GetList();

            foreach (var empresa in empresas) control.AddItem(empresa.RazonSocial, empresa.Id);
        }

        #endregion
    }
}
