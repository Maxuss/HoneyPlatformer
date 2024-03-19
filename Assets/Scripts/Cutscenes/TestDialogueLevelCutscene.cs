using System.Collections;
using Controller;
using Level;
using UnityEngine;

namespace Cutscenes
{
    public class TestDialogueLevelCutscene: MonoBehaviour, ILevelEntranceCutscene
    {
        public void StartCutscene()
        {
            StartCoroutine(Cutscene());
        }

        private IEnumerator Cutscene()
        {
            ToastManager.Instance.ShowToast("Игрок не должен двигаться");
            PlayerController.Instance.IsDisabled = true;
            PlayerController.Instance.StillCommitMovement = false;
            PlayerController.Instance.InCutscene = true;

            yield return new WaitForSeconds(3f);
            
            PlayerController.Instance.IsDisabled = false;
            PlayerController.Instance.StillCommitMovement = true;
            PlayerController.Instance.InCutscene = false;
        }
    }
}