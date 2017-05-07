using UnityEngine;

public class CameraMovement : MonoBehaviour {
	public float DampTime = 0.15f;
	public Transform Target;
	[HideInInspector] public bool IsMoving;

	private void Awake()
	{
		DefsGame.CameraMovement = this;
	}

	void Start()
	{

	}

	// Update is called once per frame
	void Update ()
	{
		if (Target&&IsMoving)
		{
			transform.position = Vector3.Lerp(transform.position, Target.position, DampTime) + new Vector3(0.01f, 0f, -10f);
		}
	}

	public void UpdatePosition()
	{
		transform.position = new Vector3(Target.position.x, Target.position.y, -10f);
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
