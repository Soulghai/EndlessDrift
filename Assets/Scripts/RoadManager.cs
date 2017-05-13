using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoadManager : MonoBehaviour
{
	public static event Action OnGameplayStart;
	public GameObject[] RoadObjects;
	public GameObject CoinSensorObject;
	public RoadPictureArray Pictures;

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
		BuildFirstFloor, BuildSecondFloor, BuildClimbItem, BuildDescentItem
	}

	private BuildState _buildState;

	private bool _isGameplay;
	private List<GameObject> _roadItems = new List<GameObject>();

	private RoadItem _lastRoadItem;
	private float _spriteSize;

	private RoadType _firstRoadItemType;

	private const float RoadSpaceParam = 3.91f;

	private bool _ignoreFirst;

	private readonly List<CoinSensor> _coinObjects = new List<CoinSensor>();
	private int _roadCounter = 0;

	// Use this for initialization
	void Start ()
	{
		for (int i = 0; i < 20; i++)
		{
			GameObject go = (GameObject)Instantiate(CoinSensorObject, Vector3.zero, Quaternion.identity);
			_coinObjects.Add(go.GetComponent<CoinSensor>());
		}

		Pictures.CreateCachedArr();
		_spriteSize = Pictures.SpriteSize;
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
		foreach (GameObject roadItemGo in _roadItems)
		{
			if (roadItemGo)
			{
				RoadItem ri = roadItemGo.GetComponent<RoadItem>();
				if (ri) ri.Remove();
				Destroy(roadItemGo);
			}
		}
		_roadItems.Clear();

		foreach (CoinSensor cs in _coinObjects)
		{
			cs.Hide(false);
		}
	}

	private void CreateStartRoadItems()
	{
		Clear();

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
			DefsGame.CameraMovement.SetPosition(new Vector3(0, 6f, -10f));

			go1.GetComponent<RoadItem>().RoadPictureItem = Pictures.GetSuitablePicture(
			type1, RoadType.LeftToUp, _buildState, Vector3.zero, true);
		}
		else
		{
			type1 = RoadType.DownToLeft;
			prefab = GetPrefab(type1, _buildState);
			go1 = (GameObject)Instantiate(prefab, Vector3.zero, Quaternion.identity);
			type2 = RoadType.LeftToUp;
			prefab = GetPrefab(type2, _buildState);
			go2 = (GameObject)Instantiate(prefab, Vector3.zero, Quaternion.identity);
			DefsGame.CameraMovement.SetPosition(new Vector3(0, -6f, -10f));

			go1.GetComponent<RoadItem>().RoadPictureItem = Pictures.GetSuitablePicture(
				type1, RoadType.RightToDown, _buildState, Vector3.zero, true);
		}

		go2.GetComponent<RoadItem>().RoadPictureItem = Pictures.GetSuitablePicture(
			type2, type1, _buildState, Vector3.zero, true);

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

		_roadCounter = 2;
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

		RoadType prevType = _lastRoadItem.Type;

		GameObject prefab = GetPrefab(type, _buildState);

		GameObject go = (GameObject)Instantiate(prefab, position, Quaternion.identity);
		_lastRoadItem = go.GetComponent<RoadItem>();


		_lastRoadItem.RoadPictureItem = Pictures.GetSuitablePicture(type, prevType, _buildState, position, isFirstEnter);

		++_roadCounter;

		if ((_roadCounter == 4)||(_roadCounter-4 % 8 == 0)){
			AddCoins();
		}

		_roadItems.Add(go);
	}

	private void AddCoins()
	{
		float angle = 0f;

		if (_lastRoadItem.Type == RoadType.RightToDown) angle = 0f; else
		if (_lastRoadItem.Type == RoadType.DownToLeft) angle = 270f; else
		if (_lastRoadItem.Type == RoadType.LeftToUp) angle = 180f; else
		if (_lastRoadItem.Type == RoadType.UpToRight) angle = 90f;

		float angleChangeValue = 90f/10f;
		Vector3 position;
		for (int i = 0; i < 10; i++)
		{
			angle += angleChangeValue;
			position = _lastRoadItem.transform.position + new Vector3(
				           6.5f*Mathf.Cos(angle*Mathf.Deg2Rad),
				           6.5f*Mathf.Sin(angle*Mathf.Deg2Rad),
				           transform.position.z);
			CoinSensor cs = GetInactiveCoinSensor();
			cs.transform.position = position;
			cs.Show();
		}
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
			if (Random.value > 0.5f) return BuildState.BuildDescentItem;
			return buildState;
		}


		if (buildState == BuildState.BuildClimbItem)
		{
			if (Random.value > 0.8f) return BuildState.BuildSecondFloor;
			return BuildState.BuildDescentItem;
		}

		if (buildState == BuildState.BuildDescentItem)
		{
			if (Random.value > 0.8f) return BuildState.BuildFirstFloor;
			return BuildState.BuildClimbItem;
		}

		return buildState;
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

	private CoinSensor GetInactiveCoinSensor()
	{
		foreach (CoinSensor cs in _coinObjects)
		{
			if (!cs.IsVisible) return cs;
		}

		GameObject go = (GameObject)Instantiate(CoinSensorObject, Vector3.zero, Quaternion.identity);
		CoinSensor cs2 = go.GetComponent<CoinSensor>();
		_coinObjects.Add(cs2);
		return cs2;
	}
}
