namespace Logictracker.Web.Helpers.FussionChartHelpers
{
    /// <summary>
    /// Represents a fusion chart xml definition file element.
    /// </summary>
    public interface IFusionChartXML
    {
        /// <summary>
        /// Returns the xml line that represents the item.
        /// </summary>
        /// <returns></returns>
        string ToXML();
    }
}
