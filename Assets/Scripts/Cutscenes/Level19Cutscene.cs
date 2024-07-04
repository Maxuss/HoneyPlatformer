using Level;
using Program;
using UnityEngine;

namespace Cutscenes
{
    public class Level19Cutscene: MonoBehaviour, ILevelEntranceCutscene
    {
        public void StartCutscene()
        {
            ToastManager.Instance.ShowToast("Телепорты также могут перемещать объекты");
        }
    }
}