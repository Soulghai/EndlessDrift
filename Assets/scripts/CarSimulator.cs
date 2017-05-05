using UnityEngine;

public class CarSimulator : MonoBehaviour {
	public GameObject GoalPointObject;
	public Car Car;
	public float Velocity = 70f;
	[HideInInspector] public float CurrVelocity;
	private float _radius = 6.5f;
	private Vector3 _goalPoint;
	private bool _isCrash;
	private bool _isMoving;


	// Use this for initialization
	void Start ()
	{
		CurrVelocity = 0f;
		transform.position = Car.transform.position;
		_radius = -transform.position.y;
		Vector2 currentForwardNormal = transform.up*_radius;
		_goalPoint = new Vector3(transform.position.x + currentForwardNormal.x,
			transform.position.y + currentForwardNormal.y, transform.position.z);
		GoalPointObject.transform.position = _goalPoint;

		//transform.Rotate(Vector3.forward, -90f);

		DefsGame.CameraMovement.StartMoving();
	}

	void OnEnable() {
		Car.OnCrash += Crash;
	}

	void OnDisable() {
		Car.OnCrash -= Crash;
	}

	// Update is called once per frame
	void Update () {
		transform.RotateAround(_goalPoint, Vector3.forward, CurrVelocity * Time.deltaTime);


		if (_isCrash)
		{
			if (CurrVelocity > 0f) CurrVelocity -= 1.0f;
			else
			{
				CurrVelocity = 0f;
			}
		}
		else if (_isMoving)
		{
			if (InputController.IsTouchOnScreen(TouchPhase.Began))
			{
				Vector2 currentForwardNormal = transform.up * -_radius;
				_goalPoint = new Vector3(transform.position.x + currentForwardNormal.x,
					transform.position.y + currentForwardNormal.y, transform.position.z);
				GoalPointObject.transform.position = _goalPoint;

				CurrVelocity *= -1f;

				//Body.transform.DORotate(Vector3.forward*180f, 1f);
				transform.Rotate(Vector3.forward, 180f);
				//Car.Rotate(new Vector3(0f, 0f, transform.rotation.z));
			}
		}
		else
		{
			if (InputController.IsTouchOnScreen(TouchPhase.Began))
			{
				StartMove();
			}
		}
	}

	private void StartMove()
	{
		_isMoving = true;
		CurrVelocity = Velocity;
		DefsGame.CameraMovement.StartMoving();
	}

	public void Crash()
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
