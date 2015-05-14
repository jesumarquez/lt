using System;
namespace Geocoder.Core.Interfaces
{
	public interface IObjetoPuntuado<T> : IComparable<IObjetoPuntuado<T>>
	{
		double Puntaje
		{
			get;
			set;
		}
		T Objeto
		{
			get;
			set;
		}
	}
}
