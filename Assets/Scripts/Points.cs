using UnityEngine;
using UnityEngine.UI;

public class Points : MonoBehaviour {
	public Text textField;
	private bool _isShowAnimation = false;
	private float _startScale;
	private bool _isPointAdded;
	private float _time = 0f;
	private const float Delay = 0.8f;

	// Use this for initialization
	void Start () {
		textField.text = "0";
		Color color = textField.color;
		color.a = 0f;
		textField.color = color;
		_startScale = textField.transform.localScale.x;
	}

	public void ShowAnimation() {
		_isShowAnimation = true;
	}

	public void ResetCounter() {
		DefsGame.currentPointsCount = 0;
		textField.text = "0";
	}
	
	// Update is called once per frame
	void Update () {
		if (_isShowAnimation) {
			Color color = textField.color;
			if (textField.color.a < 1f) {
				color.a += 0.1f;
			} else {
				_isShowAnimation = false;
				color.a = 1f;
			}
			textField.color = color;
		}

		if (_isPointAdded) {
			_time += Time.deltaTime;
			if (_time > Delay) {
				_time = 0f;
				_isPointAdded = false;
				AddPointVisual ();
			}
		}

		if (textField.transform.localScale.x > _startScale) {
			textField.transform.localScale = new Vector3 (textField.transform.localScale.x - 2.5f*Time.deltaTime, textField.transform.localScale.y - 2.5f*Time.deltaTime, 1f);
		}
	}

	public void AddPoint(int count)
	{
		DefsGame.currentPointsCount += count;
		if (DefsGame.gameBestScore < DefsGame.currentPointsCount) {
			DefsGame.gameBestScore = DefsGame.currentPointsCount;
		}
		_isPointAdded = true;
	}

	void AddPointVisual()
	{
		textField.text = DefsGame.currentPointsCount.ToString ();
		textField.transform.localScale = new Vector3 (_startScale*1.4f, _startScale*1.4f, 1f);
	}

	public void UpdateVisual() {
		textField.text = DefsGame.currentPointsCount.ToString ();
	}
}
