namespace Logictracker.Process.Import.Client.Mapping
{
    public partial class Operation
    {
        public static Types.Operation GetEquivalent(OperationType operationType)
        {
            switch(operationType)
            {
                case OperationType.Add: return Types.Operation.Add;
                case OperationType.AddOrModify: return Types.Operation.AddOrModify;
                case OperationType.Modify: return Types.Operation.Modify;
                case OperationType.Delete: return Types.Operation.Delete;
                case OperationType.None: return Types.Operation.None;
                default: return Types.Operation.AddOrModify;
            }
        }
    }
}
