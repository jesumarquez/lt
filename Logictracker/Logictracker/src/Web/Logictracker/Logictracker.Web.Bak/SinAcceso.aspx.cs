using System.Linq;
using Logictracker.Culture;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker
{
    public partial class SinAcceso : SessionSecuredPage
    {
        protected override InfoLabel LblInfo { get { return null; } }
        protected override string PageTitle { get { return string.Format("{0} - {1}", ApplicationTitle, CultureManager.GetError("NO_MODULE_ACCESS")); } }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            var module = Session["module_name"];
            Session.Remove("module_name");
            if(module != null)
            {
                var funcion = DAOFactory.FuncionDAO.FindAll().FirstOrDefault(f => f.Descripcion == module.ToString());
                if(funcion != null)
                {
                    lblModulo.Text = CultureManager.GetMenu(funcion.Descripcion);
                }
            }
        }
    }
}