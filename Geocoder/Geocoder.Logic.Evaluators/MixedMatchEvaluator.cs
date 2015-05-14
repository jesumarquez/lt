using System;
using System.Collections.Generic;
namespace Geocoder.Logic.Evaluators
{
	public class MixedMatchEvaluator : IEvaluator
	{
		public const double Points = 900.0;
		public double Evaluar(Tokenizer tokens, Tokenizer option)
		{
			double result;
			if (tokens.Count != option.Count)
			{
				result = 0.0;
			}
			else
			{
				List<string> list = new List<string>(tokens.ToArray());
				for (int i = 0; i < option.Count; i++)
				{
					if (!list.Contains(option[i]))
					{
						result = 0.0;
						return result;
					}
					list.Remove(option[i]);
				}
				result = 900.0;
			}
			return result;
		}
	}
}
