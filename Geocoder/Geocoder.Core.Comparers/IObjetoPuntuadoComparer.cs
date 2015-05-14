using Geocoder.Core.Interfaces;
using System;
using System.Collections.Generic;
namespace Geocoder.Core.Comparers
{
	public class IObjetoPuntuadoComparer<T> : IComparer<IObjetoPuntuado<T>>
	{
		public int Compare(IObjetoPuntuado<T> x, IObjetoPuntuado<T> y)
		{
			return y.Puntaje.CompareTo(x.Puntaje);
		}
	}
}
