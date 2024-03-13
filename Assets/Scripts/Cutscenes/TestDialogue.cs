using System.Collections;
using Controller;
using Dialogue;
using Level;
using UnityEngine;

namespace Cutscenes
{
    public class TestDialogue: MonoBehaviour
    {
        public void Start()
        {
            ToastManager.Instance.ShowToast("Тут будет катсцена (правда)");
        }
    }
}