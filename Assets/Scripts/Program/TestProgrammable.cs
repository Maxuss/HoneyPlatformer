using Objects;
using Program.Action;
using Program.Trigger;

namespace Program
{
    public class TestProgrammable: Programmable
    {
        public override ITrigger[] ApplicableTriggers { get; } =
        {
            new TimeoutTrigger(5f),
            new TimeoutTrigger(1f)
        };

        public override IAction[] ApplicableActions { get; } =
        {
            LaserEmitter.ToggleLaserAction
        };
    }
}