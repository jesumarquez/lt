#region Usings

using System;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;

#endregion

namespace Logictracker.Types.ValueObject
{
    [Serializable]
    public class ReferenciaGeograficaVO
    {
        public ReferenciaGeograficaVO(ReferenciaGeografica dom)
        {
            Tipo = dom.TipoReferenciaGeografica.Descripcion;
            if (dom.Direccion != null)
            {
                Calle = dom.Direccion.IdCalle.ToString();
                Altura = dom.Direccion.Altura;
                Partido = dom.Direccion.Partido;
                Provincia = dom.Direccion.Provincia;
                Pais = dom.Direccion.Pais;
                Inicio = dom.Vigencia.Inicio.ToString();
                Fin = dom.Vigencia.Fin.ToString();
                Empresa = dom.Empresa != null ? dom.Empresa.RazonSocial : string.Empty;
                Linea = dom.Linea != null ? dom.Linea.Descripcion : string.Empty;
            }
        }

        public string Tipo { get; set; }

        public string Calle { get; set; }

        public int Altura { get; set; }

        public string Partido { get; set; }

        public string Provincia { get; set; }

        public string Pais { get; set; }

        public string Inicio { get; set; }

        public string Fin { get; set; }

        public string Linea { get; set; }

        public string Empresa { get; set; }
    }
}