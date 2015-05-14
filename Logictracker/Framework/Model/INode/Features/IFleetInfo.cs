namespace Urbetrack.Model
{
    /// <summary>
    /// Datos de Flota para INode
    /// </summary>
    public interface IFleetInfo
    {
        /// <summary>
        /// (parenti01_fantasia) codigo o interno del vehiculo en la plataforma.
        /// </summary>
        string OrganizationUnit { get; set; }

        /// <summary>
        /// (parenti02_descri) codigo o interno del vehiculo en la plataforma.
        /// </summary>
        string RegionalUnit { get; set; }

        /// <summary>
        /// (parenti17_descripcion) Flota o Tipo de coche
        /// </summary>
        string FleetUnit { get; set; }

        /// <summary>
        /// (parenti03_interno) codigo o interno del vehiculo en la plataforma.
        /// </summary>
        string VehicleUnit { get; set; }

        /// <summary>
        /// (parenti08_codigo) codigo del dispositivo en la plataforma.
        /// </summary>
        string CodeUnit { get; set; }

        /// <summary>
        /// (rela_parenti02) clave primaria de la tabla de Base(s) Regional
        /// </summary>
        int CodeRegionalUnit { get; set; }

        /// <summary>
        /// (rela_parenti01) clave primaria de la tabla de Distrito
        /// </summary>
        int CodeOrganizationUnit { get; set; }
    }
}
