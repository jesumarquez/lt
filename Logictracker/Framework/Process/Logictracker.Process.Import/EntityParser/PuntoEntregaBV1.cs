using System;
using System.Drawing;
using System.Globalization;
using Logictracker.DAL.Factories;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;

namespace Logictracker.Process.Import.EntityParser
{
    public class PuntoEntregaBV1: EntityParserBase
    {
        // BRINKS
        protected override string EntityName { get { return "Punto de Entrega"; } }
        
        public PuntoEntregaBV1() { }

        public PuntoEntregaBV1(DAOFactory daoFactory) : base(daoFactory) { }

        #region IEntityParser Members

        public override object Parse(int empresa, int linea, IData data)
        {
            /*
            CodigoCliente;NombreCliente;CodigoParada;NombreParada;EstadoParada;TipoParada;Latitud;Longitud

            80;BRINKS ARGENTINA;00000;BRINKS RABANAL;;P;-34.66493700;-58.43795400;

            Observaciones:
            EstadoParada: vacio=activa; I=Inactiva
            TipoParada: P=Plata; O=Operativa; C=Cliente
            */
            var oEmpresa = DaoFactory.EmpresaDAO.FindById(empresa);
            var oLinea = DaoFactory.LineaDAO.FindById(linea);
            if (oLinea == null) ThrowProperty("LINEA");

            var codigoCliente = data[Properties.PuntoEntregaB.CodigoCliente].Trim();
            if (string.IsNullOrEmpty(codigoCliente)) ThrowProperty("CODIGO_CLIENTE");

            var descCliente = data[Properties.PuntoEntregaB.NombreCliente].Trim();
            if (string.IsNullOrEmpty(descCliente)) ThrowProperty("DESCRIPCION_CLIENTE");

            var codigoPuntoEntrega = data[Properties.PuntoEntregaB.CodigoPuntoEntrega].Trim();
            if (string.IsNullOrEmpty(codigoPuntoEntrega)) ThrowProperty("CODIGO_PUNTO_ENTREGA");

            var descPuntoEntrega = data[Properties.PuntoEntregaB.NombrePuntoEntrega].Trim();
            if (string.IsNullOrEmpty(descPuntoEntrega)) ThrowProperty("DESCRIPCION_PUNTO_ENTREGA");

            var tipo = data[Properties.PuntoEntregaB.Tipo].Trim();
            if (string.IsNullOrEmpty(tipo)) ThrowProperty("TIPO_ENTREGA");
            
            var latitud = data[Properties.PuntoEntregaB.Latitud].Trim();
            if (string.IsNullOrEmpty(latitud)) ThrowProperty("LATITUD");
            
            var longitud = data[Properties.PuntoEntregaB.Longitud].Trim();
            if (string.IsNullOrEmpty(longitud)) ThrowProperty("LONGITUD");

            latitud = latitud.Replace(',', '.');
            longitud = longitud.Replace(',', '.');
            var lat = Convert.ToDouble(latitud, CultureInfo.InvariantCulture);
            var lon = Convert.ToDouble(longitud, CultureInfo.InvariantCulture);
            
            var oCliente = DaoFactory.ClienteDAO.FindByCode(new[] { empresa }, new[] { linea }, codigoCliente);
            if (oCliente == null)
            {
                var referenciaGeografica = GetNewGeoRef(oEmpresa, oLinea, codigoCliente, descCliente, lat, lon, tipo);

                oCliente = new Cliente
                               {
                                   Codigo = codigoCliente,
                                   Descripcion = descCliente,
                                   DescripcionCorta = descCliente.Truncate(17),
                                   Empresa = oEmpresa,
                                   Linea = oLinea,
                                   ReferenciaGeografica = referenciaGeografica,
                                   Nomenclado = true
                               };

                DaoFactory.ClienteDAO.SaveOrUpdate(oCliente);
            }

            var oPuntoEntrega = DaoFactory.PuntoEntregaDAO.FindByCode(new[] {empresa}, new[] {linea}, new[] {oCliente.Id}, codigoPuntoEntrega);
            if (oPuntoEntrega == null)
            {
                var referenciaGeografica = GetNewGeoRef(oEmpresa, oLinea, codigoPuntoEntrega, descPuntoEntrega, lat, lon, tipo);

                oPuntoEntrega = new PuntoEntrega
                                    {
                                        Cliente = oCliente,
                                        Codigo = codigoPuntoEntrega,
                                        Descripcion = descPuntoEntrega,
                                        Nombre = descPuntoEntrega,
                                        ReferenciaGeografica = referenciaGeografica,
                                        Nomenclado = true
                                    };
            }
            else if (!oPuntoEntrega.ReferenciaGeografica.IgnoraLogiclink && (oPuntoEntrega.ReferenciaGeografica.Latitude != lat || oPuntoEntrega.ReferenciaGeografica.Longitude != lon))
            {
                oPuntoEntrega.ReferenciaGeografica.Direccion.Vigencia.Fin = DateTime.UtcNow;
                oPuntoEntrega.ReferenciaGeografica.Poligono.Vigencia.Fin = DateTime.UtcNow;

                var posicion = GetNewDireccion(lat, lon);
                var poligono = new Poligono { Radio = 50, Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
                poligono.AddPoints(new[] { new PointF((float)lon, (float)lat) });

                oPuntoEntrega.ReferenciaGeografica.AddHistoria(posicion, poligono, DateTime.UtcNow);
                oPuntoEntrega.ReferenciaGeografica.Vigencia.Fin = DateTime.UtcNow;

                DaoFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(oPuntoEntrega.ReferenciaGeografica);
            }
            
            return oPuntoEntrega;
        }

        private ReferenciaGeografica GetNewGeoRef(Empresa empresa, Linea linea, string codigo, string descripcion, double latitud, double longitud, string codigoTipo)
        {
            var tipo = DaoFactory.TipoReferenciaGeograficaDAO.FindByCodigo(new[] {empresa.Id}, new[] {linea != null ? linea.Id : -1}, codigoTipo);

            if (tipo == null) ThrowProperty("TIPO_PUNTO_ENTREGA");

            var puntoDeInteres = new ReferenciaGeografica
            {
                Codigo = codigo,
                Descripcion = descripcion,
                Empresa = empresa,
                Linea = linea,
                TipoReferenciaGeografica = tipo,
                Vigencia = new Vigencia { Inicio = DateTime.UtcNow, Fin = DateTime.UtcNow },
                Icono = tipo.Icono
            };

            var posicion = GetNewDireccion(latitud, longitud);

            var poligono = new Poligono { Radio = 50, Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
            poligono.AddPoints(new[] { new PointF((float)longitud, (float)latitud) });

            puntoDeInteres.Historia.Add(new HistoriaGeoRef
                                        {
                                            ReferenciaGeografica = puntoDeInteres,
                                            Direccion = posicion,
                                            Poligono = poligono,
                                            Vigencia = new Vigencia { Inicio = DateTime.UtcNow }
                                        });

            DaoFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(puntoDeInteres);

            return puntoDeInteres;
        }

        public override void SaveOrUpdate(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as PuntoEntrega;
            if (ValidateSaveOrUpdate(item)) DaoFactory.PuntoEntregaDAO.SaveOrUpdate(item);
        }

        public override void Delete(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as PuntoEntrega;
            if (ValidateDelete(item)) DaoFactory.PuntoEntregaDAO.Delete(item);
        }

        public override void Save(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as PuntoEntrega;
            if (ValidateSave(item)) DaoFactory.PuntoEntregaDAO.SaveOrUpdate(item);
        }

        public override void Update(object parsedData, int empresa, int linea, IData data)
        {
            var item = parsedData as PuntoEntrega;
            if (ValidateUpdate(item)) DaoFactory.PuntoEntregaDAO.SaveOrUpdate(item);
        }

        #endregion

        private static Direccion GetNewDireccion(double latitud, double longitud)
        {
            return new Direccion
            {
                Altura = -1,
                IdMapa = -1,
                Provincia = string.Empty,
                IdCalle = -1,
                IdEsquina = -1,
                IdEntrecalle = -1,
                Latitud = latitud,
                Longitud = longitud,
                Partido = string.Empty,
                Pais = string.Empty,
                Calle = string.Empty,
                Descripcion = string.Format("({0}, {1})", latitud.ToString(CultureInfo.InvariantCulture), longitud.ToString(CultureInfo.InvariantCulture)),
                Vigencia = new Vigencia { Inicio = DateTime.UtcNow }
            };
        }

        protected virtual PuntoEntrega GetPuntoEntrega(int empresa, int linea, int cliente, IData data)
        {
            var codigo = data[Properties.PuntoEntrega.Codigo];           
            if (codigo == null) ThrowCodigo();

            var sameCode = DaoFactory.PuntoEntregaDAO.FindByCode(new[] { empresa }, new[] { linea }, new[] { cliente }, codigo);
            return sameCode ?? new PuntoEntrega();
        }
    }

}
