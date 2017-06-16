// ADS
// Дать награду игроку
public struct OnGiveReward { public bool isAvailable; }
// Rewarded реклама готова к показу
public struct OnRewardedAvailable { public bool isAvailable; }

// Запрос на показ рекламы 
public struct OnShowVideoAds { public bool isAvailable; }
// Запрос на показ видео рекламы
public struct OnShowRewarded { public bool isAvailable; }

// Можно дарить подарок
public struct OnGiftAvailable { public bool isAvailable; }
// Показываем нотификейшины на экране
public struct OnShowNotifications {}

//example
//событие с несколькими параметрами
/*public struct GameSettingEvent
{
    public bool useAds;
    public bool useAnalytics;
    public float coinsFactor;
    public int startingCoins;
}*/