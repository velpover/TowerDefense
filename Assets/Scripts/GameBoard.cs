using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField] Transform ground = default;
    [SerializeField] GameTile tilePrefab;
    [SerializeField] Texture2D gridTeture;

    GameTileContentFactory contentFactory;
    Queue<GameTile> searchFrontier = new Queue<GameTile>();

    Vector2Int size;

    GameTile[] tiles;

    bool showPath,showGrid;

    public bool ShowPath {

        get => showPath;
        set
        {
            showPath = value;
            if (showPath)
            {
                for(int i = 0; i < tiles.Length; i++)
                {
                    tiles[i].ShowPath();
                }
            }
            else
            {
                for(int i = 0; i < tiles.Length; i++)
                {
                    tiles[i].HidePath();
                }
            }
        }
    }

    public bool ShowGrid
    {
        get => showGrid;
        set
        {
            showGrid = value;

            Material mat = ground.GetComponent<MeshRenderer>().material;

            if (showGrid)
            {
                mat.mainTexture = gridTeture;
                mat.mainTexture.wrapMode = TextureWrapMode.Repeat;
                mat.mainTextureScale = size;
            }
            else
            {
                mat.mainTexture = null;
            }
        }
    }
    public void Initialize(Vector2Int size,GameTileContentFactory contentFactory)
    {
        this.size = size;
        this.contentFactory = contentFactory;

        ground.localScale = new Vector3(size.x, size.y, 1f);

        Vector2 offset = new Vector2((size.x-1)*0.5f, (size.y-1)*0.5f);

        tiles = new GameTile[size.x * size.y];

        for (int y = 0,index = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++,index++)
            {
                tiles[index] = Instantiate(tilePrefab);

                tiles[index].transform.SetParent(transform, false);
                tiles[index].transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);
                tiles[index].Content = contentFactory.Get(GameTileContentType.Empty);
                if (x > 0)
                {
                    GameTile.MakeRigthLefttNeighbors(tiles[index], tiles[index - 1]);
                }
                if (y > 0)
                {
                    GameTile.MakeTopDownNeighbors(tiles[index], tiles[index - size.x]);
                }

                tiles[index].IsAlternative = (x & 1) == 0;
                if((y & 1) == 0)
                {
                    tiles[index].IsAlternative = !tiles[index].IsAlternative;
                }
            }
        }

        ToggleDestination(tiles[Random.Range(0,size.x*size.y)]);
    }

    private bool FindPath()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].Content.Type == GameTileContentType.Destination)
            {
                tiles[i].BecomeDestination();
                searchFrontier.Enqueue(tiles[i]);
            }
            else
            {
                tiles[i].ClearPath();
            }
        }

        if(searchFrontier.Count == 0) return false;

        while (searchFrontier.Count > 0)
        {
            GameTile tile = searchFrontier.Dequeue();

            if (tile != null)
            {
                if (tile.IsAlternative)
                {
                    searchFrontier.Enqueue(tile.GrowPathTop());
                    searchFrontier.Enqueue(tile.GrowPathDown());
                    searchFrontier.Enqueue(tile.GrowPathLeft());
                    searchFrontier.Enqueue(tile.GrowPathRight());
                }
                else
                {
                    searchFrontier.Enqueue(tile.GrowPathRight());
                    searchFrontier.Enqueue(tile.GrowPathLeft());
                    searchFrontier.Enqueue(tile.GrowPathDown());
                    searchFrontier.Enqueue(tile.GrowPathTop());
                }
              
            }
        }
        for(int i=0;i< tiles.Length; i++)
        {
            if (!tiles[i].HasPath)
            {
                return false;
            }
        }
        if (showPath)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].ShowPath();
            }
        }

        return true;
    }

    public GameTile GetTile(Ray ray)
    {

        if (Physics.Raycast(ray,out RaycastHit hit))
        {
            int x = (int)(hit.point.x + size.x * 0.5f);
            int y = (int)(hit.point.z + size.y * 0.5f);

            if (x >= 0 && x < size.x && y >= 0 && y < size.y)
            {
                return tiles[x + y * size.x];
            }
        }
        return null;
    }

    public void ToggleDestination(GameTile tile)
    {
        if(tile.Content.Type == GameTileContentType.Destination)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
            if (!FindPath())
            {
                tile.Content = contentFactory.Get(GameTileContentType.Destination);
                FindPath();
            }
        }
        else if(tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Destination);
            FindPath();
        }
    }

    public void ToggleWall (GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Wall)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
            FindPath();
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Wall);
            if (!FindPath())
            {
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
                FindPath();
            }
        }
    }
    
}
