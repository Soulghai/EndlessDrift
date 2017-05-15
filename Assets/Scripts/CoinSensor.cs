using UnityEngine;

public class CoinSensor : MonoBehaviour
{
	public GameObject Сoin;

	[HideInInspector] public bool IsVisible;
	private bool _isShowAnimation;
	private bool _isHideAnimation;

    private Collider2D _collider;
    private GameObject _target;

	private float _lifeDelay = 4.0f;
	private float _lifeTime;

	private AudioClip _sndTakeCoin;

    // Use this for initialization
	void Awake ()
	{
	    _collider = GetComponent<Collider2D>();
		_sndTakeCoin = Resources.Load<AudioClip>("snd/bonus_spin");
	}

    public void Init(GameObject target)
    {
        _target = target;
        transform.localScale = new Vector3 (0f, 0f, 0f);
    }
	
	// Update is called once per frame
	void Update ()
	{
		_lifeTime += Time.deltaTime;
		if (_lifeTime >= _lifeDelay)
		{
			Hide(true);
		}

	    if (_isShowAnimation)
	    {
            transform.localScale = new Vector3 (transform.localScale.x + 0.1f, transform.localScale.y + 0.1f, 1f);
            if (transform.localScale.x >= 1.1f) {
                _isShowAnimation = false;
                transform.localScale = new Vector3 (1f, 1f, 1f);
            }
	    }
      else
	    if (_isHideAnimation)
	    {
	        transform.localScale = new Vector3 (transform.localScale.x - 0.1f, transform.localScale.y - 0.1f, 1f);
	        if (transform.localScale.x <= 0f) {
	            //GameEvents.Send(OnAddCoinsVisual, 1);
	            //Destroy (gameObject);
	            _isHideAnimation = false;
		        transform.localScale = new Vector3 (0f, 0f, 0f);
		        _collider.enabled = false;
		        IsVisible = false;
	        }
	    }

	    if (IsVisible && _target)
	    {
	        transform.position = Vector3.Lerp(transform.position, _target.transform.position, transform.position.z);
	    }
	}

    public void Show(float lifeDelay)
    {
	    _lifeDelay = lifeDelay;
        _isShowAnimation = true;
        _collider.enabled = true;
        IsVisible = true;

        transform.localScale = new Vector3 (0f, 0f, 1f);

	    _lifeTime = 0;
    }

    public void Hide(bool anim)
    {
	    _isHideAnimation = anim;
	    if (_isHideAnimation)
	    {

	    }
	    else
	    {
		    transform.localScale = new Vector3 (0f, 0f, 0f);
		    _collider.enabled = false;
		    IsVisible = false;
	    }

    }

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			GameObject coin = (GameObject)Instantiate (Сoin, transform.position, Quaternion.identity);
			Coin coinScript = coin.GetComponent<Coin> ();
			coinScript.Show();
			coinScript.MoveToEnd();

			Defs.PlaySound(_sndTakeCoin);

			Hide(false);
		}
	}
}
