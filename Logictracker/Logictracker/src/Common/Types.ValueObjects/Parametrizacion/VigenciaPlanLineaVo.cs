using System;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Parametrizacion
{
    [Serializable]
    public class VigenciaPlanLineaVo
    {   
        public const int IndexDesde = 0;
        public const int IndexHasta = 1;
        public const int IndexPlan = 2;
        public const int IndexEmpresa = 3;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexDesde, ResourceName = "Labels", VariableName = "DESDE", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Inicio { get; set; }

        [GridMapping(Index = IndexHasta, ResourceName = "Labels", VariableName = "HASTA", InitialSortExpression = false, AllowGroup = false, IncludeInSearch = true)]
        public string Fin { get; set; }
        
        [GridMapping(Index = IndexPlan, ResourceName = "Entities", VariableName = "PARENTI73", InitialSortExpression = false, AllowGroup = false, IncludeInSearch = true)]
        public string Plan { get; set; }

        [GridMapping(Index = IndexEmpresa, ResourceName = "Labels", VariableName = "EMPRESA", InitialSortExpression = false, AllowGroup = false, IncludeInSearch = true)]
        public string Empresa { get; set; }

        public VigenciaPlanLineaVo(VigenciaPlanLinea vigenciaPlanLinea)
        {
            Id = vigenciaPlanLinea.Id;
            Inicio = vigenciaPlanLinea.Inicio.ToDisplayDateTime().ToString("dd/MM/yyyy");
            Fin = vigenciaPlanLinea.Fin != null ? vigenciaPlanLinea.Fin.Value.ToDisplayDateTime().ToString("dd/MM/yyyy") : "";
            Plan = vigenciaPlanLinea.Plan.CodigoAbono;
            switch (vigenciaPlanLinea.Plan.Empresa)
            {
                case 1: Empresa = "Claro"; break;
                case 2: Empresa = "Movistar"; break;
                case 3: Empresa = "Personal"; break;
                case 4: Empresa = "Orbcom"; break;
                default: Empresa = ""; break;
            }
        }
    }
}
