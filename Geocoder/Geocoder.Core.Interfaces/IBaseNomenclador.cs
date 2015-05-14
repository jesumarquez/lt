using Geocoder.Core.VO;
using System;
using System.Collections.Generic;
namespace Geocoder.Core.Interfaces
{
	public interface IBaseNomenclador : IDisposable
	{
		string SessionFactoryConfigPath
		{
			get;
			set;
		}
		ProvinciaVO[] BuscarProvincias();
		IList<PartidoVO> GetPartidos(int idMapProvincia);
		IList<LocalidadVO> GetLocalidades(int idPartido, int idProvincia);
		IList<PoligonalVO> GetPoligonales(int idPartido, int idProvincia);
		IList<PoligonalVO> GetCruces(int idPoligonal, int idMapaUrbano);
		DireccionVO GetEsquinaMasCercana(double lat, double lon);
		DireccionVO GetDireccionMasCercana(double lat, double lon);
		DireccionVO ValidarCruce(int poligonal, int poligonal2, int idMapaUrbano);
		DireccionVO ValidarAltura(int poligonal, int altura, int idMapaUrbano);
	}
}
