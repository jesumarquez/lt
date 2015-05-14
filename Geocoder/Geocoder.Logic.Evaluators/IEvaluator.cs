using System;
namespace Geocoder.Logic.Evaluators
{
	public interface IEvaluator
	{
		double Evaluar(Tokenizer tokens, Tokenizer option);
	}
}
