#region Usings

using System;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Utils;

#endregion

namespace Logictracker.Web.Documentos
{
    public class DocumentFieldComparer : ICustomComparer<Documento>
    {
        private readonly string field;
        private bool descending;

        public DocumentFieldComparer(string field)
        {
            this.field = field;
        }

        #region ICustomComparer<Documento> Members

        public int Compare(Documento x, Documento y)
        {
            var xNull = !x.Valores.ContainsKey(field);
            var yNull = !y.Valores.ContainsKey(field);

            if (xNull && yNull) return 0;
            if (xNull) return descending ? 1 : -1;
            if (yNull) return descending ? -1 : 1;


            var xc = x.Valores[field] as IComparable;
            var yc = y.Valores[field] as IComparable;

            int retval;

            if (xc == null || yc == null)
                retval = x.Valores[field].ToString().CompareTo(y.Valores[field]);
            else
                retval = xc.CompareTo(yc);

            return descending ? -retval : retval;
        }

        public bool Descending
        {
            get { return descending; }
            set { descending = value; }
        }

        #endregion
    }
}