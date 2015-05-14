using System;
using System.Collections.Generic;
namespace Geocoder.Logic.Evaluators
{
	public class Evaluador
	{
		private Tokenizer tokens;
		private Tokenizer option;
		private List<IEvaluator> evaluators;
		public Tokenizer Tokens
		{
			get
			{
				return this.tokens;
			}
			set
			{
				this.tokens = value;
			}
		}
		public Tokenizer Option
		{
			get
			{
				return this.option;
			}
			set
			{
				this.option = value;
			}
		}
		public List<IEvaluator> Evaluators
		{
			get
			{
				return this.evaluators;
			}
			set
			{
				this.evaluators = value;
			}
		}
		public Evaluador() : this(null, new IEvaluator[0])
		{
		}
		public Evaluador(Tokenizer ambos, params IEvaluator[] evaluator) : this(ambos, ambos, evaluator)
		{
		}
		public Evaluador(Tokenizer tokens, Tokenizer option, params IEvaluator[] evaluator)
		{
			this.tokens = tokens;
			this.option = option;
			this.evaluators = new List<IEvaluator>(evaluator);
		}
		public double Evaluar(Tokenizer opt)
		{
			this.Option = opt;
			return this.Evaluar();
		}
		public double Evaluar()
		{
			double result;
			foreach (IEvaluator current in this.evaluators)
			{
				double num = current.Evaluar(this.tokens, this.option);
				if (num > 0.0)
				{
					result = num;
					return result;
				}
			}
			result = 0.0;
			return result;
		}
	}
}
