using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace HandlerTest.Classes
{
    public interface ITestApp
    {
        Data Data { get; }
        Empresa Empresa { get; }
        Linea Linea { get; }
        Coche Coche { get; }
        Dispositivo Dispositivo { get; }
        IPosition Position { get; }
        IConfig Config { get; }
        ICiclo Ciclo { get; }
        IMensajeria Mensajeria { get; }
        event EventHandler CocheChanged;
        event EventHandler BaseChanged;

    }
}
