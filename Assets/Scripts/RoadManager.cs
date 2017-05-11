using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoadManager : MonoBehaviour
{
	public static event Action OnGameplayStart;
	public GameObject[] RoadObjects;

	[HideInInspector]
	public enum RoadType
	{
		UpToRight, RightToDown, DownToLeft, LeftToUp

//		BridgeUpToRight, BridgeRightToDown, BridgeDownToLeft, BridgeLeftToUp,
//		ClimbUpToRight, ClimbRightToDown, ClimbDownToLeft, ClimbLeftToUp,
//		DescentUpToRight, DescentRightToDown, DescentDownToLeft, DescentLeftToUp
	}

	public enum BuildState
	{
		BuildFirstFloor, BuildSecondFloor, BuildClimbItem
	}

	private BuildState _buildState;

	private bool _isGameplay;
	private List<GameObject> _roadItems = new List<GameObject>();

	private RoadItem _lastRoadItem;
	private float _spriteSize;

	private RoadType _firstRoadItemType;

	private const float RoadSpaceParam = 3.91f;

	private bool _ignoreFirst;
//	private bool _isFirstLaunch = true;

	// Use this for initialization
	void Start ()
	{
		CreateStartRoadItems();
		_isGameplay = false;
	}

	void OnEnable() {
		CarSimulator.OnGameOver += Respown;
		Car.OnAddRoadItem += OnAddRoadItem;
	}
	void OnDisable() {
		CarSimulator.OnGameOver -= Respown;
		Car.OnAddRoadItem -= OnAddRoadItem;
	}

	private void Respown(float delay)
	{
		Invoke("RespownWithDelay", 0.3f);

//		DefsGame.CameraMovement.IsMovingToTarget = false;
//		DefsGame.CameraMovement.IsMoveToPosition = true;
//		if (_firstRoadItemType == RoadType.UpToRight)
//		{
//			DefsGame.CameraMovement.TargetPosition = new Vector3(0, 6f, -10f);
//		}
//		else
//		{
//			DefsGame.CameraMovement.TargetPosition = new Vector3(0, -6f, -10f);
//		}
	}

	private void RespownWithDelay()
	{
		Clear();
		CreateStartRoadItems();
		_isGameplay = false;
	}

	private void Clear()
	{
		foreach (GameObject roadItem in _roadItems)
		{
			Destroy(roadItem);
		}
		_roadItems.Clear();
	}

	private void CreateStartRoadItems()
	{
		_buildState = BuildState.BuildFirstFloor;

		RoadType type1;
		RoadType type2;
		GameObject go1;
		GameObject go2;
		GameObject prefab;
		if (Random.value < 0.5f)
		{
			type1 = RoadType.UpToRight;
			prefab = GetPrefab(type1, _buildState);
			go1 = (GameObject)Instantiate(prefab, Vector3.zero, Quaternion.identity);
			type2 = RoadType.RightToDown;
			prefab = GetPrefab(type2, _buildState);
			go2 = (GameObject)Instantiate(prefab, Vector3.zero, Quaternion.identity);
//			if (_isFirstLaunch)
//			{
//				_isFirstLaunch = false;
				DefsGame.CameraMovement.SetPosition(new Vector3(0, 6f, -10f));
//			}
		}
		else
		{
			type1 = RoadType.DownToLeft;
			prefab = GetPrefab(type1, _buildState);
			go1 = (GameObject)Instantiate(prefab, Vector3.zero, Quaternion.identity);
			type2 = RoadType.LeftToUp;
			prefab = GetPrefab(type2, _buildState);
			go2 = (GameObject)Instantiate(prefab, Vector3.zero, Quaternion.identity);
//			if (_isFirstLaunch)
//			{
//				_isFirstLaunch = false;
				DefsGame.CameraMovement.SetPosition(new Vector3(0, -6f, -10f));
//			}
		}

		_roadItems.Add(go1);
		_roadItems.Add(go2);

		if (Random.value < 0.5f)
		{
			_firstRoadItemType = type1;
			_lastRoadItem = go2.GetComponent<RoadItem>();
		}
		else
		{
			_firstRoadItemType = type2;
			_lastRoadItem = go1.GetComponent<RoadItem>();
		}

		if (_spriteSize < 0.1f)
		{
			SpriteRenderer spr = _lastRoadItem.GetComponent<SpriteRenderer>();
			_spriteSize = spr.sprite.bounds.size.x;
		}

		_ignoreFirst = true;
	}

	// Update is called once per frame
	void Update () {
		if (DefsGame.CameraMovement.IsMoveToPosition) return;

		if (!_isGameplay&&InputController.IsTouchOnScreen(TouchPhase.Began))
		{
			DefsGame.CarSimulator.Car.enabled = true;
			DefsGame.CarSimulator.SetStartRoadType(_firstRoadItemType);
			DefsGame.CarSimulator.Respown();
			DefsGame.CarSimulator.StartMove();
			_isGameplay = true;
			GameEvents.Send(OnGameplayStart);
		}
	}

	private void OnAddRoadItem(bool isFirstEnter)
	{
		if (_ignoreFirst)
		{
			_ignoreFirst = false;
			return;
		}
		RoadType type = FindSuitableItem(_lastRoadItem.Type, isFirstEnter);

		Vector3 position = GetNewRoadItemPosition(type, isFirstEnter);

		_buildState = GetBuildState(_buildState);

		GameObject prefab = GetPrefab(type, _buildState);

		GameObject go = (GameObject)Instantiate(prefab, position, Quaternion.identity);
		_lastRoadItem = go.GetComponent<RoadItem>();

		_roadItems.Add(go);
	}

	private GameObject GetPrefab(RoadType type, BuildState buildState)
	{
		int id = (int) type + 4 * (int) buildState;
		return RoadObjects[id];
	}

	private BuildState GetBuildState(BuildState buildState)
	{
		if (buildState == BuildState.BuildFirstFloor)
		{
			if (Random.value > 0.5f) return BuildState.BuildClimbItem;
			return buildState;
		}

		if (buildState == BuildState.BuildSecondFloor)
		{
			if (Random.value > 0.5f) return BuildState.BuildClimbItem;
			return buildState;
		}


		float ran = Random.value;
		if (ran <= 0.4f) return BuildState.BuildFirstFloor;
		if (ran <= 0.8f) return BuildState.BuildSecondFloor;

		return BuildState.BuildClimbItem;
	}

	private Vector3 GetNewRoadItemPosition(RoadType type, bool isFirstEnter)
	{
		Vector3 position = _lastRoadItem.transform.position;
		if (isFirstEnter)
		{
			if ((type == RoadType.UpToRight)&&(_lastRoadItem.Type == RoadType.DownToLeft))
			{
				position = _lastRoadItem.transform.position + new Vector3(
					           0f, - _spriteSize - RoadSpaceParam, 0f);
			}
			else if ((type == RoadType.RightToDown)&&(_lastRoadItem.Type == RoadType.LeftToUp))
			{
				position = _lastRoadItem.transform.position + new Vector3(
					           -_spriteSize - RoadSpaceParam, 0f, 0f);
			}
			else if ((type == RoadType.DownToLeft)&&(_lastRoadItem.Type == RoadType.UpToRight))
			{
				position = _lastRoadItem.transform.position + new Vector3(
					           0f, _spriteSize + RoadSpaceParam, 0f);
			}
			else if ((type == RoadType.LeftToUp)&&(_lastRoadItem.Type == RoadType.RightToDown))
			{
				position = _lastRoadItem.transform.position + new Vector3(
					           _spriteSize + RoadSpaceParam, 0f, 0f);
			}
		}
		else
		{
			if ((type == RoadType.UpToRight)&&(_lastRoadItem.Type == RoadType.DownToLeft))
			{
				position = _lastRoadItem.transform.position + new Vector3(
					           _spriteSize + RoadSpaceParam, 0f, 0f);
			}
			else if ((type == RoadType.RightToDown)&&(_lastRoadItem.Type == RoadType.LeftToUp))
			{
				position = _lastRoadItem.transform.position + new Vector3(
					           0f, -_spriteSize - RoadSpaceParam, 0f);
			}
			else if ((type == RoadType.DownToLeft)&&(_lastRoadItem.Type == RoadType.UpToRight))
			{
				position = _lastRoadItem.transform.position + new Vector3(
					           -_spriteSize - RoadSpaceParam, 0f, 0f);
			}
			else if ((type == RoadType.LeftToUp)&&(_lastRoadItem.Type == RoadType.RightToDown))
			{
				position = _lastRoadItem.transform.position + new Vector3(
					           0f, _spriteSize + RoadSpaceParam, 0f);
			}
		}
		return position;
	}

	private RoadType FindSuitableItem(RoadType type, bool isFirstEnter)
	{
		float ran = Random.value;
		if (isFirstEnter)
		{	//{ UpToRight, RightToDown, DownToLeft, LeftToUp }
			if (type == RoadType.UpToRight)
			{
				if (ran > 0.5f) return RoadType.DownToLeft;
				return RoadType.RightToDown;
			}
			if (type == RoadType.RightToDown)
			{
				if (ran > 0.5f) return RoadType.LeftToUp;
				return RoadType.DownToLeft;
			}
			if (type == RoadType.DownToLeft)
			{
				if (ran > 0.5f) return RoadType.UpToRight;
				return RoadType.LeftToUp;
			}
			if (type == RoadType.LeftToUp)
			{
				if (ran > 0.5f) return RoadType.RightToDown;
				return RoadType.UpToRight;
			}
		}
		else
		{	//{ UpToRight, RightToDown, DownToLeft, LeftToUp }
			if (type == RoadType.UpToRight)
			{
				if (ran > 0.5f) return RoadType.DownToLeft;
				return RoadType.LeftToUp;
			}
			if (type == RoadType.RightToDown)
			{
				if (ran > 0.5f) return RoadType.LeftToUp;
				return RoadType.UpToRight;
			}
			if (type == RoadType.DownToLeft)
			{
				if (ran > 0.5f) return RoadType.UpToRight;
				return RoadType.RightToDown;
			}
			if (type == RoadType.LeftToUp)
			{
				if (ran > 0.5f) return RoadType.RightToDown;
				return RoadType.DownToLeft;
			}
		}

		return RoadType.RightToDown;
	}
}
