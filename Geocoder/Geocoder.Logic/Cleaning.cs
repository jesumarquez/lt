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
	public class Cleaning : BaseNomenclador, ICleaning, IBaseNomenclador, IDisposable
	{
		private const int DICTIONARY_SIZE = 500;
		private static readonly Dictionary<string, IList<Provincia>> provinciasNomencladas = new Dictionary<string, IList<Provincia>>();
		private static readonly Dictionary<string, IList<Partido>> partidosNomenclados = new Dictionary<string, IList<Partido>>();
		private static readonly Dictionary<string, IList<Poligonal>> poligonalesNomencladas = new Dictionary<string, IList<Poligonal>>();
		public Cleaning()
		{
		}
		public Cleaning(string sessionFactoryConfigPath) : base(sessionFactoryConfigPath)
		{
		}
		public static void ClearCache()
		{
			Cleaning.provinciasNomencladas.Clear();
			Cleaning.partidosNomenclados.Clear();
			Cleaning.poligonalesNomencladas.Clear();
		}
		public IList<ProvinciaVO> NomenclarProvincia(string provincia)
		{
			IList<Provincia> list = this.NomenclarProvinciaI(provincia);
			List<ProvinciaVO> list2 = new List<ProvinciaVO>(list.Count);
			foreach (Provincia current in list)
			{
				list2.Add(new ProvinciaVO(current));
			}
			return list2;
		}
		private IList<Provincia> NomenclarProvinciaI(string provincia)
		{
			IList<Provincia> result;
			if (Cleaning.provinciasNomencladas.ContainsKey(provincia))
			{
				result = Cleaning.provinciasNomencladas[provincia];
			}
			else
			{
				Tokenizer tokenizer = Tokenizer.FromString(provincia, 0);
				IList<Provincia> provincias = base.DAOFactory.ProvinciaDAO.GetProvincias(tokenizer.ToArray());
				IList<Provincia> list = Cleaning.Nomenclar<Provincia>(tokenizer, provincias);
				Cleaning.provinciasNomencladas.Add(provincia, list);
				if (Cleaning.provinciasNomencladas.Count > 500)
				{
					using (Dictionary<string, IList<Provincia>>.Enumerator enumerator = Cleaning.provinciasNomencladas.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							KeyValuePair<string, IList<Provincia>> current = enumerator.Current;
							Cleaning.provinciasNomencladas.Remove(current.Key);
						}
					}
				}
				result = list;
			}
			return result;
		}
		public IList<PartidoVO> NomenclarPartido(string partido, int provincia)
		{
			IList<Partido> list = this.NomenclarPartidoI(partido, provincia);
			List<PartidoVO> list2 = new List<PartidoVO>(list.Count);
			foreach (Partido current in list)
			{
				list2.Add(new PartidoVO(current));
			}
			return list2;
		}
		private IList<Partido> NomenclarPartidoI(string partido, int provincia)
		{
			string key = provincia + "||" + partido;
			IList<Partido> result;
			if (Cleaning.partidosNomenclados.ContainsKey(key))
			{
				result = Cleaning.partidosNomenclados[key];
			}
			else
			{
				Tokenizer tokenizer = Tokenizer.FromString(partido, 1);
				IList<Partido> enProvincia = base.DAOFactory.PartidoDAO.GetEnProvincia(tokenizer.ToArray(), provincia);
				IList<Localidad> byProvincia = base.DAOFactory.LocalidadDAO.GetByProvincia(tokenizer.ToArray(), provincia);
				double num = 0.0;
				IList<Partido> list = Cleaning.Nomenclar<Partido>(tokenizer, enProvincia, out num);
				double num2 = 0.0;
				IList<Localidad> list2 = Cleaning.Nomenclar<Localidad>(tokenizer, byProvincia, out num2);
				bool flag = list.Count > 0;
				bool flag2 = list2.Count > 0;
				bool flag3 = num > num2;
				bool flag4 = num2 > num;
				bool flag5 = Math.Abs(num2 - num) < 0.01;
				if (flag && flag2 && flag5)
				{
					List<int> list3 = new List<int>(list.Count);
					foreach (Partido current in list)
					{
						list3.Add(current.Id);
					}
					foreach (Localidad current2 in list2)
					{
						if (!list3.Contains(current2.Partido.Id))
						{
							list.Add(current2.Partido);
						}
					}
				}
				else
				{
					if (flag2 && !flag3)
					{
						if (!flag || flag4)
						{
							list.Clear();
							List<int> list3 = new List<int>(list2.Count);
							foreach (Localidad current2 in list2)
							{
								if (!list3.Contains(current2.Partido.Id))
								{
									list3.Add(current2.Partido.Id);
									list.Add(current2.Partido);
								}
							}
						}
					}
				}
				Cleaning.partidosNomenclados.Add(key, list);
				if (Cleaning.partidosNomenclados.Count > 500)
				{
					using (Dictionary<string, IList<Partido>>.Enumerator enumerator3 = Cleaning.partidosNomenclados.GetEnumerator())
					{
						if (enumerator3.MoveNext())
						{
							KeyValuePair<string, IList<Partido>> current3 = enumerator3.Current;
							Cleaning.partidosNomenclados.Remove(current3.Key);
						}
					}
				}
				result = list;
			}
			return result;
		}
		public IList<PoligonalVO> NomenclarPoligonal(string poligonal, int partido, int provincia)
		{
			IList<Poligonal> list = this.NomenclarPoligonalI(poligonal, partido, provincia);
			List<PoligonalVO> list2 = new List<PoligonalVO>(list.Count);
			foreach (Poligonal current in list)
			{
				list2.Add(new PoligonalVO(current));
			}
			return list2;
		}
		private IList<Poligonal> NomenclarPoligonalI(string poligonal, int partido, int provincia)
		{
			string key = string.Concat(new object[]
			{
				provincia,
				"||",
				partido,
				"||",
				poligonal
			});
			IList<Poligonal> result;
			if (Cleaning.poligonalesNomencladas.ContainsKey(key))
			{
				result = Cleaning.poligonalesNomencladas[key];
			}
			else
			{
				Tokenizer tokenizer = Tokenizer.FromString(poligonal, 3);
				IList<Poligonal> enPartidoYProvincia = base.DAOFactory.PoligonalDAO.GetEnPartidoYProvincia(tokenizer.ToArray(), partido, provincia);
				IList<Poligonal> list = Cleaning.Nomenclar<Poligonal>(tokenizer, enPartidoYProvincia);
				Cleaning.poligonalesNomencladas.Add(key, list);
				if (Cleaning.poligonalesNomencladas.Count > 500)
				{
					using (Dictionary<string, IList<Poligonal>>.Enumerator enumerator = Cleaning.poligonalesNomencladas.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							KeyValuePair<string, IList<Poligonal>> current = enumerator.Current;
							Cleaning.poligonalesNomencladas.Remove(current.Key);
						}
					}
				}
				result = list;
			}
			return result;
		}
		public IList<PoligonalVO> NomenclarPoligonal(string poligonal, int partido, int provincia, int altura)
		{
			IList<Poligonal> list = this.NomenclarPoligonalI(poligonal, partido, provincia, altura);
			List<PoligonalVO> list2 = new List<PoligonalVO>(list.Count);
			foreach (Poligonal current in list)
			{
				list2.Add(new PoligonalVO(current));
			}
			return list2;
		}
		private IList<Poligonal> NomenclarPoligonalI(string poligonal, int partido, int provincia, int altura)
		{
			Tokenizer tokenizer = Tokenizer.FromString(poligonal, 3);
			IList<Poligonal> enPartidoYProvincia = base.DAOFactory.PoligonalDAO.GetEnPartidoYProvincia(tokenizer.ToArray(), partido, provincia, altura);
			return Cleaning.Nomenclar<Poligonal>(tokenizer, enPartidoYProvincia);
		}
		public IList<PoligonalVO> NomenclarPoligonal(string poligonal, int provincia)
		{
			IList<Poligonal> list = this.NomenclarPoligonalI(poligonal, provincia);
			List<PoligonalVO> list2 = new List<PoligonalVO>(list.Count);
			foreach (Poligonal current in list)
			{
				list2.Add(new PoligonalVO(current));
			}
			return list2;
		}
		private IList<Poligonal> NomenclarPoligonalI(string poligonal, int provincia)
		{
			string key = provincia + "||-||" + poligonal;
			IList<Poligonal> result;
			if (Cleaning.poligonalesNomencladas.ContainsKey(key))
			{
				result = Cleaning.poligonalesNomencladas[key];
			}
			else
			{
				Tokenizer tokenizer = Tokenizer.FromString(poligonal, 3);
				IList<Poligonal> enProvincia = base.DAOFactory.PoligonalDAO.GetEnProvincia(tokenizer.ToArray(), provincia);
				IList<Poligonal> list = Cleaning.Nomenclar<Poligonal>(tokenizer, enProvincia);
				Cleaning.poligonalesNomencladas.Add(key, list);
				if (Cleaning.poligonalesNomencladas.Count > 500)
				{
					using (Dictionary<string, IList<Poligonal>>.Enumerator enumerator = Cleaning.poligonalesNomencladas.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							KeyValuePair<string, IList<Poligonal>> current = enumerator.Current;
							Cleaning.poligonalesNomencladas.Remove(current.Key);
						}
					}
				}
				result = list;
			}
			return result;
		}
		public IList<DireccionVO> NomenclarDireccion(string calle, int altura, string esquina, string partido, string provincia)
		{
			List<DireccionVO> list = new List<DireccionVO>();
			int provincia2 = -1;
			int partido2 = -1;
			Partido partido3 = null;
			IList<DireccionVO> result;
			if (string.IsNullOrEmpty(calle.Trim()) || (altura <= 0 && string.IsNullOrEmpty(esquina.Trim())))
			{
				result = list;
			}
			else
			{
				if (!string.IsNullOrEmpty(provincia.Trim()))
				{
					IList<Provincia> list2 = this.NomenclarProvinciaI(provincia);
					if (list2.Count == 1)
					{
						Provincia provincia3 = list2[0];
						provincia2 = provincia3.MapId;
						if (provincia3.MapId == 199)
						{
							partido3 = provincia3.Partidos[0];
							partido2 = partido3.PolId;
						}
					}
					if (partido3 == null && !string.IsNullOrEmpty(partido))
					{
						IList<Partido> list3 = this.NomenclarPartidoI(partido, provincia2);
						if (list3.Count == 1)
						{
							partido3 = list3[0];
							partido2 = partido3.PolId;
						}
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(partido))
					{
						IList<Partido> list3 = this.NomenclarPartidoI(partido, -1);
						if (list3.Count == 1)
						{
							partido3 = list3[0];
							partido2 = partido3.PolId;
						}
					}
				}
				IList<Poligonal> list4 = this.NomenclarPoligonalI(calle, partido2, provincia2);
				if (list4.Count == 0)
				{
					result = list;
				}
				else
				{
					if (altura > 0)
					{
						foreach (Poligonal current in list4)
						{
							if (current.AlturaMinima - 1 <= altura && current.AlturaMaxima >= altura)
							{
								DireccionVO direccionVO = base.ValidarAltura(current.PolId, altura, current.MapId);
								if (direccionVO != null)
								{
									list.Add(direccionVO);
								}
							}
						}
					}
					else
					{
						IList<Poligonal> list5 = this.NomenclarPoligonalI(esquina, partido2, provincia2);
						foreach (Poligonal current in list4)
						{
							foreach (Poligonal current2 in list5)
							{
								if (current.MapId == current2.MapId)
								{
									DireccionVO direccionVO = base.ValidarCruce(current.PolId, current2.PolId, current.MapId);
									if (direccionVO != null)
									{
										list.Add(direccionVO);
									}
								}
							}
						}
					}
					result = list;
				}
			}
			return result;
		}
		private static IList<T> Nomenclar<T>(Tokenizer tokens, IEnumerable<T> options) where T : ITokenizable
		{
			double num;
			return Cleaning.Nomenclar<T>(tokens, options, out num);
		}
		private static IList<T> Nomenclar<T>(Tokenizer tokens, IEnumerable<T> options, out double puntos) where T : ITokenizable
		{
			Evaluador eval = new Evaluador(tokens, new IEvaluator[]
			{
				new ExactMatchEvaluator(),
				new MixedMatchEvaluator(),
				new WordMatchEvaluator(),
				new StartMatchEvaluator()
			});
			return Cleaning.Nomenclar<T>(eval, tokens, options, out puntos);
		}
		private static IList<T> NomenclarAmbiguo<T>(Tokenizer tokens, IEnumerable<T> options, out double puntos) where T : ITokenizable
		{
			Evaluador eval = new Evaluador(tokens, new IEvaluator[]
			{
				new WordMatchEvaluator(),
				new StartMatchEvaluator()
			});
			return Cleaning.Nomenclar<T>(eval, tokens, options, out puntos);
		}
		private static IList<T> Nomenclar<T>(Evaluador eval, Tokenizer tokens, IEnumerable<T> options, out double puntos) where T : ITokenizable
		{
			List<T> list = new List<T>();
			double num = 0.0;
			foreach (T current in options)
			{
				double num2 = eval.Evaluar(Tokenizer.Create(current));
				if (num2 != 0.0 && num2 >= num)
				{
					if (num2 == num)
					{
						list.Add(current);
					}
					else
					{
						if (num2 > num)
						{
							list.Clear();
							list.Add(current);
							num = num2;
						}
					}
				}
			}
			puntos = num;
			return list;
		}
		public IList<DireccionVO> GetSmartSearch(string frase)
		{
			Parser parser = new Parser();
			ParsedDirection[] array = parser.ParseTokens(frase);
			List<ObjetoPuntuado<DireccionVO>> list = new List<ObjetoPuntuado<DireccionVO>>();
			ParsedDirection[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				ParsedDirection direccion = array2[i];
				IList<ObjetoPuntuado<DireccionVO>> collection = this.NomenclarDireccionSmart(direccion);
				list.AddRange(collection);
			}
			double num = 0.0;
			if (list.Count > 0)
			{
				foreach (ObjetoPuntuado<DireccionVO> current in list)
				{
					if (current.Puntaje > num)
					{
						num = current.Puntaje;
					}
				}
			}
			List<IObjetoPuntuado<DireccionVO>> list2 = new List<IObjetoPuntuado<DireccionVO>>();
			List<double> list3 = new List<double>();
			List<double> list4 = new List<double>();
			foreach (ObjetoPuntuado<DireccionVO> current2 in list)
			{
				if (current2.Puntaje > num * 0.5)
				{
					bool flag = false;
					for (int j = 0; j < list3.Count; j++)
					{
						double num2 = list3[j];
						double num3 = list4[j];
						if (current2.Objeto.Latitud == num2 && current2.Objeto.Longitud == num3)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						list2.Add(current2);
						list3.Add(current2.Objeto.Latitud);
						list4.Add(current2.Objeto.Longitud);
					}
				}
			}
			list2.Sort(new IObjetoPuntuadoComparer<DireccionVO>());
			List<DireccionVO> list5 = new List<DireccionVO>(list2.Count);
			foreach (IObjetoPuntuado<DireccionVO> current3 in list2)
			{
				list5.Add(current3.Objeto);
			}
			return list5;
		}
		public IList<ObjetoPuntuado<DireccionVO>> NomenclarDireccionSmart(ParsedDirection direccion)
		{
			List<ObjetoPuntuado<DireccionVO>> list = new List<ObjetoPuntuado<DireccionVO>>();
			IList<ObjetoPuntuado<DireccionVO>> result;
			if (string.IsNullOrEmpty(direccion.Calle.Trim()) || (direccion.Altura <= 0 && string.IsNullOrEmpty(direccion.Esquina.Trim())))
			{
				result = list;
			}
			else
			{
				double num = 0.0;
				double num2 = 0.0;
				double num3 = 0.0;
				double item = 0.0;
				IList<Provincia> list2 = null;
				IList<Partido> list3 = null;
				if (!string.IsNullOrEmpty(direccion.Provincia.Trim()))
				{
					Tokenizer tokenizer = Tokenizer.FromString(direccion.Provincia, 0);
					IList<Provincia> provincias = base.DAOFactory.ProvinciaDAO.GetProvincias(tokenizer.ToArray());
					list2 = Cleaning.Nomenclar<Provincia>(tokenizer, provincias, out num);
					if (!string.IsNullOrEmpty(direccion.Localidad))
					{
						Tokenizer tokenizer2 = Tokenizer.FromString(direccion.Localidad, 1);
						List<Partido> list4 = new List<Partido>();
						foreach (Provincia current in provincias)
						{
							list4.AddRange(base.DAOFactory.PartidoDAO.GetEnProvincia(tokenizer2.ToArray(), current.MapId));
						}
						List<int> list5 = new List<int>();
						list3 = Cleaning.Nomenclar<Partido>(tokenizer2, list4, out num2);
						foreach (Partido current2 in list3)
						{
							list5.Add(current2.Id);
						}
						List<Localidad> list6 = new List<Localidad>();
						list6.AddRange(base.DAOFactory.LocalidadDAO.GetByProvincia(tokenizer2.ToArray(), -1));
						IList<Localidad> list7 = Cleaning.Nomenclar<Localidad>(tokenizer2, list6, out num3);
						foreach (Localidad current3 in list7)
						{
							if (!list5.Contains(current3.Partido.Id))
							{
								list3.Add(current3.Partido);
								list5.Add(current3.Partido.Id);
							}
						}
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(direccion.Localidad))
					{
						Tokenizer tokenizer2 = Tokenizer.FromString(direccion.Localidad, 1);
						List<Partido> list4 = new List<Partido>();
						list4.AddRange(base.DAOFactory.PartidoDAO.GetEnProvincia(tokenizer2.ToArray(), -1));
						List<int> list5 = new List<int>();
						list3 = Cleaning.Nomenclar<Partido>(tokenizer2, list4, out num2);
						foreach (Partido current2 in list3)
						{
							list5.Add(current2.Id);
						}
						List<Localidad> list6 = new List<Localidad>();
						list6.AddRange(base.DAOFactory.LocalidadDAO.GetByProvincia(tokenizer2.ToArray(), -1));
						IList<Localidad> list7 = Cleaning.Nomenclar<Localidad>(tokenizer2, list6, out num3);
						foreach (Localidad current3 in list7)
						{
							if (!list5.Contains(current3.Partido.Id))
							{
								list3.Add(current3.Partido);
								list5.Add(current3.Partido.Id);
							}
						}
					}
				}
				Tokenizer tokenizer3 = Tokenizer.FromString(direccion.Calle, 3);
				List<Poligonal> list8 = new List<Poligonal>();
				List<double> list9 = new List<double>();
				if (list3 != null)
				{
					foreach (Partido current2 in list3)
					{
						IList<Poligonal> enPartidoYProvincia = base.DAOFactory.PoligonalDAO.GetEnPartidoYProvincia(tokenizer3.ToArray(), current2.PolId, current2.Provincia.MapId);
						IList<Poligonal> list10 = Cleaning.NomenclarAmbiguo<Poligonal>(tokenizer3, enPartidoYProvincia, out item);
						list8.AddRange(list10);
						for (int i = 0; i < list10.Count; i++)
						{
							list9.Add(item);
						}
					}
				}
				else
				{
					if (list2 != null)
					{
						foreach (Provincia current in list2)
						{
							IList<Poligonal> enPartidoYProvincia = base.DAOFactory.PoligonalDAO.GetEnPartidoYProvincia(tokenizer3.ToArray(), -1, current.MapId);
							IList<Poligonal> list10 = Cleaning.NomenclarAmbiguo<Poligonal>(tokenizer3, enPartidoYProvincia, out item);
							list8.AddRange(list10);
							for (int i = 0; i < list10.Count; i++)
							{
								list9.Add(item);
							}
						}
					}
					else
					{
						IList<Poligonal> enPartidoYProvincia = base.DAOFactory.PoligonalDAO.GetEnPartidoYProvincia(tokenizer3.ToArray(), -1, -1);
						IList<Poligonal> list10 = Cleaning.NomenclarAmbiguo<Poligonal>(tokenizer3, enPartidoYProvincia, out item);
						list8.AddRange(list10);
						for (int i = 0; i < list10.Count; i++)
						{
							list9.Add(item);
						}
					}
				}
				if (direccion.Altura > 0)
				{
					for (int i = 0; i < list8.Count; i++)
					{
						Poligonal poligonal = list8[i];
						if (poligonal.AlturaMinima - 1 <= direccion.Altura && poligonal.AlturaMaxima >= direccion.Altura)
						{
							DireccionVO direccionVO = base.ValidarAltura(poligonal.PolId, direccion.Altura, poligonal.MapId);
							if (direccionVO != null)
							{
								list.Add(new ObjetoPuntuado<DireccionVO>(num + num2 + list9[i], direccionVO));
							}
						}
					}
				}
				else
				{
					List<double> list11 = new List<double>();
					List<Poligonal> list12 = new List<Poligonal>();
					Tokenizer tokenizer4 = Tokenizer.FromString(direccion.Esquina, 3);
					if (list3 != null)
					{
						foreach (Partido current2 in list3)
						{
							IList<Poligonal> enPartidoYProvincia2 = base.DAOFactory.PoligonalDAO.GetEnPartidoYProvincia(tokenizer4.ToArray(), current2.PolId, current2.Provincia.MapId);
							double item2;
							IList<Poligonal> list10 = Cleaning.NomenclarAmbiguo<Poligonal>(tokenizer4, enPartidoYProvincia2, out item2);
							list12.AddRange(list10);
							for (int i = 0; i < list10.Count; i++)
							{
								list11.Add(item2);
							}
						}
					}
					else
					{
						if (list2 != null)
						{
							foreach (Provincia current in list2)
							{
								IList<Poligonal> enPartidoYProvincia2 = base.DAOFactory.PoligonalDAO.GetEnPartidoYProvincia(tokenizer4.ToArray(), -1, current.MapId);
								double item2;
								IList<Poligonal> list10 = Cleaning.NomenclarAmbiguo<Poligonal>(tokenizer4, enPartidoYProvincia2, out item2);
								list12.AddRange(list10);
								for (int i = 0; i < list10.Count; i++)
								{
									list11.Add(item2);
								}
							}
						}
						else
						{
							IList<Poligonal> enPartidoYProvincia2 = base.DAOFactory.PoligonalDAO.GetEnPartidoYProvincia(tokenizer4.ToArray(), -1, -1);
							double item2;
							IList<Poligonal> list10 = Cleaning.NomenclarAmbiguo<Poligonal>(tokenizer4, enPartidoYProvincia2, out item2);
							list12.AddRange(list10);
							for (int i = 0; i < list10.Count; i++)
							{
								list11.Add(item2);
							}
						}
					}
					for (int i = 0; i < list8.Count; i++)
					{
						Poligonal poligonal = list8[i];
						for (int j = 0; j < list12.Count; j++)
						{
							Poligonal poligonal2 = list12[j];
							if (poligonal.MapId == poligonal2.MapId)
							{
								DireccionVO direccionVO = base.ValidarCruce(poligonal.PolId, poligonal2.PolId, poligonal.MapId);
								if (direccionVO != null)
								{
									list.Add(new ObjetoPuntuado<DireccionVO>(num + num2 + (list9[i] + list11[j]) / 2.0, direccionVO));
								}
							}
						}
					}
				}
				result = list;
			}
			return result;
		}
	}
}
