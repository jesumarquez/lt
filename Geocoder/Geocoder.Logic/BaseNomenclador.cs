using Geocoder.Core;
using Geocoder.Core.Interfaces;
using Geocoder.Core.VO;
using Geocoder.Data.DAO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Geocoder.Logic
{
	public class BaseNomenclador : MarshalByRefObject, IBaseNomenclador
	{
        protected const string DataBaseConfiguration = "GeocoderConfigPath";
		private DAOFactory _df;
		protected DAOFactory DAOFactory { get { return _df ?? (_df = new DAOFactory(SessionFactoryConfigPath)); } }
		
        protected static bool Activated { get; set; }
		public string SessionFactoryConfigPath { get; set; }
		public BaseNomenclador() : this(null) { }
		public BaseNomenclador(string sessionFactoryConfigPath)
		{
			if (sessionFactoryConfigPath == null)
                sessionFactoryConfigPath = ConfigurationManager.AppSettings["GeocoderConfigPath"];
			if (sessionFactoryConfigPath == null)
				throw new ArgumentNullException("sessionFactoryConfigPath");
			
            SessionFactoryConfigPath = sessionFactoryConfigPath;
			Abreviaturas.SessionFactoryConfigPath = sessionFactoryConfigPath;
			if (!Activated)
			{
                Activated = true;//SoftwareProtection.VerifyKey();
				if (!Activated) throw new ApplicationException("El producto no esta activado");
			}
		}
		public void Dispose()
		{
			_df = null;
			SessionFactoryConfigPath = null;
			GC.Collect();
		}
		public ProvinciaVO[] BuscarProvincias()
		{
			return (from prov in DAOFactory.ProvinciaDAO.GetTodas()
				    select new ProvinciaVO(prov)).ToArray<ProvinciaVO>();
		}
		public IList<PartidoVO> GetPartidos(int idMapProvincia)
		{
			var byProvinciaMapId = DAOFactory.PartidoDAO.GetByProvinciaMapId(idMapProvincia);
			var list = new List<PartidoVO>(byProvinciaMapId.Count);
		    list.AddRange(byProvinciaMapId.Select(current => new PartidoVO(current)));
		    return list;
		}
		public IList<LocalidadVO> GetLocalidades(int idPartido, int idProvincia)
		{
			var byPartido = DAOFactory.LocalidadDAO.GetByPartido(idPartido, idProvincia);
			var list = new List<LocalidadVO>(byPartido.Count);
		    list.AddRange(byPartido.Select(current => new LocalidadVO(current)));
		    return list;
		}
		public IList<PoligonalVO> GetPoligonales(int idPartido, int idProvincia)
		{
			var enPartidoYProvincia = DAOFactory.PoligonalDAO.GetEnPartidoYProvincia(idPartido, idProvincia);
			var list = new List<PoligonalVO>(enPartidoYProvincia.Count);
		    list.AddRange(enPartidoYProvincia.Select(current => new PoligonalVO(current)));
		    return list;
		}
		public IList<PoligonalVO> GetCruces(int idPoligonal, int idMapaUrbano)
		{
			var byCMapId = DAOFactory.PoligonalDAO.GetByCMapId(idMapaUrbano, idPoligonal);
			var cruces = byCMapId.Cruces;
			var list = new List<PoligonalVO>(cruces.Count);
		    list.AddRange(cruces.Select(current => new PoligonalVO(current.Esquina)));
		    return list;
		}
		public DireccionVO GetEsquinaMasCercana(double lat, double lon)
		{
			var esquinaMasCercana = DAOFactory.CruceDAO.GetEsquinaMasCercana(lat, lon);
            return (esquinaMasCercana == null) ? null : new DireccionVO(esquinaMasCercana);
		}
		public DireccionVO GetDireccionMasCercana(double lat, double lon)
		{
			var direccionMasCercana = DAOFactory.AlturaDAO.GetDireccionMasCercana(lat, lon);
			DireccionVO result;
			if (direccionMasCercana == null)
			{
				result = null;
			}
			else
			{
				var num = lat - direccionMasCercana.LatitudInicio;
                var num2 = lon - direccionMasCercana.LongitudInicio;
                var num3 = num * num + num2 * num2;
                var num4 = lat - direccionMasCercana.LatitudFin;
                var num5 = lon - direccionMasCercana.LongitudFin;
                var num6 = num4 * num4 + num5 * num5;
                var num7 = num3 / (num3 + num6);
                var alturaExacta = Convert.ToInt32(Math.Round((direccionMasCercana.AlturaInicio + (direccionMasCercana.AlturaFin - direccionMasCercana.AlturaInicio) * num7) / 10.0) * 10.0);
				result = new DireccionVO(new Direccion(direccionMasCercana, alturaExacta));
			}
			return result;
		}
		public DireccionVO ValidarCruce(int poligonal, int poligonal2, int idMapaUrbano)
		{
			var cruce = DAOFactory.CruceDAO.GetCruce(poligonal, poligonal2, idMapaUrbano);
			return (cruce.Count > 0) ? new DireccionVO(cruce[0]) : null;
		}
		public DireccionVO ValidarAltura(int poligonal, int altura, int idMapaUrbano)
		{
			var altura2 = DAOFactory.AlturaDAO.GetAltura(poligonal, altura, idMapaUrbano);
		    return altura2 == null ? null : new DireccionVO(new Direccion(altura2, altura));
		}
	}
}
