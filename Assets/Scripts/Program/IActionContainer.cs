using System;
using Controller;
using Objects;
using Program.UI;

namespace Program
{
    public interface IActionContainer: IInteractable
    {
        public string Name { get; }
        public string Description { get; }
        public ActionInfo[] SupportedActions { get; }
        public ProgrammableType Type { get; }
        
        public ActionData SelectedAction { get; set; }

        public void Begin(ActionData action);

        void IInteractable.OnInteract()
        {
            PlayerController.Instance.IsDisabled = true;
            
            ProgrammableUIManager.Instance.OpenFor(this);
        }
    }

    public enum ProgrammableType
    {
        Emitter,
        Executor
    }
}