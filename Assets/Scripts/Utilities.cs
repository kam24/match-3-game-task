using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Utilities
    {
        private static Func<int, int, bool> IsLowerLayer = (int originLayer, int otherLayer) => originLayer > otherLayer;
        private static Func<int, int, bool> IsUpperLayer = (int originLayer, int otherLayer) => originLayer < otherLayer;

        public static List<Tile> FindTiles(Bounds bounds, float boxCastSizePercents, int originLayer, Func<int, int, bool> isConditionMet)
        {
            var tiles = new List<Tile>();

            Vector2 boxCastSize = bounds.size * boxCastSizePercents;
            RaycastHit2D[] hits = Physics2D.BoxCastAll(bounds.center, boxCastSize, 0f, Vector2.zero);

            foreach (var hit in hits)
                if (hit.transform.TryGetComponent(out Tile tile) && isConditionMet(originLayer, tile.Layer))
                    tiles.Add(tile);

            return tiles;
        }

        public static List<Tile> FindLowerTiles(Bounds bounds, float boxCastSizePercents, int originLayer)
        {
            return FindTiles(bounds, boxCastSizePercents, originLayer, IsLowerLayer);
        }

        public static bool HasUpperTile(Bounds bounds, float boxCastSizePercents, int originLayer)
        {
            var tiles = FindTiles(bounds, boxCastSizePercents, originLayer, IsUpperLayer);
            return tiles.Count > 0;
        }
    }
}
