using Geocoder.Core;
using Geocoder.Data.DAO;
using System.Collections.Generic;
using System.Threading;

namespace Geocoder.Logic
{
	public class Abreviaturas
	{
		private DAOFactory Df;
		public static string SessionFactoryConfigPath = "";
		private static Abreviaturas _instance;
		private static readonly object SingletonLock = new object();
		private readonly Dictionary<string, string> DicPoly = new Dictionary<string, string>();
		private readonly Dictionary<string, string> DicLoc = new Dictionary<string, string>();
		private readonly Dictionary<string, string> DicPart = new Dictionary<string, string>();
		private readonly Dictionary<string, string> DicProv = new Dictionary<string, string>();
		private DAOFactory DaoFactory
		{
			get
			{
				DAOFactory arg_1E_0;
				if ((arg_1E_0 = this.Df) == null)
				{
					arg_1E_0 = (this.Df = new DAOFactory(Abreviaturas.SessionFactoryConfigPath));
				}
				return arg_1E_0;
			}
		}
		public static Abreviaturas Instance
		{
			get
			{
				object singletonLock;
				Monitor.Enter(singletonLock = Abreviaturas.SingletonLock);
				Abreviaturas result;
				try
				{
					Abreviaturas arg_23_0;
					if ((arg_23_0 = Abreviaturas._instance) == null)
					{
						arg_23_0 = (Abreviaturas._instance = new Abreviaturas());
					}
					result = arg_23_0;
				}
				finally
				{
					Monitor.Exit(singletonLock);
				}
				return result;
			}
		}
		public string Abreviatura(int nivel, string palabra)
		{
			Dictionary<string, string> dictionary = this.GetDictionary(nivel);
			string result;
			if (dictionary == null)
			{
				result = palabra;
			}
			else
			{
				result = (dictionary.ContainsKey(palabra) ? dictionary[palabra] : palabra);
			}
			return result;
		}
		public bool IsAbreviatura(int nivel, string palabra)
		{
			Dictionary<string, string> dictionary = this.GetDictionary(nivel);
			return dictionary != null && dictionary.ContainsValue(palabra);
		}
		private Dictionary<string, string> GetDictionary(int nivel)
		{
			Dictionary<string, string> result;
			switch (nivel)
			{
			case 0:
				result = this.DicProv;
				break;
			case 1:
				result = this.DicPart;
				break;
			case 2:
				result = this.DicLoc;
				break;
			case 3:
				result = this.DicPoly;
				break;
			default:
				result = null;
				break;
			}
			return result;
		}
		public bool HasAbreviatura(int nivel, string palabra)
		{
			Dictionary<string, string> dictionary = this.GetDictionary(nivel);
			return dictionary != null && dictionary.ContainsKey(palabra);
		}
		private Abreviaturas()
		{
			List<Abreviatura> all = this.DaoFactory.AbreviaturaDAO.GetAll();
			foreach (Abreviatura current in all)
			{
				Dictionary<string, string> dictionary = this.GetDictionary(current.Nivel);
				if (dictionary != null)
				{
					dictionary.Add(current.Literal, current.Abreviado);
				}
			}
		}
	}
}
