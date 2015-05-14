using System;
using System.Collections.Generic;
namespace Geocoder.Logic.Evaluators
{
	public class PuntajeEvaluator : IEvaluator
	{
		public static int EXACT_MATCH = 100;
		public static int CARACTER_IGUAL_POSICION = 50;
		public static int CARACTER_OTRA_POSICION = 40;
		public static int NO_MATCH = -4;
		public static int FULL_MATCH = 50;
		public static int MATCH_TOKEN = 10;
		public double Evaluar(Tokenizer tokens, Tokenizer option)
		{
			double num = 0.0;
			int num2 = 0;
			int num3 = 0;
			int num4 = -1;
			int num5 = 0;
			int num6 = 0;
			List<TokenPosicionado> list = new List<TokenPosicionado>(tokens.Count);
			List<int> list2 = new List<int>(option.Count);
			for (int i = 0; i < tokens.Count; i++)
			{
				string text = tokens[i];
				list.Add(new TokenPosicionado(text, i));
				if (text.Length > num2)
				{
					num2 = text.Length;
				}
			}
			num2--;
			list.Sort(new TokenPosicionadoComparer());
			for (int j = 0; j < list.Count; j++)
			{
				double num7 = 0.0;
				string text = list[j].Token;
				int num8;
				bool flag = int.TryParse(text, out num8);
				if (flag)
				{
					num5++;
				}
				for (int i = 0; i < option.Count; i++)
				{
					if (!list2.Contains(i))
					{
						string text2 = option[i];
						double num9 = (double)Math.Min(5, text.Length) / (double)num2;
						num9 = ((num9 > 1.0) ? 1.0 : num9);
						if (text == text2)
						{
							if (flag)
							{
								num6++;
							}
							if (num4 == -1 || j == num4 + 1)
							{
								num7 = (double)(PuntajeEvaluator.CARACTER_IGUAL_POSICION * text.Length) * num9 + (double)PuntajeEvaluator.EXACT_MATCH;
							}
							else
							{
								num7 = (double)(PuntajeEvaluator.CARACTER_OTRA_POSICION * text.Length) * num9 + (double)PuntajeEvaluator.EXACT_MATCH;
							}
							num4 = i;
							num3++;
							list2.Add(i);
							break;
						}
						if (!flag)
						{
							int num10 = (text.Length > text2.Length) ? text2.Length : text.Length;
							int num11 = 0;
							for (int k = 0; k < num10; k++)
							{
								if (text[k] != text2[k])
								{
									break;
								}
								num11++;
							}
							double num13;
							if (num11 > 2 || num10 <= 2)
							{
								double num12;
								if (num4 == -1 || j == num4 + 1)
								{
									num12 = (double)PuntajeEvaluator.CARACTER_IGUAL_POSICION * num9;
								}
								else
								{
									num12 = (double)PuntajeEvaluator.CARACTER_OTRA_POSICION * num9;
								}
								num13 = (double)num11 * num12;
								if (text2.Length == 1 && num11 == 1 && !char.IsNumber(text2[0]))
								{
									num13 += (double)PuntajeEvaluator.EXACT_MATCH;
								}
							}
							else
							{
								num13 = 0.0;
							}
							if (num13 > num7)
							{
								num4 = i;
								num7 = num13;
							}
						}
					}
				}
				if (num7 > 0.0)
				{
					num += num7;
					list2.Add(num4);
				}
				else
				{
					num += (double)PuntajeEvaluator.NO_MATCH;
				}
			}
			if (num3 == tokens.Count && num3 == option.Count)
			{
				num += (double)PuntajeEvaluator.FULL_MATCH;
			}
			num += (double)(list2.Count * PuntajeEvaluator.MATCH_TOKEN);
			if (num6 < num5)
			{
				num = 0.0;
			}
			return num;
		}
	}
}
