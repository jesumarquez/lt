using System;
using System.Collections.Concurrent;
using System.Linq;
using Logictracker.Configuration;
using Logictracker.DAL.Factories;
using Logictracker.QuadTree.Data;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Positions;

namespace Logictracker.Process.SpeedTicket
{
    class DispositivoSpeedSpec
    {
        public enum State
        {
            None,
            Exc,
            Inf
        };

        private Repository _repository;

        private readonly byte[] _excesos = new byte[17];
        private readonly byte[] _infraccion = new byte[17];

        internal static DispositivoSpeedSpec Build(Dispositivo dispositivo, Repository repository)
        {
            var rv = new DispositivoSpeedSpec { _repository = repository };

            // desde el nivel 00 al 15 , los que no hay toma 200 km

            for (var i = 0; i < 16; i++)
            {
                rv._infraccion[i] = (byte)dispositivo.GetInfraccionNivel(i);
                rv._excesos[i] = (byte)dispositivo.GetExcesoNivel(i);
            };

            rv._infraccion[16] = (byte)dispositivo.GetInfraccionNivel(-1);
            rv._excesos[16] = (byte)dispositivo.GetExcesoNivel(-1);

            return rv;
        }

        internal State Evaluate(LogPosicion p)
        {
            var qLevel = _repository.GetPositionClass((float)p.Latitud, (float)p.Longitud);
            
           
            if (_infraccion[qLevel] >= p.Velocidad)
                return State.Inf;

            if (_excesos[qLevel] >= p.Velocidad)
                return State.Exc;
            
            return State.None;
        }
    }

    class QtreeInstanceManager
    {
        private readonly DAOFactory _factory;
        private ConcurrentDictionary<string, Repository> repositories;
        private ConcurrentDictionary<int, DispositivoSpeedSpec> speedSpecs;

        public QtreeInstanceManager(DAOFactory factory)
        {
            _factory = factory;

            var coches = _factory.CocheDAO.GetList(new[] { -1 }, new[] { -1 });

            foreach (var c in coches)
            {
                var key = c.Dispositivo.GetQtreeType() + "|" + c.Dispositivo.GetQtreeFile();

                var repo = repositories.GetOrAdd(key, s =>
                    {
                        var so = new GridStructure();
                        var instance = new Repository();
                        instance.Open<GeoGrillas>(Config.Qtree.QtreeDirectory, ref so);
                        return instance;
                    });

                speedSpecs.TryAdd(c.Id, DispositivoSpeedSpec.Build(c.Dispositivo, repo));
            }
        }

        public void Process(int vehicleId, DateTime start, DateTime stop)
        {
            var positions = _factory.LogPosicionDAO.GetPositionsBetweenDates(vehicleId, start, stop);
            var spec = speedSpecs[vehicleId];
            var secuence = positions.Select(p => spec.Evaluate(p));

            var heads = secuence.TakeWhile(s => s == DispositivoSpeedSpec.State.None);
            
            // todos son normales.
            if (heads.Count() == positions.Count())
                return;
            
        }

    }
}
