using UnityEngine;
using UnityEngine.UI;
using DoozyUI;
using System;

public class ScreenSkins : MonoBehaviour {
	public static event Action<int> OnAddCoinsVisual;
	public GameObject skin2;
	public GameObject skin3;
	public GameObject skin4;
	public GameObject skin5;
	public GameObject skin6;
	public GameObject skin7;
	public GameObject skin8;
	public GameObject skin9;
	public GameObject skin10;
	public GameObject skin11;
	public GameObject skin12;

	public GameObject haveNewSkin;

	void Awake () {
		DefsGame.ScreenSkins = this;
	}

	void SetSkin(int _id) {
		FlurryEventsManager.SendEvent ("candy_purchase_<" + _id.ToString() + ">");

		if (_id == DefsGame.CurrentFaceId)
			return;

		if (DefsGame.FaceAvailable [_id] == 1) {
			DefsGame.CurrentFaceId = _id;
			PlayerPrefs.SetInt ("currentFaceID", DefsGame.CurrentFaceId);
		    DefsGame.CarSimulator.Car.SetNewSkin (_id);

		} else if (DefsGame.CoinsCount >= DefsGame.FacePrice [_id-1]) {
			GameEvents.Send(OnAddCoinsVisual, -DefsGame.FacePrice [_id-1]);
			DefsGame.FaceAvailable [_id] = 1;
			DefsGame.CurrentFaceId = _id;
			PlayerPrefs.SetInt ("currentFaceID", DefsGame.CurrentFaceId);
			PlayerPrefs.SetInt ("faceAvailable_" + _id.ToString (), 1);
			DefsGame.CarSimulator.Car.SetNewSkin (_id);

			++DefsGame.QUEST_CHARACTERS_Counter;
			PlayerPrefs.SetInt ("QUEST_CHARACTERS_Counter", DefsGame.QUEST_CHARACTERS_Counter);

			//DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_NEW_SKIN, 1);

			//DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_COLLECTION, DefsGame.QUEST_CHARACTERS_Counter);

			ChooseColorForButtons ();

			FlurryEventsManager.SendEvent ("candy_purchase_completed_<" + _id.ToString() + ">");
		} else {
			HideButtons ();
			FlurryEventsManager.SendEndEvent ("candy_shop_length");

			DefsGame.ScreenCoins.Show ("candy_shop");
		}
	}

	void ShowButtons() {
		UIManager.ShowUiElement ("ScreenSkinsBtnBack");
		UIManager.ShowUiElement ("BtnSkin1");
		UIManager.ShowUiElement ("BtnSkin2");
		UIManager.ShowUiElement ("BtnSkin3");
		UIManager.ShowUiElement ("BtnSkin4");
		UIManager.ShowUiElement ("BtnSkin5");
		UIManager.ShowUiElement ("BtnSkin6");
		UIManager.ShowUiElement ("BtnSkin7");
		UIManager.ShowUiElement ("BtnSkin8");
		UIManager.ShowUiElement ("BtnSkin9");
		UIManager.ShowUiElement ("BtnSkin10");
		UIManager.ShowUiElement ("BtnSkin11");
		UIManager.ShowUiElement ("BtnSkin12");
	}

	void HideButtons() {
		UIManager.HideUiElement ("ScreenSkinsBtnBack");
		UIManager.HideUiElement ("BtnSkin1");
		UIManager.HideUiElement ("BtnSkin2");
		UIManager.HideUiElement ("BtnSkin3");
		UIManager.HideUiElement ("BtnSkin4");
		UIManager.HideUiElement ("BtnSkin5");
		UIManager.HideUiElement ("BtnSkin6");
		UIManager.HideUiElement ("BtnSkin7");
		UIManager.HideUiElement ("BtnSkin8");
		UIManager.HideUiElement ("BtnSkin9");
		UIManager.HideUiElement ("BtnSkin10");
		UIManager.HideUiElement ("BtnSkin11");
		UIManager.HideUiElement ("BtnSkin12");
	}

	public void SetSkin1() {
		SetSkin (0);
	}
		
	public void SetSkin2() {
		SetSkin (1);
	}

	public void SetSkin3() {
		SetSkin (2);
	}

	public void SetSkin4() {
		SetSkin (3);
	}

	public void SetSkin5() {
		SetSkin (4);
	}

	public void SetSkin6() {
		SetSkin (5);
	}

	public void SetSkin7() {
		SetSkin (6);
	}

	public void SetSkin8() {
		SetSkin (7);
	}

	public void SetSkin9() {
		SetSkin (8);
	}

	public void SetSkin10() {
		SetSkin (9);
	}

	public void SetSkin11() {
		SetSkin (10);
	}

	public void SetSkin12() {
		SetSkin (11);
	}

	public void Show() {
		FlurryEventsManager.SendStartEvent ("candy_shop_length");

		DefsGame.CurrentScreen = DefsGame.SCREEN_SKINS;
		DefsGame.IsCanPlay = false;
		ChooseColorForButtons ();
		ShowButtons ();
	}

	public void Hide() {
		FlurryEventsManager.SendEndEvent ("candy_shop_length");

		DefsGame.CurrentScreen = DefsGame.SCREEN_MENU;
		DefsGame.IsCanPlay = true;
		ChooseColorForButtons ();
		HideButtons ();
	}

    private void ChooseColorForButtons() {
        for (int i = 1; i < DefsGame.FaceAvailable.Length; i++)
        {
            if (DefsGame.FaceAvailable[i] == 1)
            {
                skin2.GetComponentInChildren<UIButton>().GetComponent<Image>().color = Color.white;
                skin2.GetComponentInChildren<Text>().text = "";
            }
            else
            {
                skin2.GetComponentInChildren<UIButton>().GetComponent<Image>().color = Color.black;
                skin2.GetComponentInChildren<Text>().text = DefsGame.FacePrice[i - 1].ToString();
            }
        }
    }

	public bool CheckAvailableSkinBool() {
		for (int i = 1; i < DefsGame.FaceAvailable.Length; i++) {
			if ((DefsGame.FaceAvailable[i] == 0)&&(DefsGame.CoinsCount >= DefsGame.FacePrice[i-1])) {
				haveNewSkin.SetActive (true);
				return true;
			}
		}
		haveNewSkin.SetActive (false);
		return false;
	}
}
