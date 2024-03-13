using UnityEngine;

namespace Level
{
    public class SpawnTransform: MonoBehaviour, ISpawnPos
    {
        public Transform SpawnPosition => transform;
    }
}