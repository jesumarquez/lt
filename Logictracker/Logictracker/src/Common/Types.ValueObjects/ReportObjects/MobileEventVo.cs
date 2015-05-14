using System;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class MobileEventVo
    {
        public const int KeyIndexId = 0;

        public const int IndexIntern = 0;
        public const int IndexIconUrl = 1;
        public const int IndexDriver = 2;
        public const int IndexResponsable = 3;
        public const int IndexEventTime = 4;
        public const int IndexReception = 5;
        public const int IndexDuration = 6;
        public const int IndexMessage = 7;
        public const int IndexAtendido = 8;
        public const int IndexUsuario = 9;
        public const int IndexFecha = 10;
        public const int IndexMensaje = 11;
        public const int IndexObservacion = 12;
        public const int IndexInitialCross = 13;
        public const int IndexFinalCross = 14;
        public const int IndexTieneFoto = 15;

        [GridMapping(Index = IndexIntern, ResourceName = "Entities", VariableName = "PARENTI03", IsInitialGroup = true, IncludeInSearch = true)]
        public string Intern { get { return _mobileEvent.Intern; } }

        [GridMapping(Index = IndexIconUrl, HeaderText = "", Width = "40px", AllowGroup = false)]
        public string IconUrl { get { return _mobileEvent.IconUrl; } }

        [GridMapping(Index = IndexDriver, ResourceName = "Labels", VariableName = "CHOFER", IncludeInSearch = true)]
        public string Driver { get { return _mobileEvent.Driver; } }

        [GridMapping(Index = IndexResponsable, ResourceName = "Labels", VariableName = "RESPONSABLE", IncludeInSearch = true)]
        public string Responsable { get { return _mobileEvent.Responsable; } }

        [GridMapping(Index = IndexEventTime, ResourceName = "Labels", VariableName = "DATE", DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", AllowGroup = false, InitialSortExpression = true, SortDirection = GridSortDirection.Ascending)]
        public DateTime? EventTime { get { return _mobileEvent.EventTime; } }

        [GridMapping(Index = IndexReception, ResourceName = "Labels", VariableName = "RECEPCION", DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", AllowGroup = false)]
        public DateTime? Reception { get { return _mobileEvent.Reception; } }

        [GridMapping(Index = IndexDuration, ResourceName = "Labels", VariableName = "DURACION", AllowGroup = false)]
        public TimeSpan? Duration { get { return _mobileEvent.Duration; } }

        [GridMapping(Index = IndexMessage, ResourceName = "Entities", VariableName = "PAREVEN01", IncludeInSearch = true)]
        public string Message { get { return _mobileEvent.Message; } }

        [GridMapping(Index = IndexAtendido, ResourceName = "Labels", VariableName = "ATENDIDO", AllowGroup = true)]
        public string Atendido { get { return _mobileEvent.Atendido ? "SI" : "NO"; } }

        [GridMapping(Index = IndexUsuario, ResourceName = "Entities", VariableName = "SOCUSUA01", AllowGroup = true, IncludeInSearch = true)]
        public string Usuario { get { return _mobileEvent.Usuario != null ? _mobileEvent.Usuario.NombreUsuario : string.Empty; } }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "FECHA_ATENCION", IncludeInSearch = true)]
        public string Fecha { get { return _mobileEvent.AtencionEvento != null ? _mobileEvent.AtencionEvento.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") : string.Empty; } }

        [GridMapping(Index = IndexMensaje, ResourceName = "Labels", VariableName = "MENSAJE_ATENCION", IncludeInSearch = true, AllowGroup = true)]
        public string Mensaje
        {
            get
            {
                return _mobileEvent.AtencionEvento != null && _mobileEvent.AtencionEvento.Mensaje != null
                            ? _mobileEvent.AtencionEvento.Mensaje.Descripcion
                            : CultureManager.GetControl("DDL_NO_MESSAGE");
            }
        }

        [GridMapping(Index = IndexObservacion, ResourceName = "Labels", VariableName = "OBSERVACION", IncludeInSearch = true)]
        public string Observacion { get { return _mobileEvent.AtencionEvento != null ? _mobileEvent.AtencionEvento.Observacion : string.Empty; } }

        [GridMapping(Index = IndexInitialCross, ResourceName = "Labels", VariableName = "ESQUINA_INICIAL", AllowGroup = false, IncludeInSearch = true)]
        public string InitialCross
        {
            get { return HideCornerNearest ? string.Empty : _mobileEvent.InitialCross; }
        }

        [GridMapping(Index = IndexFinalCross, ResourceName = "Labels", VariableName = "ESQUINA_FINAL", AllowGroup = false, IncludeInSearch = true)]
        public string FinalCross
        {
            get { return HideCornerNearest ? string.Empty : _mobileEvent.FinalCross; }
        }

        [GridMapping(Index = IndexTieneFoto, IsTemplate = true, HeaderText = "", AllowGroup = false)]
        public bool TieneFoto { get { return _mobileEvent.TieneFoto; } }

        [GridMapping(IsDataKey = true)]
        public int Id { get { return _mobileEvent.Id; } }

        private readonly MobileEvent _mobileEvent;

        public bool HideCornerNearest { get; set; }

        public MobileEventVo(MobileEvent mobileEvent)
        {
            _mobileEvent = mobileEvent;
        }
    }
}
