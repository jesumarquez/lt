using System;
using System.Collections.Generic;
using System.Drawing;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Mantenimiento
{
    [Serializable]
    public class TicketMantenimiento : IAuditable, IHasEmpresa, IHasVehiculo, IHasEmpleado, IHasTaller
    {
        public static class EstadosTicket
        {   
            public const short Ingresado = 0;
            public const short Aprobado = 1;
            public const short Terminado = 2;
            public const short Aceptado = 3;
            public const short NoAceptado = 4;
            public const short Cancelado = 9;

            public static string GetLabelVariableName(short estado)
            {
                switch (estado)
                {
                    case Ingresado: return "TICKET_MANT_INGRESADO";
                    case Aprobado: return "TICKET_MANT_APROBADO";
                    case Terminado: return "TICKET_MANT_TERMINADO";
                    case Aceptado: return "TICKET_MANT_ACEPTADO";
                    case NoAceptado: return "TICKET_MANT_NO_ACEPTADO";
                    case Cancelado: return "TICKET_MANT_CANCELADO";
                }
                return string.Empty;
            }
            public static Color GetColor(short estado)
            {
                switch (estado)
                {
                    case Ingresado: return Color.Gray;
                    case Aprobado: return Color.CornflowerBlue;
                    case Terminado: return Color.Gold;
                    case Aceptado: return Color.YellowGreen;
                    case NoAceptado: return Color.Orange;
                    case Cancelado: return Color.Red;
                }
                return Color.Gray;
            }
            public static List<short> EstadosAbiertos = new List<short> { Ingresado, Aprobado, NoAceptado };
            public static List<short> EstadosCerrados = new List<short> { Terminado, Aceptado, Cancelado};
        }
        public static class EstadosPresupuesto
        {
            public const short SinPresupuesto = 0;
            public const short Presupuestado = 1;
            public const short Recotizado = 2;
            public const short Aprobado = 3;
            public const short Terminado = 4;
            public const short VerificadoSinAprobar = 5;
            public const short AceptadoCliente = 6;
            public const short Cancelado = 9;

            public static string GetLabelVariableName(short estadoPresupuesto)
            {
                switch (estadoPresupuesto)
                {
                    case SinPresupuesto: return "PRESUPUESTO_SIN_PRESUPUESTO";
                    case Presupuestado: return "PRESUPUESTO_PRESUPUESTADO";
                    case Recotizado: return "PRESUPUESTO_RECOTIZADO";
                    case Aprobado: return "PRESUPUESTO_APROBADO";
                    case Terminado: return "PRESUPUESTO_TERMINADO";
                    case VerificadoSinAprobar: return "PRESUPUESTO_TERMINADO_SIN_APROBAR";
                    case AceptadoCliente: return "PRESUPUESTO_ACEPTADO_CLIENTE";
                    case Cancelado: return "PRESUPUESTO_CANCELADO";
                }
                return string.Empty;
            }
            public static Color GetColor(short estado)
            {
                switch (estado)
                {
                    case SinPresupuesto: return Color.Gray;
                    case Presupuestado: return Color.CornflowerBlue;
                    case Recotizado: return Color.Turquoise;
                    case Aprobado: return Color.CornflowerBlue;
                    case Terminado: return Color.Gold;
                    case VerificadoSinAprobar: return Color.LightGreen;
                    case AceptadoCliente: return Color.YellowGreen;
                    case Cancelado: return Color.Red;
                }
                return Color.Gray;
            }
        }
        public static class NivelesComplejidad
        {
            public const short Baja = 0;
            public const short Media = 1;
            public const short Alta = 2;
            public const short MuyAlta = 3;

            public static string GetLabelVariableName(short nivel)
            {
                switch (nivel)
                {
                    case Baja: return "NIVEL_COMPLEJIDAD_BAJA";
                    case Media: return "NIVEL_COMPLEJIDAD_MEDIA";
                    case Alta: return "NIVEL_COMPLEJIDAD_ALTA";
                    case MuyAlta: return "NIVEL_COMPLEJIDAD_MUY_ALTA";
                }
                return string.Empty;
            }
        }

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        public virtual Empresa Empresa { get; set; }
        public virtual Coche Vehiculo { get; set; }
        public virtual Empleado Empleado { get; set; }
        public virtual Taller Taller { get; set; }

        public virtual string Descripcion { get; set; }
        public virtual string Codigo { get; set; }

        public virtual string PrimerPresupuesto { get; set; }
        public virtual string Presupuesto { get; set; }
        public virtual double Monto { get; set; }
        public virtual DateTime FechaSolicitud { get; set; }
        public virtual DateTime? FechaTurno { get; set; }
        public virtual DateTime? FechaRecepcion { get; set; }
        public virtual DateTime? FechaPresupuestoOriginal { get; set; }
        public virtual DateTime? FechaPresupuestada { get; set; }
        public virtual DateTime? FechaRecotizacion { get; set; }
        public virtual DateTime? FechaAprobacion { get; set; }
        public virtual DateTime? FechaVerificacion { get; set; }
        public virtual DateTime? FechaEntrega { get; set; }
        public virtual DateTime? FechaTrabajoTerminado { get; set; }
        public virtual DateTime? FechaTrabajoAceptado { get; set; }

        public virtual DateTime? Entrada { get; set; }
        public virtual DateTime? Salida { get; set; }
    
        public virtual short Estado { get; set; }
        public virtual short EstadoPresupuesto { get; set; }
        public virtual short NivelComplejidad { get; set; }

        private IList<HistoriaTicketMantenimiento> _historia;
        public virtual IList<HistoriaTicketMantenimiento> Historia
        {
            get { return _historia ?? (_historia = new List<HistoriaTicketMantenimiento>()); }
            set { _historia = value; }
        }
    }
}