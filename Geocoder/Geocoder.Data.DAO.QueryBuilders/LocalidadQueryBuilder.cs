using Geocoder.Core;

namespace Geocoder.Data.DAO.QueryBuilders
{
	public class LocalidadQueryBuilder : QueryBuilder
	{
		public LocalidadQueryBuilder()
		{
			Select = " distinct locl ";
			From = " Localidad locl ";
		}
		public LocalidadQueryBuilder AddLocalidad(string[] tokens, int startWithLength)
		{
			From += " join locl.Palabras locl_pala ";
			if (!FirstWhere)
			{
				Where += " and ";
			}
			Where += " len(locl_pala.Palabra.Normalizada) > 2 and ( ";
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
						" (locl_pala.Palabra.Prefix1 >= ",
						Prefix.Prefix6Min(normalized),
						" and locl_pala.Palabra.Prefix1 <= ",
						Prefix.Prefix6Max(normalized),
						") "
					});
				}
			}
			Where += ")";
			FirstWhere = false;
			return this;
		}
		public LocalidadQueryBuilder AddProvincia(int idProvincia)
		{
			LocalidadQueryBuilder result;
			if (idProvincia < 0)
			{
				result = this;
			}
			else
			{
				if (!FirstWhere)
				{
					Where += " and ";
				}
				Where += " locl.Partido.Provincia.MapId = " + idProvincia + " ";
				FirstWhere = false;
				result = this;
			}
			return result;
		}
		public LocalidadQueryBuilder AddProvincia(string[] tokens, int startWithLength)
		{
			From += " join locl.Partido.Provincia.Palabras prov_pala ";
			if (!FirstWhere)
			{
				Where += " and ";
			}
			Where += " len(locl_pala.Palabra.Normalizada) > 2 and ( ";
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
