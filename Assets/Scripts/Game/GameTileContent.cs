using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GameTileContentType
{
    Empty, Destination, Wall, SpawnPoint, Tower
}
public class GameTileContent : MonoBehaviour
{
    [SerializeField] GameTileContentType type = default;

    GameTileContentFactory factory;

    public bool BlocksPath => Type == GameTileContentType.Wall||Type == GameTileContentType.Tower;
    public GameTileContentFactory OriginFactory
    {
        get => factory;
        set
        {
            Debug.Assert(factory == null, "Redefined origin Factory");
            factory = value;
        }

    }
    public GameTileContentType Type => type;

    public void Recycle()
    {
        factory.Reclaim(this);
    }

    public virtual void GameUpdate() { }
    
}
