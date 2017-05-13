using System.Collections.Generic;
using UnityEngine;

public class RoadPictureArray : MonoBehaviour
{

	public GameObject[] Prefabs;
	[HideInInspector] public float SpriteSize;

	private readonly List<GameObject> _cachedObjects = new List<GameObject>();

	public void CreateCachedArr()
	{
		GameObject newGo;
		for (int i = 0; i < Prefabs.Length; i++)
		{
			newGo = (GameObject) Instantiate(Prefabs[i], Vector3.zero, Quaternion.identity);
			newGo.SetActive(false);
			_cachedObjects.Add(newGo);

			newGo = (GameObject) Instantiate(Prefabs[i], Vector3.zero, Quaternion.identity);
			newGo.SetActive(false);
			_cachedObjects.Add(newGo);

			newGo = (GameObject) Instantiate(Prefabs[i], Vector3.zero, Quaternion.identity);
			newGo.SetActive(false);
			_cachedObjects.Add(newGo);

			newGo = (GameObject) Instantiate(Prefabs[i], Vector3.zero, Quaternion.identity);
			newGo.SetActive(false);
			_cachedObjects.Add(newGo);
		}

		SpriteRenderer spr = _cachedObjects[0].GetComponent<SpriteRenderer>();
		SpriteSize = spr.sprite.bounds.size.x;
	}

	public RoadPictureItem GetSuitablePicture(RoadManager.RoadType type, RoadManager.RoadType prevType, RoadManager.BuildState buildState, Vector3 position, bool isRightDirection)
	{
		RoadPictureItem rpi = GetCachedObject(buildState, isRightDirection, type, prevType);
		rpi.transform.position = position;
		RotateByType(rpi, type);
		return rpi;
	}

	private RoadPictureItem GetCachedObject(RoadManager.BuildState buildState, bool isRightDirection, RoadManager.RoadType type, RoadManager.RoadType prevType)
	{
		foreach (GameObject go in _cachedObjects)
		{
			if (!go.activeSelf)
			{
				RoadPictureItem rpi = go.GetComponent<RoadPictureItem>();
				if ((buildState == RoadManager.BuildState.BuildFirstFloor) ||
				    (buildState == RoadManager.BuildState.BuildSecondFloor))
				{
					if (rpi.BuildState == buildState)
					{
						go.SetActive (true);
						return rpi;
					}
				}
				else
				if ((rpi.BuildState == RoadManager.BuildState.BuildClimbItem)
					||(rpi.BuildState == RoadManager.BuildState.BuildDescentItem))
				{
						if (((prevType == RoadManager.RoadType.LeftToUp) && (type == RoadManager.RoadType.UpToRight))
							||((prevType == RoadManager.RoadType.UpToRight) && (type == RoadManager.RoadType.LeftToUp))
							|| ((prevType == RoadManager.RoadType.RightToDown) && (type == RoadManager.RoadType.DownToLeft))
							|| ((prevType == RoadManager.RoadType.DownToLeft) && (type == RoadManager.RoadType.RightToDown))
								
							|| ((prevType == RoadManager.RoadType.RightToDown) && (type == RoadManager.RoadType.LeftToUp) && isRightDirection)	
							|| ((prevType == RoadManager.RoadType.LeftToUp) && (type == RoadManager.RoadType.RightToDown) && isRightDirection)
							|| ((prevType == RoadManager.RoadType.UpToRight) && (type == RoadManager.RoadType.DownToLeft) && !isRightDirection)
							|| ((prevType == RoadManager.RoadType.DownToLeft) && (type == RoadManager.RoadType.UpToRight) && !isRightDirection)
						)
						{
							if (rpi.BuildState == buildState)
							{
								go.SetActive(true);
								return rpi;
							}
						}
						else
						{
							if (rpi.BuildState != buildState)
							{
								go.SetActive(true);
								return rpi;
							}
						}
				}
			}
		}

		return null;
	}

	private void RotateByType(RoadPictureItem rpi, RoadManager.RoadType type)
	{
		SpriteRenderer spr = rpi.GetComponent<SpriteRenderer>();
		switch (type)
		{
			case RoadManager.RoadType.UpToRight:
			{
				if (spr.flipX) spr.flipX = false;
				if (spr.flipY) spr.flipY = false;
			}
				break;
			case RoadManager.RoadType.RightToDown:
			{
				if (!spr.flipX) spr.flipX = true;
				if (spr.flipY) spr.flipY = false;
			}
				break;
			case RoadManager.RoadType.DownToLeft:
			{
				if (!spr.flipX) spr.flipX = true;
				if (!spr.flipY) spr.flipY = true;
			}
				break;
			case RoadManager.RoadType.LeftToUp:
			{
				if (spr.flipX) spr.flipX = false;
				if (!spr.flipY) spr.flipY = true;
			}
				break;
		}
	}

}
