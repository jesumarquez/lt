using Geocoder.Core;

namespace Geocoder.Data.DAO.QueryBuilders
{
	public class ProvinciaQueryBuilder : QueryBuilder
	{
		public ProvinciaQueryBuilder()
		{
			Select = " distinct prov ";
			From = " Provincia prov ";
		}
		public ProvinciaQueryBuilder AddProvincia(string[] tokens, int startWithLength)
		{
			From += " join prov.Palabras prov_pala ";
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
