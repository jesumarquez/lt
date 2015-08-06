using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Logictracker.Culture;
using Logictracker.DAL.Factories;
using Logictracker.Process.Geofences;
using Logictracker.Process.Geofences.Classes;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Helpers;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.Operacion.Mimico
{
    public partial class MimicoEntregas : ApplicationSecuredPage
    {
        protected override string GetRefference() { return "OPE_MIMICO_ENTREGAS"; }
        protected override InfoLabel LblInfo { get { return null; } }
        
        protected override void OnInit(EventArgs e)
        {
            if (WebSecurity.ShowDriver) cbVehiculo.ShowDriverName = true;

            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            RegisterExtJsStyleSheet();

            dtFecha.SetDate();
        }
        
        protected void BtnBuscarClick(object sender, EventArgs e)
        {
            var desde = dtFecha.SelectedDate.Value.ToDataBaseDateTime();
            var hasta = desde.AddHours(24);
            ShowInfo(desde, hasta);
        }

        public void ShowInfo(DateTime desde, DateTime hasta)
        {
            ClearMimico();
            var distribuciones = DAOFactory.ViajeDistribucionDAO.GetList(cbEmpresa.SelectedValues, 
                                                                         cbLinea.SelectedValues,
                                                                         new[] { -1 }, // TRANSPORTISTAS
                                                                         new[] { -1 }, // DEPARTAMENTOS
                                                                         new[] { -1 }, // CC                                                                          
                                                                         new[] { -1 }, // SUB CC
                                                                         cbVehiculo.SelectedValues,
                                                                         desde, 
                                                                         hasta)
                                                                .Where(v => v.Vehiculo != null);
            var ciclos = new List<Ciclo>();

            foreach (var distribucion in distribuciones)
            {
                var ciclo = new Ciclo(distribucion, DAOFactory);
                ciclos.Add(ciclo);                
            }

            var sh = new ScriptHelper(this);
            foreach (var ciclo in ciclos.OrderByDescending(c => c.Completed))
            {
                sh.RegisterStartupScript(string.Format("init_{0}_{1}", ciclo.Tipo, ciclo.Id), ciclo.Render(), true);    
            }
        }

        private void ClearMimico()
        {
            var sh = new ScriptHelper(this);
            sh.RegisterStartupScript("clear", "clearMimicos();", true);
        }
                
        private class Ciclo
        {
            private const string Template = "{{ 'id': '{0}' , 'type': '{5}', 'interno': '{1}', 'style': '{2}', 'icon': '{3}', completed: {4} }}";
            public int Id { get; set; }
            public string Tipo { get; set;}
            public string Icono { get; set; }
            public string Interno { get; set; }
            public string LabelStyle { get; set; }
            public int Completed { get; set; }
            public List<Detalle> Detalles { get; set; }
           
            public Ciclo(ViajeDistribucion distribucion, DAOFactory daoFactory)
            {
                Id = distribucion.Id;
                Tipo = "Distribucion";
                Icono = Path.Combine(IconDir, distribucion.Vehiculo.TipoCoche.IconoDefault.PathIcono);
                Interno = distribucion.Vehiculo.Interno;
                LabelStyle = GetLabelStyle(distribucion.Vehiculo, daoFactory);
                Detalles = distribucion.Detalles.Where(d => d.PuntoEntrega != null).Select((d, i) => new Detalle(d) {Descripcion = (i + 1).ToString()}).ToList();
                Completed = GetCompleted(distribucion);
            }

            private int GetCompleted(ViajeDistribucion distribucion)
            {
                var detalles = distribucion.Detalles.Where(d => d.Linea == null);
                var cantDetalles = detalles.Count();
                var entregados = detalles.Count(detalle => detalle.Entrada.HasValue || detalle.Manual.HasValue || detalle.Salida.HasValue);

                return Convert.ToInt32(entregados * 100 / cantDetalles);                
            }

            private string GetLabelStyle(Coche coche, DAOFactory daoFactory)
            {
                var upm = SharedPositions.GetLastPositions(new List<Coche> { coche }).FirstOrDefault();
                if (upm == null) return "ol_marker_labeled_red";

                string style;

                if (upm.FechaMensaje >= DateTime.UtcNow.AddMinutes(-5)) style = upm.Velocidad == 0 ? "ol_marker_labeled" : "ol_marker_labeled_green";
                else style = upm.FechaMensaje >= DateTime.UtcNow.AddHours(-48) ? "ol_marker_labeled_yellow" : "ol_marker_labeled_red";

                return style;
            }

            public string Render()
            {
                var m = string.Format(Template, Id, Interno, LabelStyle, Icono, Completed, Tipo);
                var st = string.Concat("[", string.Join(",", Detalles.Select(d=>d.Render()).ToArray()), "]");

                return string.Format("addMimico('{0}', {1}, {2});", Id, m, st);
            }
        }

        private class Detalle
        {
            public int Id { get; set; }
            public string Descripcion { get; set; }
            public short Estado { get; set; }
            public string PopupText { get; set; }
            public int Duracion { get; set; }
            public int PorcentajeDelTotal { get; set;}
            public int PorcentajeParcial { get; set; }
            public bool Completado { get; set; }
            public DateTime Programado { get; set; }
            public DateTime? Automatico { get; set; }
            public double Latitud { get; set;}
            public double Longitud { get; set; }
            
            public Detalle(EntregaDistribucion entrega)
            {
                Id = entrega.Id;
                Descripcion = entrega.Orden.ToString();
                PopupText = string.Format("<div>Cliente: <span>{5}</span></div><div>Estado: <span>{4}</span></div><div>Programado: <span>{0}</span><br/>Entrada: <span>{1}</span><br/>Salida: <span>{2}</span><br/>Manual: <span>{3}</span></div>",
                                  entrega.Programado.ToDisplayDateTime().ToString("HH:mm"),
                                  (entrega.Entrada.HasValue ? entrega.Entrada.Value.ToDisplayDateTime().ToString("HH:mm") : ""),
                                  (entrega.Salida.HasValue ? entrega.Salida.Value.ToDisplayDateTime().ToString("HH:mm") : ""),
                                  (entrega.Manual.HasValue ? entrega.Manual.Value.ToDisplayDateTime().ToString("HH:mm") : ""),
                                  CultureManager.GetLabel(EntregaDistribucion.Estados.GetLabelVariableName(entrega.Estado)),
                                  entrega.PuntoEntrega != null ? entrega.PuntoEntrega.Descripcion : entrega.Linea.Descripcion);

                Programado = entrega.Programado;
                Automatico = entrega.Entrada;
                Estado = entrega.Estado;
                Latitud = entrega.ReferenciaGeografica.Latitude;
                Longitud = entrega.ReferenciaGeografica.Longitude;
            }

            public string Render()
            {
                return string.Format("{{ 'id': '{0}', 'name': '{1}', 'estado': {2}, 'details': '{3}' }}",
                                Id,
                                Descripcion,
                                Estado,
                                PopupInfo().Replace("'", "\\'"));
            }
            private string PopupInfo()
            {
                return string.Format("<div class='popup_title'>{0}</div><div class='popup_detail'>{1}</div>", Descripcion, PopupText);
            }
            private string GetJsDate(DateTime? date)
            {
                return date.HasValue ? string.Concat("new Date", date.Value.ToDisplayDateTime().ToString("(yyyy,M,d,H,m)")) : "null";
            }
        }       
    }
}