using System;
using System.Collections.Generic;
namespace Geocoder.Core.Interfaces
{
	public interface ITokenizable
	{
		int NivelAbreviatura
		{
			get;
		}
		IList<PalabraPosicionada> Palabras
		{
			get;
		}
	}
}
