namespace Program.Action
{
    public delegate void ExecuteDelegatedAction(Programmable instance);
    
    public class DelegatedAction : IAction
    {
        public string Name { get; }
        private ExecuteDelegatedAction _action;
        public DelegatedAction(string name, ExecuteDelegatedAction action)
        {
            Name = name;
            _action = action;
        }
        
        public void Execute(Programmable instance)
        {
            _action.Invoke(instance);
        }

        public IAction Copy()
        {
            return new DelegatedAction(Name, _action);
        }
    }
}