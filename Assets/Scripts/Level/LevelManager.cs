using System;
using Save;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Level
{
    public class LevelManager: MonoBehaviour
    {
        [SerializeField]
        private Tilemap levelMap;

        public (float, float) TileSize { get; private set; }
        public (float, float) PixelSize { get; private set; }
        public Bounds MapBounds { get; private set; }
        
        public static LevelManager Instance { get; private set; }
        public const int TilePixelSize = 16;

        public void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            RecalculateBounds();
        }

        private void RecalculateBounds()
        {
            
            levelMap.CompressBounds();
            var tileSize = levelMap.size;
            var transform1 = levelMap.transform.parent;
            var position = transform1.position;
            TileSize = (tileSize.x + position.x, tileSize.y + position.y);
            PixelSize = (TileSize.Item1 * TilePixelSize, TileSize.Item2 * TilePixelSize);
            MapBounds = levelMap.localBounds;
        }

        public void SwitchLevel(Tilemap newTilemap)
        {
            SaveManager.CurrentState.Currency += (int) Math.Round(newTilemap.size.magnitude / 3.5f);
            levelMap = newTilemap;
            RecalculateBounds();
        }
    }
}