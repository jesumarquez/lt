using System;
using System.Collections.Generic;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Entidades
{
    [Serializable]
    public class Detalle : IHasTipoEntidad, IAuditable, IHasDetallePadre
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual TipoEntidad TipoEntidad { get; set; }
        public virtual Detalle DetallePadre { get; set; }
        
        public virtual string Nombre { get; set; }
        public virtual int Tipo { get; set; }
        public virtual int Representacion { get; set; }
        public virtual int Orden { get; set; }
        public virtual bool EsFiltro { get; set; }
        public virtual bool Obligatorio { get; set; }
        public virtual string Mascara { get; set; }
        public virtual string Opciones { get; set; }
        public virtual bool Baja { get; set; }

        public override string ToString() { return Nombre; }

        private List<Option> _options;
        public virtual List<Option> Options
        {
            get
            {
                if (_options == null)
                {
                    _options = new List<Option>();
                    var options = Opciones.Split('|');
                    foreach (var option in options)
                    {
                        string b = "";
                        var text = option;
                        var idx = text.IndexOf('.');
                        var a = option.Substring(0, idx);
                        text = text.Substring(idx + 1);
                        if (HasParent)
                        {
                            idx = text.IndexOf('.');
                            b = text.Substring(0, idx);
                            text = text.Substring(idx + 1);
                        }
                        var op = new Option {Index = HasParent ? b : a, Parent = HasParent ? a : "", Text = text};
                        _options.Add(op);
                    }
                }
                return _options;
            }
        }
        public virtual bool HasParent
    {
            get{return DetallePadre != null;}
    }

        public class Option
        {
            public string Index { get; set; }
            public string Parent { get; set; }
            public string Text { get; set; }

        }
    }
}