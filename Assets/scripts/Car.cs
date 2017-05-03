using UnityEngine;

public class Car : MonoBehaviour
{
	public GameObject CarSimulator;

	// Update is called once per frame
	void Update ()
	{
		transform.position = CarSimulator.transform.position;
		transform.rotation = Quaternion.Lerp(transform.rotation, CarSimulator.transform.rotation, 0.25f);
	}
}
