using System;
namespace Geocoder.Core
{
	public class Abreviatura
	{
		private int id;
		private int nivel;
		private string literal;
		private string abreviado;
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
		public virtual int Nivel
		{
			get
			{
				return this.nivel;
			}
			set
			{
				this.nivel = value;
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
				this.literal = value;
			}
		}
		public virtual string Abreviado
		{
			get
			{
				return this.abreviado;
			}
			set
			{
				this.abreviado = value;
			}
		}
		public Abreviatura() : this(3, string.Empty, string.Empty)
		{
		}
		public Abreviatura(int nivel, string literal, string abreviado)
		{
			this.nivel = nivel;
			this.literal = literal;
			this.abreviado = abreviado;
		}
	}
}
