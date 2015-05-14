#region Usings

using System.Diagnostics;
using System.Linq;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Model;

#endregion

namespace Logictracker.Description.Common
{
    [FrameworkElement(XName = "StackChannel", IsContainer = true)]
    public class StackChannelElement : FrameworkElement
    {
        public override bool LoadResources()
        {
            ILayer previusLayer = null;
            ILayer currentLayer = null;
			foreach (var nextLayer in Elements().OfType<ILayer>())
            {
            	Debug.Assert(nextLayer != null);
            	if (currentLayer != null)
            	{
            		if (!currentLayer.StackBind(previusLayer, nextLayer)) return false;
            	}
            	previusLayer = currentLayer;
            	currentLayer = nextLayer;
            }
            if (currentLayer != null)
                currentLayer.StackBind(previusLayer, null);
            return true;
        }
    }
}
