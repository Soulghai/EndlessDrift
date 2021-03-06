﻿using UnityEngine;
using UnityEngine.UI;

public class PointsBubbleManager : MonoBehaviour {
	public GameObject PointsObject;
	// Use this for initialization
	private bool _isAddPoint = false;
	private float _time = 0;
	private readonly float delay = 0.2f;
	private int _count = 0;
	private Vector3 _pos;
	
	// Update is called once per frame
	void Update () {
		if (_isAddPoint) {
			_time += Time.deltaTime;
			if (_time >= delay) {
				_isAddPoint = false;
				_time = 0;
				GameObject go = (GameObject)Instantiate (PointsObject, _pos, Quaternion.identity);
				Text text = go.GetComponentInChildren<Text> ();
				text.text = "+" + _count.ToString ();
			    switch (_count)
			    {
			        case 1:
			        {
			            text.color = new Color(170f / 255f, 75f / 255f, 196f / 255f);
			            go.transform.localScale = new Vector3(go.transform.localScale.x * 0.5f, go.transform.localScale.x * 0.5f,
			                1);
			        }
			            break;
			        case 3:
			        {
			            text.color = new Color(77f / 255f, 97f / 255f, 217f / 255f);
			            go.transform.localScale = new Vector3(go.transform.localScale.x * 0.75f,
			                go.transform.localScale.x * 0.75f,
			                1);
			        }
			            break;
			        case 10:
			        {
			            text.color = new Color(222f / 255f, 125f / 255f, 48f / 255f);
			            go.transform.localScale = new Vector3(go.transform.localScale.x * 1.5f, go.transform.localScale.x * 1.5f,
			                1);
			        }
			            break;
			        case 30:
			        {
			            text.color = new Color(42f / 255f, 131f / 255f, 30f / 255f);
			            go.transform.localScale = new Vector3(go.transform.localScale.x * 2f, go.transform.localScale.x * 2f,
			                1);
			        }
			            break;
			    }
			}
		}
	}

	public void AddPoints(int count) {
		_isAddPoint = true;
		_count = count;

		_pos = new Vector3(0f, 0f, 0f);
	}
}
