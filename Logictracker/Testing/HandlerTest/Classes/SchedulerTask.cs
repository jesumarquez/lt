namespace HandlerTest.Classes
{
    public class SchedulerTask
    {
        public string Name { get; set; }
        public string ClassName { get; set; }
        public SchedulerTask(string name, string className)
        {
            Name = name;
            ClassName = className;
        }
    }
}
