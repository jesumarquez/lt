#region Usings

using System;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;

#endregion

namespace Logictracker.Interfaces.Combustible.Parsers
{
    public class ParserMovimiento : IParser<Movimiento>
    {
        #region Public Methods

		public void Parse(String str, DAOFactory daoFactory)
        {
            STrace.Debug(GetType().FullName, String.Format("String pozo movement: {0}", str));

            var parameters = str.Split(';');

            var mov = new Movimiento
            {
                Fecha = Convert.ToDateTime(parameters[0]),
                FechaIngresoABase = Convert.ToDateTime(parameters[1]),
                Volumen = Convert.ToDouble(parameters[2]),
                Observacion = parameters[3],
                Estado = Convert.ToInt32(parameters[4]),
                Tanque = (parameters[5].Equals(String.Empty) ? null : daoFactory.TanqueDAO.FindByCode(parameters[5])),
                Caudalimetro = null,
                TipoMovimiento = daoFactory.TipoMovimientoDAO.GetByCode(parameters[7]),
                Coche = parameters[8].Equals(String.Empty) ? null : daoFactory.CocheDAO.FindByPatente(-1, parameters[8]),
                Procesado = false,
                Empleado = parameters[9].Equals(String.Empty) ? null : daoFactory.EmpleadoDAO.FindByUpcode(parameters[9])
            };

            if (mov.Volumen == 0) return;

            daoFactory.MovimientoDAO.SaveOrUpdate(mov);
        }

        #endregion
    }
}
