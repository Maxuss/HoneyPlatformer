using System.Collections;
using Controller;
using Dialogue;
using Level;
using UnityEngine;

namespace Cutscenes
{
    public class TestDialogue: MonoBehaviour
    {
        [SerializeField]
        private DialogueDefinition dialogue;
        
        private IEnumerator DialogueStart()
        {
            yield return new WaitForSeconds(1);
            Debug.Log(dialogue);
            yield return DialogueManager.Instance.StartDialogue(dialogue);
            ToastManager.Instance.ShowToast("Удерживать SHIFT - тащить объект за собой. Или толкай его!");
        }

        public void Start()
        {
            StartCoroutine(DialogueStart());
        }
    }
}