using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Types.ValueObjects.Documentos.Partes;
using Logictracker.Web.Documentos.Interfaces;

namespace Logictracker.Web.Documentos.Partes
{
    public class ParteSaveStrategy:DefaultSaveStrategy
    {
        private static readonly TimeSpan DefaultTs = TimeSpan.FromDays(10);
        private readonly List<OrderDocumentoValor> _times = new List<OrderDocumentoValor>();

        public ParteSaveStrategy(TipoDocumento tipoDoc, IDocumentView view) : base(tipoDoc, view)
        {
            
        }

        protected override void Validate(Documento doc, int idUsuario, DAOFactory daoF)
        {
            var transp = daoF.TransportistaDAO.FindById(Convert.ToInt32(doc.Valores[ParteCampos.Empresa]));
            var p = daoF.PeriodoDAO.GetByDate(transp.Empresa != null ? transp.Empresa.Id : -1, Fecha);
            if (p != null && p.Estado > 0)
                throw new ApplicationException("El periodo al que pertenece este parte ya ha sido cerrado. Periodo: " + p.Descripcion);

            base.Validate(doc, idUsuario, daoF);
            doc.ResetDictionary();
            var docs = daoF.DocumentoDAO.FindByTransportistaYCodigo(Convert.ToInt32(doc.Valores[ParteCampos.Empresa]), doc.Codigo);
            if (docs.Count > 1 || (docs.Count == 1 && ((Documento) docs[0]).Id != doc.Id))
                throw new ApplicationException("Ya existe un parte con este codigo para la misma empresa transportista");

            var minOdometro = Int32.MaxValue;
            var current = new PartePersonal(doc, daoF);
            var lastOdometro = 0;
            foreach (var turno in current.Turnos)
            {
                if (turno.OdometroInicial < lastOdometro || turno.OdometroFinal < turno.OdometroInicial)
                    throw new ApplicationException("El valor del Odometro no puede ser menor a uno anterior");
                lastOdometro = turno.OdometroFinal;
                minOdometro = Math.Min(minOdometro, turno.OdometroInicial);
            }
            
            var idVehiculo = Convert.ToInt32(doc.Valores[ParteCampos.Vehiculo]);

            var last = daoF.DocumentoDAO.FindLastForVehicle(idVehiculo, current.Turnos[0].AlPozoSalida);
            if (last != null)
            {
                var previous = new PartePersonal(last, daoF);
                var prevTurnos = previous.Turnos.Where(t => t.DelPozoLlegada < current.Turnos[0].AlPozoSalida);
                var curTurnos = current.Turnos;
                var maxPrev = prevTurnos.Count() > 0 ? prevTurnos.Max(t => t.OdometroFinal) : 0;
                var minCur = curTurnos.Count() > 0 ? curTurnos.Min(t => t.OdometroInicial) : 0;
                if (minCur < maxPrev)
                    throw new ApplicationException("El parte " + last.Codigo +
                                                   " contiene un valor de odometro mayor al actual (" + maxPrev + " > " + minCur + ")");
            }

            var superpuestos = daoF.DocumentoDAO.FindList(current.IdEmpresa, doc.Linea != null ? doc.Linea.Id : -1, idVehiculo,
                                                          current.Turnos[0].AlPozoSalida.Subtract(TimeSpan.FromDays(3)),
                                                          current.Turnos[current.Turnos.Count - 1].DelPozoLlegada.Add(TimeSpan.FromDays(3)), -1,
                                                          -1, null);
            if(superpuestos.Count > 0)
            {
                foreach (var turno in current.Turnos)
                {
                    foreach (Documento sdoc in superpuestos)
                    {
                        if (sdoc.Id == doc.Id) continue;
                        var spar = new PartePersonal(sdoc, daoF);
                        if (Superpuesto(turno, spar))
                            throw new ApplicationException("Los horarios de este parte se superponen con los del parte " + sdoc.Codigo);
                    }
                }
            }
        }
        public bool Superpuesto(TurnoPartePersonal turno, PartePersonal parte2)
        {
            foreach (var turno2 in parte2.Turnos)
            {
                if (turno2.AlPozoSalida <= turno.AlPozoSalida && turno2.AlPozoLlegada > turno.AlPozoSalida) return true;
                if (turno2.AlPozoSalida <= turno.AlPozoLlegada && turno2.AlPozoLlegada > turno.AlPozoLlegada) return true;
                if (turno2.AlPozoSalida <= turno.DelPozoSalida && turno2.AlPozoLlegada > turno.DelPozoSalida) return true;
                if (turno2.AlPozoSalida <= turno.DelPozoLlegada && turno2.AlPozoLlegada > turno.DelPozoLlegada) return true;

                if (turno2.DelPozoSalida <= turno.AlPozoSalida && turno2.DelPozoLlegada > turno.AlPozoSalida) return true;
                if (turno2.DelPozoSalida <= turno.AlPozoLlegada && turno2.DelPozoLlegada > turno.AlPozoLlegada) return true;
                if (turno2.DelPozoSalida <= turno.DelPozoSalida && turno2.DelPozoLlegada > turno.DelPozoSalida) return true;
                if (turno2.DelPozoSalida <= turno.DelPozoLlegada && turno2.DelPozoLlegada > turno.DelPozoLlegada) return true;
            }
            return false;
        }

        public override void Save(Documento doc, int idUsuario, DAOFactory daoF)
        {
            

            base.Save(doc, idUsuario, daoF);
        }
        protected override Documento FillValues(Documento doc, int idUsuario, DAOFactory daoF)
        {
            _times.Clear();
            var d = base.FillValues(doc, idUsuario, daoF);
            
            _times.Sort(new ParteTimeComparer());

            var minDate = DateTime.MaxValue;
            var dt = DateTime.MinValue;
            foreach (var ov in _times)
            {
                var valor = ov.Valor;
                var hs = Convert.ToDateTime(valor.Valor, CultureInfo.InvariantCulture);
                while (hs < dt)
                {
                    hs = hs.AddDays(1);
                    //valor.Valor = hs.ToString(CultureInfo.InvariantCulture);
                    SetValor(doc, valor.Parametro.Nombre, valor.Repeticion, hs.ToString(CultureInfo.InvariantCulture));
                }
                dt = hs;
                if(dt < minDate) minDate = dt;
            }
            d.ResetDictionary();

            SetValor(doc, ParteCampos.EstadoControl, 1, "0");

            //var horas = d.Valores[ParteCampos.SalidaAlPozo] as List<object>;
            //if (horas == null) return d;
            //if (!DateTime.TryParse(horas[0].ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) return d;

            d.Fecha = minDate;


            if (d.Linea == null)
            {
                var idVehiculo = Convert.ToInt32(d.Valores[ParteCampos.Vehiculo]);
                var vehiculo = daoF.CocheDAO.FindById(idVehiculo);
                if (vehiculo.Linea == null) throw new ApplicationException("No se puede crear un parte sin Base. El vehiculo seleccionado no está asignado a una Base.");
                doc.Linea = vehiculo.Linea;
            }

            return d;
        }
        public static void SetValor(Documento parte, string campo, int repeticion, string valor)
        {
            var param = (from DocumentoValor v in parte.Parametros
                         where v.Parametro.Nombre == campo
                               && v.Repeticion == repeticion
                         select v).ToList();

            var val = param.Count > 0 ? param[0] : null;

            if (val == null)
            {
                var par = from TipoDocumentoParametro p in parte.TipoDocumento.Parametros
                          where p.Nombre == campo
                          select p;

                val = new DocumentoValor
                          {
                              Documento = parte,
                              Parametro = par.ToList()[0],
                              Repeticion = ((short) repeticion)
                          };

                parte.Parametros.Add(val);
            }

            val.Valor = valor;
        }

        protected override DocumentoValor GetDocValue(TipoDocumentoParametro par, short repeticion)
        {
            if (par.Nombre == ParteCampos.EstadoControl) return null;

            if(par.Nombre == ParteCampos.TipoServicio)
            {
                var id = par.Nombre.Replace(' ', '_');
                if (par.Repeticion != 1) id += repeticion;

                var ctl = view.DocumentContainer.FindControl(id) as DropDownList;
                if (ctl == null) return null;

                return new DocumentoValor
                           {
                               Parametro = par,
                               Repeticion = repeticion,
                               Valor = ctl.SelectedValue
                           };
            }

            var val = base.GetDocValue(par, repeticion);

            switch(par.Nombre)
            {
                case ParteCampos.SalidaAlPozo:
                case ParteCampos.LlegadaAlPozo:
                case ParteCampos.SalidaDelPozo:
                case ParteCampos.LlegadaDelPozo:
                    var ts = GetHours(par, repeticion);
                    var dt = Fecha.Date.Add(ts);
                    val.Valor = dt.ToDataBaseDateTime().ToString(CultureInfo.InvariantCulture);
                    _times.Add(new OrderDocumentoValor(repeticion.ToString()+val.Parametro.Orden, val));
                    break;
                case ParteCampos.KilometrajeTotal:
                    break;
                case ParteCampos.Kilometraje:
                    break;
                case ParteCampos.KilometrajeGps:
                    break;
                default:
                    break;
            }

            return val;
        }

        private TimeSpan GetHours(TipoDocumentoParametro par, short repeticion)
        {
            var id = par.Nombre.Replace(' ', '_');
            if (par.Repeticion != 1) id += repeticion;

            var ctl = view.DocumentContainer.FindControl(id);
            var dval = ((TextBox) ctl).Text;
            if (!par.Obligatorio && string.IsNullOrEmpty(dval))
                return DefaultTs;

            var parts = dval.Split(':');
            if(parts.Length != 2)
                throw new ApplicationException("El campo " + par.Nombre + " no tiene el formato correcto");

            int hh, mm;
            if(!int.TryParse(parts[0], out hh) || !int.TryParse(parts[1], out mm))
                throw new ApplicationException("El campo " + par.Nombre + " no tiene el formato correcto");

            if(hh < 0 || hh > 23 || mm < 0 || mm > 59)
                throw new ApplicationException("El campo " + par.Nombre + " no tiene el formato correcto");

            var ts = new TimeSpan(0, hh, mm, 0);

            return ts;
        }
    }

    internal class OrderDocumentoValor
    {
        public string Orden;
        public DocumentoValor Valor;
        public OrderDocumentoValor(string o, DocumentoValor v)
        {
            Orden = o;
            Valor = v;
        }
    }

    internal class ParteTimeComparer : IComparer<OrderDocumentoValor>
    {
        #region IComparer<OrderDocumentoValor> Members

        public int Compare(OrderDocumentoValor x, OrderDocumentoValor y)
        {
            return x.Orden.CompareTo(y.Orden);
        }

        #endregion
    }
}