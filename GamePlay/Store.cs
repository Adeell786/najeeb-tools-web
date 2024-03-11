using System;
using System.Collections;
using System.Collections.Generic;
using CompleteProject;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    public static Store instance;
    public Button removeAdsButton;
    private int goldAmount;
    public Text goldText;

    //public int goldForPurchase;
    //public int cashPurchased;

    public Text pack1Price;
    public Text pack2Price;
    public Text packAdsPrice;

    
    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
      
        if (PlayerDataController.Instance.playerData.isRemoveAds)
        {
            
            removeAdsButton.interactable = !PlayerDataController.Instance.playerData.isRemoveAds;
            //removeAdsButton.transform.GetChild(0).gameObject.SetActive(false);
            removeAdsButton.transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color(1,1,1,0.1f);
            packAdsPrice.gameObject.SetActive(false);
        }
//        Invoke(nameof(SetPricing),1f);
        SetPricing();
    }

    public void SetPricing()
    {
        Purchaser.instance.SetPrices();
    }
    public void BuyGoldPack1()
    {

        if (!MConstants.IsInternetConnection())
        {
            MConstants.isSubMenuStay = true;
            MainMenuManager.Instance.showSubMenu(SubMenuNames.NO_INTERNET_POP);
        }
        else
        {
            Purchaser.instance.BuyConsumable1();
        }
    }

    public void BuyGoldPack2()
    {
        if (!MConstants.IsInternetConnection())
        {
            MConstants.isSubMenuStay = true;
            MainMenuManager.Instance.showSubMenu(SubMenuNames.NO_INTERNET_POP);
        }
        else
        {
            Purchaser.instance.BuyConsumable2();
        }
    }

    //public void BuyCashFromGold()
    //{
    //    if (PlayerDataController.Instance.playerData.PlayerGold < goldForPurchase)
    //    {
    //        OutOfCashMenu.isStoreBuy = true;
    //        MainMenuManager.Instance.showSubMenu(SubMenuNames.OUT_OF_CASH);
    //    }
    //    else
    //    {
    //        MainMenuManager.Instance.showSubMenu(SubMenuNames.STORECONGRATSCASH);
    //    }
    //}

    public void RemoveAds()
    {
        if (!MConstants.IsInternetConnection())
        {
            MConstants.isSubMenuStay = true;
            MainMenuManager.Instance.showSubMenu(SubMenuNames.NO_INTERNET_POP);
        }
        else
        {
            
            Purchaser.instance.BuyNonConsumable();
        }
    }

    public void GiveGold1()
    {
      MainMenuManager.Instance.showSubMenu(SubMenuNames.Store_CONGRATS_POPUP);
        goldText.text = "10000";
        goldAmount = 10000;
        MainMenuManager.Instance.AddGold(goldAmount);
        //PlayerDataController.Instance.playerData.PlayerGold = +goldAmount;
        //PlayerDataController.Instance.Save();
    }

    public void GiveGold2()
    {
       MainMenuManager.Instance.showSubMenu(SubMenuNames.Store_CONGRATS_POPUP);
        goldText.text = "25000";
        goldAmount = 25000;
        MainMenuManager.Instance.AddGold(goldAmount);
      
    }

    public void OkayButton()
    {
        MainMenuManager.Instance.CoinAnimation();
    }

    //public void OkayCashButton()
    //{
    //   // MainMenuManager.Instance.AddCash(cashPurchased);
    //    PlayerDataController.Instance.playerData.PlayerGold -= goldForPurchase;

    //    PlayerDataController.Instance.Save();
    //    MainMenuManager.Instance.RefreshData();
    //}
   
}