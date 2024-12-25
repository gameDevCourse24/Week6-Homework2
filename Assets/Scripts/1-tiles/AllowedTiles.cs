using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

/**
 * This component just keeps a list of allowed tiles.
 * Such a list is used both for pathfinding and for movement.
 */
public class AllowedTiles : MonoBehaviour  {
    [SerializeField] TileBase[] allowedTiles = null;

    public bool Contains(TileBase tile) {
        return allowedTiles.Contains(tile);
    }

    public TileBase[] Get() { return allowedTiles;  }
    public void Add(TileBase tile) { 
        Array.Resize(ref allowedTiles, allowedTiles.Length + 1);
        allowedTiles[allowedTiles.Length - 1] = tile;
    }
}
