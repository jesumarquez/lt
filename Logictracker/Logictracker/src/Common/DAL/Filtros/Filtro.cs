using System;

namespace Logictracker.DAL.Filtros
{
    [Serializable]
    public class Filtro
    {
        public virtual int IdUnion { get; set; }
        public virtual int IdDetalle { get; set; }
        public virtual int IdOperador { get; set; }
        public virtual string ValorStr { get; set; }
        public virtual DateTime ValorDt { get; set; }
        public virtual double ValorNum { get; set; }
    }
}