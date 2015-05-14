using System;
namespace Geocoder.Logic.Evaluators
{
	internal struct TokenPosicionado
	{
		public string Token;
		public int posicion;
		public TokenPosicionado(string token, int posicion)
		{
			this.Token = token;
			this.posicion = posicion;
		}
	}
}
