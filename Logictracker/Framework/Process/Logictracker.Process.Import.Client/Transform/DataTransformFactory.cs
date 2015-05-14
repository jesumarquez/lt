namespace Logictracker.Process.Import.Client.Transform
{
    public static class DataTransformFactory
    {
        public static IDataTransform GetDataTransform(int version)
        {
            switch(version)
            {
                case 1: return new DataTransformV1();
                default: return new DataTransformV1(); // last version
            }
        }
    }
}
