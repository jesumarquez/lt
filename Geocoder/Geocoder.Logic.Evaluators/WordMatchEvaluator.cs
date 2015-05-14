using System;
using System.Collections.Generic;
namespace Geocoder.Logic.Evaluators
{
	public class WordMatchEvaluator : IEvaluator
	{
		public const double Points = 10.0;
		public double Evaluar(Tokenizer tokens, Tokenizer option)
		{
			List<string> list = new List<string>(tokens.ToArray());
			List<string> list2 = new List<string>(option.ToArray());
			List<char> list3 = new List<char>(option.Iniciales.ToArray());
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < option.Count; i++)
			{
				if (list.Contains(option[i]))
				{
					list.Remove(option[i]);
					list2.Remove(option[i]);
					list3.Remove(option.Iniciales[i]);
					if (this.IsNumeric(option[i]))
					{
						num++;
					}
					else
					{
						if (option[i].Length > 1)
						{
							if (Abreviaturas.Instance.IsAbreviatura(3, option[i]))
							{
								num3++;
							}
							else
							{
								if (SpecialWords.IsSmallWord(option[i]))
								{
									num4++;
								}
								else
								{
									num++;
								}
							}
						}
						else
						{
							num2++;
						}
					}
				}
			}
			double result;
			if (num == 0)
			{
				result = 0.0;
			}
			else
			{
				double num5 = (double)(num + num4) * 10.0 + (double)num2 + (double)num3;
				for (int i = list.Count - 1; i >= 0; i--)
				{
					string text = list[i];
					if (!this.IsNumeric(option[i]))
					{
						for (int j = list2.Count - 1; j >= 0; j--)
						{
							string text2 = list2[j];
							if (!this.IsNumeric(text2))
							{
								if (text2.Length > 3)
								{
									if (!Abreviaturas.Instance.IsAbreviatura(3, text2))
									{
										int num6 = text2.Length / 2;
										if (num6 < text.Length)
										{
											int num7 = 0;
											int num8 = 0;
											while (num8 < text.Length && num8 < text2.Length)
											{
												if (!text[num8].Equals(text2[num8]))
												{
													break;
												}
												num7++;
												num8++;
											}
											if (num7 >= num6)
											{
												list.RemoveAt(i);
												list2.RemoveAt(j);
												list3.RemoveAt(j);
												num5 += 5.0;
												break;
											}
										}
									}
								}
							}
						}
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					if (!this.IsNumeric(list[i]))
					{
						if (list[i].Length == 1)
						{
							for (int j = 0; j < list2.Count; j++)
							{
								if (!this.IsNumeric(list2[j]))
								{
									if (!Abreviaturas.Instance.IsAbreviatura(3, list2[j]))
									{
										if (list[i][0].Equals(list3[j]))
										{
											num5 += 1.0;
										}
									}
								}
							}
						}
					}
				}
				result = num5;
			}
			return result;
		}
		public bool IsNumeric(string text)
		{
			int num;
			return int.TryParse(text, out num);
		}
	}
}
