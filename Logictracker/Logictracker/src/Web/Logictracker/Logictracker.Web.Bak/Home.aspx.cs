using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker
{
    public partial class Home : SessionSecuredPage
    {
        protected override InfoLabel LblInfo { get { return null; } }

        /*DAOFactory _daoFactory;
         
        protected DAOFactory DaoFactory { get { return _daoFactory ?? (_daoFactory = new DAOFactory()); } }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            gaugeActivos.AbsoluteExpiration = gaugeAverageSpeed.AbsoluteExpiration = gaugeDet1.AbsoluteExpiration = 
            gaugeEntradas.AbsoluteExpiration = gaugeInfracciones.AbsoluteExpiration = gaugeKm.AbsoluteExpiration =
            gaugeMaxSpeed.AbsoluteExpiration = gaugeMovDet.AbsoluteExpiration = gaugeBaseGeocerca.AbsoluteExpiration =
            gaugeMas1Hora.AbsoluteExpiration = gaugeTickets.AbsoluteExpiration = gaugeInactivosBase.AbsoluteExpiration = DateTime.Today.AddDays(-1);
            
            gaugeActivos.SlidingExpiration = gaugeAverageSpeed.SlidingExpiration = gaugeDet1.SlidingExpiration = 
            gaugeEntradas.SlidingExpiration = gaugeInfracciones.SlidingExpiration = gaugeKm.SlidingExpiration = 
            gaugeMaxSpeed.SlidingExpiration = gaugeMovDet.SlidingExpiration = gaugeBaseGeocerca.SlidingExpiration =
            gaugeMas1Hora.SlidingExpiration = gaugeTickets.SlidingExpiration = gaugeInactivosBase.SlidingExpiration = new TimeSpan(0, 0, 1);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var desdeHoy = DateTime.Today.ToUniversalTime();
            var hastaHoy = DateTime.UtcNow;
            var desdeAyer = desdeHoy.AddDays(-1);
            var hastaAyer = hastaHoy.AddDays(-1);
            var totalKilometers = 0.0;
            var totalMovementTime = 0.0;
            var totalStoppedTime = 0.0;
            var detenidoMotorOn = 0.0;
            var detenidoMotorOff = 0.0;
            var maxSpeed = 0.0;
            var stopsHigher1 = 0;
            var stopsHigher15 = 0;
            var mobiles = new List<int>();            
            var tiempoGeoCerca = 0.0;
            var tieneInfraccion = false;
            var infracciones = 0;
            var vehiculosConInfraccion = 0;
            var movilesActivos = 0;
            var movilesEnMovimiento = 0;
            var movilesDetenidos = 0;
            var movilesEnBase = 0;
            var movilesEnGeocerca = 0;
            var movilesEnBaseMas1Hora = 0;
            var movilesEnGeocercaMas1Hora = 0;
            var movilesInactivosBase = 0;
            
            var lineas = DaoFactory.LineaDAO.GetList(new[] { ddlEmpresa.Selected });
            var idPoiBase = lineas.Where(l => l.ReferenciaGeografica != null).Select(l => l.ReferenciaGeografica.Id);

            var codigoEntrada = Convert.ToInt32(MessageCode.InsideGeoRefference.GetMessageCode());
            var codigoSalida = Convert.ToInt32(MessageCode.OutsideGeoRefference.GetMessageCode());

            var entradasGeocerca = ReportFactory.MobileEventDAO.GetMobilesEvents(mobiles, new List<int> { codigoEntrada }, new List<int> { 0 }, desdeAyer, hastaAyer);
            var salidasGeocerca = ReportFactory.MobileEventDAO.GetMobilesEvents(mobiles, new List<int> { codigoSalida }, new List<int> { 0 }, desdeAyer, hastaAyer);
            var entradasBase = entradasGeocerca.Where(g => g.IdPuntoInteres.HasValue && idPoiBase.Contains(g.IdPuntoInteres.Value)).ToList();

            foreach (var entrada in entradasGeocerca)
            {
                for (var i = 0; i < salidasGeocerca.Count; i++)
                {
                    if (entrada.IdPuntoInteres == salidasGeocerca[i].IdPuntoInteres &&
                        entrada.Intern == salidasGeocerca[i].Intern &&
                        entrada.EventTime < salidasGeocerca[i].EventTime)
                    {
                        tiempoGeoCerca += (salidasGeocerca[i].EventTime - entrada.EventTime).TotalMinutes;
                        salidasGeocerca.RemoveAt(i);
                        break;
                    }
                }
            }

            var coches = DaoFactory.CocheDAO.FindByEmpresaYLinea(ddlEmpresa.Selected, -1);

            foreach (var coche in coches)
            {
                mobiles.Add(coche.Id);
                if ((coche.Dispositivo != null && DaoFactory.TicketDAO.GetEnCurso(coche.Dispositivo) != null) || DaoFactory.ViajeDistribucionDAO.GetEnCurso(coche) != null)
                    movilesActivos++;

                var ultimaPos = coche.RetrieveLastPosition();

                if (ultimaPos != null && ultimaPos.Velocidad > 0)
                { 
                    movilesEnMovimiento++;                    
                }
                else
                {
                    movilesDetenidos++;
                    var geoReferencias = new GeoReferenceCache(DaoFactory, coche);
                    var list = geoReferencias.GeoRefferencesStates.Where(p => p.Value.Estado == EstadosVehiculo.Dentro);
                    var entradaEnCerca = ReportFactory.MobileEventDAO.GetMobilesEvents(new List<int> { coche.Id }, new List<int> { codigoEntrada }, new List<int> { 0 }, desdeHoy, hastaHoy);

                    foreach (var item in list)
                    {
                        if (idPoiBase.Contains(item.Key))
                        {
                            movilesEnBase++;
                            if ((coche.Dispositivo == null || DaoFactory.TicketDAO.GetEnCurso(coche.Dispositivo) == null) && DaoFactory.ViajeDistribucionDAO.GetEnCurso(coche) == null)
                                movilesInactivosBase++;

                            if (entradaEnCerca[entradaEnCerca.Count - 1].EventTime < DateTime.UtcNow.AddHours(-1))
                                movilesEnBaseMas1Hora++;
                        }
                        else
                        {
                            movilesEnGeocerca++;
                            if (entradaEnCerca[entradaEnCerca.Count - 1].EventTime < DateTime.UtcNow.AddHours(-1))
                                movilesEnGeocercaMas1Hora++;
                        }
                    }
                }

                foreach (var route in ReportFactory.MobileRoutesDAO.GetMobileRoutes(coche.Id, desdeAyer, hastaAyer))
                {
                    totalKilometers += route.Kilometers;
                    if (route.MaxSpeed > maxSpeed) maxSpeed = route.MaxSpeed;

                    switch (route.VehicleStatus)
                    {
                        case "En Movimiento":
                            totalMovementTime += route.Duration;
                            break;
                        case "Detenido":
                            totalStoppedTime += route.Duration;
                            if (route.Duration > 0.25) // 1/4 de hora = 15 minutos
                            {
                                stopsHigher1++;
                                stopsHigher15++;
                            }
                            else
                            {
                                if (route.Duration > 0.0167)// 1/60 de hora = 1 hora
                                    stopsHigher1++;
                            }

                            switch (route.EngineStatus)
                            {
                                case "Encendido":
                                    detenidoMotorOn += route.Duration;
                                    break;
                                case "Apagado":
                                    detenidoMotorOff += route.Duration;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }

                    if (route.Infractions > 0)
                    {
                        tieneInfraccion = true;
                        infracciones += route.Infractions;
                    }
                }

                if (tieneInfraccion)
                {
                    vehiculosConInfraccion++;
                    tieneInfraccion = false;
                }
            }

            var tickets = DaoFactory.TicketDAO.FindAllByCamion(coches.Select(t => t.Id).ToList(), desdeAyer, hastaAyer).Select(t => t.Estado == 9).Count();

            var averageSpeed = totalKilometers > 0 && totalMovementTime > 0 ? totalKilometers / totalMovementTime : 0;
            var averageTimeGeofence = tiempoGeoCerca > 0 && entradasGeocerca.Count > 0 ? tiempoGeoCerca / entradasGeocerca.Count : 0;
            
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            lblActiva.Text = DaoFactory.CocheDAO.FindByEmpresaLineaYEstadoVehiculo(ddlEmpresa.Selected, -1, 0).Count.ToString();
            lblInactiva.Text = DaoFactory.CocheDAO.FindByEmpresaLineaYEstadoVehiculo(ddlEmpresa.Selected, -1, 2).Count.ToString();
            lblMantenimiento.Text = DaoFactory.CocheDAO.FindByEmpresaLineaYEstadoVehiculo(ddlEmpresa.Selected, -1, 1).Count.ToString();
            lblHora.Text = DateTime.Now.ToString("HH:mm:ss");
            lblActivosAhora.Text = movilesActivos.ToString();
            lblInactivosBase.Text = movilesInactivosBase.ToString();
            lblMovimiento.Text = String.Format("{0:HH:mm:ss}", new DateTime(2000, 1, 1, TimeSpan.FromHours(totalMovementTime / coches.Count).Hours, TimeSpan.FromHours(totalMovementTime / coches.Count).Minutes, TimeSpan.FromHours(totalMovementTime / coches.Count).Seconds));
            lblDetencion.Text = String.Format("{0:HH:mm:ss}", new DateTime(2000, 1, 1, TimeSpan.FromHours(totalStoppedTime / coches.Count).Hours, TimeSpan.FromHours(totalStoppedTime / coches.Count).Minutes, TimeSpan.FromHours(totalStoppedTime / coches.Count).Seconds));
            lblMayor15.Text = stopsHigher15.ToString();
            lblMayor1.Text = stopsHigher1.ToString();

            lblInfracciones.Text = infracciones.ToString();
            lblVelPromedio.Text = string.Format("{0:0.00}km/h", averageSpeed);
            lblVelMax.Text = string.Format("{0:0.00}km/h", maxSpeed);
            lblInfVehiculos.Text = vehiculosConInfraccion.ToString();
            lblKmRecorridos.Text = string.Format("{0:0.00}km", totalKilometers / coches.Count);
            lblEntradasBase.Text = entradasBase.Count.ToString();
            lblEntradasGeocerca.Text = (entradasGeocerca.Count - entradasBase.Count).ToString();
            lblPromedioGeocerca.Text = string.Format("{0:0}min", averageTimeGeofence);
            lblDetenidoOn.Text = string.Format("{0:0}min", detenidoMotorOn * 60);
            lblDetenidoOff.Text = string.Format("{0:0}min", detenidoMotorOff * 60);
            lblEnMovimiento.Text = movilesEnMovimiento.ToString();
            lblDetenidos.Text = movilesDetenidos.ToString();
            lblEnBase.Text = movilesEnBase.ToString();
            lblEnGeocerca.Text = movilesEnGeocerca.ToString();
            lblEnBaseMas1Hora.Text = movilesEnBaseMas1Hora.ToString();
            lblEnGeocercaMas1Hora.Text = movilesEnGeocercaMas1Hora.ToString();
            lblTickets.Text = tickets.ToString();
            
            var maxStop1 = (Convert.ToInt32(stopsHigher1 / 50) + 1) * 50;
            gaugeDet1.Gauges[gaugeDet1.Gauges.IndexOf("linearGaugeDet1")].Maximum = maxStop1;
            gaugeDet1.Gauges[gaugeDet1.Gauges.IndexOf("linearGaugeDet1")].Pointer.Value = stopsHigher1;
            gaugeDet1.Gauges[gaugeDet1.Gauges.IndexOf("linearGaugeDet1")].MorePointers[0].Value = stopsHigher15;

            var maxVel = (Convert.ToInt32(averageSpeed / 20) + 1) * 20;
            gaugeAverageSpeed.Gauges[gaugeAverageSpeed.Gauges.IndexOf(("radialGaugeAverageSpeed"))].Maximum = maxVel > 120 ? maxVel : 120;
            ((C1GaugeLabels)gaugeAverageSpeed.Gauges[gaugeAverageSpeed.Gauges.IndexOf(("radialGaugeAverageSpeed"))].Decorators[3]).Interval = maxVel > 120 ? 20 : 10;
            gaugeAverageSpeed.Gauges[gaugeAverageSpeed.Gauges.IndexOf(("radialGaugeAverageSpeed"))].Pointer.Value = averageSpeed;

            maxVel = (Convert.ToInt32(maxSpeed / 20) + 1) * 20;
            gaugeMaxSpeed.Gauges[gaugeMaxSpeed.Gauges.IndexOf(("radialGaugeMaxSpeed"))].Maximum = maxVel > 120 ? maxVel : 120;
            ((C1GaugeLabels)gaugeMaxSpeed.Gauges[gaugeMaxSpeed.Gauges.IndexOf(("radialGaugeMaxSpeed"))].Decorators[3]).Interval = maxVel > 120 ? 20 : 10;
            gaugeMaxSpeed.Gauges[gaugeMaxSpeed.Gauges.IndexOf(("radialGaugeMaxSpeed"))].Pointer.Value = maxSpeed;

            var maxInf = (Convert.ToInt32(infracciones / 10) + 1) * 10;
            gaugeInfracciones.Gauges[gaugeInfracciones.Gauges.IndexOf("linearGaugeInfracciones")].Maximum = maxInf;
            gaugeInfracciones.Gauges[gaugeInfracciones.Gauges.IndexOf("linearGaugeInfracciones")].Pointer.Value = infracciones;
            gaugeInfracciones.Gauges[gaugeInfracciones.Gauges.IndexOf("linearGaugeInfracciones")].MorePointers[0].Value = vehiculosConInfraccion;

            var maxKm = (Convert.ToInt32(totalKilometers / coches.Count / 100) + 1) * 100;
            gaugeKm.Gauges[gaugeKm.Gauges.IndexOf("linearGaugeKm")].Maximum = maxKm;
            gaugeKm.Gauges[gaugeKm.Gauges.IndexOf("linearGaugeKm")].Pointer.Value = totalKilometers / coches.Count;

            var maxEnt = entradasGeocerca.Count > entradasBase.Count ? (Convert.ToInt32(entradasGeocerca.Count / 10) + 1) * 10 
                                                                     : (Convert.ToInt32(entradasBase.Count / 10) + 1) * 10;
            gaugeEntradas.Gauges[gaugeEntradas.Gauges.IndexOf("linearGaugeEntradas")].Maximum = maxEnt;
            gaugeEntradas.Gauges[gaugeEntradas.Gauges.IndexOf("linearGaugeEntradas")].Pointer.Value = (entradasGeocerca.Count - entradasBase.Count);
            gaugeEntradas.Gauges[gaugeEntradas.Gauges.IndexOf("linearGaugeEntradas")].MorePointers[0].Value = entradasBase.Count;

            var maxAct = (Convert.ToInt32(movilesActivos / 10) + 1) * 10;
            gaugeActivos.Gauges[gaugeActivos.Gauges.IndexOf("linearGaugeActivos")].Maximum = maxAct;
            gaugeActivos.Gauges[gaugeActivos.Gauges.IndexOf("linearGaugeActivos")].Pointer.Value = movilesActivos;

            var maxInacBase = (Convert.ToInt32(movilesInactivosBase / 10) + 1) * 10;
            gaugeInactivosBase.Gauges[gaugeInactivosBase.Gauges.IndexOf("linearGaugeInactivosBase")].Maximum = maxInacBase;
            gaugeInactivosBase.Gauges[gaugeInactivosBase.Gauges.IndexOf("linearGaugeInactivosBase")].Pointer.Value = movilesInactivosBase;

            var maxMovDet = movilesEnMovimiento > movilesDetenidos ? (Convert.ToInt32(movilesEnMovimiento / 10) + 1) * 10 
                                                                   : (Convert.ToInt32(movilesDetenidos / 10) + 1) * 10;
            gaugeMovDet.Gauges[gaugeMovDet.Gauges.IndexOf("linearGaugeMovDet")].Maximum = maxMovDet;
            gaugeMovDet.Gauges[gaugeMovDet.Gauges.IndexOf("linearGaugeMovDet")].Pointer.Value = movilesEnMovimiento;
            gaugeMovDet.Gauges[gaugeMovDet.Gauges.IndexOf("linearGaugeMovDet")].MorePointers[0].Value = movilesDetenidos;

            var maxBaseGeo = movilesEnBase > movilesEnGeocerca ? (Convert.ToInt32(movilesEnBase / 10) + 1) * 10
                                                               : (Convert.ToInt32(movilesEnGeocerca / 10) + 1) * 10;
            gaugeBaseGeocerca.Gauges[gaugeBaseGeocerca.Gauges.IndexOf("linearGaugeBaseGeocerca")].Maximum = maxBaseGeo;
            gaugeBaseGeocerca.Gauges[gaugeBaseGeocerca.Gauges.IndexOf("linearGaugeBaseGeocerca")].Pointer.Value = movilesEnBase;
            gaugeBaseGeocerca.Gauges[gaugeBaseGeocerca.Gauges.IndexOf("linearGaugeBaseGeocerca")].MorePointers[0].Value = movilesEnGeocerca;

            var maxMas1Hora = movilesEnBaseMas1Hora > movilesEnGeocercaMas1Hora ? (Convert.ToInt32(movilesEnBaseMas1Hora / 10) + 1) * 10
                                                                                : (Convert.ToInt32(movilesEnGeocercaMas1Hora / 10) + 1) * 10;
            gaugeMas1Hora.Gauges[gaugeMas1Hora.Gauges.IndexOf("linearGaugeMas1Hora")].Maximum = maxMas1Hora;
            gaugeMas1Hora.Gauges[gaugeMas1Hora.Gauges.IndexOf("linearGaugeMas1Hora")].Pointer.Value = movilesEnBaseMas1Hora;
            gaugeMas1Hora.Gauges[gaugeMas1Hora.Gauges.IndexOf("linearGaugeMas1Hora")].MorePointers[0].Value = movilesEnGeocercaMas1Hora;

            var maxTickets = (Convert.ToInt32(tickets / 50) + 1) * 50;
            gaugeTickets.Gauges[gaugeTickets.Gauges.IndexOf("linearGaugeTickets")].Maximum = maxTickets;
            gaugeTickets.Gauges[gaugeTickets.Gauges.IndexOf("linearGaugeTickets")].Pointer.Value = tickets;
        }*/

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            var usuario = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            var homepage = usuario.HomePage;
            if(!string.IsNullOrEmpty(homepage)) Response.Redirect(homepage);
        }
    }
}
