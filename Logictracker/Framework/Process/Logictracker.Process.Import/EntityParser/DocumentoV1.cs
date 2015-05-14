using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects.Documentos;
using System.Globalization;
using System;

namespace Logictracker.Process.Import.EntityParser
{
    public class DocumentoV1: EntityParserBase
    {
        protected override string EntityName { get { return "Documento"; } }

        public DocumentoV1() {}
        public DocumentoV1(DAOFactory daoFactory) : base(daoFactory) { }

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            var documento = GetDocumento(empresa, linea, data);
            if (data.Operation == (int)Operation.Delete) return documento;

            if (documento.Id == 0)
            {
                documento.Empresa = DaoFactory.EmpresaDAO.FindById(empresa);
                documento.Linea = linea > 0 ? DaoFactory.LineaDAO.FindById(linea) : null;
                documento.Estado = Documento.Estados.Abierto;
            }

            documento.Descripcion = data.AsString(Properties.Documento.Descripcion, 64);

            var gmt = data.AsInt32(Properties.Documento.Gmt) ?? 0;

            var fecha = data.AsDateTime(Properties.Documento.Fecha, gmt);
            if(!fecha.HasValue) { ThrowProperty("Fecha");}
            documento.Fecha = fecha.Value;

            var presentacion = data.AsDateTime(Properties.Documento.FechaPresentacion, gmt);
            if (documento.TipoDocumento.RequerirPresentacion && !presentacion.HasValue && !documento.Presentacion.HasValue)
            {
                ThrowProperty("FechaPresentacion");
            }
            if (presentacion.HasValue)
            {
                documento.Presentacion = presentacion.Value;
            }

            var vencimiento = data.AsDateTime(Properties.Documento.FechaVencimiento, gmt);
            if (documento.TipoDocumento.RequerirVencimiento && !vencimiento.HasValue && !documento.Vencimiento.HasValue)
            {
                ThrowProperty("FechaVencimiento");
            }
            if (vencimiento.HasValue)
            {
                documento.Vencimiento = vencimiento.Value;
            }

            var cierre = data.AsDateTime(Properties.Documento.FechaCierre, gmt);
            if(cierre.HasValue)
            {
                documento.FechaCierre = cierre.Value;
                documento.Estado = Documento.Estados.Cerrado;
            }
            if (documento.TipoDocumento.AplicarATransportista)
            {
                var codigoTransportista = data.AsString(Properties.Documento.Transportista, 32);
                if (codigoTransportista != null)
                {
                    var transportista = DaoFactory.TransportistaDAO.FindByCodigo(empresa, linea, codigoTransportista);
                    documento.Transportista = transportista;
                }
                if (documento.Transportista == null)
                {
                    ThrowProperty("Transportista");
                }
            }
            if(documento.TipoDocumento.AplicarAVehiculo)
            {
                var interno = data.AsString(Properties.Documento.Vehiculo, 32);
                if (interno != null)
                {
                    var vehiculo = DaoFactory.CocheDAO.FindByInterno(new[] {empresa}, new[] {linea}, interno);
                    if (vehiculo != null && documento.TipoDocumento.AplicarATransportista)
                    {
                        if(vehiculo.Transportista != documento.Transportista)
                        {
                            ThrowMessage("El vehículo {0} no pertenece al transportista {1}", vehiculo.Interno, documento.Transportista.Descripcion);   
                        }
                    }
                                            

                    documento.Vehiculo = vehiculo;
                }
                if (documento.Vehiculo == null)
                {
                    ThrowProperty("Vehiculo");
                }
            } 
            if (documento.TipoDocumento.AplicarAEmpleado)
            {
                var legajo = data.AsString(Properties.Documento.Empleado, 32);
                if (legajo != null)
                {
                    var empleado = DaoFactory.EmpleadoDAO.FindByLegajo(empresa, linea, legajo);
                    documento.Empleado = empleado;
                }
                if (documento.Empleado == null)
                {
                    ThrowProperty("Empleado");
                }
            }
            if (documento.TipoDocumento.AplicarAEquipo)
            {
                var codigoEquipo = data.AsString(Properties.Documento.Equipo, 32);
                if (codigoEquipo != null)
                {
                    var equipo = DaoFactory.EquipoDAO.FindByCodigo(empresa, linea, codigoEquipo);
                    documento.Equipo = equipo;
                }
                if (documento.Equipo == null)
                {
                    ThrowProperty("Equipo");
                }
            }

            var custom = data.GetCustomFields();
            if(custom.Any())
            {
                var parametrosTipo = documento.TipoDocumento.Parametros.Cast<TipoDocumentoParametro>().ToList();
                var parametrosDoc = documento.Parametros.Cast<DocumentoValor>().ToList();
                foreach (var par in custom)
                {
                    var repIdx = par.Key.IndexOf("]");
                    var parName = repIdx < 0 ? par.Key : par.Key.Substring(repIdx + 1).Trim();
                    short repNo;
                    if (repIdx < 0 || !short.TryParse(par.Key.Substring(1, repIdx - 1), out repNo)) repNo = 0;
                    var tipoPar = parametrosTipo.FirstOrDefault(x => x.Nombre == parName);
                    if (tipoPar == null) continue;
                    var valor = parametrosDoc.FirstOrDefault(x => x.Parametro.Id == tipoPar.Id && (tipoPar.Repeticion <= 1 || repNo == x.Repeticion));
                    if(valor == null)
                    {
                        valor = new DocumentoValor { Documento = documento, Parametro = tipoPar, Repeticion = repNo };
                        parametrosDoc.Add(valor);
                    }
                                

                    var valorFinal = GetValue(empresa, linea, valor, par.Value);
                    if(valorFinal != null)
                    {
                        valor.Valor = valorFinal;
                    }
                    if (string.IsNullOrEmpty(valor.Valor) && !string.IsNullOrEmpty(valor.Parametro.Default))
                    {
                        valor.Valor = valor.Parametro.Default;
                    }
                    if (valor.Parametro.Obligatorio && string.IsNullOrEmpty(valor.Valor))
                    {
                        ThrowProperty(valor.Parametro.Nombre);
                    }
                }
                documento.Parametros.Clear();
                foreach (var p in parametrosDoc) documento.Parametros.Add(p);
            }

            return documento;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Documento;
            if(ValidateSaveOrUpdate(item)) DaoFactory.DocumentoDAO.SaveOrUpdate(item);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Documento;
            if (ValidateDelete(item)) DaoFactory.DocumentoDAO.Delete(item);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Documento;
            if (ValidateSave(item)) DaoFactory.DocumentoDAO.SaveOrUpdate(item);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as Documento;
            if (ValidateUpdate(item)) DaoFactory.DocumentoDAO.SaveOrUpdate(item);
        }

        #endregion

        protected virtual Documento GetDocumento(int empresa, int linea, IData data)
        {
            var codigo = data.AsString(Properties.Documento.Codigo, 32);
            if (codigo == null) ThrowProperty("Codigo");

            var tipo = data.AsString(Properties.Documento.TipoDocumento, 32);
            if (tipo == null) ThrowProperty("Tipo de Documento");

            var tipoDocumento = DaoFactory.TipoDocumentoDAO.FindByNombre(empresa, linea, tipo);
            if (tipoDocumento == null) ThrowProperty("Tipo de Documento");

            var sameCode = DaoFactory.DocumentoDAO.FindByCodigo(tipoDocumento.Id, codigo);
            return sameCode ?? new Documento{TipoDocumento = tipoDocumento, Codigo = codigo, FechaAlta = DateTime.UtcNow};
        }

        public string GetValue(int empresa, int linea, DocumentoValor detalle, string valor)
        {
            var tipoDato = detalle.Parametro.TipoDato.ToLower();
            switch (tipoDato)
            {
                case TipoParametroDocumento.Integer:
                    int intValue;
                    return int.TryParse(valor, out intValue) ? intValue.ToString(CultureInfo.InvariantCulture) : null;
                case TipoParametroDocumento.Float:
                    float floatValue;
                    return float.TryParse(valor.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out floatValue) ? floatValue.ToString(CultureInfo.InvariantCulture) : null;
                case TipoParametroDocumento.String:
                    return valor;
                case TipoParametroDocumento.DateTime:
                    DateTime dateTimeValue;
                    return DateTime.TryParse(valor, new CultureInfo("es-AR"), DateTimeStyles.None, out dateTimeValue) ? dateTimeValue.ToString(CultureInfo.InvariantCulture) : null;
                case TipoParametroDocumento.Boolean:
                    bool boolValue;
                    return bool.TryParse(valor, out boolValue) ? (boolValue ? "true" : "false") : null;
                case TipoParametroDocumento.Coche:
                    var vehiculo = DaoFactory.CocheDAO.FindByInterno(new[] { empresa }, new[] { linea }, valor);
                    return vehiculo == null ? null : vehiculo.Id.ToString(CultureInfo.InvariantCulture);
                case TipoParametroDocumento.Chofer:
                    var empleado = DaoFactory.EmpleadoDAO.FindByLegajo(empresa, linea, valor);
                    return empleado == null ? null : empleado.Id.ToString(CultureInfo.InvariantCulture);
                case TipoParametroDocumento.Aseguradora:
                    var transportista = DaoFactory.TransportistaDAO.FindByCodigo(empresa, linea, valor);
                    return transportista == null ? null : transportista.Id.ToString(CultureInfo.InvariantCulture);
                case TipoParametroDocumento.Equipo:
                    var equipo = DaoFactory.EquipoDAO.FindByCodigo(empresa, linea, valor);
                    return equipo == null ? null : equipo.Id.ToString(CultureInfo.InvariantCulture);
                case TipoParametroDocumento.Cliente:
                    var cliente = DaoFactory.ClienteDAO.FindByCode(new[]{empresa}, new[]{linea}, valor);
                    return cliente == null ? null : cliente.Id.ToString(CultureInfo.InvariantCulture);
                case TipoParametroDocumento.Tanque:
                    var tanque = DaoFactory.TanqueDAO.FindByCode(valor);
                    return tanque == null ? null : tanque.Id.ToString(CultureInfo.InvariantCulture);
                case TipoParametroDocumento.CentroCostos:
                    var centroCosto = DaoFactory.CentroDeCostosDAO.FindByCodigo(empresa, linea, valor);
                    return centroCosto == null ? null : centroCosto.Id.ToString(CultureInfo.InvariantCulture);
                case TipoParametroDocumento.Image:
                    return null;
            }

            return null;
        }
    }
}

        
