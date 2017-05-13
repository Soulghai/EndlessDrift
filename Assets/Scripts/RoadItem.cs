using UnityEngine;

public class RoadItem : MonoBehaviour
{
	public RoadManager.RoadType Type;
	public Transform StartPoint1;
	public Transform StartPoint2;
	public RoadManager.BuildState BuildState;
	[HideInInspector] public RoadPictureItem RoadPictureItem;

	private int _edgeLineCollideCounter;

	public int CrossEdgeLine()
	{
		++_edgeLineCollideCounter;
		if (_edgeLineCollideCounter >= 2)
		{
			Invoke ("Remove", 2f);
			Destroy(gameObject, 2f);
		}
		return _edgeLineCollideCounter;
	}

	public void Remove()
	{
		if (RoadPictureItem) RoadPictureItem.gameObject.SetActive(false);
	}
}
