namespace Logictracker.Web.Controls
{
    public interface IDataBinder
    {
        void DataBind(IBindableListControl control);
        int ItemAllValue { get; }
        int ItemNoneValue { get; }
        string ItemAllName { get; }
        string ItemNoneName { get; }
    }
}
