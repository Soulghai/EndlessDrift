using System;
using UnityEngine;

public class CarSimulator : MonoBehaviour
{
	public static event Action <float> OnGameOver;
	public static event Action <int> OnAddPoints;
	public Car Car;
	public float Velocity = 72f;
	private float _acceleration;
	[HideInInspector] public float CurrVelocity;
	private float _radius = 6f;
	private Vector3 _goalPoint;
	private bool _isCrash;
	private bool _isMoving;
	private bool _isActive;

	private RoadManager.RoadType _startType;
	private float _oldAddPointsValue;
	private float _accToAddPoints;
	private float _speedToAddPoints;
	private float _pointsValue;
	private AudioClip _sndRotate;

	// Use this for initialization

	void Awake()
	{
		DefsGame.CarSimulator = this;
		_sndRotate = Resources.Load<AudioClip>("snd/char_next");
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
		if (_startType == RoadManager.RoadType.UpToRight)
		{
			CurrVelocity = -Velocity;
			transform.RotateAround(_goalPoint, Vector3.forward, -80f);
		}
		else if (_startType == RoadManager.RoadType.RightToDown)
		{
			CurrVelocity = Velocity;
			transform.RotateAround(_goalPoint, Vector3.forward, 80f);
		}
		else if (_startType == RoadManager.RoadType.DownToLeft)
		{
			CurrVelocity = -Velocity;
			transform.RotateAround(_goalPoint, Vector3.forward, 100f);
		}
		else if (_startType == RoadManager.RoadType.LeftToUp)
		{
			CurrVelocity = Velocity;
			transform.RotateAround(_goalPoint, Vector3.forward, -100f);
		}

		_accToAddPoints = 0.0001f;
		_speedToAddPoints = 0f;
		_oldAddPointsValue = 0f;
		_pointsValue = 0f;

		Car.Respown();
	}

	// Update is called once per frame
	void Update () {
		if (!_isActive) return;

		transform.RotateAround(_goalPoint, Vector3.forward, CurrVelocity * Time.deltaTime);

		if (_isCrash)
		{
			if (CurrVelocity > 1f) CurrVelocity -= 1.8f;
			else
			if (CurrVelocity < -1f) CurrVelocity += 1.8f;
			else
			{
				Stop();
				//Invoke("Respown", 0.5f);

				GameEvents.Send(OnGameOver, 0f);
			}
		}
		else if (_isMoving)
		{

			bool accIsChanged = false;
			if (DefsGame.currentPointsCount < 30)
			{
				_acceleration = 0.03f;
				accIsChanged = true;
			} else if (DefsGame.currentPointsCount < 50)
			{
				_acceleration = 0.015f;
				accIsChanged = true;
			} else if (DefsGame.currentPointsCount < 100)
			{
				_acceleration = 0.01f;
				accIsChanged = true;
			} else if (DefsGame.currentPointsCount < 200)
			{
				_acceleration = 0.005f;
				accIsChanged = true;
			}
			if (accIsChanged)
				if (CurrVelocity < 0f)
				{
					_acceleration *= -1;
				}

			CurrVelocity += _acceleration;
			_speedToAddPoints += _accToAddPoints;
			_pointsValue += _speedToAddPoints;

			if (_pointsValue - _oldAddPointsValue >= 1.0f)
			{
				GameEvents.Send(OnAddPoints, (int)(_pointsValue-_oldAddPointsValue));
				_oldAddPointsValue = _pointsValue;
			}

			if ((InputController.IsTouchOnScreen(TouchPhase.Began))
				&&(DefsGame.CameraMovement.IsMovingToTarget))
			{
				Vector2 currentForwardNormal = transform.up * -_radius;
				_goalPoint = new Vector3(transform.position.x + currentForwardNormal.x,
					transform.position.y + currentForwardNormal.y, transform.position.z);

				CurrVelocity *= -1f;
				_acceleration *= -1f;

				//Body.transform.DORotate(Vector3.forward*180f, 1f);
				transform.Rotate(Vector3.forward, 180f);
				//Car.Rotate(new Vector3(0f, 0f, transform.rotation.z));
				Defs.PlaySound(_sndRotate, 4f);
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
