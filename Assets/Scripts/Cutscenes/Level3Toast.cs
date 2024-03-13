using Level;
using UnityEngine;

namespace Cutscenes
{
    public class Level3Toast: MonoBehaviour, ILevelEntranceCutscene
    {
        public void StartCutscene()
        {
            ToastManager.Instance.ShowToast("V - режим редактирования");
        }
    }
}