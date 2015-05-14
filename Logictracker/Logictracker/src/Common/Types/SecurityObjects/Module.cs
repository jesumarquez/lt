#region Usings

using System;
using Logictracker.Types.BusinessObjects;

#endregion

namespace Logictracker.Types.SecurityObjects
{
    /// <summary>
    /// Application Module
    /// </summary>
    [Serializable]
    public class Module
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item">MovMenu item for this module</param>
        public Module(MovMenu item)
        {
            Id = item.Funcion.Id;
            Name = item.Funcion.Descripcion;
            RefName = item.Funcion.Ref;
            Url = item.Funcion.Url;
            Add = item.Alta;
            Delete = item.Baja;
            Edit = item.Modificacion;
            View = item.Consulta;
            Report = item.Reporte;
            GroupId = item.Funcion.Sistema.Id;
            GroupUrl = item.Funcion.Sistema.Url;
            Group = item.Funcion.Sistema.Descripcion;
            GroupOrder = item.Funcion.Sistema.Orden;
            ModuleOrder = item.Orden;
            ModuleSubGroup = item.Funcion.Modulo;
            Parameters = item.Funcion.Parametros;
        }

        /// <summary>
        /// MovMenu Id
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Module Name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Module Reference string
        /// </summary>
        public string RefName { get; private set; }

        /// <summary>
        /// Module Start Page URL
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Can Create Items
        /// </summary>
        public bool Add { get; private set; }

        /// <summary>
        /// Can Delete Items
        /// </summary>
        public bool Delete { get; private set; }

        /// <summary>
        /// Can Edit Items
        /// </summary>
        public bool Edit { get; private set; }

        /// <summary>
        /// Can View Items
        /// </summary>
        public bool View { get; private set; }

        /// <summary>
        /// Can Generate Reportes
        /// </summary>
        public bool Report { get; private set; }

        /// <summary>
        /// Module Group Name
        /// </summary>
        public string Group { get; private set; }

        /// <summary>
        /// The group url.
        /// </summary>
        public string GroupUrl { get; private set; }

        /// <summary>
        /// The query parameters that should be used when invoking the module.
        /// </summary>
        public string Parameters { get; private set; }

        /// <summary>
        /// The id of the associated group.
        /// </summary>
        public int GroupId { get; private set; }

        /// <summary>
        /// The order of the associated group.
        /// </summary>
        public int GroupOrder { get; private set; }

        /// <summary>
        /// The order of the module.
        /// </summary>
        public int ModuleOrder { get; private set; }

        /// <summary>
        /// The sub group to wich the module is associated.
        /// </summary>
        public string ModuleSubGroup { get; private set; }
    }
}