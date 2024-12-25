using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
// using System;


/**
 * This class demonstrates the CaveGenerator on a Tilemap.
 * 
 * By: Erel Segal-Halevi
 * Since: 2020-12
 */

public class TilemapCaveGenerator: MonoBehaviour {
    [SerializeField] Tilemap tilemap = null;

    [Tooltip("The tile that represents a wall (an impassable block)")]
    [SerializeField] TileBase wallTile = null;

    [Tooltip("The tile that represents a floor (a passable block)")]
    [SerializeField] TileBase floorTile = null;

    [Tooltip("The tile that represents a floor (a passable block)")]
    [SerializeField] GameObject player = null;

    [Tooltip("The percent of walls in the initial random map")]
    [Range(0, 1)]
    [SerializeField] float randomFillPercent = 0.5f;

    [Tooltip("Length and height of the grid")]
    [SerializeField] int gridSize = 100;

    [Tooltip("How many steps do we want to simulate?")]
    [SerializeField] int simulationSteps = 20;

    [Tooltip("For how long will we pause between each simulation step so we can look at the result?")]
    [SerializeField] float pauseTime = 1f;

    private CaveGenerator caveGenerator;

    void Start()  {
        //To get the same random numbers each time we run the script
        Random.InitState(100);

        caveGenerator = new CaveGenerator(randomFillPercent, gridSize);
        caveGenerator.RandomizeMap();

        //For testing that init is working
        GenerateAndDisplayTexture(caveGenerator.GetMap());

        //Start the simulation
        SimulateCavePattern();

        setPlayerInitPosition();
    }


    //Do the simulation in a coroutine so we can pause and see what's going on
async void SimulateCavePattern()  {
        for (int i = 0; i < simulationSteps; i++)   {
            await Awaitable.WaitForSecondsAsync(pauseTime);

            //Calculate the new values
            caveGenerator.SmoothMap();

            //Generate texture and display it on the plane
            GenerateAndDisplayTexture(caveGenerator.GetMap());
        }
        Debug.Log("Simulation completed!");
    }



    //Generate a black or white texture depending on if the pixel is cave or wall
    //Display the texture on a plane
    private void GenerateAndDisplayTexture(int[,] data) {
        for (int y = 0; y < gridSize; y++) {
            for (int x = 0; x < gridSize; x++) {
                var position = new Vector3Int(x, y, 0);
                var tile = data[x, y] == 1 ? wallTile: floorTile;
                tilemap.SetTile(position, tile);
            }
        }
    }

    private void setPlayerInitPosition() {
        System.Random random = new System.Random();
        int randomHeightPosition = 0, randomWidthPosition = 0;
        Vector3Int randomCellPosition = new Vector3Int(randomWidthPosition, randomHeightPosition, 0);
        do {
            randomHeightPosition = random.Next(0, 100);
            randomWidthPosition = random.Next(0, 100);
            randomCellPosition = new Vector3Int(randomWidthPosition, randomHeightPosition, 0);
            Debug.Log("Position = ("+ randomWidthPosition+ ", "+ randomHeightPosition+ ")");
        }while(tilemap.GetTile(randomCellPosition).name == "mountains" && foundHundredTiles(randomCellPosition));
        player.transform.position = tilemap.CellToWorld(randomCellPosition);
    }

    private bool foundHundredTiles(Vector3Int newPosition) {
        if(newPosition == null) return false;
         BoundsInt bounds = tilemap.cellBounds;
         int counter = 0;
         TileBase[] allowedTiles = {floorTile};
         TilemapGraph graph = new TilemapGraph(tilemap, allowedTiles);

        foreach (Vector3Int position in bounds.allPositionsWithin) {
            TileBase tile = tilemap.GetTile(position);

            if (tile.name == "grass") {
                if(BFS.GetPath<Vector3Int>(graph, tilemap.WorldToCell(newPosition), position).Count != 0) counter++;
            }
            if (counter == 100) return true;
        }

        return false;
    }
}
