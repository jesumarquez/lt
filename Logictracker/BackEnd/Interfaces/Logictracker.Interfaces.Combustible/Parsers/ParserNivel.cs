#region Usings

using System;
using Logictracker.DAL.Factories;
using Logictracker.Interfaces.Combustible.Helpers;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;

#endregion

namespace Logictracker.Interfaces.Combustible.Parsers
{
    public class ParserNivel : IParser<VolumenHistorico>
    {
        #region Public Methods

		public void Parse(String str, DAOFactory daoFactory)
        {
            var parameters = str.Split(';');

            var tank = daoFactory.TanqueDAO.FindByCode(parameters[3]);

            var vol = new VolumenHistorico
                       {
                           Fecha = Convert.ToDateTime(parameters[0]),
                           Volumen = Convert.ToDouble(parameters[1]),
                           EsTeorico = parameters[2].Equals("1"),
                           Tanque = tank,
                           VolumenAgua = Convert.ToDouble(parameters[4])
                       };

            if (vol.Volumen < 0) return;

            daoFactory.VolumenHistoricoDAO.SaveOrUpdate(vol);

            EventHelper.EvaluateStockCriticoAndReposicion(vol.Volumen, tank, vol.Fecha);
            EventHelper.EvaluteWaterLevel(vol.VolumenAgua, tank, vol.Fecha);
        }
        #endregion
    }
}
