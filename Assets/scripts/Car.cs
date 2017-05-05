using System;
using UnityEngine;

public class Car : MonoBehaviour
{
	public static event Action OnCrash;

	public CarSimulator CarSimulator;
	private bool _isCash;
	private float _crashRotation;

	// Update is called once per frame
	void Update ()
	{
		transform.position = CarSimulator.transform.position;
		if (_isCash)
		{
			if (_crashRotation > 0.5f)
			{
				transform.RotateAround(Vector3.forward, _crashRotation*Mathf.Deg2Rad);
				_crashRotation -= _crashRotation*0.1f;
			}
		}
		else
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, CarSimulator.transform.rotation, 0.25f);
		}
	}

	public void Crash(float velocity)
	{
		_isCash = true;
		if (velocity > 0f)
			_crashRotation = 20f;
		else
			_crashRotation = -20f;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("WallInside"))
		{
			DefsGame.CameraMovement.StopMoving();
			GameEvents.Send(OnCrash);
			Crash(CarSimulator.CurrVelocity);
		} else

		if (other.CompareTag("WallOutside"))
		{
			DefsGame.CameraMovement.StopMoving();
			GameEvents.Send(OnCrash);
			Crash(CarSimulator.CurrVelocity);
		}
	}
}
