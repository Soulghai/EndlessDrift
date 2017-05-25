using UnityEngine;

public class RoadItem : MonoBehaviour
{
	public RoadManager.RoadType Type;
	public Transform StartPoint1;
	public Transform StartPoint2;
	public Transform CenterPoint;
	public RoadManager.BuildState BuildState;
	private RoadPictureItem _roadPicture;
	[HideInInspector] public bool IsWaitToRemove;

	private int _edgeLineCollideCounter;
	private Vector2 _localCenter;
//	public GameObject Target;

	public void SetRoadPicture(RoadPictureItem roadPicture)
	{
		_roadPicture = roadPicture;
		float angle = (90f + 45f) * Mathf.Deg2Rad;
//			if (Type == RoadManager.RoadType.RightToDown) angle = 0f; else
		if (Type == RoadManager.RoadType.DownToLeft) angle = (180f + 45f) * Mathf.Deg2Rad; else
		if (Type == RoadManager.RoadType.LeftToUp) angle = (270f + 45f) * Mathf.Deg2Rad; else
		if (Type == RoadManager.RoadType.UpToRight) angle = (45f)* Mathf.Deg2Rad;

		Vector3 localCenter = _roadPicture.GetCenter();
		_localCenter = new Vector2(localCenter.x * Mathf.Cos(angle),
			localCenter.y * Mathf.Sin(angle));

		_localCenter = new Vector2(_localCenter.x + transform.position.x, _localCenter.y + transform.position.y);
		D.Log(_localCenter);

//		if (Target) Target.transform.position = _localCenter;
	}

	public void SetZIndex(int value)
	{
		_roadPicture.SetZIndex(value);
	}

	public int CrossEdgeLine()
	{
		++_edgeLineCollideCounter;
		if (_edgeLineCollideCounter >= 2)
		{
//			Invoke ("StartRemovingProcess", 3f);
			IsWaitToRemove = true;
//			gameObject.SetActive(false);
		}
		return _edgeLineCollideCounter;
	}

	void Update()
	{

		if (IsWaitToRemove&&DefsGame.CameraMovement.IsMovingToTarget)
		{
			float distance = Vector2.Distance(DefsGame.CarSimulator.Car.transform.position, _localCenter );
			if (distance > 16f) Remove();
		}
	}

//	private void StartRemovingProcess()
//	{
//		IsWaitToRemove = true;
//	}

	public void Remove()
	{
		if (_roadPicture) _roadPicture.gameObject.SetActive(false);
		Destroy(gameObject);
	}
}
