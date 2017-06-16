using UnityEngine;
using System;
using Random = UnityEngine.Random;
using DoozyUI;

public class ScreenGame : MonoBehaviour {
	public static event Action OnNewGame;

	public GameObject screenAnimationObject;

    private ScreenColorAnimation _screenAnimation;
	public GameObject[] backgrounds;
    private int _currentBackgroundId = 0;
    private GameObject _backgroundPrev;
    private GameObject _backgroundNext;
    private bool _isBackgroundChange = false;
    private Points _points;
    private Coins _coins;
    private BestScore _bestScore;
//    private PointsBubbleManager _poinsBmScript;
	//GameObject hint = null;
	//SpriteRenderer hintSprite;

    private float _time = 0f;
    private bool _isNextLevel;
    private int _state = -1;
    private float _missDelay = 0f;
	[HideInInspector] public bool IsGameOver = false;
//    private Vector3 _cameraStartPos;

	public static event Action OnShowMenu;
	//int hintCounter;
	//bool isHint = false;

    private bool _isScreenReviveDone = false;
    private bool _isScreenShareDone = false;
    private bool _isScreenRateDone = false;

    private bool _isReviveUsed;

    private AudioClip _sndLose;
    private AudioClip _sndShowScreen;
    private AudioClip _sndGrab;
    private AudioClip _sndClose;
	
	private bool _isWaitReward;
	private bool IsRewardedVideoReadyToShow;

	void Awake ()
	{
	    DefsGame.ScreenGame = this;
	}

	// Use this for initialization
	void Start () {
		Defs.AudioSourceMusic = GetComponent<AudioSource> ();
		_screenAnimation = screenAnimationObject.GetComponent<ScreenColorAnimation> ();
		_points = GetComponent<Points> ();
		_coins = GetComponent<Coins> ();
		_bestScore = GetComponent<BestScore> ();
//		_poinsBmScript = GetComponent<PointsBubbleManager> ();

		_state = 0;

		/*hintCounter = PlayerPrefs.GetInt ("hintCounter", 3);
		if (hintCounter >= 3) {
			isHint = true;
			hint = (GameObject)Instantiate (hintPerefab, new Vector3(0.3f, -1.0f,1), Quaternion.identity);
			hintSprite = hint.GetComponent<SpriteRenderer>();

			hint.SetActive (true);
		} */

		_sndLose = Resources.Load<AudioClip>("snd/fail");
		_sndShowScreen = Resources.Load<AudioClip>("snd/showScreen");
		_sndGrab = Resources.Load<AudioClip>("snd/grab");
		_sndClose = Resources.Load<AudioClip>("snd/button");
	}

	void Init() {
		DefsGame.GameServices.ReportProgressWithGlobalID (DefsGame.GameServices.ACHIEVEMENT_FIRST_WIN, DefsGame.GameBestScore);

		++DefsGame.QUEST_GAMEPLAY_Counter;
		PlayerPrefs.SetInt ("QUEST_GAMEPLAY_Counter", DefsGame.QUEST_GAMEPLAY_Counter);

		DefsGame.GameServices.ReportProgressWithGlobalID (DefsGame.GameServices.ACHIEVEMENT_MASTER, DefsGame.QUEST_GAMEPLAY_Counter);

		++DefsGame.GameplayCounter;
	
		PlayerPrefs.SetInt ("QUEST_THROW_CounterCounter", DefsGame.QUEST_THROW_Counter);

		DefsGame.GameServices.ReportProgressWithGlobalID (DefsGame.GameServices.ACHIEVEMENT_FiFIELD_OF_CANDIES, DefsGame.QUEST_THROW_Counter);

		// Сохраняемся тут, чтобы не тормозить игру
		PlayerPrefs.SetInt ("QUEST_BOMBS_Counter", DefsGame.QUEST_BOMBS_Counter);
		DefsGame.GameServices.ReportProgressWithGlobalID (DefsGame.GameServices.ACHIEVEMENT_EXPLOSIVE, DefsGame.QUEST_BOMBS_Counter);

		_points.UpdateVisual ();
		_coins.UpdateVisual ();
		_bestScore.UpdateVisual ();

		_isNextLevel = false;

		_isScreenReviveDone = false;
		_isScreenShareDone = false;
		_isScreenRateDone = false;

		_isReviveUsed = false;

		GameEvents.Send(OnNewGame);
		
		if (DefsGame.GameplayCounter > 1)
		GlobalEvents<OnShowNotifications>.Call(new OnShowNotifications());
	}

    private void OnEnable() {
	    CarSimulator.OnGameOver += OnCrash;
		CarSimulator.OnAddPoints += OnAddPoints;
		RoadManager.OnGameplayStart += OnGameplayStart;
	    GlobalEvents<OnGiveReward>.Happened += GetReward;
	    GlobalEvents<OnRewardedAvailable>.Happened += IsRewardedVideoAvailable;
	}

    private void OnDisable() {
	    CarSimulator.OnGameOver -= OnCrash;
	    CarSimulator.OnAddPoints -= OnAddPoints;
	    RoadManager.OnGameplayStart -= OnGameplayStart;
	    GlobalEvents<OnGiveReward>.Happened -= GetReward;
	    GlobalEvents<OnRewardedAvailable>.Happened -= IsRewardedVideoAvailable;
	}
	
	private void IsRewardedVideoAvailable(OnRewardedAvailable e) {
		IsRewardedVideoReadyToShow = e.isAvailable;
	}

	void OnAddPoints (int pointsCount)
	{
		if (IsGameOver)
			return;
		D.Log ("Ball_OnBallInBasket");
		_isNextLevel = true;

		_points.AddPoint (pointsCount);
//		_poinsBmScript.AddPoints (pointsCount);
	}

	void OnCrash ()
	{
		if (IsGameOver)
			return;

		Defs.PlaySound (_sndLose);

		if (DefsGame.IS_ACHIEVEMENT_MISS_CLICK == 0) {
			++DefsGame.QUEST_MISS_Counter;
			PlayerPrefs.SetInt ("QUEST_MISS_Counter", DefsGame.QUEST_MISS_Counter);
			DefsGame.GameServices.ReportProgressWithGlobalID (DefsGame.GameServices.ACHIEVEMENT_MISS_CLICK, DefsGame.QUEST_MISS_Counter);
		}

		_state = 3;
	}

	void OnGameplayStart ()
	{
		if (IsGameOver)
			return;

		++DefsGame.QUEST_THROW_Counter;

		if (DefsGame.GameplayCounter == 1) {
			_points.ShowAnimation ();
		}
		if (_state == 1) {
			DefsGame.CurrentScreen = DefsGame.SCREEN_GAME;
			_points.ResetCounter ();
			UIManager.ShowUiElement ("scrMenuWowSlider");
			_state = 2;
			FlurryEventsManager.SendStartEvent ("attempt_length");
		}

		//isHint = false;
	}

	public void EndCurrentGame() {
		/*if (!isScreenReviveDone) {
			isScreenReviveDone = true;
			if (PublishingService.Instance.IsRewardedVideoReady() && DefsGame.currentPointsCount >= 4) {
				UIManager.ShowUiElement ("ScreenRevive");
				UIManager.ShowUiElement ("ScreenReviveBtnRevive");
				UIManager.ShowUiElement ("ScreenReviveBtnBack");
				D.Log ("isScreenReviveDone"); 
				Defs.PlaySound (sndShowScreen);

				FlurryEventsManager.SendEvent ("RV_revive_impression");
				return;
			}
		}

		if (!isScreenShareDone) {
			isScreenShareDone = true;
			if ((DefsGame.currentPointsCount >= 50) && (DefsGame.currentPointsCount == DefsGame.gameBestScore)) {
				UIManager.ShowUiElement ("ScreenShare");
				UIManager.ShowUiElement ("ScreenShareBtnShare");
				UIManager.ShowUiElement ("ScreenShareBtnBack");
				Defs.PlaySound (sndShowScreen);
				D.Log ("isScreenShareDone"); 

				FlurryEventsManager.SendEvent ("high_score_share_impression");
				return;
			}
		}

		if (!isScreenRateDone) {
			isScreenRateDone = true;
			if ((DefsGame.rateCounter < 3) && (DefsGame.currentPointsCount >= 100)
				&& (DefsGame.gameplayCounter % 20 == 0)) {
				++DefsGame.rateCounter;
				PlayerPrefs.SetInt ("rateCounter", DefsGame.rateCounter);
				UIManager.ShowUiElement ("ScreenRate");
				UIManager.ShowUiElement ("ScreenRateBtnRate");
				UIManager.ShowUiElement ("ScreenRateBtnBack");
				Defs.PlaySound (sndShowScreen);
				D.Log ("isScreenRateDone"); 

				FlurryEventsManager.SendEvent ("rate_us_impression", "revive_screen");
				return;
			}
		}*/

		_state = 6;

		//PublishingService.Instance.ShowSceneTransition();
	}

    // Update is called once per frame
	void Update () {
		BtnEscapeUpdate ();
		BackgroundUpdate ();

	    switch (_state)
	    {
	        case 0:
	            _state = 1;
	            Init();
	            return;
	        case 1:
	            /*if (isHint) {
                if (hintSprite.color.a < 1f) {
                    Color _color = hintSprite.color;
                    _color.a += 0.05f;
                    hintSprite.color = _color;
                }
            }*/
	            break;
	        case 2:
	            if (_isNextLevel)
	            {
	                //bubbleField.NextLevel ();
	                _isNextLevel = false;
	            }
	            else
	            {
	                //if (!DefsGame.wowSlider.UpdateSlider ()) {
	                //	GameOver ();
	                //	return;
	                //}
	            }
	            break;
	        case 3:
	            _time += Time.deltaTime;
	            if (_time >= _missDelay)
	            {
	                _time = 0f;
	                _screenAnimation.SetAlphaMax(0.93f);
	                _screenAnimation.SetAnimation(false, 0.1f);
	                _screenAnimation.Show();
	                _screenAnimation.SetAnimation(true, 0.02f);
	                _screenAnimation.SetColor(1.0f, 0.21f, 0.21f);
	                _screenAnimation.SetAutoHide(true);

	                _state = 4;
//		            _cameraStartPos = Camera.main.transform.position;
	            }
	            break;
	        case 4:
	            if (!_screenAnimation.isActiveAndEnabled)
	            {
	                _state = 5;
//	                Camera.main.transform.position = new Vector3(_cameraStartPos.x, _cameraStartPos.y, _cameraStartPos.z);
	                EndCurrentGame();
	            }
	            else
	            {
//	                Camera.main.transform.position = new Vector3(_cameraStartPos.x + Random.Range(-0.015f, 0.015f),
//	                    _cameraStartPos.y + Random.Range(-0.015f, 0.015f), _cameraStartPos.z);
	            }
	            break;
	        case 5:
	            break;
	        case 6:
	            /*FlurryEventsManager.SendEndEvent ("attempt_length");
            FlurryEventsManager.SendEventPlayed (isReviveUsed, fail_reason);

            if ((DefsGame.gameBestScore == DefsGame.currentPointsCount)&&(DefsGame.gameBestScore != 0)) {
                DefsGame.gameServices.SubmitScore (DefsGame.gameBestScore);
                PlayerPrefs.SetInt ("BestScore", DefsGame.gameBestScore);
            }*/
	            PlayerPrefs.SetInt("coinsCount", DefsGame.CoinsCount);

	            HintCheck();
	            IsGameOver = false;
	            NextBackground();
	            GameEvents.Send(OnShowMenu);
	            DefsGame.CurrentScreen = DefsGame.SCREEN_MENU;

		        Init();
		        _state = 1;
//	            _state = 7;
	            break;
	        case 7:
	            _time += Time.deltaTime;
	            if (_time >= 0.8f)
	            {
	                _time = 0f;
	                Init();
	                _state = 1;
	            }
	            break;
	    }

	    /*if ((!isHint)&&(hint)&&(hint.activeSelf)) {
            if (hintSprite.color.a > 0f) {
                Color _color = hintSprite.color;
                _color.a -= 0.05f;
                hintSprite.color = _color;
            } else {
                hint.SetActive (false);
            }
        }*/
	}

	void HintCheck(){
		/*if (DefsGame.currentPointsCount < 3) {
			++hintCounter;
			if (hintCounter >= 3) {
				isHint = true;
				if (!hint) {
					hint = (GameObject)Instantiate (hintPerefab, new Vector3 (0.3f, -1.0f, 1), Quaternion.identity);
					hintSprite = hint.GetComponent<SpriteRenderer> ();
				}
				Color _color = hintSprite.color;
				_color.a = 0;
				hintSprite.color = _color;
				hint.SetActive (true);
			} 
		} else {
			if (hintCounter != 0) {
				isHint = false;
				hintCounter = 0;
				PlayerPrefs.SetInt ("hintCounter", 0);
			}
		}*/
	}

	public void Revive() {
		FlurryEventsManager.SendEvent ("RV_revive");
		MyAds.ShowRewardedAds();
		_isWaitReward = true;
	}
	
	private void GetReward(OnGiveReward e)
	{
		if (_isWaitReward)
		{
			_isWaitReward = false;
			if (e.isAvailable)
			{
				_state = 2;
				_isNextLevel = true;
				IsGameOver = false;
				_isReviveUsed = true;
//				_bubbleField.Hide();

				ReviveClose();
				Defs.PlaySound(_sndGrab);

				FlurryEventsManager.SendEvent("RV_revive_complete");
			}
			else
			{
				ReviveClose();
				_state = 6;
			}
		}
	}

	public void Share() {
		UIManager.HideUiElement ("ScreenShare");
		UIManager.HideUiElement ("ScreenShareBtnShare");
		UIManager.HideUiElement ("ScreenShareBtnBack");
		if (SystemInfo.deviceModel.Contains ("iPad")) {
			Defs.ShareVoxel.ShareClick ();
		} else {
			Defs.Share.ShareClick ();
		}
		FlurryEventsManager.SendEvent ("high_score_share");
		Defs.PlaySound (_sndGrab);
		EndCurrentGame ();
	}


	public void Rate() {
		UIManager.HideUiElement ("ScreenRate");
		UIManager.HideUiElement ("ScreenRateBtnRate");
		UIManager.HideUiElement ("ScreenRateBtnBack");
		Defs.PlaySound (_sndGrab);
		Defs.Rate.RateUs ();
		FlurryEventsManager.SendEvent ("rate_us_impression", "revive_screen");
		EndCurrentGame ();
	}

	public void ReviveClose() {
		UIManager.HideUiElement ("ScreenRevive");
		UIManager.HideUiElement ("ScreenReviveBtnRevive");
		UIManager.HideUiElement ("ScreenReviveBtnBack");
		Defs.PlaySound (_sndClose);
		EndCurrentGame ();

		FlurryEventsManager.SendEvent ("RV_revive_home");
	}

	public void ShareClose() {
		UIManager.HideUiElement ("ScreenShare");
		UIManager.HideUiElement ("ScreenShareBtnShare");
		UIManager.HideUiElement ("ScreenShareBtnBack");
		Defs.PlaySound (_sndClose);
		EndCurrentGame ();

		FlurryEventsManager.SendEvent ("high_score_home");
	}


	public void RateClose() {
		UIManager.HideUiElement ("ScreenRate");
		UIManager.HideUiElement ("ScreenRateBtnRate");
		UIManager.HideUiElement ("ScreenRateBtnBack");
		Defs.PlaySound (_sndClose);
		EndCurrentGame ();
	}

	void NextBackground() {
		if (backgrounds.Length == 0) return;

		_isBackgroundChange = true;
		_backgroundPrev = backgrounds [_currentBackgroundId];
		++_currentBackgroundId;
		if (_currentBackgroundId >= backgrounds.Length)
			_currentBackgroundId = 0;

		_backgroundNext = backgrounds [_currentBackgroundId];
		_backgroundNext.SetActive (true);
		Color color = _backgroundPrev.GetComponent<SpriteRenderer> ().color;
		color.a = 0;
		_backgroundNext.GetComponent<SpriteRenderer> ().color = color;
	}

	void BackgroundUpdate() {
		if (backgrounds.Length == 0) return;
		if (_isBackgroundChange) {
			Color color = _backgroundPrev.GetComponent<SpriteRenderer> ().color;
			if (color.a > 0) {
				color.a -= 0.05f;
			}
			_backgroundPrev.GetComponent<SpriteRenderer> ().color = color;

			color = _backgroundNext.GetComponent<SpriteRenderer> ().color;
			if (color.a < 1) {
				color.a += 0.05f;
			}
			_backgroundNext.GetComponent<SpriteRenderer> ().color = color;

			if (color.a >= 1) {
				_isBackgroundChange = false;
				_backgroundPrev.SetActive (false);
				_backgroundPrev = null;
			}
		}
	}

	void BtnEscapeUpdate() {
		/*if (InputController.IsTouchOnScreen(TouchPhase.Began)) {
			DefsGame.QUEST_BOMBS_Counter += 50;
			DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_EXPLOSIVE, DefsGame.QUEST_BOMBS_Counter);

			//if (DefsGame.QUEST_BOMBS_Counter % 100 == 0) {
				DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_FiFIELD_OF_CANDIES, DefsGame.QUEST_BOMBS_Counter);
			//}
		}*/

		//if (Input.GetKeyDown (KeyCode.A))
		if (InputController.IsEscapeClicked ())
		if (DefsGame.CurrentScreen == DefsGame.SCREEN_EXIT) {
			HideExitPanel ();
		}
		else	
		if (DefsGame.CurrentScreen == DefsGame.SCREEN_MENU) {
			ShowExitPanel ();
		}
		else if (DefsGame.CurrentScreen == DefsGame.SCREEN_GAME) {
			if (_isScreenReviveDone)
				ReviveClose ();
			else if (_isScreenShareDone)
				ShareClose ();
			else if (_isScreenRateDone)
			RateClose (); else
				GameOver ();
		}else if (DefsGame.CurrentScreen == DefsGame.SCREEN_SKINS) {
			DefsGame.ScreenSkins.Hide ();
			GameEvents.Send (OnShowMenu);
		}else if (DefsGame.CurrentScreen == DefsGame.SCREEN_IAPS) {
			DefsGame.ScreenCoins.Hide ();
			GameEvents.Send (OnShowMenu);
		}
	}

	void GameOver() {
		IsGameOver = true;
		_state = 3;
	}

	public void HideExitPanel() {
		DefsGame.CurrentScreen = DefsGame.SCREEN_MENU;
		UIManager.HideUiElement ("PanelExit");
		UIManager.HideUiElement ("PanelExitBtnYes");
		UIManager.HideUiElement ("PanelExitBtnNo");
	}

	void ShowExitPanel() {
		DefsGame.CurrentScreen = DefsGame.SCREEN_EXIT;
		UIManager.ShowUiElement ("PanelExit");
		UIManager.ShowUiElement ("PanelExitBtnYes");
		UIManager.ShowUiElement ("PanelExitBtnNo");
	}

	public void Quit() {
		Application.Quit ();
	}
}
