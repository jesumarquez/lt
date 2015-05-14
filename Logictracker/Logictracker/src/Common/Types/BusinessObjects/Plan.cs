using System;
using Logictracker.Types.InterfacesAndBaseClasses;
using System.Text;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class Plan : IAuditable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion
        
        public virtual int TipoLinea { get; set; }
        public virtual int Empresa { get; set; }
        public virtual string CodigoAbono { get; set; }
        public virtual double CostoMensual { get; set; }
        public virtual int AbonoDatos { get; set; }
        public virtual int UnidadMedida { get; set; }
        public virtual bool Baja { get; set; }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append(CodigoAbono + " - ");
            
            switch (Empresa)
            {
                case 1: str.Append("Claro"); break;
                case 2: str.Append("Movistar"); break;
                case 3: str.Append("Personal"); break;
                case 4: str.Append("Orbcom"); break;
            }
            
            return str.ToString();
        }
    }
}