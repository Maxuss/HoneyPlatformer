using UnityEngine;

namespace Level
{
    public class EntranceDoor: MonoBehaviour, ISpawnPos
    {
        [SerializeField] public Transform restartPosition;
        public Transform SpawnPosition => restartPosition;
    }
}