#region Usings

using System;
using Logictracker.Types.BusinessObjects.Messages;

#endregion

namespace Logictracker.Types.ValueObjects.Mensajes
{
    [Serializable]
    public class AccionVo
    {
        public const int IndexDescripcion = 0;
        public const int IndexLocation = 1;
        public const int IndexGrabaEnBase = 2;
        public const int IndexCambiaMensaje = 3;
        public const int IndexEsPopUp = 4;
        public const int IndexRequiereAtencion = 5;
        public const int IndexEsAlarmaSonora = 6;
        public const int IndexEnviaMails = 7;
        public const int IndexEnviaSms = 8;
        public const int IndexHabilitaUsuario = 9;
        public const int IndexInHabilitaUsuario = 10;
        public const int IndexModificaIcono = 11;
        public const int IndexPideFoto = 12;
        public const int IndexEvaluaGeocerca = 13;
        public const int IndexReportaAssistCargo = 14;

        public int Id { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexLocation, ResourceName = "Labels", VariableName = "LOCATION")]
        public string Location { get; set; }

        [GridMapping(Index = IndexGrabaEnBase, IsTemplate = true, ResourceName = "Labels", VariableName = "GRABA_EN_BASE", Width = "60px", AllowGroup = false)]
        public bool GrabaEnBase { get; set; }

        [GridMapping(Index = IndexCambiaMensaje, IsTemplate = true, ResourceName = "Labels", VariableName = "CAMBIA_MENSAJE", Width = "60px", AllowGroup = false)]
        public bool CambiaMensaje { get; set; }

        [GridMapping(Index = IndexEsPopUp, IsTemplate = true, ResourceName = "Labels", VariableName = "ES_POPUP", Width = "60px", AllowGroup = false)]
        public bool EsPopUp { get; set; }

        [GridMapping(Index = IndexRequiereAtencion, IsTemplate = true, ResourceName = "Labels", VariableName = "REQUIERE_ATENCION", Width = "60px", AllowGroup = false)]
        public bool RequiereAtencion { get; set; }

        [GridMapping(Index = IndexEsAlarmaSonora, IsTemplate = true, ResourceName = "Labels", VariableName = "ES_ALARMA_SONORA", Width = "60px", AllowGroup = false)]
        public bool EsAlarmaSonora { get; set; }

        [GridMapping(Index = IndexEnviaMails, IsTemplate = true, ResourceName = "Labels", VariableName = "ENVIA_MAILS", Width = "60px", AllowGroup = false)]
        public bool EsAlarmaDeMail { get; set; }

        [GridMapping(Index = IndexEnviaSms, IsTemplate = true, ResourceName = "Labels", VariableName = "ENVIA_SMS", Width = "60px", AllowGroup = false)]
        public bool EsAlarmaSms { get; set; }

        [GridMapping(Index = IndexHabilitaUsuario, IsTemplate = true, ResourceName = "Labels", VariableName = "HABILITA_USUARIO", Width = "60px", AllowGroup = false)]
        public bool Habilita { get; set; }

        [GridMapping(Index = IndexInHabilitaUsuario, IsTemplate = true, ResourceName = "Labels", VariableName = "INHABILITA_USUARIO", Width = "60px", AllowGroup = false)]
        public bool Inhabilita { get; set; }

        [GridMapping(Index = IndexModificaIcono, IsTemplate = true, ResourceName = "Labels", VariableName = "CHANGE_ICON", Width = "60px", AllowGroup = false)]
        public bool ModificaIcono { get; set; }

        [GridMapping(Index = IndexPideFoto, IsTemplate = true, ResourceName = "Labels", VariableName = "PIDE_FOTO", Width = "60px", AllowGroup = false)]
        public bool PideFoto { get; set; }

        [GridMapping(Index = IndexEvaluaGeocerca, IsTemplate = true, ResourceName = "Labels", VariableName = "ACTION_EVALUATES_GEOFENCE", Width = "60px", AllowGroup = false)]
        public bool EvaluaGeocerca { get; set; }

        [GridMapping(Index = IndexReportaAssistCargo, IsTemplate = true, ResourceName = "Labels", VariableName = "REPORTA_ASSISTCARGO", Width = "60px", AllowGroup = false)]
        public bool ReportaAssistCargo { get; set; }

        public byte Alpha { get; set; }
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }

        public AccionVo(Accion accion)
        {
            Id = accion.Id;
            Descripcion = accion.Descripcion;
            Location = accion.Empresa != null
                           ? accion.Linea != null ? string.Concat(accion.Empresa.RazonSocial, " - ", accion.Linea.Descripcion)
                                 : accion.Empresa.RazonSocial : accion.Linea != null ? accion.Linea.Descripcion : "-";
            GrabaEnBase = accion.GrabaEnBase;
            EsPopUp = accion.EsPopUp;
            RequiereAtencion = accion.RequiereAtencion;
            EsAlarmaSonora = accion.EsAlarmaSonora;
            EsAlarmaDeMail = accion.EsAlarmaDeMail;
            EsAlarmaSms = accion.EsAlarmaSms;
            Habilita = accion.Habilita;
            Inhabilita = accion.Inhabilita;
            ModificaIcono = accion.ModificaIcono;
            EvaluaGeocerca = accion.EvaluaGeocerca;
            CambiaMensaje = accion.CambiaMensaje;
            PideFoto = accion.PideFoto;
            ReportaAssistCargo = accion.ReportarAssistCargo;

            Alpha = accion.Alfa;
            Red = accion.Red;
            Green = accion.Green;
            Blue = accion.Blue;
        }
    }
}
