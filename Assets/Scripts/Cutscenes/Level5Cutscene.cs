using System.Collections;
using Controller;
using Level;
using UnityEngine;
using Utils;

namespace Cutscenes
{
    public class Level5Cutscene: MonoBehaviour, ILevelEntranceCutscene
    {
        [SerializeField]
        private Transform beePos;
        [SerializeField]
        private Transform shieldPos;
        
        public void StartCutscene()
        {
            StartCoroutine(Cutscene());
        }

        private IEnumerator Cutscene()
        {
            PlayerController.Instance.IsDisabled = true;
            PlayerController.Instance.InCutscene = true;
            CameraController.Instance.DisableFollow = true;
            PlayerController.Instance.StillCommitMovement = false;
            yield return new WaitForSeconds(1f);
            yield return CameraController.Instance.TransitionToPoint(beePos.position.XY(), 2f);
            ToastManager.Instance.ShowToast("Генераторы щитов отгоняют ос!");
            yield return new WaitForSeconds(0.5f);
            yield return CameraController.Instance.TransitionToPoint(shieldPos.position.XY(), 2f);
            yield return new WaitForSeconds(1f);
            CameraController.Instance.DisableFollow = false;
            PlayerController.Instance.IsDisabled = false;
            PlayerController.Instance.StillCommitMovement = true;
            PlayerController.Instance.InCutscene = false;
        }
    }
}