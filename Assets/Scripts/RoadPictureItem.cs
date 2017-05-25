﻿using UnityEngine;

public class RoadPictureItem : MonoBehaviour {

	public RoadManager.BuildState BuildState;

	public Vector3 GetCenter()
	{
		return GetComponent<SpriteRenderer>().sprite.bounds.center;
	}

	public void SetZIndex(int value)
	{
		GetComponent<SpriteRenderer>().sortingOrder = value;
	}
}
