using System;

namespace LogicTracker.App.Web.Api.Models
{
    public class FormularioRow
    {
        public string id { get; set; }
        public string idFormulario { get; set; }
        public bool changedData { get; set; }
        public CampoFormulario[] campoFormulario { get; set; }
    }
}