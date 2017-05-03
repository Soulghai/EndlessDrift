using UnityEngine;

public class CarParticle : MonoBehaviour
{
	public Car Car;

	private Vector3 _startPosition;

	// Use this for initialization
	void Start ()
	{
		_startPosition = Car.transform.position - transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.position = Car.transform.position + _startPosition;
	}
}
