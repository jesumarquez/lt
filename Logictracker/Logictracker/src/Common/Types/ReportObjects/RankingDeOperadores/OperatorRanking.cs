#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects.RankingDeOperadores
{
    /// <summary>
    /// Represents the ranking of a operator.
    /// </summary>
    [Serializable]
    public class OperatorRanking
    {
        #region Public Properties

        /// <summary>
        /// The operator ID
        /// </summary>
        public int IdOperador { get; set; }

        /// <summary>
        /// The operator.
        /// </summary>
        public string Operador { get; set; }

        /// <summary>
        /// The identification code of the operator.
        /// </summary>
        public string Legajo { get; set; }

        /// <summary>
        /// The total amount of traveled kilometers.
        /// </summary>
        public double Kilometros { get; set; }

        /// <summary>
        /// The tototal hour part of the time traveled.
        /// </summary>
        public TimeSpan HorasMovimiento { get { return TimeSpan.FromHours(Hours); } }

        /// <summary>
        /// The overall qualification.
        /// </summary>
        public double Puntaje { get; set; }

        /// <summary>
        /// The total amount of minor infractions.
        /// </summary>
        public int InfraccionesLeves { get; set; }

        /// <summary>
        /// The total amount of average infractions.
        /// </summary>
        public int InfraccionesMedias { get; set; }

        /// <summary>
        /// The total amount of serious infractions.
        /// </summary>
        public int InfraccionesGraves { get; set; }

        /// <summary>
        /// The total amount of infractions.
        /// </summary>
        public int InfraccionesTotales { get { return InfraccionesLeves + InfraccionesMedias + InfraccionesGraves; } }

        /// <summary>
        /// Auxiliar property for handling hours.
        /// </summary>
        public double Hours { get; set; }

        #endregion
    }
}