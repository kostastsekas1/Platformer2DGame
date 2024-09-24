using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralLevelGenerator : MonoBehaviour
{
    public Tilemap tilemap; 
    public TileBase platformTile; 
    public int chunkWidth; 
    public int chunkHeight;
    public SpriteRenderer rendererlevel = null;
    private int minPlatformSpacing = 2;
    private int maxPlatformSpacing = 5;
    private int minPlatformHeight = 2;
    private int maxPlatformHeight = 6;
  
    private bool inTrigger = false;

    public void OnDrawGizmos()
    {
        if (tilemap == null)
        {
            return;
        }

        Vector3 position0 = tilemap.transform.position;
        Vector3 position1 = new Vector3(position0.x,position0.y + chunkHeight,position0.z);
        Vector3 position2 = new Vector3(position0.x+chunkWidth,position0.y, position0.z);
        Vector3 position3 =new Vector3(position0.x+chunkWidth,position0.y+chunkHeight,position0.z);
        Gizmos.DrawLine(position0,position1);
        Gizmos.DrawLine(position0,position2);
        Gizmos.DrawLine(position1,position3);
        Gizmos.DrawLine(position2,position3);
    }

    void Start()
    {
        GeneratePlatformSections();
    }
    private void Update()
    {
        if (inTrigger&&Input.GetKeyDown(KeyCode.I))
        {
            tilemap.ClearAllTiles();
            GeneratePlatformSections();
        }
    }

    void GeneratePlatformSections()
    {
        int offsetX = 0;
        while (offsetX < chunkWidth )
        {
            int platformType = Random.Range(0, 3);
            int startY = FindFloorHeight(offsetX) + Random.Range(minPlatformHeight, maxPlatformHeight + 1);
            int platformLength = Random.Range(4, 8);

            if ((platformType == 0 || platformType == 2 )&& startY< chunkHeight) 
            {
                float GenerateVertical = Random.Range(0f, 1f);

                if (GenerateVertical < 0.04) {
                    int verticalplatformlength = Random.Range(7, 11);
                    for (int j = startY; j < startY + verticalplatformlength && j < chunkHeight; j++)
                    {
                        tilemap.SetTile(new Vector3Int(offsetX, j, 0), platformTile);
                    }
                    continue;
                }

                for (int i = offsetX; i < offsetX + platformLength && i < chunkWidth; i++)
                {
                    tilemap.SetTile(new Vector3Int(i, startY, 0), platformTile);
                }
            }
            offsetX += Random.Range(minPlatformSpacing, maxPlatformSpacing); 
        }
    }
    int FindFloorHeight(int x)
    {
        for (int y = 0; y < chunkHeight; y++)
        {
            if (tilemap.GetTile(new Vector3Int(x, y, 0)) == platformTile)
            {
                Debug.Log(y);
                return y;
            }
        }
        return 0;
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
           
            inTrigger = true;

            if (rendererlevel == null)
            {
                return;
            }
            rendererlevel.color = new Color(1f, 1f, 1f, 0.1f);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
           
            inTrigger = false;

            if (rendererlevel == null)
            {
                return;
            }
            rendererlevel.color = new Color(1f, 1f, 1f, 0f);
        }
    }



}
