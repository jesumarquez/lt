using System;

namespace Logictracker.Web.BaseClasses.BasePages
{
    /// <summary>
    /// Google Analytics Custom Variable Definition
    /// </summary>
    public class GaCustomVar
    {
        public static class Scopes
        {
            public const int VisitorLevel = 1;
            public const int SessionLevel = 2;
            public const int Pagelevel = 3;
        }
        public int Index { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int Scope { get; set; }
        public GaCustomVar(int index, string name, string value, int scope)
        {
            if (index < 1 || index > 5)
            {
                throw new ApplicationException("Valor inválido para index. debe ser un valor entre 1 y 5.");
            }
            if(scope < 1 || scope > 3)
            {
                throw new ApplicationException("Valor inválido para scope. debe ser un valor entre 1 y 3.");
            }
            Index = index;
            Name = name;
            Value = value;
            Scope = scope;
        }
    }
}
