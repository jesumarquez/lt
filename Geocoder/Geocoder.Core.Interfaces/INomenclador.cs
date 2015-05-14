using Geocoder.Core.VO;
using System;
using System.Collections.Generic;
namespace Geocoder.Core.Interfaces
{
	public interface INomenclador : IBaseNomenclador, IDisposable
	{
		IList<DireccionVO> GetDireccion(string calle, int altura, string esquina, string partido, int provincia);
		IList<ProvinciaVO> GetProvincias(string nombre);
		IList<PartidoVO> GetPartidoEnProvinciaMapId(string nombre, int idMapProvincia);
		IList<DireccionVO> GetSmartSearch(string frase);
		IList<DireccionVO> GetSmartSearchLatLon(string frase, double lat, double lon);
	}
}
