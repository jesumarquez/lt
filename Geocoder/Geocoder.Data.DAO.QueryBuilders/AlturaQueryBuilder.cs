using Geocoder.Core;
using System;
using System.Globalization;

namespace Geocoder.Data.DAO.QueryBuilders
{
	public class AlturaQueryBuilder : QueryBuilder
	{
		public AlturaQueryBuilder(int altura)
		{
			Select = " distinct altura ";
			From = " Altura altura ";
			Where = string.Concat(new object[]
			{
				" (",
				altura,
				" >= altura.AlturaInicio and ",
				altura,
				" < altura.AlturaFin) "
			});
			FirstWhere = false;
		}
		public AlturaQueryBuilder AddCalle(string[] tokens, int startWithLength)
		{
			From += " join altura.Poligonal.Palabras poli_pala ";
			if (!FirstWhere)
			{
				Where += " and ";
			}
			Where += " len(poli_pala.Palabra.Normalizada) > 2 and ( ";
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
						" (poli_pala.Palabra.Prefix1 >= ",
						Prefix.Prefix6Min(normalized),
						" and poli_pala.Palabra.Prefix1 <= ",
						Prefix.Prefix6Max(normalized),
						") "
					});
				}
			}
			Where += ")";
			FirstWhere = false;
			return this;
		}
		public AlturaQueryBuilder AddPartido(string[] tokens, int startWithLength)
		{
			From += " join altura.Poligonal.Partido.Palabras part_pala ";
			if (!FirstWhere)
			{
				Where += " and ";
			}
			Where += " len(part_pala.Palabra.Normalizada) > 2 and ( ";
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
						" (part_pala.Palabra.Prefix1 >= ",
						Prefix.Prefix6Min(normalized),
						" and part_pala.Palabra.Prefix1 <= ",
						Prefix.Prefix6Max(normalized),
						") "
					});
				}
			}
			Where += ")";
			FirstWhere = false;
			return this;
		}
		public AlturaQueryBuilder AddLatitud(double latitud, double latitud2)
		{
			var num = Math.Min(latitud, latitud2);
			var num2 = Math.Max(latitud, latitud2);
			if (!FirstWhere)
			{
				Where += " and ";
			}
			Where += " altura.LatitudInicio > " + num.ToString(CultureInfo.InvariantCulture) + " ";
			Where += " and altura.LatitudInicio < " + num2.ToString(CultureInfo.InvariantCulture) + " ";
			FirstWhere = false;
			return this;
		}
		public AlturaQueryBuilder AddLongitud(double longitud, double longitud2)
		{
			var num = Math.Min(longitud, longitud2);
			var num2 = Math.Max(longitud, longitud2);
			if (!FirstWhere)
			{
				Where += " and ";
			}
			Where += " altura.LongitudInicio > " + num.ToString(CultureInfo.InvariantCulture) + " ";
			Where += " and altura.LongitudInicio < " + num2.ToString(CultureInfo.InvariantCulture) + " ";
			FirstWhere = false;
			return this;
		}
		public AlturaQueryBuilder AddProvincia(int idProvincia)
		{
			if (!FirstWhere)
			{
				Where += " and ";
			}
			Where += " altura.Poligonal.Partido.Provincia.MapId = " + idProvincia + " ";
			FirstWhere = false;
			return this;
		}
		public AlturaQueryBuilder AddProvincia(string[] tokens, int startWithLength)
		{
			From += " join altura.Poligonal.Partido.Provincia.Palabras prov_pala ";
			if (!FirstWhere)
			{
				Where += " and ";
			}
			Where += " len(prov_pala.Palabra.Normalizada) > 2 and ( ";
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
						" (prov_pala.Palabra.Prefix1 >= ",
						Prefix.Prefix6Min(normalized),
						" and prov_pala.Palabra.Prefix1 <= ",
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
