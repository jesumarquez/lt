using System;
namespace Geocoder.Logic.Evaluators
{
	public class ExactMatchEvaluator : IEvaluator
	{
		public const double Points = 1000.0;
		public double Evaluar(Tokenizer tokens, Tokenizer option)
		{
			double result;
			if (tokens.Count != option.Count)
			{
				result = 0.0;
			}
			else
			{
				for (int i = 0; i < tokens.Count; i++)
				{
					if (!tokens[i].Equals(option[i]))
					{
						result = 0.0;
						return result;
					}
				}
				result = 1000.0;
			}
			return result;
		}
	}
}
