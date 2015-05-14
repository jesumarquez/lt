using System.Web.UI;
using Logictracker.Types.BusinessObjects.Documentos;

namespace Logictracker.Web.Documentos.Interfaces
{
    public interface IPresentStrategy
    {
        void CrearForm();
        void SetValores(Documento documento);
        Control GetControlFromView(string id);
        void SetDefaults();
    }
}