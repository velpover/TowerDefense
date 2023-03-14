using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class GameTileContentFactory : ScriptableObject 
{ 
    [SerializeField] GameTileContent destiantionPrefab;
    [SerializeField] GameTileContent emptyPrefab;
    [SerializeField] GameTileContent wallPrefab;

    Scene contentScene;
    public void Reclaim(GameTileContent content)
    {
        Debug.Assert(content.OriginFactory == this, "Wrong factory reclaimed");

        Destroy(content.gameObject);
    }

    private GameTileContent Get(GameTileContent prefab)
    {
        GameTileContent instance = Instantiate(prefab);

        instance.OriginFactory = this;

        MoveToFactoryScene(instance.gameObject);

        return instance;
    }

    public GameTileContent Get(GameTileContentType type)
    {
        switch (type)
        {
            case GameTileContentType.Destination: return Get(destiantionPrefab);

            case GameTileContentType.Empty: return Get(emptyPrefab);

            case GameTileContentType.Wall: return Get(wallPrefab);

            default:
                Debug.Log("Unsupport type" + type);
                return null;
        }

    }
    private void MoveToFactoryScene(GameObject obj)
    {
        if (!contentScene.isLoaded)
        {
            if (Application.isEditor)
            {
                contentScene = SceneManager.GetSceneByName(name);

                if (!contentScene.isLoaded)
                {
                    contentScene = SceneManager.CreateScene(name);
                }
            }
            else
            {
                contentScene = SceneManager.CreateScene(name);
            }
        }

        SceneManager.MoveGameObjectToScene(obj, contentScene);
    }


}
