using System;
using UnityEngine;

public class Car : MonoBehaviour
{
	public static event Action <float> OnCrash;
	public static event Action <bool> OnAddRoadItem;

	public CarSimulator CarSimulator;
	public ParticleSystem[] Particles;
	private bool _isCrash;
	private float _crashRotation;
//	private AudioClip _sndEngine;
	private AudioSource _sndEngine;

	void Start()
	{
		_sndEngine = GetComponent<AudioSource>();
//		_sndEngine = Resources.Load<AudioClip>("snd/engine");
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
		if (_isCrash)
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

		_isCrash = false;

		_sndEngine.Play(0);
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
		_isCrash = true;
		_crashRotation = velocity / 3f;
		_sndEngine.Stop();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (_isCrash) return;

		if (other.CompareTag("WallInside"))
		{
			DefsGame.CameraMovement.StopMoving();
			GameEvents.Send(OnCrash, 0f);
			Crash(CarSimulator.CurrVelocity);
		} else

		if (other.CompareTag("WallOutside"))
		{
			DefsGame.CameraMovement.StopMoving();
			GameEvents.Send(OnCrash, 0f);
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
				if (!DefsGame.CameraMovement.IsMovingToTarget)
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
				if (!DefsGame.CameraMovement.IsMovingToTarget)
					DefsGame.CameraMovement.StartMoving();
			}
		}
	}
}
