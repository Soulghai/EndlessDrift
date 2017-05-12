﻿using UnityEngine;

public class CoinSensor : MonoBehaviour
{
	public GameObject Сoin;

	private bool _isVisible;
	private bool _isShowAnimation;
	private bool _isHideAnimation;

    private Collider2D _collider;
    private GameObject _target;

    // Use this for initialization
	void Start ()
	{
	    _collider = GetComponent<Collider2D>();
	}

    public void Init(GameObject target)
    {
        _target = target;
        transform.localScale = new Vector3 (0f, 0f, 0f);
    }
	
	// Update is called once per frame
	void Update () {
	    if (_isShowAnimation)
	    {
            transform.localScale = new Vector3 (transform.localScale.x + 0.1f, transform.localScale.y + 0.1f, 1f);
            if (transform.localScale.x >= 1f) {
                _isShowAnimation = false;
                transform.localScale = new Vector3 (1f, 1f, 1f);
            }
	    }
//      else
//	    if (_isHideAnimation)
//	    {
//	        transform.localScale = new Vector3 (transform.localScale.x - 0.1f, transform.localScale.y - 0.1f, 1f);
//	        if (transform.localScale.x <= 0f) {
//	            //GameEvents.Send(OnAddCoinsVisual, 1);
//	            //Destroy (gameObject);
//	            _isHideAnimation = false;
//	            transform.localScale = new Vector3 (0f, 0f, 0f);
//	            _isVisible = false;
//	        }
//	    }

	    if (_isVisible)
	    {
	        transform.position = Vector3.Lerp(transform.position, _target.transform.position, 0.3f);
	    }
	}

    public void Show(bool isAnimation) {
        _isShowAnimation = isAnimation;
        _collider.enabled = true;
        _isVisible = true;

        if (_isShowAnimation)
        {
            transform.localScale = new Vector3 (0f, 0f, 1f);
        }
    }

    public void Hide()
    {
        _collider.enabled = false;
//        _isHideAnimation = true;

	    transform.localScale = new Vector3 (0f, 0f, 0f);
	    _isVisible = false;
    }

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			GameObject coin = (GameObject)Instantiate (Сoin, transform.position, Quaternion.identity);
			Coin coinScript = coin.GetComponent<Coin> ();
			coinScript.MoveToEnd();

			Hide();
		}
	}
}
