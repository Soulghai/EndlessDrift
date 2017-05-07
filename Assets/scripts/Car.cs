using System;
using UnityEngine;

public class Car : MonoBehaviour
{
	public static event Action OnCrash;
	public static event Action <bool> OnAddRoadItem;

	public CarSimulator CarSimulator;
	public ParticleSystem[] Particles;
	private bool _isCash;
	private float _crashRotation;

	void Start()
	{
		foreach (ParticleSystem particle in Particles)
		{
			particle.Stop();
		}
		enabled = false;
	}

	// Update is called once per frame
	void Update ()
	{
		transform.position = CarSimulator.transform.position;
		if (_isCash)
		{
			if (Mathf.Abs(_crashRotation) > 0.2f)
			{
//				transform.RotateAround(Vector3.forward, _crashRotation*Mathf.Deg2Rad);
				transform.Rotate(Vector3.forward, _crashRotation);
				_crashRotation = Mathf.Lerp(_crashRotation, 0, 0.07f);
			}
			else
			{
				enabled = false;
			}
		}
		else
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, CarSimulator.transform.rotation, 0.25f);
		}
	}

	public void Respown()
	{
		transform.position = CarSimulator.transform.position;
		transform.rotation = CarSimulator.transform.rotation;
		foreach (ParticleSystem particle in Particles)
		{
			particle.Stop();
		}

		_isCash = false;
	}

	public void StartMove()
	{
		foreach (ParticleSystem particle in Particles)
		{
			particle.Play();
		}
	}

	public void Crash(float velocity)
	{
		_isCash = true;
		_crashRotation = velocity / 3f;
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
		} else

		if (other.CompareTag("RoadEdgeLine"))
		{
			RoadItem ri = other.gameObject.GetComponentInParent<RoadItem>();
			int counter = ri.CrossEdgeLine();
			if (counter == 1)
			{
				GameEvents.Send(OnAddRoadItem, true);
			}
			if (counter == 2)
			{
				if (!DefsGame.CameraMovement.IsMoving)
					DefsGame.CameraMovement.StartMoving();
			}
		}else

		if (other.CompareTag("RoadEdgeLine2"))
		{
			RoadItem ri = other.gameObject.GetComponentInParent<RoadItem>();
			int counter = ri.CrossEdgeLine();
			if (counter == 1)
			{
				GameEvents.Send(OnAddRoadItem, false);
			}
			if (counter == 2)
			{
				if (!DefsGame.CameraMovement.IsMoving)
					DefsGame.CameraMovement.StartMoving();
			}
		}
	}
}
