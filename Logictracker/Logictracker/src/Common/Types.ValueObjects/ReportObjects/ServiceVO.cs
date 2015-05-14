#region Usings

using System;

#endregion

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    /// <summary>
    /// Info about the current state of the service.
    /// </summary>
    [Serializable]
    public class ServiceVo
    {
        public const int IndexName = 0;
        public const int IndexDescription = 1;
        public const int IndexStatus = 2;

        /// <summary>
        /// The name of the service.
        /// </summary>
        [GridMapping(Index = IndexName, ResourceName = "Labels", VariableName = "NAME", AllowGroup = false, InitialSortExpression = true)]
        public String Name { get; set; }

        /// <summary>
        /// The description of the service.
        /// </summary>
        [GridMapping(Index = IndexDescription, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = false)]
        public String Description { get; set; }

        /// <summary>
        /// The current status of the service.
        /// </summary>
        [GridMapping(Index = IndexStatus, ResourceName = "Labels", VariableName = "STATE")]
        public String Status { get; set; }

    }
}
