using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Depends on floorTilemap to be initialized
public class DecorationPlacer : MonoBehaviour
{
    public string area;

    [Range(0, 1)]
    public float propChance;
    [Range(0, 1)]
    public float simpleChance;

    public TileBase[] simpleProps;
    public TileBase[] colliderProps;
    public GameObject[] complexProps;
    public GameObject[] outOfBoundProps;
    public Tilemap decorationTilemap;

    public DungeonRoomPlacer roomPlacer;
    private Tilemap floor;
    private Tilemap wall;
    private Tilemap ceil;
    private TileBase floorTile;
    private TileBase wallTile;
    private TileBase ceilTile;

    private GameObject decorationsContainer;
    //private bool didInit = false;

    //void Start()
    //{
    //    if (!didInit)
    //        PlaceDecorations();
    //}

    public void PlaceDecorations()
    {
        //if (didInit)
        //    Debug.Log("Returning...");

        Debug.Log("Placing decorations!");

        decorationsContainer = GameObject.Find("DecorationsContainer");
        if (decorationsContainer == null)
            decorationsContainer = new GameObject("DecorationsContainer");

        //didInit = true;

        floor = roomPlacer.floor;
        wall = roomPlacer.wall;
        ceil = roomPlacer.ceil;
        floorTile = roomPlacer.defaultFloor;
        wallTile = roomPlacer.defaultWall;
        ceilTile = roomPlacer.defaultCeil;

        int numProps = simpleProps.Length + complexProps.Length;

        if (area == "Jungle")
        {
            int offsetX = 16; // copied from fog offset
            int offsetY = 9;
            BoundsInt bounds = floor.cellBounds;
            Vector3Int min = bounds.min;
            Vector3Int max = bounds.max;

            Vector3Int pos;
            for (int x = min.x - offsetX; x <= max.x + offsetX; x++)
            {
                for (int y = min.y - offsetY; y <= max.y + offsetY; y++)
                {
                    pos = new Vector3Int(x, y, 0);
                    if (floor.GetTile(pos) == null)
                    {
                        if (wall.GetTile(pos) == null)
                        {
                            // === Out of bounds ===
                            //wallTilemap.SetTile(pos, wallTile);
                            ceil.SetTile(pos, ceilTile);
                            if (Random.Range(0f, 1f) < propChance)
                            {
                                Collider2D hit = Physics2D.OverlapBox(new Vector2(x, y), new Vector2(1, 1), 0);
                                if (hit)
                                {
                                    continue;
                                }

                                int rand = Random.Range(0, outOfBoundProps.Length);
                                Instantiate(outOfBoundProps[rand], pos, Quaternion.identity, decorationsContainer.transform);
                            }
                        }
                    }
                    else
                    {
                        // === On floor ===
                        if (Random.Range(0f, 1f) < propChance)
                        {
                            // check theres nothing on this tile
                            Collider2D[] hits = Physics2D.OverlapPointAll(new Vector2(x, y));
                            bool isOverlapping = false;
                            foreach (Collider2D hit in hits)
                            {
                                if (hit && hit.tag != "Room")
                                {
                                    //Debug.Log("Hit something at " + pos + ", not putting down artifact");
                                    isOverlapping = true;
                                    break;
                                }
                            }
                            if (isOverlapping)
                                continue;

                            if (Random.Range(0f, 1f) < simpleChance)
                            {
                                if (Random.Range(0f, 1f) < 0.9f)
                                {
                                    decorationTilemap.SetTile(pos, simpleProps[Random.Range(0, simpleProps.Length)]);
                                }
                                else
                                {
                                    decorationTilemap.SetTile(pos, colliderProps[Random.Range(0, colliderProps.Length)]);
                                }
                            }
                            else
                            {
                                Instantiate(complexProps[Random.Range(0, complexProps.Length)], pos, Quaternion.identity, decorationsContainer.transform);
                            }
                        }
                    }
                }
            }
        }

        if (area == "Dungeon")
        {
            int offsetX = 16; // copied from fog offset
            int offsetY = 9;
            BoundsInt bounds = floor.cellBounds;
            Vector3Int min = bounds.min;
            Vector3Int max = bounds.max;

            Vector3Int pos;
            for (int x = min.x - offsetX; x <= max.x + offsetX; x++)
            {
                for (int y = min.y - offsetY; y <= max.y + offsetY; y++)
                {
                    pos = new Vector3Int(x, y, 0);
                    if (floor.GetTile(pos) == null)
                    {
                        if (wall.GetTile(pos) == null)
                        {

                        }
                    }
                    else
                    {
                        // === On floor ===
                        if (Random.Range(0f, 1f) < propChance)
                        {
                            // check theres nothing on this tile
                            Collider2D[] hits = Physics2D.OverlapPointAll(new Vector2(x, y));
                            bool isOverlapping = false;
                            foreach (Collider2D hit in hits)
                            {
                                if (hit && hit.tag != "Room")
                                {
                                    //Debug.Log("Hit something at " + pos + ", not putting down artifact");
                                    isOverlapping = true;
                                    break;
                                }
                            }
                            if (isOverlapping)
                                continue;

                            if (Random.Range(0f, 1f) < simpleChance)
                            {
                                if (Random.Range(0f, 1f) < 0.95f)
                                {
                                    decorationTilemap.SetTile(pos, simpleProps[Random.Range(0, simpleProps.Length)]);
                                }
                                else
                                {
                                    decorationTilemap.SetTile(pos, colliderProps[Random.Range(0, colliderProps.Length)]);
                                }
                            }
                            else
                            {
                                Instantiate(complexProps[Random.Range(0, complexProps.Length)], pos, Quaternion.identity, decorationsContainer.transform);
                            }
                        }
                    }
                }
            }
        }

        if (area == "Temple")
        {
            int offsetX = 16; // copied from fog offset
            int offsetY = 9;
            BoundsInt bounds = floor.cellBounds;
            Vector3Int min = bounds.min;
            Vector3Int max = bounds.max;

            Vector3Int pos;
            for (int x = min.x - offsetX; x <= max.x + offsetX; x++)
            {
                for (int y = min.y - offsetY; y <= max.y + offsetY; y++)
                {
                    pos = new Vector3Int(x, y, 0);
                    if (floor.GetTile(pos) == null)
                    {

                    }
                    else
                    {
                        // === On floor ===
                        if (Random.Range(0f, 1f) < propChance)
                        {
                            // check theres nothing on this tile
                            Collider2D[] hits = Physics2D.OverlapPointAll(new Vector2(x, y));
                            bool isOverlapping = false;
                            foreach (Collider2D hit in hits)
                            {
                                if (hit && hit.tag != "Room")
                                {
                                    //Debug.Log("Hit something at " + pos + ", not putting down artifact");
                                    isOverlapping = true;
                                    break;
                                }
                            }
                            if (isOverlapping)
                                continue;

                            if (Random.Range(0f, 1f) < 0.95f)
                            {
                                decorationTilemap.SetTile(pos, simpleProps[Random.Range(0, simpleProps.Length)]);
                            }
                            else
                            {
                                decorationTilemap.SetTile(pos, colliderProps[Random.Range(0, colliderProps.Length)]);
                            }
                        }
                    }
                }
            }
        }
    }

    private float Dist(float x, float y)
    {
        return Mathf.Sqrt(x * x + y * y);
    }

    public void ClearDecorations()
    {
        if (decorationsContainer == null)
        {
            decorationsContainer = GameObject.Find("DecorationsContainer");
            if (decorationsContainer == null)
                decorationsContainer = new GameObject("DecorationsContainer");
        }

        for (int i = decorationsContainer.transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(decorationsContainer.transform.GetChild(i).gameObject);

        decorationTilemap.ClearAllTiles();
    }
}
