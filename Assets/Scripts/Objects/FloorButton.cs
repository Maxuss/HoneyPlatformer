using Program;
using Program.Action;
using Program.Trigger;

namespace Objects
{
    public class FloorButton: Programmable
    {
        public override ITrigger[] ApplicableTriggers { get; }
        public override IAction[] ApplicableActions { get; }
    }
}