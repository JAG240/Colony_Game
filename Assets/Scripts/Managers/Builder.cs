using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class Builder : MonoBehaviour
{
    [SerializeField] GridManager gridManager;
    [SerializeField] Tile[] buildableTiles;
    private Tile activeTile;
    private Tilemap decor;
    private WorldGen worldGen;

    void Start()
    {
        decor = gridManager.stones;
        worldGen = GameObject.Find("World").GetComponent<WorldGen>();
    }

    public void SetTile(int tile)
    {
        activeTile = buildableTiles[tile];
    }

    public void PlaceTile(Vector3 position, Tile tile)
    {
        Vector3Int convertedPos = Vector3Int.RoundToInt(position);
        gridManager.RemoveSpot(gridManager.GetSpot(convertedPos));
        decor.SetTile(convertedPos, tile);
    }

    public void PlacePlannedBuild(Vector3 position)
    {
        Vector3Int convertedPos = Vector3Int.RoundToInt(position);

        if (!worldGen.GetSpotAvailabilty(convertedPos.x, convertedPos.y))
        {
            GameObject buildingPlan = new GameObject("BuildingPlan");
            SpriteRenderer spriteRenderer = buildingPlan.AddComponent<SpriteRenderer>() as SpriteRenderer;
            spriteRenderer.sprite = activeTile.sprite;
            Color newColor = new Color(activeTile.color.r, activeTile.color.g, activeTile.color.b, 0.5f);
            spriteRenderer.color = newColor;
            spriteRenderer.sortingLayerName = "Layer 3";
            buildingPlan.transform.position = new Vector3(convertedPos.x, convertedPos.y, 0f);
            buildingPlan.transform.localScale = new Vector3(1f, 1f, 0f);
            BuildObjScript buildObjScript = buildingPlan.AddComponent<BuildObjScript>() as BuildObjScript;
            buildObjScript.builder = this;
            buildObjScript.tile = activeTile;
            buildObjScript.SetInQueue(true);
            worldGen.SetSpotAvailabilty(convertedPos.x, convertedPos.y, true);
        }
    }
}
