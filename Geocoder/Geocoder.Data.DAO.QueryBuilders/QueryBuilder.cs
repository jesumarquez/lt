namespace Geocoder.Data.DAO.QueryBuilders
{
	public abstract class QueryBuilder
	{
		protected string Select = "";
		protected string From = "";
		protected string Where = "";
		protected bool FirstWhere = true;
		protected static int GetMaxLength(string[] tokens)
		{
			var num = 0;
			for (var i = 0; i < tokens.Length; i++)
			{
				var text = tokens[i];
				if (text.Length > num)
				{
					num = text.Length;
				}
			}
			return num;
		}
		public string GetQuery()
		{
			return string.Concat(new[]
			{
				string.Concat(new[]
				{
					"select ",
					Select,
					" from ",
					From,
					" where ",
					Where
				})
			});
		}
		public static PoligonalQueryBuilder CreatePoligonalQueryBuilder()
		{
			return new PoligonalQueryBuilder();
		}
		public static CruceQueryBuilder CreateCruceQueryBuilder()
		{
			return new CruceQueryBuilder();
		}
		public static AlturaQueryBuilder CreateAlturaQueryBuilder(int altura)
		{
			return new AlturaQueryBuilder(altura);
		}
		public static PartidoQueryBuilder CreatePartidoQueryBuilder()
		{
			return new PartidoQueryBuilder();
		}
		public static ProvinciaQueryBuilder CreateProvinciaQueryBuilder()
		{
			return new ProvinciaQueryBuilder();
		}
		public static LocalidadQueryBuilder CreateLocalidadQueryBuilder()
		{
			return new LocalidadQueryBuilder();
		}
	}
}
