#region Usings

using System;
using Urbetrack.Types.BusinessObjects.Dispositivos;
using Urbetrack.Types.BusinessObjects.Vehiculos;

#endregion

namespace Urbetrack.Model
{
    public static partial class ExtensionMethods
    {
    	///<summary>
    	///</summary>
    	///<param name="d">Dispositivo</param>
    	///<param name="vehicle">Coche</param>
    	///<returns>INode</returns>
    	public static INode ToNode(this Dispositivo d, Coche vehicle)
		{
			var iNodeCode = d.Id;
			var iNodeClassType = d.TipoDispositivo.Fabricante;
			var iNodeType = Type.GetType(iNodeClassType);

			if (iNodeType == null) return null;

			var constructor = iNodeType.GetConstructor(new Type[0]);
			if (constructor == null) return null;

			var iNode = (INode)constructor.Invoke(null);

			iNode.NodeCode = iNodeCode;
			iNode.Identifier = d.Imei;

			if (iNode is IFleetInfo)
			{
				var iFleetInfo = iNode as IFleetInfo;

				//var vehicle = new DAOFactory().CocheDAO.FindMobileByDevice(EditObject.Id);
				//var vehicle = d.Coche;

				var company = vehicle != null ? vehicle.Empresa ?? d.Empresa : d.Empresa;
				var location = vehicle != null ? vehicle.Linea ?? d.Linea : d.Linea;

				iFleetInfo.CodeRegionalUnit = location != null ? location.Id : 0;
				iFleetInfo.CodeOrganizationUnit = company != null ? company.Id : 0;
			}

			return iNode;
		}

		///<summary>
		///</summary>
		///<param name="d"></param>
		///<param name="vehicle"></param>
		///<returns></returns>
		public static String GetNodeConfig(this Dispositivo d, Coche vehicle)
		{
			var node = (IFoteable)d.ToNode(vehicle);
			return node == null ? null : node.GetConfig();
		}
    }
}