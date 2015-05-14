using System;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Process.Import.EntityParser
{
    public class DistribucionBV1 : EntityParserBase
    {
        // BRINKS
        protected override string EntityName { get { return "DistribucionB"; } }

        public DistribucionBV1() {}

        public DistribucionBV1(DAOFactory daoFactory) : base(daoFactory) { }

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var oEmpresa = DaoFactory.EmpresaDAO.FindById(empresa);
            var oLinea = DaoFactory.LineaDAO.FindById(linea);
            if (oLinea == null) ThrowProperty("LINEA");
            const int vigencia = 24;

            var codRuta = data[Properties.DistribucionB.CodigoRuta].Trim();
            if (string.IsNullOrEmpty(codRuta)) ThrowProperty("RUTA");
            
            var codEntrega = data[Properties.DistribucionB.CodigoEntrega].Trim();
            if (string.IsNullOrEmpty(codEntrega)) ThrowProperty("ENTREGA");

            var codPuntoEntrega = data[Properties.DistribucionB.CodigoPuntoEntrega].Trim();
            if (string.IsNullOrEmpty(codPuntoEntrega)) ThrowProperty("PUNTO_ENTREGA");

            var estado = data[Properties.DistribucionB.Estado].Trim();

            var fecha = data[Properties.DistribucionB.Fecha].Trim();
            if (string.IsNullOrEmpty(fecha)) ThrowProperty("FECHA");
            if (fecha.Length != 8) ThrowProperty("FECHA");
            
            var hora = data[Properties.DistribucionB.HoraProgramada].Trim();
            if (string.IsNullOrEmpty(hora)) ThrowProperty("HORA");
            if (hora.Length != 8) ThrowProperty("HORA");

            int dia, mes, anio, hr, min, seg;
            if (!int.TryParse(fecha.Substring(0, 4), out anio)) ThrowProperty("AÑO");
            if (!int.TryParse(fecha.Substring(4, 2), out mes)) ThrowProperty("MES");
            if (!int.TryParse(fecha.Substring(6, 2), out dia)) ThrowProperty("DIA");
            if (!int.TryParse(hora.Substring(0, 2), out hr)) ThrowProperty("HORA");
            if (!int.TryParse(hora.Substring(3, 2), out min)) ThrowProperty("MINUTO");
            if (!int.TryParse(hora.Substring(6, 2), out seg)) ThrowProperty("SEGUNDO");

            var gmt = new TimeSpan(-3, 0, 0);
            var date = new DateTime(anio, mes, dia, hr, min, seg).Subtract(gmt);

            var item = GetDistribucion(empresa, linea, codRuta);
            if (data.Operation == (int) Operation.Delete) return item;

            if (!string.IsNullOrEmpty(estado))
            {
                if (item.Id != 0)
                {
                    var ptoEntrega = item.Detalles.FirstOrDefault(d => d.PuntoEntrega != null && d.Descripcion == codEntrega);
                    if (ptoEntrega == null) return item;
                    
                    item.Detalles.Remove(ptoEntrega);

                    DaoFactory.EntregaDistribucionDAO.Delete(ptoEntrega);
                }
                return item;
            }

            if (item.Id == 0)
            {
                Coche vehiculo = null;
                var interno = data[Properties.DistribucionB.Interno].Trim();
                if (!string.IsNullOrEmpty(interno))
                {
                    vehiculo = DaoFactory.CocheDAO.GetByInternoEndsWith(new[] { empresa }, new[] { linea }, interno);
                    if (vehiculo == null)
                    {
                        STrace.Error("Logiclink", string.Format("Interno {0} no encontrado para el viaje: {1}", interno, codRuta));
                    }
                }
                else
                {
                    STrace.Error("Logiclink", "Interno vacío para el viaje: " + codRuta);
                }
                
                item.Empresa = oEmpresa;
                item.Linea = oLinea;
                item.Vehiculo = vehiculo;
                item.Inicio = date;
                item.Fin = date;
                item.Tipo = ViajeDistribucion.Tipos.Desordenado;
                item.RegresoABase = true;
                item.Estado = ViajeDistribucion.Estados.Pendiente;
                item.Alta = DateTime.UtcNow;
                item.NumeroViaje = 1;
            }

            // Entregas
            if (item.Detalles.Count == 0)
            {   // Si no existe, agrego la salida de base
                var origen = new EntregaDistribucion
                                 {
                                     Linea = oLinea,
                                     Descripcion = oLinea.Descripcion,
                                     Estado = EntregaDistribucion.Estados.Pendiente,
                                     Orden = 0,
                                     Programado = date,
                                     ProgramadoHasta = date,
                                     Viaje = item
                                 };
                item.Detalles.Add(origen);

                var llegada = new EntregaDistribucion
                {
                    Linea = oLinea,
                    Descripcion = oLinea.Descripcion,
                    Estado = EntregaDistribucion.Estados.Pendiente,
                    Orden = item.Detalles.Count,
                    Programado = date,
                    ProgramadoHasta = date,
                    Viaje = item
                };
                item.Detalles.Add(llegada);
            }

            TipoServicioCiclo tipoServicio = null;
            var tipoServ = DaoFactory.TipoServicioCicloDAO.FindDefault(new[] {empresa}, new[] {linea});
            if (tipoServ != null && tipoServ.Id > 0) tipoServicio = tipoServ;

            var puntoEntrega = DaoFactory.PuntoEntregaDAO.GetByCode(new[] {empresa}, new[] {linea}, new[] {-1}, codPuntoEntrega);
            if (puntoEntrega == null)
            {
                ThrowProperty("PUNTO_ENTREGA_INEXISTENTE");
            }
            else
            {
                var end = date.AddHours(vigencia);
                puntoEntrega.ReferenciaGeografica.Vigencia.Inicio = date.Date.ToDataBaseDateTime();
                if (puntoEntrega.ReferenciaGeografica.Vigencia.Fin < end)
                    puntoEntrega.ReferenciaGeografica.Vigencia.Fin = end;

                DaoFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(puntoEntrega.ReferenciaGeografica);
                DaoFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);
            }

            var entrega = new EntregaDistribucion
                              {
                                  Cliente = puntoEntrega.Cliente,
                                  PuntoEntrega = puntoEntrega,
                                  Descripcion = codEntrega,
                                  Estado = EntregaDistribucion.Estados.Pendiente,
                                  Orden = item.Detalles.Count - 1,
                                  Programado = date,
                                  ProgramadoHasta = date,
                                  TipoServicio = tipoServicio,
                                  Viaje = item
                              };

            item.Detalles.Add(entrega);

            var maxDate = item.Detalles.Max(d => d.Programado);
            item.Fin = maxDate;

            var ultimo = item.Detalles.Last(e => e.Linea != null);
            if (ultimo.Id > 0)
            {
                ultimo.Programado = maxDate;
                ultimo.ProgramadoHasta = maxDate;
                ultimo.Orden = item.Detalles.Count - 1;
                DaoFactory.EntregaDistribucionDAO.SaveOrUpdate(ultimo);
            }

            return item;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as ViajeDistribucion;
            if (ValidateSaveOrUpdate(tipo)) DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(tipo);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as ViajeDistribucion;
            if (ValidateDelete(tipo)) DaoFactory.ViajeDistribucionDAO.Delete(tipo);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as ViajeDistribucion;
            if (ValidateSave(tipo)) DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(tipo);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var tipo = parsedData as ViajeDistribucion;
            if (ValidateUpdate(tipo)) DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(tipo);
        }

        #endregion

        protected virtual ViajeDistribucion GetDistribucion(int empresa, int linea, string codigo)
        {
            if (string.IsNullOrEmpty(codigo)) ThrowCodigo();

            var sameCode = DaoFactory.ViajeDistribucionDAO.FindByCodigo(empresa, linea, codigo);
            return sameCode ?? new ViajeDistribucion {Codigo = codigo};
        }
    }
}
