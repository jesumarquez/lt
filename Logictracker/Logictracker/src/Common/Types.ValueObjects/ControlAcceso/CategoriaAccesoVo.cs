using System;
using Logictracker.Types.BusinessObjects.ControlAcceso;

namespace Logictracker.Types.ValueObjects.ControlAcceso
{
    [Serializable]
    public class CategoriaAccesoVo
    {
        public static class Index
        {
            public const int Empresa = 0;
            public const int Linea = 1;
            public const int Nombre = 2;
            public const int Descripcion = 3;
        }
        [GridMapping(Index = Index.Empresa, ResourceName = "Entities", VariableName = "PARENTI01", AllowGroup = true)]
        public string Empresa { get; set; }

        [GridMapping(Index = Index.Linea, ResourceName = "Entities", VariableName = "PARENTI02", AllowGroup = true)]
        public string Linea { get; set; }

        [GridMapping(Index = Index.Nombre, ResourceName = "Labels", VariableName = "NAME", AllowGroup = false, IncludeInSearch = true)]
        public string Nombre { get; set; }

        [GridMapping(Index = Index.Descripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        public int Id { get; set; }

        public CategoriaAccesoVo(CategoriaAcceso categoria)
        {
            Id = categoria.Id;
            Empresa = categoria.Empresa != null ? categoria.Empresa.RazonSocial : string.Empty;
            Linea = categoria.Linea != null ? categoria.Linea.Descripcion : string.Empty;
            Nombre = categoria.Nombre;
            Descripcion = categoria.Descripcion;
        }
        
    }
}
