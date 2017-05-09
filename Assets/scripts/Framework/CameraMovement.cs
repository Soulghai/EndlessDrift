using UnityEngine;

public class CameraMovement : MonoBehaviour {
	public float DampTime = 0.15f;
	public Transform Target;
	[HideInInspector] public bool IsMoving;
	[HideInInspector] public bool IsMoveToPosition;
	[HideInInspector] public Vector3 TargetPosition;

	private void Awake()
	{
		DefsGame.CameraMovement = this;
	}

	// Update is called once per frame
	void Update ()
	{
		if (Target&&IsMoving)
		{
			transform.position = Vector3.Lerp(transform.position, Target.position, DampTime) + new Vector3(0.0f, 0f, -10f);
		}else if (IsMoveToPosition)
		{
			transform.position = Vector3.Lerp(transform.position, TargetPosition, DampTime) + new Vector3(0.0f, 0f, -10f);
		}
	}

	public void StartMoving()
	{
		IsMoving = true;
	}

	public void StopMoving()
	{
		IsMoving = false;
	}

	public void SetPosition(Vector3 position)
	{
		transform.position = new Vector3(position.x, position.y, -10f);
	}
}
