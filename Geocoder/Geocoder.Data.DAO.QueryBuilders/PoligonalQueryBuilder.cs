using Geocoder.Core;

namespace Geocoder.Data.DAO.QueryBuilders
{
	public class PoligonalQueryBuilder : QueryBuilder
	{
		public PoligonalQueryBuilder()
		{
			Select = " distinct poli ";
			From = " Poligonal poli ";
		}
		public PoligonalQueryBuilder AddCalle(string[] tokens, int startWithLength)
		{
			From += " join poli.Palabras poli_pala ";
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
		public PoligonalQueryBuilder AddAltura(int altura)
		{
			From += " join poli.Alturas altu  ";
			if (!FirstWhere)
			{
				Where += " and ";
			}
			object where = Where;
			Where = string.Concat(new []
			{
				where,
				" (",
				altura,
				" > altu.AlturaInicio and ",
				altura,
				" < altu.AlturaFin) "
			});
			FirstWhere = false;
			return this;
		}
		public PoligonalQueryBuilder AddPartido(int partido)
		{
			PoligonalQueryBuilder result;
			if (partido < 0)
			{
				result = this;
			}
			else
			{
				if (!FirstWhere)
				{
					Where += " and ";
				}
				Where += string.Format(" poli.Partido.PolId = {0} ", partido);
				FirstWhere = false;
				result = this;
			}
			return result;
		}
		public PoligonalQueryBuilder AddPartido(string[] tokens, int startWithLength)
		{
			From += " join poli.Partido.Palabras part_pala ";
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
		public PoligonalQueryBuilder AddProvincia(int idProvincia)
		{
			PoligonalQueryBuilder result;
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
				Where += " poli.Partido.Provincia.MapId = " + idProvincia + " ";
				FirstWhere = false;
				result = this;
			}
			return result;
		}
		public PoligonalQueryBuilder AddProvincia(string[] tokens, int startWithLength)
		{
			From += " join poli.Partido.Provincia.Palabras prov_pala ";
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
