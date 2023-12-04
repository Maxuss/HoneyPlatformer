namespace Program.Action
{
    public delegate void ExecuteDelegatedFloatAction(Programmable instance, float value);
    
    public class DelegatedFloatAction : IFloatAction
    {
        public string Name { get; }
        public float FloatData { get; set; }

        private ExecuteDelegatedFloatAction _action;
        public DelegatedFloatAction(string name, ExecuteDelegatedFloatAction action)
        {
            Name = name;
            _action = action;
        }

        public void Execute(Programmable instance)
        {
            _action?.Invoke(instance, FloatData);
        }

        public IAction Copy()
        {
            return new DelegatedFloatAction(Name, _action);
        }

    }
}