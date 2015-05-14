using Logictracker.Process.Import.Client.DataStrategy;

namespace Logictracker.Process.Import.Client.Mapping
{
    public partial class DataSource
    {
        private IDataStrategy _dataStrategy;
        public IDataStrategy DataStrategy
        { 
            get { return _dataStrategy ?? (_dataStrategy = CreateDataStrategy(Type, Parameter)); }
        }
        private IDataStrategy CreateDataStrategy(string type, IDataSourceParameter[] parameters)
        {
            var qualified = type.IndexOf('.') > -1 || Type.IndexOf(',') > -1;
            var typeName = qualified
                               ? type
                               : string.Format("Logictracker.Process.Import.Client.DataStrategy.{0}DataStrategy", type);

            var dataStrategyType = qualified
                                       ? System.Type.GetType(typeName, false)
                                       : GetType().Assembly.GetType(typeName, false, true);

            if(dataStrategyType == null)
            {
                Logger.Error("No se pudo encontrar la clase " + typeName);
                return null;
            }

            var constructor = dataStrategyType.GetConstructor(new[] {typeof (IDataSourceParameter[])});
            if(constructor == null)
            {
                Logger.Error("No se encuentra un constructor adecuado para la clase " + typeName);
                return null;
            }
            return constructor.Invoke(new object[]{parameters}) as IDataStrategy;
        }
    }
}
