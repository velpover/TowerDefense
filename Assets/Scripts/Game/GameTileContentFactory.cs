using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameTileContentFactory : GameObjectFactory
{ 
    [SerializeField] GameTileContent destiantionPrefab;
    [SerializeField] GameTileContent emptyPrefab;
    [SerializeField] GameTileContent wallPrefab;
    [SerializeField] GameTileContent spawnPointPrefab;
    [SerializeField] GameTileContent towerPrefab;

    public void Reclaim(GameTileContent content)
    {
        Debug.Assert(content.OriginFactory == this, "Wrong factory reclaimed");

        Destroy(content.gameObject);
    }

    private GameTileContent Get(GameTileContent prefab)
    {
        GameTileContent instance = CreateGameObjectInstance(prefab);

        instance.OriginFactory = this;

        return instance;
    }

    public GameTileContent Get(GameTileContentType type)
    {
        switch (type)
        {
            case GameTileContentType.Destination: return Get(destiantionPrefab);

            case GameTileContentType.Empty: return Get(emptyPrefab);

            case GameTileContentType.Wall: return Get(wallPrefab);

            case GameTileContentType.SpawnPoint: return Get(spawnPointPrefab);

            case GameTileContentType.Tower: return Get(towerPrefab);

            default:
                Debug.Log("Unsupport type" + type);
                return null;
        }

    }

}
