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
	private Vector3 _localCenter;

	public void SetRoadPicture(RoadPictureItem roadPicture)
	{
		_roadPicture = roadPicture;
		float angle = 0f;
//			if (Type == RoadManager.RoadType.RightToDown) angle = 0f; else
		if (Type == RoadManager.RoadType.DownToLeft) angle = 270f; else
		if (Type == RoadManager.RoadType.LeftToUp) angle = 180f; else
		if (Type == RoadManager.RoadType.UpToRight) angle = 90f;

		Vector3 localCenter = _roadPicture.GetCenter();
		_localCenter = new Vector3(localCenter.x * Mathf.Cos(angle * Mathf.Deg2Rad),
			localCenter.y * Mathf.Sin(angle * Mathf.Deg2Rad), transform.position.z);

		D.Log(transform.position + _localCenter);
	}

	public int CrossEdgeLine()
	{
		++_edgeLineCollideCounter;
		if (_edgeLineCollideCounter >= 2)
		{
			Invoke ("StartRemovingProcess", 3f);
//			gameObject.SetActive(false);
		}
		return _edgeLineCollideCounter;
	}

	void Update()
	{

		if (IsWaitToRemove&&DefsGame.CameraMovement.IsMovingToTarget)
		{
			float distance = Vector2.Distance(DefsGame.CarSimulator.Car.transform.position, transform.position + _localCenter );
			if (distance > 6f) Remove();
		}
	}

	private void StartRemovingProcess()
	{
		IsWaitToRemove = true;
	}

	public void Remove()
	{
		if (_roadPicture) _roadPicture.gameObject.SetActive(false);
		Destroy(gameObject);
	}
}
