using DoozyUI;
using UnityEngine;

public class GameOverNotifications : MonoBehaviour {
	private bool _isRewardedAvailable;
	private bool _isGiftAvailable;

	void OnEnable()
	{
		RoadManager.OnGameplayStart += HideNotifications;
		GlobalEvents<OnShowNotifications>.Happened += ShowNotifications;
		GlobalEvents<OnRewardedAvailable>.Happened += IsRewardedAvailable;
		GlobalEvents<OnGiftAvailable>.Happened += IsGiftAvailable;
	}

	private void OnDisable()
	{
		RoadManager.OnGameplayStart -= HideNotifications;
		GlobalEvents<OnShowNotifications>.Happened -= ShowNotifications;
		GlobalEvents<OnRewardedAvailable>.Happened -= IsRewardedAvailable;
		GlobalEvents<OnGiftAvailable>.Happened -= IsGiftAvailable;
	}
	
	private void ShowNotifications(OnShowNotifications e)
	{
		int notificationCounter = 0;
		
		UIManager.ShowUiElement ("NotifyShare");
		UIManager.ShowUiElement("NotifyRate");
		if (notificationCounter < 3 && _isGiftAvailable)
		{
			++notificationCounter;
			UIManager.ShowUiElement ("NotifyGift");
		}
		
		if (notificationCounter < 3 && _isRewardedAvailable)
		{
			++notificationCounter;
			UIManager.ShowUiElement ("NotifyRewarded");
		}
	}
	
	private void HideNotifications()
	{
		UIManager.HideUiElement ("NotifyShare");
		UIManager.HideUiElement ("NotifyGift");
		UIManager.HideUiElement ("NotifyRewarded");
		UIManager.HideUiElement ("NotifyRate");
	}
	
	private void IsRewardedAvailable(OnRewardedAvailable e) {
		_isRewardedAvailable = e.isAvailable;
		if (!e.isAvailable) UIManager.HideUiElement ("NotifyRewarded");
	}
	
	private void IsGiftAvailable(OnGiftAvailable e) {
		_isGiftAvailable = e.isAvailable;
		if (!e.isAvailable) UIManager.HideUiElement ("NotifyGift");
	}
}
