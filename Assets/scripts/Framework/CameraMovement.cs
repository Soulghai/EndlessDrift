using UnityEngine;

public class CameraMovement : MonoBehaviour {
	public float dampTime = 0.15f;
	public Transform target;

	// Update is called once per frame
	void Update ()
	{
		if (target)
		{
			transform.position = Vector3.Lerp(transform.position, target.position, dampTime) + new Vector3(0.01f, 0f, -10f);
		}

	}
}
