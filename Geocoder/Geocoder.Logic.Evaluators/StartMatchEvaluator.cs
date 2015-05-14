using System;
namespace Geocoder.Logic.Evaluators
{
	public class StartMatchEvaluator : IEvaluator
	{
		public const double Points = 1.0;
		public double Evaluar(Tokenizer tokens, Tokenizer option)
		{
			double result;
			if (tokens.Count > 1 || option.Count > 1)
			{
				result = 0.0;
			}
			else
			{
				int num = 0;
				for (int i = 0; i < tokens.Count; i++)
				{
					string text = tokens[i];
					if (!this.IsNumeric(text))
					{
						for (int j = 0; j < option.Count; j++)
						{
							string text2 = option[j];
							if (text2.Length > 3)
							{
								if (!SpecialWords.IsSmallWord(text2))
								{
									double num2 = Math.Floor((double)text2.Length * 0.75);
									if (num2 < (double)text.Length)
									{
										int num3 = 0;
										int num4 = 0;
										while (num4 < text.Length && num4 < text2.Length)
										{
											if (text[num4] != text2[num4])
											{
												break;
											}
											num3++;
											num4++;
										}
										if ((double)num3 >= num2)
										{
											num += num3;
										}
									}
								}
							}
						}
					}
				}
				result = (double)num * 1.0;
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
