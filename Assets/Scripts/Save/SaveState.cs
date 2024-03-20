using System.Collections.Generic;

namespace Save
{
    // TODO: save more data
    [System.Serializable]
    public struct SaveState
    {
        public int SaveIndex { get; set; }
        public int LevelIndex { get; set; }
        
        public int Currency { get; set; }
        public bool MetDon { get; set; }
        public List<DonUpgrade> DonUpgrades { get; set; }
    }

    [System.Serializable]
    public enum DonUpgrade
    {
        HigherJumps,
        FasterSpeed,
        FasterCamera
    }
}