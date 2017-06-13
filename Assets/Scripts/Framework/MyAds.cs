using UnityEngine;

public class MyAds : MonoBehaviour {
	private static int _videoAdCounter;
	public static int noAds;

	void OnEnable()
	{
		GlobalEvents<OnRewardedVideoAvailable>.Happened += IsRewardedVideoAvailable;
	}

	void OnDisable()
	{
		GlobalEvents<OnRewardedVideoAvailable>.Happened -= IsRewardedVideoAvailable;
	}
	
	private void IsRewardedVideoAvailable(OnRewardedVideoAvailable e) {
		if (!e.isAvailable)
		{
			_videoAdCounter = 0;
		}
	}

	public static void ShowVideoAds()
	{
		if (_videoAdCounter >= 4)
		{
			GlobalEvents<OnShowVideoAds>.Call(new OnShowVideoAds());
		}
		else
		{
			++_videoAdCounter;
		}
	}
	
	public static void ShowRewardedAds()
	{
		GlobalEvents<OnShowRewarded>.Call(new OnShowRewarded());
	}
}
