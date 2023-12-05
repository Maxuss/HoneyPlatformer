using Dialogue;
using Objects;
using Program.Action;
using Program.Trigger;
using UnityEngine;

namespace Program
{
    public class TestProgrammable: Programmable
    {
        [SerializeField]
        private DialogueDefinition dialogueDefinition;
        
        public override ITrigger[] ApplicableTriggers { get; } =
        {
            new TimeoutTrigger(1f),
            new TimeoutTrigger(5f),
            new TimeoutTrigger(10f)
        };

        public override IAction[] ApplicableActions { get; } =
        {
            new DelegatedAction("Test dialogue", instance =>
            {
                instance.StartCoroutine(DialogueManager.Instance.StartDialogue((instance as TestProgrammable)?.dialogueDefinition));
            }),
            LaserEmitter.ToggleLaserAction,
            new DelegatedFloatAction("Debug float data", (instance, val) =>
            {
                Debug.Log($"Executed: {val}");
            }),
            new DelegatedFloatAction("Debug float data x2", (instance, val) =>
            {
                Debug.Log($"Executed: {val * 2}");
            }),
            new DelegatedFloatAction("Debug float data x3", (instance, val) =>
            {
                Debug.Log($"Executed: {val * 3}");
            }),

        };
    }
}