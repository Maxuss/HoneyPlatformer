namespace Program.Trigger
{
    public interface ITrigger
    {
        public bool ShouldTrigger(Programmable programmable);

        public void Start();

        public void Activated()
        {
            
        }
    }
}