using Geocoder.Core;
using Geocoder.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Geocoder.Logic
{
	public class Tokenizer
	{
		public List<string> Tokens
		{
			get;
			set;
		}
		public List<char> Iniciales
		{
			get;
			set;
		}
		public string Frase
		{
			get;
			protected set;
		}
		public int Nivel
		{
			get;
			protected set;
		}
		public int Count
		{
			get
			{
				return this.Tokens.Count;
			}
		}
		public string this[int key]
		{
			get
			{
				return this.Tokens[key];
			}
		}
		protected Tokenizer()
		{
			this.Tokens = new List<string>();
			this.Iniciales = new List<char>();
		}
		protected Tokenizer(ITokenizable tokenizable)
		{
			this.Tokens = new List<string>();
			this.Iniciales = new List<char>();
			foreach (PalabraPosicionada current in tokenizable.Palabras)
			{
				this.Tokens.Add(current.Palabra.Normalizada);
				this.Iniciales.Add(current.Palabra.Literal[0]);
			}
		}
		protected Tokenizer(string frase, int nivel)
		{
			this.Nivel = nivel;
			this.Frase = frase;
			this.Tokens = new List<string>();
			this.Iniciales = new List<char>();
			this.Tokenizar();
		}
		public static Tokenizer FromString(string frase, int nivel)
		{
			return Tokenizer.FromString(frase, nivel, false);
		}
		public static Tokenizer FromString(string frase, int nivel, bool omitirNormalizacion)
		{
			Tokenizer tokenizer = new Tokenizer(frase, nivel);
			if (!omitirNormalizacion)
			{
				tokenizer.Normalizar();
			}
			return tokenizer;
		}
		public static Tokenizer Create(ITokenizable tokenizable)
		{
			return new Tokenizer(tokenizable);
		}
		public string[] ToArray()
		{
			return this.Tokens.ToArray();
		}
		private void Tokenizar()
		{
			if (this.Frase.Length == 0)
			{
				throw new ArgumentException("String vacia , No se puede tokenizar");
			}
			StringBuilder stringBuilder = new StringBuilder();
			string frase = this.Frase;
			int i = 0;
			while (i < frase.Length)
			{
				char c = frase[i];
				char c2 = c;
				switch (c2)
				{
				case ' ':
				case '"':
				case '\'':
				case '(':
				case ')':
				case ',':
				case '-':
				case '.':
					goto IL_92;
				case '!':
				case '#':
				case '$':
				case '%':
				case '&':
				case '*':
				case '+':
					goto IL_D1;
				default:
					if (c2 != ';')
					{
						goto IL_D1;
					}
					goto IL_92;
				}
				IL_DB:
				i++;
				continue;
				IL_92:
				if (stringBuilder.Length > 0)
				{
					this.Tokens.Add(stringBuilder.ToString());
					this.Iniciales.Add(stringBuilder[0]);
					stringBuilder = new StringBuilder();
				}
				goto IL_DB;
				IL_D1:
				stringBuilder.Append(c);
				goto IL_DB;
			}
			if (stringBuilder.Length > 0)
			{
				this.Tokens.Add(stringBuilder.ToString());
				this.Iniciales.Add(stringBuilder[0]);
			}
		}
		public void Normalizar()
		{
			for (int i = 0; i < this.Tokens.Count; i++)
			{
				this.Tokens[i] = Normalizer.Normalizar(this.Tokens[i]);
			}
			Sinonimos.Instance.GetSinonimo(this);
			for (int i = 0; i < this.Tokens.Count; i++)
			{
				this.Tokens[i] = Abreviaturas.Instance.Abreviatura(this.Nivel, this.Tokens[i]);
			}
		}
	}
}
