using System;
using System.Globalization;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Messages
{
    [Serializable]
    public class Accion : IAuditable, ISecurable
    {
        Type IAuditable.TypeOf() { return GetType(); }

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual int Id { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual Sonido Sonido { get; set; }
        public virtual byte Red { get; set; }
        public virtual byte Green { get; set; }
        public virtual byte Blue { get; set; }
        public virtual byte Alfa { get; set; }
        public virtual Icono PopUpIcon { get; set; }
        public virtual int PopIcon { get; set; }
        public virtual string PopUpTitle { get; set; }
        public virtual TipoCoche TipoVehiculo { get; set; }
        public virtual TipoReferenciaGeografica TipoReferenciaGeografica { get; set; }
        public virtual Mensaje Mensaje { get; set; }
        public virtual Transportista Transportista { get; set; }
        public virtual Departamento Departamento { get; set; }
        public virtual CentroDeCostos CentroDeCostos { get; set; }
        public virtual bool ReportaDepartamento { get; set; }
        public virtual bool ReportaCentroDeCostos { get; set; }
        public virtual bool RequiereAtencion { get; set; }

        public virtual string Condicion { get; set; }
        public virtual bool EsPopUp { get; set; }
        public virtual bool EsAlarmaSonora { get; set; }
        public virtual bool EsAlarmaDeMail { get; set; }
        public virtual bool EsAlarmaSms { get; set; }
        public virtual bool Habilita { get; set; }
        public virtual int HorasHabilitado { get; set; }
        public virtual bool Inhabilita { get; set; }
        public virtual Usuario UsuarioHabilitado { get; set; }
        public virtual Usuario UsuarioInhabilitado { get; set; }
        public virtual Perfil PerfilHabilitado { get; set; }
        public virtual string DestinatariosMail { get; set; }
        public virtual string DestinatariosSms { get; set; }
        public virtual bool Baja { get; set; }
        public virtual bool GrabaEnBase { get; set; }
        public virtual string AsuntoMail { get; set; }
        public virtual Boolean ModificaIcono { get; set; }
        public virtual Int32 IconoMensaje { get; set; }
        public virtual String PathIconoMensaje { get; set; }
        public virtual bool PideFoto { get; set; }
        public virtual bool CambiaMensaje { get; set; }
        public virtual int SegundosFoto { get; set; }
        public virtual string MensajeACambiar { get; set; }
        public virtual bool ReportarAssistCargo { get; set;}
        public virtual string CodigoAssistCargo { get; set;}

        /// <summary>
        /// Define si la Accion esta condicionada por el estado del movil dentro o fuera de una geocerca de tipo <see cref="TipoGeocerca"/>
        /// </summary>
        public virtual bool EvaluaGeocerca { get; set; }

        /// <summary>
        /// Define si la Accion sera valida solo dentro o fuera de una geocerca de tipo <see cref="TipoGeocerca"/>
        /// </summary>
        public virtual bool DentroGeocerca { get; set; }

        /// <summary>
        /// Tipo de geocerca para la evaluacion de validez de estado (dentro | fuera)
        /// </summary>
        public virtual TipoReferenciaGeografica TipoGeocerca { get; set; }

        public virtual bool EnviaReporte { get; set; }
        public virtual string Reporte { get; set; }
        public virtual string DestinatariosMailReporte { get; set; }
        public virtual bool ReportaResponsableCuenta { get; set; }

        public virtual string RGB
        {
            get
            {
                var RR = String.Format("{0:x}", Red);
                var GG = String.Format("{0:x}", Green);
                var BB = String.Format("{0:x}", Blue);
                var result = RR.Length == 2 ? RR : "0" + RR;

                result += GG.Length == 2 ? GG : "0" + GG;
                result += BB.Length == 2 ? BB : "0" + BB;

                return result;
            }
            set
            {
                var RRGGBB = value;

                if (RRGGBB.StartsWith("#")) RRGGBB = RRGGBB.Substring(1, RRGGBB.Length - 1);

                if (value.Length != 6) return;

                Red = byte.Parse(RRGGBB.Substring(0, 2), NumberStyles.HexNumber);
                Green = byte.Parse(RRGGBB.Substring(2, 2), NumberStyles.HexNumber);
                Blue = byte.Parse(RRGGBB.Substring(4, 2), NumberStyles.HexNumber);
            }
        }

        #region Public Methods

        public override bool Equals(object obj)
        {
            if (this == obj) return true;

            if ((obj == null) || (obj.GetType() != GetType())) return false;

            var castObj = obj as Accion;

            return (castObj != null) && (Id == castObj.Id);
        }

        public override int GetHashCode()
        {
            var hash = 57;

            hash = 27*hash*Id.GetHashCode();

            return hash;
        }

        #endregion
    }
}