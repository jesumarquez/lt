using System;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Services.Helpers;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{   
    [Serializable]
    public class MobileMessageVo
    {
        public const int IndexFechaHora = 0;
        public const int IndexMensaje = 1;
        public const int IndexChofer = 2;
        public const int IndexResponsable = 3;
        public const int IndexVelocidad = 4;
        public const int IndexGeocerca = 5;

        private string _hora;

        [GridMapping(Index = IndexFechaHora, ResourceName = "Labels", VariableName = "FECHA_HORA", DataFormatString = "{0:G}", AllowGroup = false, InitialSortExpression = true)]
        public DateTime FechaHora { get; set; }

        [GridMapping(Index = IndexMensaje, ResourceName = "Labels", VariableName = "EVENTO")]
        public string Mensaje { get; set; }

        [GridMapping(Index = IndexChofer, ResourceName = "Labels", VariableName = "CHOFER")]
        public string Chofer { get; set; }

        [GridMapping(Index = IndexResponsable, ResourceName = "Labels", VariableName = "RESPONSABLE")]
        public string Responsable { get; set; }

        [GridMapping(Index = IndexVelocidad, ResourceName = "Labels", VariableName = "VELOCIDAD", AllowGroup = false, AllowMove = false)]
        public int? Velocidad { get; set; }

        [GridMapping(Index = IndexGeocerca, ResourceName = "Labels", VariableName = "GEOCERCA")]
        public string Geocerca
        {
            get
            {
                if (_verDirecciones)
                {
                    var dao = new DAOFactory();
                    var coche = dao.CocheDAO.FindById(_movilId);
                    var maxMonths = coche.Empresa != null ? coche.Empresa.MesesConsultaPosiciones : 3;
                    var position = dao.LogPosicionDAO.GetFirstPositionOlderThanDate(_movilId, FechaHora.ToDataBaseDateTime(), maxMonths);
                    var geocercas = dao.ReferenciaGeograficaDAO.GetList(new[] {coche.Empresa != null ? coche.Empresa.Id : -1},
                                                                        new[] {coche.Linea != null ? coche.Linea.Id : -1},
                                                                        new[] {-1});
                    foreach (var geocerca in geocercas)
                    {
                        if (geocerca.Poligono != null && geocerca.Poligono.Contains(position.Latitud, position.Longitud))
                            return geocerca.Descripcion;
                    }
                    return GeocoderHelper.GetDescripcionEsquinaMasCercana(position.Latitud, position.Longitud);
                }

                return string.Empty;
            }
        }

        public string Hora
        {
            get { return (_hora)?? string.Empty; }
            set { _hora = value; }
        }

        [GridMapping(AllowGroup = false, IsDataKey = true)]
        public int Indice { get; set; }

        private bool _verDirecciones;
        private int _movilId;

        public MobileMessageVo(MobileMessage message, bool verDirecciones)
        {
            _verDirecciones = verDirecciones;
            _movilId = message.MovilId;

            FechaHora = message.FechaYHora;
            Mensaje = message.Mensaje;
            Velocidad = message.Velocidad;
            Chofer = message.Chofer;
            Responsable = message.Responsable;
            Hora = message.Hora;
            Indice = message.Indice;
        }
    }
}

