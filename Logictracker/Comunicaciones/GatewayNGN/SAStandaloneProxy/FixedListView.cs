#region Usings

using System.Windows.Forms;

#endregion

namespace Urbetrack.GatewayMovil
{
    class FixedListView : ListView
    {
        public FixedListView() 
        {  
            // Activamos el DoubleBuffering  
            // http://en.wikipedia.org/wiki/Double_buffering  
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);  
   
            // Activamos el evento OnNotifyMessage, asi tenemos la oportunidad  
            // de filtrar los mensajes de Windows antes de que lleguen al WndProc  
            // del formulario  
            SetStyle(ControlStyles.EnableNotifyMessage, true);  
        }  
   
        protected override void OnNotifyMessage(Message m)  
        {  
            // Filtramos el mensaje WM_ERASEBKGND  
   
            if (m.Msg != 0x14)  
            {  
                base.OnNotifyMessage(m);  
            }  
        }  
    }
}