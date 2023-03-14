using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] Vector2Int boardSize = new Vector2Int(11, 11);

    [SerializeField] GameBoard board = default;

    [SerializeField] GameTileContentFactory factory;

    Camera cameraRay;

    Ray TouchRay => cameraRay.ScreenPointToRay(Input.mousePosition);
	void Awake()
	{   
        cameraRay = Camera.main;

		board.Initialize(boardSize,factory);
        board.ShowGrid = true;
	}

    private void Update()
    {
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
            board.ToggleWall(tile);
        }
    }

    private void HandleAlternativeTouch()
    {
        GameTile tile = board.GetTile(TouchRay);

        if (tile != null)
        {
            board.ToggleDestination(tile);
        }
    }
}
