namespace Objects
{
    public delegate void MirrorChanged();
    
    public static class LaserManager
    {
        public static MirrorChanged OnMirrorChanged;
    }
}