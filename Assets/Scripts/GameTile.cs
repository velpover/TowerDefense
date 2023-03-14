using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    [SerializeField] Transform arrow = default;

	GameTileContent content;

	public GameTileContent Content
    {
		get => content;
        set
        {	
			Debug.Assert(value != null,"Null Set GameTile Content");
			if(content != null)
            {
				content.Recycle();
            }
			content = value;
			content.transform.localPosition = transform.localPosition;
        }
    }

	GameTile top, down, left, right, nextOnPath;


	static Quaternion tophRotation = Quaternion.Euler(90f, 0, 0),
					  rigthRotation = Quaternion.Euler(90f, 90f, 0f),
					  downRotation = Quaternion.Euler(90f, 180f, 0f),
					  leftRotation = Quaternion.Euler(90f, 270f, 0f);

	int distance;

	public bool HasPath => distance!=int.MaxValue;
	public bool IsAlternative { get; set; }

	public static void MakeRigthLefttNeighbors(GameTile rigth, GameTile left)
	{
		Debug.Assert(rigth.left == null && left.right == null, "Redefined neighbors!");

		rigth.left = left;
		left.right = rigth;

	}

	public static void MakeTopDownNeighbors(GameTile top, GameTile down)
	{
		Debug.Assert(top.down == null && down.top == null, "Redefined neighbors!");
		
		top.down = down;
		down.top = top;
	}

	public void ClearPath()
    {
		distance = int.MaxValue;
		nextOnPath = null;
    }

	public void BecomeDestination()
    {
		distance = 0;
		nextOnPath = null; 
    }

	private GameTile GrowPathTo(GameTile neighbor)
    {
		Debug.Assert(HasPath, "No Path");

		if(neighbor == null || neighbor.HasPath){
			return null;
        }

		neighbor.distance = distance + 1;
		neighbor.nextOnPath = this;

		return neighbor.Content.Type != GameTileContentType.Wall ? neighbor : null;
    }

	public void ShowPath()
    {
        if (distance == 0)
        {
			arrow.gameObject.SetActive(false);
			return;
        }
		arrow.gameObject.SetActive(true);

		arrow.transform.localRotation = nextOnPath == right ? rigthRotation :
										nextOnPath == left ? leftRotation :
										nextOnPath == top ? tophRotation :
										downRotation;
    }

	public void HidePath()
    {
		arrow.gameObject.SetActive(false);
    } 
	public GameTile GrowPathTop() => GrowPathTo(top);
	public GameTile GrowPathDown() => GrowPathTo(down);
	public GameTile GrowPathLeft() => GrowPathTo(left);
	public GameTile GrowPathRight() => GrowPathTo(right);

}
