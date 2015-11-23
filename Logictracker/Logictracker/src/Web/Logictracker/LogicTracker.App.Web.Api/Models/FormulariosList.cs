using System;

namespace LogicTracker.App.Web.Api.Models
{
    public class FormulariosList
    {
        public string id { get; set; }
        public string nombre { get; set; }
        public string primeraviso { get; set; }
        public string requerirpresentacion { get; set; }
        public string requerirvencimiento { get; set; }
        public string segundoaviso { get; set; }
        public string strategy { get; set; }
        public string template { get; set; }
        public FormulariosParametros[] parametros { get; set; }
    }
}