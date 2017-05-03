using UnityEngine;

public class CarSimulator : MonoBehaviour {
	public GameObject GoalPointObject;
	public Car Car;
	public float Velocity = 70f;
	private float _radius = 6.5f;
	private Vector3 _goalPoint;


	// Use this for initialization
	void Start ()
	{
		transform.position = Car.transform.position;
		_radius = -transform.position.y;
		Vector2 currentForwardNormal = transform.up*_radius;
		_goalPoint = new Vector3(transform.position.x + currentForwardNormal.x,
			transform.position.y + currentForwardNormal.y, transform.position.z);
		GoalPointObject.transform.position = _goalPoint;

		//transform.Rotate(Vector3.forward, -90f);
	}

	// Update is called once per frame
	void Update () {
		transform.RotateAround(_goalPoint, Vector3.forward, Velocity*Time.deltaTime);

		if (InputController.IsTouchOnScreen(TouchPhase.Began))
		{
			Vector2 currentForwardNormal = transform.up*-_radius;
			_goalPoint = new Vector3(transform.position.x + currentForwardNormal.x,
				transform.position.y + currentForwardNormal.y, transform.position.z);
			GoalPointObject.transform.position = _goalPoint;

			Velocity *= -1f;


			//Body.transform.DORotate(Vector3.forward*180f, 1f);
			transform.Rotate(Vector3.forward, 180f);
			//Car.Rotate(new Vector3(0f, 0f, transform.rotation.z));
		}

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
