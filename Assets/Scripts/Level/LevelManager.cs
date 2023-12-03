using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Level
{
    public class LevelManager: MonoBehaviour
    {
        [SerializeField]
        private Tilemap levelMap;

        public (int, int) TileSize { get; private set; }
        public (int, int) PixelSize { get; private set; }
        public Bounds MapBounds { get; private set; }
        
        public static LevelManager Instance { get; private set; }
        public const int TilePixelSize = 16;

        public void Start()
        {
            RecalculateBounds();

            Instance = this;
        }
        
        private void RecalculateBounds()
        {
            levelMap.CompressBounds();
            var tileSize = levelMap.size;
            TileSize = (tileSize.x, tileSize.y);
            PixelSize = (tileSize.x * TilePixelSize, tileSize.y * TilePixelSize);
            MapBounds = levelMap.localBounds;
        }

        public void SwitchLevel(Tilemap newTilemap)
        {
            levelMap = newTilemap;
            RecalculateBounds();
        }
    }
}