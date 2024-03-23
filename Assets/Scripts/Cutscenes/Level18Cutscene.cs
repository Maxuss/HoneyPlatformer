using System.Collections;
using Controller;
using Dialogue;
using Level;
using Save;
using UnityEngine;

namespace Cutscenes
{
    public class Level18Cutscene: MonoBehaviour, ILevelEntranceCutscene
    {        
        public void StartCutscene()
        {
            ToastManager.Instance.ShowToast("Телепорты позволяют перемещать объекты на разные расстояния.");
        }
    }
}