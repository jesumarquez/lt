using System;
using System.Collections.Generic;
namespace Geocoder.SmartSearch
{
	public static class ProbsDic
	{
		private static Dictionary<string, double> probs;
		private static double[] calles;
		private static double[] alturas;
		private static double[] localidades;
		private static double[] provincias;
		static ProbsDic()
		{
			ProbsDic.probs = new Dictionary<string, double>();
			ProbsDic.probs.Add("C", 0.9);
			ProbsDic.probs.Add("P", 0.06);
			ProbsDic.probs.Add("L", 0.04);
			ProbsDic.probs.Add("CA", 0.5);
			ProbsDic.probs.Add("CE", 0.5);
			ProbsDic.probs.Add("PC", 0.8);
			ProbsDic.probs.Add("PL", 0.2);
			ProbsDic.probs.Add("LC", 0.8);
			ProbsDic.probs.Add("LP", 0.2);
			ProbsDic.probs.Add("CAL", 0.6);
			ProbsDic.probs.Add("CAP", 0.4);
			ProbsDic.probs.Add("CEL", 0.8);
			ProbsDic.probs.Add("CEP", 0.2);
			ProbsDic.probs.Add("PCA", 0.5);
			ProbsDic.probs.Add("PCE", 0.5);
			ProbsDic.probs.Add("PLC", 0.5);
			ProbsDic.probs.Add("LPC", 0.5);
			ProbsDic.probs.Add("LCA", 0.5);
			ProbsDic.probs.Add("LCE", 0.5);
			ProbsDic.probs.Add("CALP", 0.5);
			ProbsDic.probs.Add("CELP", 0.5);
			ProbsDic.probs.Add("CAPL", 0.5);
			ProbsDic.probs.Add("CEPL", 0.5);
			ProbsDic.probs.Add("PCAL", 0.5);
			ProbsDic.probs.Add("PCEL", 0.5);
			ProbsDic.probs.Add("PLCA", 0.5);
			ProbsDic.probs.Add("PLCE", 0.5);
			ProbsDic.probs.Add("LPCA", 0.5);
			ProbsDic.probs.Add("LPCE", 0.5);
			ProbsDic.probs.Add("LCAP", 0.5);
			ProbsDic.probs.Add("LCEP", 0.5);
			ProbsDic.calles = new double[]
			{
				1.0,
				0.74,
				0.4,
				0.24,
				0.03,
				0.01,
				0.005,
				0.002,
				0.001,
				0.001,
				0.0
			};
			double[] array = new double[2];
			array[0] = 1.0;
			ProbsDic.alturas = array;
			ProbsDic.localidades = new double[]
			{
				1.0,
				0.58,
				0.17,
				0.05,
				0.01,
				0.002,
				0.002,
				0.0
			};
			ProbsDic.provincias = new double[]
			{
				1.0,
				0.5,
				0.125,
				0.04,
				0.0
			};
		}
		public static double GetTypeProb(ParserNode node)
		{
			ParserNode parserNode = node;
			bool flag = false;
			double result;
			while (parserNode.ParentNode != null)
			{
				if (flag && parserNode.ParentNode.NodeType == node.NodeType)
				{
					result = 0.0;
					return result;
				}
				if (!flag && parserNode.ParentNode.NodeType != node.NodeType)
				{
					flag = true;
				}
				parserNode = parserNode.ParentNode;
			}
			result = (ProbsDic.probs.ContainsKey(node.TypePath) ? ProbsDic.probs[node.TypePath] : 0.0);
			return result;
		}
		public static double GetProb(ParserNode node)
		{
			double result;
			switch (node.NodeType)
			{
			case ParserNodeTypes.Calle:
				if (ProbsDic.calles.Length <= node.NodeTypeIndex)
				{
					result = 0.0;
				}
				else
				{
					result = ProbsDic.calles[node.NodeTypeIndex];
				}
				break;
			case ParserNodeTypes.Equina:
				if (ProbsDic.calles.Length <= node.NodeTypeIndex)
				{
					result = 0.0;
				}
				else
				{
					result = ProbsDic.calles[node.NodeTypeIndex];
				}
				break;
			case ParserNodeTypes.Altura:
				if (ProbsDic.alturas.Length <= node.NodeTypeIndex)
				{
					result = 0.0;
				}
				else
				{
					result = ProbsDic.alturas[node.NodeTypeIndex];
				}
				break;
			case ParserNodeTypes.Localidad:
				if (ProbsDic.localidades.Length <= node.NodeTypeIndex)
				{
					result = 0.0;
				}
				else
				{
					result = ProbsDic.localidades[node.NodeTypeIndex];
				}
				break;
			case ParserNodeTypes.Provincia:
				if (ProbsDic.provincias.Length <= node.NodeTypeIndex)
				{
					result = 0.0;
				}
				else
				{
					result = ProbsDic.provincias[node.NodeTypeIndex];
				}
				break;
			default:
				result = 0.0;
				break;
			}
			return result;
		}
		public static ParserNodeTypes[] GetPossibleNext(ParserNode node)
		{
			List<ParserNodeTypes> list = new List<ParserNodeTypes>();
			ParserNodeTypes[] result;
			if (node == null)
			{
				list.Add(ParserNodeTypes.Calle);
				list.Add(ParserNodeTypes.Localidad);
				list.Add(ParserNodeTypes.Provincia);
				result = list.ToArray();
			}
			else
			{
				if (!ProbsDic.probs.ContainsKey(node.TypePath))
				{
					result = list.ToArray();
				}
				else
				{
					switch (node.NodeType)
					{
					case ParserNodeTypes.Calle:
						list.Add(ParserNodeTypes.Calle);
						list.Add(ParserNodeTypes.Altura);
						list.Add(ParserNodeTypes.Equina);
						break;
					case ParserNodeTypes.Equina:
						list.Add(ParserNodeTypes.Equina);
						list.Add(ParserNodeTypes.Localidad);
						list.Add(ParserNodeTypes.Provincia);
						break;
					case ParserNodeTypes.Altura:
						list.Add(ParserNodeTypes.Localidad);
						list.Add(ParserNodeTypes.Provincia);
						break;
					case ParserNodeTypes.Localidad:
						list.Add(ParserNodeTypes.Calle);
						list.Add(ParserNodeTypes.Altura);
						list.Add(ParserNodeTypes.Localidad);
						list.Add(ParserNodeTypes.Provincia);
						break;
					case ParserNodeTypes.Provincia:
						list.Add(ParserNodeTypes.Calle);
						list.Add(ParserNodeTypes.Altura);
						list.Add(ParserNodeTypes.Localidad);
						list.Add(ParserNodeTypes.Provincia);
						break;
					}
					result = list.ToArray();
				}
			}
			return result;
		}
	}
}
