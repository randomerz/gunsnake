using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Depends on floorTilemap to be initialized
public class Fog : MonoBehaviour
{
    public bool coverOld;

    public VisionPoint playerVP;
    public static List<VisionPoint> torches = new List<VisionPoint>();

    [System.Serializable]
    public struct VisionPoint
    {
        public Transform transform;
        public float r;

        public VisionPoint(Transform transform, float radius)
        {
            this.transform = transform;
            this.r = radius;
        }
    }



    public Tilemap tilemapHalf;
    public Tilemap tilemapFull;
    public TileBase fogTile;

    public Tilemap floorTilemap;
    public Tilemap wallTilemap;

    private bool didInit = false;
    public static bool isActive;
    private static Fog instance;

    private static Vector3Int nullVector = new Vector3Int(-999, -999, -999);

    void Awake()
    {
        if (!didInit)
            isActive = false;
    }

    public void Init()
    {
        if (didInit)
            return;
        didInit = true;

        isActive = true;
        instance = this;

        Torch.fogController = this;
        playerVP.transform = Player.body[0].transform;

        int offsetX = 16;
        int offsetY = 9;
        BoundsInt bounds = floorTilemap.cellBounds;
        Vector3Int min = bounds.min;
        Vector3Int max = bounds.max;

        for (int x = min.x - offsetX; x <= max.x + offsetX; x++)
        {
            for (int y = min.y - offsetY; y <= max.y + offsetY; y++)
            {
                //tilemapHalf.SetTile(new Vector3Int(x, y, 0), fogTile);
                tilemapFull.SetTile(new Vector3Int(x, y, 0), fogTile);
            }
        }

        TimeTickSystem.OnTick_Last += TimeTickSystem_OnTick;
    }

    private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e)
    {
        Vector3Int offset = new Vector3Int((int)playerVP.transform.position.x, (int)playerVP.transform.position.y, 0);

        if (!coverOld)
        {
            SetTilesInRadius(offset, playerVP.r, null);
        }
        else
        {
            for (int x = (int)-playerVP.r - 2; x <= playerVP.r + 2; x += 1)
            {
                for (int y = (int)-playerVP.r - 2; y <= playerVP.r + 2; y += 1)
                {
                    if (Dist(x, y) <= playerVP.r + 2)
                    {
                        if (Dist(x, y) <= playerVP.r)
                        {
                            //tilemapHalf.SetTile(new Vector3Int(x, y, 0) + offset, null);
                            tilemapFull.SetTile(new Vector3Int(x, y, 0) + offset, null);
                        }
                        else
                        {
                            //tilemapHalf.SetTile(new Vector3Int(x, y, 0) + offset, null);
                            tilemapFull.SetTile(new Vector3Int(x, y, 0) + offset, fogTile);
                        }
                    }
                }
            }
        }

        // lazy
        foreach (VisionPoint vp in torches)
        {
            Vector3Int pos = new Vector3Int((int)vp.transform.position.x, (int)vp.transform.position.y, 0);
            SetTilesInRadius(pos, vp.r, null);
        }
    }

    private void SetTilesInRadius(Vector3Int pos, float radius, TileBase tile)
    {
        for (int x = (int)-radius; x <= radius; x += 1)
        {
            for (int y = (int)-radius; y <= radius; y += 1)
            {
                if (Dist(x, y) <= radius)
                {
                    //tilemapHalf.SetTile(new Vector3Int(x, y, 0) + offset, null);
                    tilemapFull.SetTile(new Vector3Int(x, y, 0) + pos, tile);
                }
            }
        }
    }

    private float Dist(float x, float y)
    {
        return Mathf.Sqrt(x * x + y * y);
    }

    private IEnumerator FloodIterHelper(Vector3Int start, Vector3Int min, Vector3Int max, TileBase tile)
    {
        Vector3Int size = max - min;
        bool[,] spaces = new bool[size.x, size.y];

        Queue<Vector3Int> q = new Queue<Vector3Int>();
        q.Enqueue(start - min); // min is offset
        q.Enqueue(nullVector); // end of segment

        //Debug.Log("Flooding " + min + " to " + max);
        while (q.Count > 1)
        {
            while (q.Peek() != nullVector)
            {
                Vector3Int pos = q.Dequeue();

                if (pos.x < 0 || pos.y < 0 || size.x <= pos.x || size.y <= pos.y)
                    continue;
                if (spaces[pos.x, pos.y])
                    continue;

                tilemapFull.SetTile(pos + min, tile);
                spaces[pos.x, pos.y] = true;

                q.Enqueue(pos + Vector3Int.right);
                q.Enqueue(pos + Vector3Int.up);
                q.Enqueue(pos + Vector3Int.left);
                q.Enqueue(pos + Vector3Int.down);
            }

            q.Enqueue(nullVector);
            q.Dequeue(); // remove null vec

            yield return null;
        }
    }

    public static void Flood(Vector3Int start, Vector3Int min, Vector3Int max)
    {
        if (instance.coverOld)
            return;
        instance.StartCoroutine(instance.FloodIterHelper(start, min, max, null));
    }


    public void AddTorch(Transform transform, float radius)
    {
        torches.Add(new VisionPoint(transform, radius));
    }
}
