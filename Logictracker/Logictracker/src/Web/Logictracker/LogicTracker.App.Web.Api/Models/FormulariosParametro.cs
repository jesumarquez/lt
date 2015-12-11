using System;

namespace LogicTracker.App.Web.Api.Models
{
    public class FormulariosParametros
    {
        public string id { get; set; }
        public string pordefault { get; set; }
        public string largo { get; set; }
        public string nombre { get; set; }
        public string obligatorio { get; set; }
        public string orden { get; set; }
        public string precision { get; set; }
        public string repeticion { get; set; }
        public string tipodato { get; set; }
        public string tipodocumento { get; set; }
        public FormulariosSpinnerList[] spinnerlist { get; set; }
    }
}