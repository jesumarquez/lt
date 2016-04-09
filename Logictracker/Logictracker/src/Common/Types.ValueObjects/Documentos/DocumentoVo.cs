using System;
using Logictracker.Types.BusinessObjects.Documentos;

namespace Logictracker.Types.ValueObjects.Documentos
{
    [Serializable]
    public class DocumentoVo
    {
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;
        public const int IndexVehiculo = 2;
        public const int IndexEmpleado = 3;
        public const int IndexTransportista = 4;
        public const int IndexEquipo = 5;
        public const int IdexEstado = 6;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", InitialSortExpression = true, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IdexEstado, ResourceName = "Labels", VariableName = "Usuario", AllowGroup = true)]
        public string Usuario { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = true, IncludeInSearch = true)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexEmpleado, ResourceName = "Entities", VariableName = "PARENTI09", AllowGroup = true, IncludeInSearch = true)]
        public string Empleado { get; set; }

        [GridMapping(Index = IndexTransportista, ResourceName = "Entities", VariableName = "PARENTI07", AllowGroup = true, IncludeInSearch = true)]
        public string Transportista { get; set; }

        [GridMapping(Index = IndexEquipo, ResourceName = "Entities", VariableName = "PARENTI19", AllowGroup = true, IncludeInSearch = true)]
        public string Equipo { get; set; }

        [GridMapping(Index = IdexEstado, ResourceName = "Labels", VariableName = "ESTADO", AllowGroup = true)]
        public string Estado { get; set; }

        public DocumentoVo(Documento documento)
        {
            Id = documento.Id;
            Codigo = documento.Codigo;
            Descripcion = documento.Descripcion;
            Vehiculo = string.Empty;
            if (documento.Vehiculo != null)
            {
                try { Vehiculo = documento.Vehiculo.Interno; } catch {}
            }
            Empleado = documento.Empleado != null ? documento.Empleado.Entidad.Descripcion : string.Empty;
            Equipo = documento.Equipo != null ? documento.Equipo.Descripcion : string.Empty;

            if (documento.Transportista != null)
                Transportista = documento.Transportista.Descripcion;
            else if (documento.Vehiculo != null && documento.Vehiculo.Transportista != null)
                Transportista = documento.Vehiculo.Transportista.Descripcion;
            else if (documento.Empleado != null && documento.Empleado.Transportista != null)
                Transportista = documento.Empleado.Transportista.Descripcion;
            else 
                Transportista = string.Empty;

            foreach (var item in documento.Parametros)
            {
                if (item.Parametro.Nombre.Equals("Técnico"))
                {
                    Usuario = item.Valor;
                    break;
                }
            }
            switch (documento.Estado)
            {
                case 0: Estado = "Abierto"; break;
                case 1: Estado = "Prestado"; break;
                case 9: Estado = "Cerrado"; break;
            }
        }   
    }
}
