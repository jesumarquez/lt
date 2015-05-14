using System;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Empleados
{
    [Serializable]
    public class EmpleadoVo
    {
        public const int IndexLegajo = 0;
        public const int IndexNroTarjeta = 1;
        public const int IndexDescripcion = 2;
        public const int IndexCoche = 3;
        public const int IndexTransportista = 4;
        public const int IndexEmpresa = 5;
        public const int IndexLinea = 6;
        public const int IndexCategoriaAcceso = 7;

        public int Id { get; set; }

        [GridMapping(Index = IndexLegajo, ResourceName = "Labels", VariableName = "LEGAJO", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Legajo { get; set; }

        [GridMapping(Index = IndexNroTarjeta, ResourceName = "Entities", VariableName = "TARJETA", AllowGroup = false, IncludeInSearch = true)]
        public string NroTarjeta { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "NAME", AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexCoche, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = false, IncludeInSearch = true)]
        public string Coche
        {
            get
            {
                if (_coche == null)
                {
                    var coche = DAOFactory.CocheDAO.FindByChofer(Id);
                    _coche = coche != null ? coche.Interno : string.Empty;
                }
                return _coche;
            }
        }
        private string _coche;
                
        [GridMapping(Index = IndexTransportista, ResourceName = "Entities", VariableName = "PARENTI07")]
        public string Transportista { get; set; }

        [GridMapping(Index = IndexEmpresa, ResourceName = "Entities", VariableName = "PARENTI01")]
        public string Empresa { get; set; }

        [GridMapping(Index = IndexLinea, ResourceName = "Entities", VariableName = "PARENTI02")]
        public string Linea { get; set; }

        [GridMapping(Index = IndexCategoriaAcceso, ResourceName = "Entities", VariableName = "PARENTI15")]
        public string CategoriaAcceso { get; set; }

        public string Cuil { get; set; }
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public int Antiguedad { get; set; }
        public string Art { get; set; }
        public string DomicilioCalle { get; set; }
        public int DomicilioAltura { get; set; }
        public string DomicilioEsquina { get; set; }
        public string DomicilioEntreCalle { get; set; }
        public string DomicilioPartido { get; set; }
        public string DomicilioProvincia { get; set; }

        public string DomicilioPais { get; set; }
        public DateTime? Falta { get; set; }
        public string Licencia { get; set; }

        private DAOFactory DAOFactory { get; set; }
        

        public EmpleadoVo(Empleado empleado, DAOFactory daoFactory)
        {
            DAOFactory = daoFactory;
            Id = empleado.Id;
            Empresa = empleado.Empresa != null ? empleado.Empresa.RazonSocial : empleado.Linea != null ? empleado.Linea.Empresa.RazonSocial : string.Empty;
            Linea = empleado.Linea != null ? empleado.Linea.Descripcion : string.Empty;
            Legajo = empleado.Legajo;
            Descripcion = (empleado.Entidad != null) ? empleado.Entidad.Descripcion: string.Empty;
            
            Cuil = (empleado.Entidad != null)?empleado.Entidad.Cuil: string.Empty;
            TipoDocumento = (empleado.Entidad != null)? empleado.Entidad.TipoDocumento : string.Empty;
            NroDocumento = (empleado.Entidad != null)?empleado.Entidad.NroDocumento : string.Empty;
            Antiguedad = empleado.Antiguedad;
            Art = empleado.Art;
            DomicilioCalle = (empleado.Entidad != null) ? (empleado.Entidad.Direccion != null)? empleado.Entidad.Direccion.Descripcion :  string.Empty : string.Empty;
            DomicilioAltura = (empleado.Entidad != null)?(empleado.Entidad.Direccion != null)?empleado.Entidad.Direccion.Altura : 0 : 0;
            DomicilioPartido = (empleado.Entidad != null) ? (empleado.Entidad.Direccion != null)? empleado.Entidad.Direccion.Partido :string.Empty: string.Empty;
            DomicilioProvincia = (empleado.Entidad != null) ? (empleado.Entidad.Direccion != null)? empleado.Entidad.Direccion.Provincia :  string.Empty : string.Empty;
            DomicilioPais = (empleado.Entidad != null) ? (empleado.Entidad.Direccion != null)? empleado.Entidad.Direccion.Pais : string.Empty : string.Empty;
            Falta = empleado.Falta;
            Licencia = (empleado.Licencia) ?? string.Empty;
            NroTarjeta = (empleado.Tarjeta != null)? empleado.Tarjeta.Numero : string.Empty;
            Transportista = empleado.Transportista != null ? empleado.Transportista.Descripcion : string.Empty;
            CategoriaAcceso = empleado.Categoria != null ? empleado.Categoria.Nombre : string.Empty;
        }
    }
}