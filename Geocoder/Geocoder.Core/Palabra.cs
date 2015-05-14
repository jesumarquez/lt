using System;
namespace Geocoder.Core
{
	[Serializable]
	public class Palabra
	{
		private int id;
		private string literal;
		private string normalizada;
		private int prefix1;
		private int prefix2;
		private int prefix3;
		public virtual int Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}
		public virtual string Literal
		{
			get
			{
				return this.literal;
			}
			set
			{
				this.literal = value.ToLower();
			}
		}
		public virtual string Normalizada
		{
			get
			{
				return this.normalizada;
			}
			set
			{
				if (this.normalizada != value && value != null)
				{
					this.GenerarPrefix(value);
				}
				this.normalizada = value;
			}
		}
		public virtual int Prefix1
		{
			get
			{
				return this.prefix1;
			}
			set
			{
				this.prefix1 = value;
			}
		}
		public virtual int Prefix2
		{
			get
			{
				return this.prefix2;
			}
			set
			{
				this.prefix2 = value;
			}
		}
		public virtual int Prefix3
		{
			get
			{
				return this.prefix3;
			}
			set
			{
				this.prefix3 = value;
			}
		}
		public Palabra() : this(null, null)
		{
		}
		public Palabra(string literal, string normalizada)
		{
			this.id = 0;
			this.literal = literal;
			this.normalizada = normalizada;
			if (normalizada != null)
			{
				this.GenerarPrefix(normalizada);
			}
		}
		private void GenerarPrefix(string palabra)
		{
			this.prefix1 = Prefix.Prefix6Min(palabra);
			if (palabra.Length > 6)
			{
				this.prefix2 = Prefix.Prefix12Min(palabra);
			}
			if (palabra.Length > 12)
			{
				this.prefix3 = Prefix.Prefix18Min(palabra);
			}
		}
	}
}
