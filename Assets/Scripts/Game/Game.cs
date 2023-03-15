using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] Vector2Int boardSize = new Vector2Int(11, 11);

    [SerializeField] GameBoard board = default;

    [SerializeField] GameTileContentFactory factory;

    [SerializeField] EnemyFactory enemyFactory;

    [SerializeField, Range(0.1f, 10f)] float spawnSpeed = 1f;

    float spawnTime = 0f;

    Camera cameraRay;

    EnemyCollection enemies = new EnemyCollection();

    Ray TouchRay => cameraRay.ScreenPointToRay(Input.mousePosition);

	void Awake()
	{   
        cameraRay = Camera.main;

		board.Initialize(boardSize,factory);
        board.ShowGrid = true;
	}

    private void Update()
    {
        spawnTime += spawnSpeed * Time.deltaTime;

        while (spawnTime >= 1f)
        {
            spawnTime -= 1f;
            SpawnEnemy();
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            HandleAlternativeTouch();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            board.ShowPath = !board.ShowPath;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            board.ShowGrid = !board.ShowGrid;
        }

        enemies.GameUpdate();

        Physics.SyncTransforms();
        board.GameUpdate();
    }
    private void OnValidate()
    {
        if (boardSize.x < 2)
        {
            boardSize.x = 2;
        }
        if(boardSize.y < 2)
        {
            boardSize.y = 2;
        }
    }

    private void HandleTouch()
    {
        GameTile tile = board.GetTile(TouchRay);

        if (tile != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                board.ToggleTower(tile);
            }
            else
            {
                board.ToggleWall(tile);
            }
            
        }
    }

    private void HandleAlternativeTouch()
    {
        GameTile tile = board.GetTile(TouchRay);

        if (tile != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                board.ToggleSpawnPoint(tile);
            }
            else
            {
                board.ToggleDestination(tile);
            }
            
        }
    }

    private void SpawnEnemy()
    {
        GameTile spawnPoint = board.GetSpawnPoint(Random.Range(0, board.SpawnPointCount));

        Enemy enemy = enemyFactory.Get();

        enemy.SpawnOn(spawnPoint);

        enemies.Add(enemy);
    }
}
