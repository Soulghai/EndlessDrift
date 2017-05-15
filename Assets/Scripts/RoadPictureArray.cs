using System.Collections.Generic;
using UnityEngine;

public class RoadPictureArray : MonoBehaviour
{

	public GameObject[] Prefabs;

	private readonly List<GameObject> _cachedObjects = new List<GameObject>();

	public void CreateCachedArr()
	{
		GameObject newGo;
		for (int i = 0; i < 2; i++)
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

		for (int i = 2; i < Prefabs.Length; i++)
		{
			newGo = (GameObject) Instantiate(Prefabs[i], Vector3.zero, Quaternion.identity);
			newGo.SetActive(false);
			_cachedObjects.Add(newGo);

			newGo = (GameObject) Instantiate(Prefabs[i], Vector3.zero, Quaternion.identity);
			newGo.SetActive(false);
			_cachedObjects.Add(newGo);
		}
	}

	public RoadPictureItem GetSuitablePicture(RoadManager.RoadType type, RoadManager.RoadType prevType, RoadManager.BuildState buildState, Vector3 position, bool isRightDirection)
	{
		RoadPictureItem rpi = GetCachedObject(buildState, isRightDirection, type, prevType);
		if (rpi == null) rpi = GetCachedObject(buildState, isRightDirection, type, prevType, true);
		rpi.transform.position = position;
		RotateByType(rpi, type);
		return rpi;
	}

	private RoadPictureItem GetCachedObject(RoadManager.BuildState buildState, bool isRightDirection, RoadManager.RoadType type, RoadManager.RoadType prevType, bool isCreate = false)
	{
		GameObject foundedGo = null;
		RoadPictureItem rpi;
		foreach (GameObject go in _cachedObjects)
		{
			if ((!go.activeSelf)||(isCreate))
			{
				rpi = go.GetComponent<RoadPictureItem>();
				if ((buildState == RoadManager.BuildState.BuildFirstFloor) ||
				    (buildState == RoadManager.BuildState.BuildSecondFloor))
				{
					if (rpi.BuildState == buildState)
					{
						if (!go.activeSelf)
						{
							foundedGo = go;
							break;
						}
						else
						{
							foundedGo = CreateNewPicture(go);
							break;
						}
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
								if (!go.activeSelf)
								{
									foundedGo = go;
									break;
								}
								else
								{
									foundedGo = CreateNewPicture(go);
									break;
								}
							}
						}
						else
						{
							if (rpi.BuildState != buildState)
							{
								if (!go.activeSelf)
								{
									foundedGo = go;
									break;
								}
								else
								{
									foundedGo = CreateNewPicture(go);
									break;
								}
							}
						}
				}
			}
		}

		if (foundedGo)
		{
			foundedGo.SetActive(true);
			rpi = foundedGo.GetComponent<RoadPictureItem>();
			return rpi;
		}

		return null;
	}

	private GameObject CreateNewPicture(GameObject go)
	{
		GameObject newGo = (GameObject) Instantiate(go, Vector3.zero, Quaternion.identity);
		newGo.SetActive(false);
		_cachedObjects.Add(newGo);
		return newGo;
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
