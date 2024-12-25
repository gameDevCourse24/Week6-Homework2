using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
 * This component allows the player to move by clicking the arrow keys,
 * but only if the new position is on an allowed tile.
 */
public class KeyboardMoverByTile: KeyboardMover {
    [SerializeField] Tilemap tilemap = null;
    [SerializeField] AllowedTiles allowedTiles = null;
    [SerializeField] TileBase shallowWater = null;
    [SerializeField] TileBase mediumWater = null;
    [SerializeField] TileBase deepWater = null;
    [SerializeField] TileBase mountain = null;
    [SerializeField] TileBase grass = null;
    private bool isHammerCollected = false;

    private TileBase TileOnPosition(Vector3 worldPosition) {
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        return tilemap.GetTile(cellPosition);
    }

    void Update()  {
        Vector3 newPosition = NewPosition();
        TileBase tileOnNewPosition = TileOnPosition(newPosition);
        
        if (allowedTiles.Contains(tileOnNewPosition)) {
            if(isHammerCollected && tileOnNewPosition.name == "mountains") {
                tilemap.SetTile(tilemap.WorldToCell(newPosition), grass);
            }
            else{
                if(tileOnNewPosition.name == "goat_0") {
                    if (!allowedTiles.Contains(mountain)) allowedTiles.Add(mountain);
                    tilemap.SetTile(tilemap.WorldToCell(newPosition), TileOnPosition(transform.position));
                    Debug.Log("Collected goat");
                }
                else if(tileOnNewPosition.name == "boat_0") {
                    if(!(allowedTiles.Contains(shallowWater) && allowedTiles.Contains(mediumWater) && allowedTiles.Contains(deepWater))) {
                        allowedTiles.Add(shallowWater);
                        allowedTiles.Add(mediumWater);
                        allowedTiles.Add(deepWater);
                    }
                    tilemap.SetTile(tilemap.WorldToCell(newPosition), TileOnPosition(transform.position));
                    Debug.Log("Collected boat");
                }
                else if(tileOnNewPosition.name == "hammer_0") {
                    isHammerCollected = true;
                    if (!allowedTiles.Contains(mountain)) allowedTiles.Add(mountain);
                    tilemap.SetTile(tilemap.WorldToCell(newPosition), TileOnPosition(transform.position));
                    Debug.Log("Collected hammer");
                }
            }
            transform.position = newPosition;
        } else {
            Debug.LogError("You cannot walk on " + tileOnNewPosition + "!");
        }
    }
}
