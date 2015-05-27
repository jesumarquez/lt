#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Iesi.Collections;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Support
{
    public class SupportTicket : IAuditable, ISecurable, IHasCategoria, IHasSubcategoria, IHasNivel, IHasDispositivo, IHasVehiculo
    {
        #region ISecurable

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get { return null; } }

        #endregion

        public virtual int Id { get; set;}
        public virtual DateTime Fecha { get; set; }
        public virtual string Nombre { get; set; }
        public virtual string Telefono { get; set; }
        public virtual string Mail { get; set; }
        public virtual short TipoProblema { get; set; }
        public virtual short Categoria { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string FileName { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual bool Baja { get; set; }
        public virtual short Nivel { get; set; }
        public virtual string MailList { get; set; }
        public virtual int Funcion { get; set; }
        public virtual DateTime FechaInicio { get; set; }
        public virtual Coche Vehiculo { get; set; }
        public virtual Dispositivo Dispositivo { get; set; }
        public virtual string NroParte { get; set; }
        public virtual Categoria CategoriaObj { get; set; }
        public virtual Subcategoria Subcategoria { get; set; }
        public virtual Nivel NivelObj { get; set; }

        private ISet<SupportTicketDetail> _states;
        
        public virtual ISet<SupportTicketDetail> States { get { return _states ?? (_states = new HashSet<SupportTicketDetail>()); } }

        public virtual void ClearStates() { States.Clear(); }

        public virtual void AddDetail(SupportTicketDetail detail) { States.Add(detail); }

        public virtual short CurrentState
        {
            get
            {
                return States.Count == 0 ? (short) 0 : States.OfType<SupportTicketDetail>().Last().Estado;
            }
        }

        public virtual DateTime FinIncidencia
        {
            get
            {
                if(CurrentState != 7 && CurrentState != 8) // si no esta Cerrado o Rechazado
                {
                    return DateTime.UtcNow;
                }
                var states = States.OfType<SupportTicketDetail>().OrderByDescending(s => s.Fecha);
                var last = states.First().Fecha;
                if (CurrentState == 8) return last;
                var b = new List<int>{0, 1, 2, 3, 6};
                foreach (var detail in states)
                {
                    if(b.Contains(detail.Estado)) break;
                    last = detail.Fecha;
                }
                return last;
            }
        }
        public virtual DateTime FechaEnCurso
        {
            get
            {
                var b = new List<int> { 2, 3, 4, 5, 6, 7, 8 };
                foreach (SupportTicketDetail state in States)
                {
                    if (b.Contains(state.Estado)) return state.Fecha;
                }
                return DateTime.UtcNow;
            }
        }

        public virtual TimeSpan TiempoIncidencia
        {
            get
            {
                return FinIncidencia.Subtract(FechaInicio);
            }
        }

        public virtual TimeSpan TiempoAtencion
        {
            get
            {
                return FechaEnCurso.Subtract(Fecha);
            }
        }

        public virtual TimeSpan TiempoResolucion
        {
            get
            {
                return FinIncidencia.Subtract(FechaEnCurso);
            }
        }

        public virtual TimeSpan TiempoEsperaCliente
        {
            get
            {
                var tiempo = TimeSpan.Zero;
                var last = new DateTime?();
                foreach (SupportTicketDetail state in States)
                {
                    if (state.Estado == 3 && !last.HasValue)
                    {
                        last = state.Fecha;
                    }
                    if(state.Estado != 3 && last.HasValue)
                    {
                        tiempo = tiempo.Add(state.Fecha.Subtract(last.Value));
                        last = new DateTime?();
                    }
                }
                return tiempo;
            }
        }

        public virtual Type TypeOf() { return GetType(); }
    }
}
