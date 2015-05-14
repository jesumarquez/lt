using System;
namespace Geocoder.Logic.Evaluators
{
	public class CharMatchEvaluator : IEvaluator
	{
		public const double Points = 1.0;
		public double Evaluar(Tokenizer tokens, Tokenizer option)
		{
			int num = 0;
			for (int i = 0; i < tokens.Count; i++)
			{
				string text = tokens[i];
				for (int j = 0; j < option.Count; j++)
				{
					string text2 = option[j];
					if (text2.Length > 3)
					{
						if (!SpecialWords.IsSmallWord(text2))
						{
							int num2 = text2.Length / 2;
							if (num2 < text.Length)
							{
								int num3 = 0;
								int num4 = 0;
								while (num4 < text.Length && num4 < text2.Length)
								{
									if (text[num4] == text2[num4])
									{
										num3++;
									}
									num4++;
								}
								if (num3 >= num2)
								{
									num += num3;
								}
							}
						}
					}
				}
			}
			return (double)num * 1.0;
		}
	}
}
