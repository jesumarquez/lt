using System;
using Logictracker.Culture;
using Logictracker.Types.ReportObjects.RankingDeOperadores;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class InfractionDetailVo
    {
        public const int IndexOperador = 0;
        public const int IndexTipoInfraccion = 1;
        public const int IndexCalificacion = 2;
        public const int IndexVehiculo = 3;
        public const int IndexCornerNearest = 4;
        public const int IndexInicio = 5;
        public const int IndexDuracion = 6;
        public const int IndexPico = 7;
        public const int IndexExceso = 8;
        public const int IndexPonderacion = 9;
        public const int IndexCalificacionStr = 10;

        [GridMapping(Index = IndexOperador, ResourceName = "Labels", VariableName = "CHOFER", IsInitialGroup = true, InitialGroupIndex = 0)]
        public string Operador { get { return InfractionDetail.Operador; } }

        [GridMapping(Index = IndexTipoInfraccion, ResourceName = "Labels", VariableName = "TIPO_INFRACCION", AllowGroup = true )]
        public string TipoInfraccion { get { return InfractionDetail.TipoInfraccion; } }

        [GridMapping(Index = IndexCalificacion, ResourceName = "Labels", VariableName = "CALIFICACION", IsInitialGroup = true, InitialGroupIndex = 1)]
        public int Calificacion { get { return InfractionDetail.Calificacion; } }

        [GridMapping(Index = IndexCalificacionStr, ResourceName = "Labels", VariableName = "CALIFICACION_STR", Visible = false)]
        public string CalificacionStr 
        { 
            get
            {
                switch (InfractionDetail.Calificacion)
                {
                    case 1:
                        return CultureManager.GetLabel("LEVE");
                    case 2:
                        return CultureManager.GetLabel("MEDIA");
                    case 3:
                        return CultureManager.GetLabel("GRAVE");
                }
                return CultureManager.GetLabel("SIN_DEFINIR");
            } 
        }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03")]
        public string Vehiculo { get { return InfractionDetail.Vehiculo; } }

        private string _cornerNearest;
        [GridMapping(Index = IndexCornerNearest, ResourceName = "Labels", VariableName = "ESQUINA")]
        public string CornerNearest
        {
            get { return HideCornerNearest ? string.Empty :_cornerNearest ?? (_cornerNearest = InfractionDetail.CornerNearest); }
        }

        [GridMapping(Index = IndexInicio, ResourceName = "Labels", VariableName = "INICIO", DataFormatString = "{0:G}", AllowGroup = false, InitialSortExpression = true)]
        public DateTime Inicio { get { return InfractionDetail.Inicio; } }

        [GridMapping(Index = IndexDuracion, ResourceName = "Labels", VariableName = "DURACION", AllowGroup = false)]
        public TimeSpan Duracion { get { return InfractionDetail.Duracion; } }

        [GridMapping(Index = IndexPico, ResourceName = "Labels", VariableName = "PICO", AllowGroup = false)]
        public int Pico { get { return InfractionDetail.Pico; } }

        [GridMapping(Index = IndexExceso, ResourceName = "Labels", VariableName = "EXCESO", AllowGroup = false)]
        public int Exceso { get { return InfractionDetail.Exceso; } }

        [GridMapping(Index = IndexPonderacion, ResourceName = "Labels", VariableName = "PONDERACION", AllowGroup = false)]
        public double Ponderacion { get { return InfractionDetail.Ponderacion; } }

        [GridMapping(IsDataKey = true)]
        public int Id { get { return InfractionDetail.Id; } }

        protected InfractionDetail InfractionDetail;

        public bool HideCornerNearest { get; set; }

        public InfractionDetailVo(InfractionDetail infractionDetail)
        {
            InfractionDetail = infractionDetail;
        }
    }
}
