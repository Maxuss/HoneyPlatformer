using NPC;
using UnityEngine;

namespace Cutscenes
{
    public class Level25Cutscene: MonoBehaviour, ILevelEntranceCutscene
    {
        [SerializeField]
        private NpcController captain;
        
        public void StartCutscene()
        {
            captain.transform.position -= new Vector3(0f, 100f, 0);
            StartCoroutine(captain.Walk(12f, .8f));
        }
    }

}