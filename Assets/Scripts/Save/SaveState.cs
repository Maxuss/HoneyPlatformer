namespace Save
{
    // TODO: save more data
    [System.Serializable]
    public struct SaveState
    {
        public int SaveIndex { get; set; }
        public int LevelIndex { get; set; }
    }
}