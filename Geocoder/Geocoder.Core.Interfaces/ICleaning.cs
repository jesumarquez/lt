using Geocoder.Core.VO;
using System;
using System.Collections.Generic;
namespace Geocoder.Core.Interfaces
{
	public interface ICleaning : IBaseNomenclador, IDisposable
	{
		IList<ProvinciaVO> NomenclarProvincia(string provincia);
		IList<PartidoVO> NomenclarPartido(string partido, int provincia);
		IList<PoligonalVO> NomenclarPoligonal(string poligonal, int partido, int provincia);
		IList<PoligonalVO> NomenclarPoligonal(string poligonal, int provincia);
		IList<PoligonalVO> NomenclarPoligonal(string poligonal, int partido, int provincia, int altura);
		IList<DireccionVO> NomenclarDireccion(string calle, int altura, string esquina, string partido, string provincia);
		IList<DireccionVO> GetSmartSearch(string frase);
	}
}
