using System;
using System.Collections.Generic;
namespace Geocoder.Logic.Evaluators
{
	internal class TokenPosicionadoComparer : IComparer<TokenPosicionado>
	{
		public int Compare(TokenPosicionado x, TokenPosicionado y)
		{
			return -x.Token.Length.CompareTo(y.Token.Length);
		}
	}
}
