using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Parametrizacion
{
    [Serializable]
    public class PlanVo
    {   
        public const int IndexCodigoAbono = 0;
        public const int IndexTipoLinea = 1;
        public const int IndexEmpresa = 2;
        public const int IndexCosto = 3;
        public const int IndexAbonoDatos = 4;
        public const int IndexUnidadMedida= 5;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexCodigoAbono, ResourceName = "Labels", VariableName = "CODIGO_ABONO", InitialSortExpression = true, AllowGroup = true, IncludeInSearch = true)]
        public string CodigoAbono { get; set; }

        [GridMapping(Index = IndexTipoLinea, ResourceName = "Labels", VariableName = "TIPO_LINEA", InitialSortExpression = false, AllowGroup = true, IncludeInSearch = false)]
        public string TipoLinea { get; set; }

        [GridMapping(Index = IndexEmpresa, ResourceName = "Labels", VariableName = "EMPRESA", InitialSortExpression = false, AllowGroup = true, IncludeInSearch = false)]
        public string Empresa { get; set; }

        [GridMapping(Index = IndexCosto, ResourceName = "Labels", VariableName = "COSTO_MENSUAL", InitialSortExpression = false, AllowGroup = false, IncludeInSearch = false)]
        public string Costo { get; set; }

        [GridMapping(Index = IndexAbonoDatos, ResourceName = "Labels", VariableName = "ABONO_DATOS", InitialSortExpression = false, AllowGroup = false, IncludeInSearch = false)]
        public string AbonoDatos { get; set; }

        [GridMapping(Index = IndexUnidadMedida, ResourceName = "Labels", VariableName = "UNIDAD_MEDIDA", InitialSortExpression = false, AllowGroup = true, IncludeInSearch = false)]
        public string UnidadMedida { get; set; }

        public PlanVo(Plan plan)
        {
            Id = plan.Id;
            CodigoAbono = plan.CodigoAbono;
            TipoLinea = plan.TipoLinea == 1 ? CultureManager.GetLabel("CELULAR") : CultureManager.GetLabel("SATELITAL");
            switch (plan.Empresa)
            {
                case 1: Empresa = "Claro"; break;
                case 2: Empresa = "Movistar"; break;
                case 3: Empresa = "Personal"; break;
                case 4: Empresa = "Orbcom"; break;
                default: Empresa = ""; break;
            }
            Costo = plan.CostoMensual.ToString("#0.00");
            AbonoDatos = plan.AbonoDatos.ToString("#0");
            UnidadMedida = plan.UnidadMedida == 1 ? "KB" : "MB";
        }
    }
}
