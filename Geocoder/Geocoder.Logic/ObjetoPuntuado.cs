using Geocoder.Core.Interfaces;
using System;

namespace Geocoder.Logic
{
	[Serializable]
	public class ObjetoPuntuado<T> : IObjetoPuntuado<T>, IComparable<IObjetoPuntuado<T>> where T : class
	{
		public double Puntaje
		{
			get;
			set;
		}
		public T Objeto
		{
			get;
			set;
		}
		public ObjetoPuntuado() : this(0.0, default(T))
		{
		}
		public ObjetoPuntuado(double puntaje, T objeto)
		{
			this.Puntaje = puntaje;
			this.Objeto = objeto;
		}
		public int CompareTo(IObjetoPuntuado<T> other)
		{
			return -this.Puntaje.CompareTo(other.Puntaje);
		}
	}
}
