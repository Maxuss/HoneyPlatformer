namespace Program.Action
{
    public interface IAction
    {
        public string Name { get; }

        public void Execute(Programmable instance);

        public IAction Copy();
    }
}