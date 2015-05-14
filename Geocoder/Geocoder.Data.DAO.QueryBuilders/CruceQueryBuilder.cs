using Geocoder.Core;
using System;
using System.Globalization;

namespace Geocoder.Data.DAO.QueryBuilders
{
	public class CruceQueryBuilder : QueryBuilder
	{
		public CruceQueryBuilder()
		{
			Select = " distinct crux ";
			From = " Cruce crux ";
		}
		public CruceQueryBuilder AddCalle(string[] tokens, int startWithLength)
		{
			From += " join crux.Poligonal.Palabras crux_poli_pala ";
			if (!FirstWhere)
			{
				Where += " and ";
			}
			Where += " len(crux_poli_pala.Palabra.Normalizada) > 2 and ( ";
			var flag = true;
			var maxLength = GetMaxLength(tokens);
			for (var i = 0; i < tokens.Length; i++)
			{
				if (maxLength <= 2 || tokens[i].Length > 2)
				{
					if (!flag)
					{
						Where += " or ";
					}
					flag = false;
					var normalized = (tokens[i].Length > startWithLength) ? tokens[i].Substring(0, startWithLength) : tokens[i];
					Where += string.Concat(new object[]
					{
						" (crux_poli_pala.Palabra.Prefix1 >= ",
						Prefix.Prefix6Min(normalized),
						" and crux_poli_pala.Palabra.Prefix1 <= ",
						Prefix.Prefix6Max(normalized),
						") "
					});
				}
			}
			Where += ")";
			FirstWhere = false;
			return this;
		}
		public CruceQueryBuilder AddEsquina(string[] tokens, int startWithLength)
		{
			From += " join crux.Esquina.Palabras crux_esqu_pala ";
			if (!FirstWhere)
			{
				Where += " and ";
			}
			Where += " len(crux_esqu_pala.Palabra.Normalizada) > 2 and ( ";
			var flag = true;
			var maxLength = GetMaxLength(tokens);
			for (var i = 0; i < tokens.Length; i++)
			{
				if (maxLength <= 2 || tokens[i].Length > 2)
				{
					if (!flag)
					{
						Where += " or ";
					}
					flag = false;
					var normalized = (tokens[i].Length > startWithLength) ? tokens[i].Substring(0, startWithLength) : tokens[i];
					Where += string.Concat(new object[]
					{
						" (crux_esqu_pala.Palabra.Prefix1 >= ",
						Prefix.Prefix6Min(normalized),
						" and crux_esqu_pala.Palabra.Prefix1 <= ",
						Prefix.Prefix6Max(normalized),
						") "
					});
				}
			}
			Where += ")";
			FirstWhere = false;
			return this;
		}
		public CruceQueryBuilder AddPartido(string[] tokens, int startWithLength)
		{
			From += " join crux.Poligonal.Partido.Palabras crux_part_pala ";
			if (!FirstWhere)
			{
				Where += " and ";
			}
			Where += " len(crux_part_pala.Palabra.Normalizada) > 2 and ( ";
			var flag = true;
			var maxLength = GetMaxLength(tokens);
			for (var i = 0; i < tokens.Length; i++)
			{
				if (maxLength <= 2 || tokens[i].Length > 2)
				{
					if (!flag)
					{
						Where += " or ";
					}
					flag = false;
					var normalized = (tokens[i].Length > startWithLength) ? tokens[i].Substring(0, startWithLength) : tokens[i];
					Where += string.Concat(new object[]
					{
						" (crux_part_pala.Palabra.Prefix1 >= ",
						Prefix.Prefix6Min(normalized),
						" and crux_part_pala.Palabra.Prefix1 <= ",
						Prefix.Prefix6Max(normalized),
						") "
					});
				}
			}
			Where += ")";
			FirstWhere = false;
			return this;
		}
		public CruceQueryBuilder AddLatitud(double latitud, double latitud2)
		{
			var num = Math.Min(latitud, latitud2);
			var num2 = Math.Max(latitud, latitud2);
			if (!FirstWhere)
			{
				Where += " and ";
			}
			Where += " crux.Latitud > " + num.ToString(CultureInfo.InvariantCulture) + " ";
			Where += " and crux.Latitud < " + num2.ToString(CultureInfo.InvariantCulture) + " ";
			FirstWhere = false;
			return this;
		}
		public CruceQueryBuilder AddLongitud(double longitud, double longitud2)
		{
			var num = Math.Min(longitud, longitud2);
			var num2 = Math.Max(longitud, longitud2);
			if (!FirstWhere)
			{
				Where += " and ";
			}
			Where += " crux.Longitud > " + num.ToString(CultureInfo.InvariantCulture) + " ";
			Where += " and crux.Longitud < " + num2.ToString(CultureInfo.InvariantCulture) + " ";
			FirstWhere = false;
			return this;
		}
		public CruceQueryBuilder AddProvincia(int idProvincia)
		{
			if (!FirstWhere)
			{
				Where += " and ";
			}
			Where += " crux.Poligonal.Partido.Provincia.MapId = " + idProvincia + " ";
			Where += " and crux.Esquina.Partido.Provincia.MapId = " + idProvincia + " ";
			FirstWhere = false;
			return this;
		}
		public CruceQueryBuilder AddProvincia(string[] tokens, int startWithLength)
		{
			From += " join crux.Poligonal.Partido.Provincia.Palabras crux_prov_pala ";
			if (!FirstWhere)
			{
				Where += " and ";
			}
			Where += " len(crux_prov_pala.Palabra.Normalizada) > 2 and ( ";
			var flag = true;
			var maxLength = GetMaxLength(tokens);
			for (var i = 0; i < tokens.Length; i++)
			{
				if (maxLength <= 2 || tokens[i].Length > 2)
				{
					if (!flag)
					{
						Where += " or ";
					}
					flag = false;
					var normalized = (tokens[i].Length > startWithLength) ? tokens[i].Substring(0, startWithLength) : tokens[i];
					Where += string.Concat(new object[]
					{
						" (crux_prov_pala.Palabra.Prefix1 >= ",
						Prefix.Prefix6Min(normalized),
						" and crux_prov_pala.Palabra.Prefix1 <= ",
						Prefix.Prefix6Max(normalized),
						") "
					});
				}
			}
			Where += ")";
			FirstWhere = false;
			return this;
		}
	}
}
