using UnityEngine;

public class CameraMovement : MonoBehaviour {
	public float DampTime = 0.15f;
	public Transform Target;
	private bool _isMoving;

	void Start()
	{
		DefsGame.CameraMovement = this;
	}

	// Update is called once per frame
	void Update ()
	{
		if (Target&&_isMoving)
		{
			transform.position = Vector3.Lerp(transform.position, Target.position, DampTime) + new Vector3(0.01f, 0f, -10f);
		}
	}

	public void StartMoving()
	{
		_isMoving = true;
	}

	public void StopMoving()
	{
		_isMoving = false;
	}
}
