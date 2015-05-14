namespace Logictracker.Description.Runtime
{
    public class SmartProperty
    {
        private readonly SmartProperty _binding;
        private object _value;

        public SmartProperty(object target)
        {
            _value = _binding = null;
            if (target is SmartProperty)
                _binding = target as SmartProperty;
            else
                _value = target;
        }

        public object GetValue()
        {
            return _binding != null ? _binding.GetValue() : _value;
        }

        public void SetValue(object value)
        {
            if (_binding != null)
            {
                _binding.SetValue(value);
                return;
            }
            _value = value;
        }
    }
}
