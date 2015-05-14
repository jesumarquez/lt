using System.Linq;
using Geocoder.Core;
using Geocoder.Core.Comparers;
using Geocoder.Core.Interfaces;
using Geocoder.Core.VO;
using Geocoder.Logic.Evaluators;
using System;
using System.Collections.Generic;
using Geocoder.SmartSearch;

namespace Geocoder.Logic
{
	[Serializable]
	public class Nomenclador : BaseNomenclador, INomenclador
	{
		public Nomenclador() { }
		public Nomenclador(string sessionFactoryConfigPath) : base(sessionFactoryConfigPath) { }
		
        public IList<ProvinciaVO> GetProvincias(string nombre)
		{
			var ambos = Tokenizer.FromString(nombre, 0);
			var evaluador = new Evaluador(ambos, new IEvaluator[] { new PuntajeEvaluator()});
			var num = evaluador.Evaluar();
			var todas = DAOFactory.ProvinciaDAO.GetTodas();
			var list = new List<IObjetoPuntuado<Provincia>>();
			foreach (var current in todas)
			{
				var num2 = evaluador.Evaluar(Tokenizer.Create(current));
				if (num2 > num / 2.0)
					list.Add(new ObjetoPuntuado<Provincia>(num2, current));
			}
			list.Sort();
			var list2 = new List<ProvinciaVO>(list.Count);
            list2.AddRange(list.Select(current2 => new ProvinciaVO(current2.Objeto)));
            return list2;
		}
		public IList<PartidoVO> GetPartidoEnProvinciaMapId(string nombre, int idMapProvincia)
		{
			var ambos = Tokenizer.FromString(nombre, 1);
			var evaluador = new Evaluador(ambos, new IEvaluator[] { new PuntajeEvaluator() });
			var num = evaluador.Evaluar();
			var byProvinciaMapId = DAOFactory.PartidoDAO.GetByProvinciaMapId(idMapProvincia);
			var list = new List<IObjetoPuntuado<Partido>>();
			foreach (var current in byProvinciaMapId)
			{
				var num2 = evaluador.Evaluar(Tokenizer.Create(current));
				if (num2 > num / 2.0) list.Add(new ObjetoPuntuado<Partido>(num2, current));
			}
			list.Sort();
			var list2 = new List<PartidoVO>(list.Count);
		    list2.AddRange(list.Select(current2 => new PartidoVO(current2.Objeto)));
		    return list2;
		}
		public IList<PartidoVO> GetPartidoEnProvinciaId(string nombre, int idProvincia)
		{
			var ambos = Tokenizer.FromString(nombre, 1);
			var evaluador = new Evaluador(ambos, new IEvaluator[] { new PuntajeEvaluator() });
			var num = evaluador.Evaluar();
			var byProvinciaId = DAOFactory.PartidoDAO.GetByProvinciaId(idProvincia);
			var list = new List<IObjetoPuntuado<Partido>>();
			foreach (var current in byProvinciaId)
			{
				var num2 = evaluador.Evaluar(Tokenizer.Create(current));
				if (num2 > num / 2.0) list.Add(new ObjetoPuntuado<Partido>(num2, current));
			}
			list.Sort();
			var list2 = new List<PartidoVO>(list.Count);
		    list2.AddRange(list.Select(current2 => new PartidoVO(current2.Objeto)));
		    return list2;
		}
		public IList<DireccionVO> GetDireccion(string calle, int altura, string esquina, string partido, int provincia)
		{
			var flag = string.IsNullOrEmpty(partido.Trim());
			IList<IObjetoPuntuado<Direccion>> list;
			if (altura > 0)
				list = (flag ? GetByAlturaEnProvincia(calle, altura, provincia) : GetByAlturaEnPartidoProvincia(calle, altura, partido, provincia));
			else
				list = (flag ? GetByEsquinaEnProvincia(calle, esquina, provincia) : GetByEsquinaEnPartidoProvincia(calle, esquina, partido, provincia));
			
            var list2 = new List<DireccionVO>(list.Count);
		    list2.AddRange(list.Select(current => new DireccionVO(current.Objeto)));
		    return list2;
		}
		private IList<IObjetoPuntuado<Direccion>> GetByAlturaEnProvincia(string calle, int altura, int provincia)
		{
			var list = new List<IObjetoPuntuado<Direccion>>();
			var tokenizer = Tokenizer.FromString(calle, 3);
			var evaluador = new Evaluador(tokenizer, tokenizer, new IEvaluator[0]);
			evaluador.Evaluators.Add(new PuntajeEvaluator());
			var num = evaluador.Evaluar();
			var list2 = (provincia < 0) ? DAOFactory.PoligonalDAO.GetByAlturaSinProvincia(tokenizer.ToArray(), altura) : DAOFactory.PoligonalDAO.GetByAlturaEnProvincia(tokenizer.ToArray(), altura, provincia);
			foreach (var current in list2)
			{
				evaluador.Option = Tokenizer.Create(current.Poligonal);
				var num2 = evaluador.Evaluar();
				if (num2 > num / 2.0) list.Add(new ObjetoPuntuado<Direccion>(num2, current));
			}
			list.Sort();
			return list;
		}
		private IList<IObjetoPuntuado<Direccion>> GetByAlturaEnPartidoProvincia(string calle, int altura, string partido, int provincia)
		{
			var list = new List<IObjetoPuntuado<Direccion>>();
			var tokenizer = Tokenizer.FromString(calle, 3);
			var tokenizer2 = Tokenizer.FromString(partido, 1);
			var evaluador = new Evaluador(tokenizer, tokenizer, new IEvaluator[0]);
			evaluador.Evaluators.Add(new PuntajeEvaluator());
			var evaluador2 = new Evaluador(tokenizer2, tokenizer2, new IEvaluator[0]);
			evaluador2.Evaluators.Add(new PuntajeEvaluator());
			var num = evaluador.Evaluar();
			var num2 = evaluador2.Evaluar();
			var list2 = (provincia < 0) ? DAOFactory.PoligonalDAO.GetByAlturaSinProvincia(tokenizer.ToArray(), altura) : DAOFactory.PoligonalDAO.GetByAlturaEnProvincia(tokenizer.ToArray(), altura, provincia);
			foreach (var current in list2)
			{
				evaluador.Option = Tokenizer.Create(current.Poligonal);
				evaluador2.Option = Tokenizer.Create(current.Poligonal.Partido);
				var num3 = evaluador.Evaluar();
				var num4 = evaluador2.Evaluar();
				foreach (var current2 in current.Poligonal.Partido.Localidades)
				{
					evaluador2.Option = Tokenizer.Create(current2);
					var num5 = evaluador2.Evaluar();
					if (num5 > num4) num4 = num5; 
				}
				if (num3 > num / 2.0 && num4 > num2 / 2.0)
				{
					list.Add(new ObjetoPuntuado<Direccion>(num3 + num4, current));
				}
			}
			list.Sort();
			return list;
		}
		private IList<IObjetoPuntuado<Direccion>> GetByEsquinaEnProvincia(string calle, string esquina, int provincia)
		{
			var list = new List<IObjetoPuntuado<Direccion>>();
			var tokenizer = Tokenizer.FromString(calle, 3);
			var tokenizer2 = Tokenizer.FromString(esquina, 3);
			var evaluador = new Evaluador(tokenizer, tokenizer, new IEvaluator[0]);
			var evaluador2 = new Evaluador(tokenizer2, tokenizer2, new IEvaluator[0]);
			evaluador.Evaluators.Add(new PuntajeEvaluator());
			evaluador2.Evaluators.Add(new PuntajeEvaluator());
			var num = evaluador.Evaluar();
			var num2 = evaluador2.Evaluar();
			var list2 = (provincia < 0) ? DAOFactory.PoligonalDAO.GetByEsquinaSinProvincia(tokenizer.ToArray(), tokenizer2.ToArray()) : DAOFactory.PoligonalDAO.GetByEsquinaEnProvincia(tokenizer.ToArray(), tokenizer2.ToArray(), provincia);
			foreach (var current in list2)
			{
				evaluador.Option = Tokenizer.Create(current.Poligonal);
				evaluador2.Option = Tokenizer.Create(current.Esquina);
				var num3 = evaluador.Evaluar();
				var num4 = evaluador2.Evaluar();
				if (num3 > num / 2.0 && num4 > num2 / 2.0)
					list.Add(new ObjetoPuntuado<Direccion>(num3 + num4, current));
			}
			list.Sort();
			return list;
		}
		private IList<IObjetoPuntuado<Direccion>> GetByEsquinaEnPartidoProvincia(string calle, string esquina, string partido, int provincia)
		{
			var list = new List<IObjetoPuntuado<Direccion>>();
			var tokenizer = Tokenizer.FromString(calle, 3);
			var tokenizer2 = Tokenizer.FromString(esquina, 3);
			var tokenizer3 = Tokenizer.FromString(partido, 1);
			var evaluador = new Evaluador(tokenizer, tokenizer, new IEvaluator[0]);
            var evaluador2 = new Evaluador(tokenizer2, tokenizer2, new IEvaluator[0]);
            var evaluador3 = new Evaluador(tokenizer3, tokenizer3, new IEvaluator[0]);
			evaluador.Evaluators.Add(new PuntajeEvaluator());
			evaluador2.Evaluators.Add(new PuntajeEvaluator());
			evaluador3.Evaluators.Add(new PuntajeEvaluator());
            var num = evaluador.Evaluar();
            var num2 = evaluador2.Evaluar();
            var num3 = evaluador3.Evaluar();
            var list2 = (provincia < 0) ? DAOFactory.PoligonalDAO.GetByEsquinaSinProvincia(tokenizer.ToArray(), tokenizer2.ToArray()) : DAOFactory.PoligonalDAO.GetByEsquinaEnProvincia(tokenizer.ToArray(), tokenizer2.ToArray(), provincia);
            foreach (var current in list2)
			{
				evaluador.Option = Tokenizer.Create(current.Poligonal);
				evaluador2.Option = Tokenizer.Create(current.Esquina);
				evaluador3.Option = Tokenizer.Create(current.Poligonal.Partido);
                var num4 = evaluador.Evaluar();
                var num5 = evaluador2.Evaluar();
                var num6 = evaluador3.Evaluar();
                foreach (var current2 in current.Poligonal.Partido.Localidades)
				{
					evaluador3.Option = Tokenizer.Create(current2);
                    var num7 = evaluador3.Evaluar();
					if (num7 > num6) num6 = num7;
				}
				if (num4 > num / 2.0 && num5 > num2 / 2.0 && num6 > num3 / 2.0)
				{
					list.Add(new ObjetoPuntuado<Direccion>(num4 + num5 + num6, current));
				}
			}
			list.Sort();
			return list;
		}
		public IList<DireccionVO> GetSmartSearch(string frase)
		{
			return GetSmartSearchLatLon(frase, 1.7976931348623157E+308, 1.7976931348623157E+308);
		}
		public IList<DireccionVO> GetSmartSearchLatLon(string frase, double lat, double lon)
		{
            var parser = new Parser();
            var array = parser.ParseTokens(frase);
            var dictionary = new Dictionary<string, IObjetoPuntuado<Direccion>>();
            var array2 = array;
            for (var i = 0; i < array2.Length; i++)
			{
                var parsedDirection = array2[i];
                var direccionSmartSearch = GetDireccionSmartSearch(parsedDirection.Calle, parsedDirection.Altura, parsedDirection.Esquina, parsedDirection.Localidad, parsedDirection.Provincia, lat, lon);
                foreach (var current in direccionSmartSearch)
				{
                    var key = current.Objeto.Latitud + "," + current.Objeto.Longitud;
					current.Puntaje *= parsedDirection.Probabilidad;
					if (!dictionary.ContainsKey(key))
					{
						dictionary.Add(key, current);
					}
					else
					{
						if (current.Puntaje > dictionary[key].Puntaje)
						{
							dictionary[key] = current;
						}
					}
				}
			}
            var list = new List<IObjetoPuntuado<Direccion>>(dictionary.Values);
			list.Sort(new IObjetoPuntuadoComparer<Direccion>());
            var list2 = new List<DireccionVO>(list.Count);
            foreach (var current2 in list)
			{
				list2.Add(new DireccionVO(current2.Objeto));
			}
			return list2;
		}
		public List<IObjetoPuntuado<Direccion>> GetDireccionSmartSearch(string calle, int altura, string esquina, string partido, string provincia, double lat, double lon)
		{
            var flag = string.IsNullOrEmpty(partido.Trim());
            var flag2 = string.IsNullOrEmpty(provincia.Trim());
            var flag3 = altura > 0;
            var list = new List<IObjetoPuntuado<Direccion>>();
            var list2 = new List<bool>();
            var list3 = new List<bool>();
            var tokenizer = Tokenizer.FromString(calle, 3);
            var tokenizer2 = flag3 ? null : Tokenizer.FromString(esquina, 3);
            var tokenizer3 = flag ? null : Tokenizer.FromString(partido, 1);
            var tokenizer4 = flag2 ? null : Tokenizer.FromString(provincia, 0);
            var evaluador = new Evaluador(tokenizer, tokenizer, new IEvaluator[0]);
            var evaluador2 = flag3 ? null : new Evaluador(tokenizer2, tokenizer2, new IEvaluator[0]);
            var evaluador3 = flag ? null : new Evaluador(tokenizer3, tokenizer3, new IEvaluator[0]);
            var evaluador4 = flag2 ? null : new Evaluador(tokenizer4, tokenizer4, new IEvaluator[0]);
			evaluador.Evaluators.Add(new PuntajeEvaluator());
			if (!flag3)
			{
				evaluador2.Evaluators.Add(new PuntajeEvaluator());
			}
			if (!flag)
			{
				evaluador3.Evaluators.Add(new PuntajeEvaluator());
			}
			if (!flag2)
			{
				evaluador4.Evaluators.Add(new PuntajeEvaluator());
			}
            var num = evaluador.Evaluar();
            var num2 = flag3 ? 0.0 : evaluador2.Evaluar();
            var num3 = flag ? 0.0 : evaluador3.Evaluar();
            var num4 = flag2 ? 0.0 : evaluador4.Evaluar();
            var direccion = DAOFactory.PoligonalDAO.GetDireccion(tokenizer.ToArray(), altura, flag3 ? null : tokenizer2.ToArray(), flag ? null : tokenizer3.ToArray(), flag2 ? null : tokenizer4.ToArray(), lat, lon);
            foreach (var current in direccion)
			{
				evaluador.Option = Tokenizer.Create(current.Poligonal);
				if (!flag3)
				{
					evaluador2.Option = Tokenizer.Create(current.Esquina);
				}
				if (!flag)
				{
					evaluador3.Option = Tokenizer.Create(current.Poligonal.Partido);
				}
				if (!flag2)
				{
					evaluador4.Option = Tokenizer.Create(current.Poligonal.Partido.Provincia);
				}
                var num5 = evaluador.Evaluar();
                var num6 = flag3 ? 0.0 : evaluador2.Evaluar();
                var num7 = flag ? 0.0 : evaluador3.Evaluar();
                var num8 = flag2 ? 0.0 : evaluador4.Evaluar();
				if (!flag)
				{
                    foreach (var current2 in current.Poligonal.Partido.Localidades)
					{
						evaluador3.Option = Tokenizer.Create(current2);
                        var num9 = evaluador3.Evaluar();
						if (num9 > num7) num7 = num9;
					}
				}
				if (num5 > num / 2.0 && (flag3 || num6 > num2 / 2.0) && (flag || num7 > num3 / 2.0) && (flag2 || num8 > num4 / 2.0))
				{
					list.Add(new ObjetoPuntuado<Direccion>(num5 + num6 + num7 + num8, current));
					list2.Add(num4 > 0.0 && num8 == num4);
					list3.Add(num3 > 0.0 && num7 == num3);
				}
			}
			return list;
		}
	}
}
