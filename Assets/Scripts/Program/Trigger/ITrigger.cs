namespace Program.Trigger
{
    public interface ITrigger
    {
        public string Name { get; }
        
        public bool ShouldTrigger(Programmable programmable);

        public void Start();

        public ITrigger Copy();

        public void Activated()
        {
            
        }
    }
}