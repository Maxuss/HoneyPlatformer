using System;
using System.Collections.Generic;

namespace Save
{
    // TODO: save more data
    [Serializable]
    public struct SaveState
    {
        public int SaveIndex;
        public int LevelIndex;
        public string LevelName;

        public int Currency;
        public bool MetDon;
        public List<DonUpgrade> DonUpgrades;
        
    }

    [Serializable]
    public enum DonUpgrade
    {
        BaseUpgrade,
        MovSpeed,
        CamSpeed,
        JumpHeight,
        EmPillow,
        Hints,
        FullAccess,
    }
}