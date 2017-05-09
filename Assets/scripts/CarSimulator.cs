using System;
using UnityEngine;

public class CarSimulator : MonoBehaviour
{
	public static event Action <float> OnGameOver;
	public static event Action <int> OnAddPoints;
	public GameObject GoalPointObject;
	public Car Car;
	public float Velocity = 70f;
	private float _acceleration = 0.01f;
	[HideInInspector] public float CurrVelocity;
	private float _radius = 6f;
	private Vector3 _goalPoint;
	private bool _isCrash;
	private bool _isMoving;
	private bool _isActive;

	private RoadManager.RoadType _startType;
	private float _oldCurrVelocity;
	private const float SpeedToAddPoints = 0.3f;

	// Use this for initialization

	void Awake()
	{
		DefsGame.CarSimulator = this;
	}

	void OnEnable() {
		Car.OnCrash += Crash;
	}

	void OnDisable() {
		Car.OnCrash -= Crash;
	}

	public void SetStartRoadType(RoadManager.RoadType type)
	{
		_startType = type;
	}

	public void Respown()
	{
		_isActive = true;
		_isCrash = false;

		transform.position = new Vector3(0f, -6f, 0f);
		transform.rotation = Quaternion.identity;

		Vector2 currentForwardNormal = transform.up * _radius;
		_goalPoint = new Vector3(transform.position.x + currentForwardNormal.x,
			transform.position.y + currentForwardNormal.y, transform.position.z);
		GoalPointObject.transform.position = _goalPoint;
		if (_startType == RoadManager.RoadType.UpToRight)
		{
			CurrVelocity = -Velocity;
			_acceleration = -0.01f;
			transform.RotateAround(_goalPoint, Vector3.forward, -80f);
		}
		else if (_startType == RoadManager.RoadType.RightToDown)
		{
			CurrVelocity = Velocity;
			_acceleration = 0.01f;
			transform.RotateAround(_goalPoint, Vector3.forward, 80f);
		}
		else if (_startType == RoadManager.RoadType.DownToLeft)
		{
			CurrVelocity = -Velocity;
			_acceleration = -0.01f;
			transform.RotateAround(_goalPoint, Vector3.forward, 100f);
		}
		else if (_startType == RoadManager.RoadType.LeftToUp)
		{
			CurrVelocity = Velocity;
			_acceleration = 0.01f;
			transform.RotateAround(_goalPoint, Vector3.forward, -100f);
		}

		_oldCurrVelocity = Mathf.Abs(CurrVelocity) + SpeedToAddPoints;

		Car.Respown();
	}

	// Update is called once per frame
	void Update () {
		if (!_isActive) return;

		transform.RotateAround(_goalPoint, Vector3.forward, CurrVelocity * Time.deltaTime);

		if (_isCrash)
		{
			if (CurrVelocity > 1f) CurrVelocity -= 1.9f;
			else
			if (CurrVelocity < -1f) CurrVelocity += 1.9f;
			else
			{
				Stop();
				//Invoke("Respown", 0.5f);

				GameEvents.Send(OnGameOver, 0f);
			}
		}
		else if (_isMoving)
		{
			CurrVelocity += _acceleration;

			if (Mathf.Abs(CurrVelocity) - _oldCurrVelocity >= SpeedToAddPoints)
			{
				_oldCurrVelocity += SpeedToAddPoints;
				GameEvents.Send(OnAddPoints, 1);
			}

			if ((InputController.IsTouchOnScreen(TouchPhase.Began))
				&&(DefsGame.CameraMovement.IsMoving))
			{
				Vector2 currentForwardNormal = transform.up * -_radius;
				_goalPoint = new Vector3(transform.position.x + currentForwardNormal.x,
					transform.position.y + currentForwardNormal.y, transform.position.z);
				GoalPointObject.transform.position = _goalPoint;

				CurrVelocity *= -1f;
				_acceleration *= -1f;

				//Body.transform.DORotate(Vector3.forward*180f, 1f);
				transform.Rotate(Vector3.forward, 180f);
				//Car.Rotate(new Vector3(0f, 0f, transform.rotation.z));
			}
		}
	}

	private void Stop()
	{
		_isActive = false;
		_isMoving = false;
	}

	public void StartMove()
	{
		_isActive = true;
		_isMoving = true;
		Car.StartMove();
	}

	public void Crash(float delay)
	{
		_isCrash = true;
	}

//	var point : Vector3;
//	var speed = 20.0;
//
//	private var v : Vector3;
//
//	function Start() {
//		v = transform.position - point;
//	}
//
//	function Update() {
//		v = Quaternion.AngleAxis(Time.deltaTime * speed, Vector3.forward) * v;
//		transform.position = point + v;
//	}
}
