using System.Collections.Generic;

namespace Geocoder.Logic
{
	internal class FraseSinonimo
	{
		public List<string> Frase
		{
			get;
			set;
		}
		public List<string> Sinonimo
		{
			get;
			set;
		}
		public void SetNormalized(string[] frase, string[] sinonimo)
		{
			this.Frase = new List<string>();
			this.Sinonimo = new List<string>();
			for (int i = 0; i < frase.Length; i++)
			{
				this.Frase.Add(Normalizer.Normalizar(frase[i]));
			}
			for (int i = 0; i < sinonimo.Length; i++)
			{
				this.Sinonimo.Add(Normalizer.Normalizar(sinonimo[i]));
			}
		}
		public bool EqualsFrase(List<string> frase)
		{
			bool result;
			if (frase.Count != this.Frase.Count)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < this.Frase.Count; i++)
				{
					if (frase[i] != this.Frase[i])
					{
						result = false;
						return result;
					}
				}
				result = true;
			}
			return result;
		}
	}
}
