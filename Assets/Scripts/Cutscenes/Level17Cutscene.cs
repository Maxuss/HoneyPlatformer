using I18N;
using Level;
using UnityEngine;

namespace Cutscenes
{
    public class Level17Cutscene: MonoBehaviour, ILevelEntranceCutscene
    {
        public void StartCutscene()
        {
            ToastManager.Instance.ShowToast(LocalizationManager.Instance.Translated("toast.level17"));
        }
    }
}