using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using GoogleMobileAds.Common;
using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;
//using Loadings;


namespace Game.Ads
{

    #region Classes

    [System.Serializable]
    public class AdsIDs
    {
        public bool isTestAd;
        public AndroidIDs androidIDs;
        public IOSIDs iOSIDs;
    }
    [System.Serializable]
    public class AndroidIDs
    {
        public string AdsAppId = "";
        public string appOpenId = "";
        public string smallBannerId = "";
        public string ReactBannerId = "";
        public string InterstitialId = "";
        public string InterstitialNonVideoId = "";
        public string RewardedInterstitialId = "";
        public string RewardedId = "";
        public string nativeId;
    }
    [System.Serializable]
    public class IOSIDs
    {
        public string AdsAppId = "ca-app-pub-3940256099942544~3347511713";
        public string appOpenId = "ca-app-pub-3940256099942544/5662855259";
        public string smallBannerId = "ca-app-pub-3940256099942544/2934735716";
        public string reactBannerId = "ca-app-pub-3940256099942544/2934735716";
        public string InterstitialId = "ca-app-pub-3940256099942544/4411468910";
        public string RewardedInterstitialId = "ca-app-pub-3940256099942544/6978759866";
        public string RewardedId = "ca-app-pub-3940256099942544/1712485313";
        public string nativeId;
    }

    #endregion

    public class AdsController : MonoBehaviour//Singleton<AdsController>
    {
        
        //public static AdsController Instance;

       
        #region Instance

        private static AdsController sInstance;
        public static AdsController Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = GameObject.FindObjectOfType<AdsController>();
                    if (sInstance == null)
                    {
                        sInstance = new GameObject("AdsController").AddComponent<AdsController>();
                        sInstance.Awake();
                    }
                }
                return sInstance;
            }
            set
            {
                sInstance = value;
            }
        }

        #endregion
       

        public AdsIDs adsIDs;
        private readonly TimeSpan APPOPEN_TIMEOUT = TimeSpan.FromSeconds(20);//TimeSpan.FromHours(4);
        //private float APPOPEN_TIMEOUT = 20f;
        private DateTime appOpenExpireTime;
        //private float appOpenExpireTime;
        private AppOpenAd appOpenAd;
        private BannerView bannerView;
        private BannerView reactBannerView;
        private InterstitialAd interstitialAd;
        private InterstitialAd interstitialAdNonVideo;
        private RewardedAd rewardedAd;
        private RewardedInterstitialAd rewardedInterstitialAd;
        private float deltaTime;
        //[Space(20)]
        //[Header("***AD Events***")]
        //public UnityEvent OnAdLoadedEvent;
        //public UnityEvent OnAdFailedToLoadEvent;
        //public UnityEvent OnAdOpeningEvent;
        //public UnityEvent OnAdFailedToShowEvent;
        //public UnityEvent OnUserEarnedRewardEvent;
        //public UnityEvent OnAdClosedEvent;
        public Canvas canvas;
        public Text infoText;
      //  public SceneLoader sceneLoader;

        //public TextMeshProUGUI statusText;

        #region Akaash

        private string smallBannerAdUnitId=null;
        private string reactBannerAdUnitId;
        private string interstitialAdUnitId;
        private string interstitialNonVideoAdUnitId;
        private string rewardedInterstitialAdUnitId;
        private string rewardedAdUnitId;
        private string appOpenAdUnitId;

        private float lastCheckTime;
        private bool isAwakeDone;

        private void Awake()
        {
            /*
            if (sInstance && sInstance != this)
            {
                Destroy(this);
                return;
            }

            if (!isAwakeDone)
            {
                sInstance = this;
                isAwakeDone = true;
                lastCheckTime = Time.unscaledTime + 10f;
                DontDestroyOnLoad(this);
            }
            */
            if (Instance == null)
                Instance = this;
            DontDestroyOnLoad(this);
        }

        //private void Update()
        //{
        //    appOpenExpireTime -= Time.deltaTime;
        //}

        WaitForSeconds wait = new WaitForSeconds(1.5f);
        IEnumerator RequestDelay()
        {
            if (EnableAds)
            {
                //yield return wait;
                RequestAndLoadAppOpenAd();//Akaash
                yield return wait;
                ShowAppOpenAd();
                yield return wait;
                RequestBannerAd(AdPosition.Bottom);
                //ShowBanner();
                //RequestReactBannerAd(AdPosition.BottomLeft);
               
                //RequestAndLoadNonVideoInterstitialAd();//Akaash
                yield return wait;
                RequestAndLoadInterstitialAd();//Akaash
                yield return wait;
                RequestReactBannerAd(AdPosition.BottomLeft);
            }
            yield return wait;
            RequestAndLoadRewardedAd();//Akaash
            //yield return wait;
            //RequestAndLoadRewardedInterstitialAd();//Akaash
        }
        public bool EnableAds=true;
        private void SetIDs()
        {

            /// These ad units are configured to always serve test ads.
#if UNITY_EDITOR
            smallBannerAdUnitId = reactBannerAdUnitId = interstitialAdUnitId = interstitialNonVideoAdUnitId = rewardedInterstitialAdUnitId = rewardedAdUnitId = appOpenAdUnitId = "unused";
#elif UNITY_ANDROID
        smallBannerAdUnitId = adsIDs.isTestAd? "ca-app-pub-3940256099942544/6300978111" : adsIDs.androidIDs.smallBannerId;
        reactBannerAdUnitId = adsIDs.isTestAd? "ca-app-pub-3940256099942544/6300978111" : adsIDs.androidIDs.ReactBannerId;
        interstitialAdUnitId = adsIDs.isTestAd? "ca-app-pub-3940256099942544/1033173712" : adsIDs.androidIDs.InterstitialId;
        interstitialNonVideoAdUnitId = adsIDs.isTestAd? "ca-app-pub-3940256099942544/1033173712" : adsIDs.androidIDs.InterstitialNonVideoId;
        rewardedInterstitialAdUnitId = adsIDs.isTestAd? "ca-app-pub-3940256099942544/5354046379" : adsIDs.androidIDs.RewardedInterstitialId;
        rewardedAdUnitId = adsIDs.isTestAd? "ca-app-pub-3940256099942544/5224354917" : adsIDs.androidIDs.RewardedId;
        appOpenAdUnitId = adsIDs.isTestAd ? "ca-app-pub-3940256099942544/3419835294" : adsIDs.androidIDs.appOpenId;
#elif UNITY_IPHONE
        smallBannerAdUnitId = adsIDs.isTestAd? "ca-app-pub-3940256099942544/2934735716" : adsIDs.iOSIDs.smallBannerId;
        reactBannerAdUnitId = adsIDs.isTestAd? "ca-app-pub-3940256099942544/2934735716" : adsIDs.iOSIDs.ReactBannerId;
        interstitialAdUnitId = adsIDs.isTestAd? "ca-app-pub-3940256099942544/4411468910" : adsIDs.iOSIDs.InterstitialId;
        rewardedInterstitialAdUnitId = adsIDs.isTestAd? "ca-app-pub-3940256099942544/6978759866" : adsIDs.iOSIDs.RewardedInterstitialId;
        rewardedAdUnitId = adsIDs.isTestAd? "ca-app-pub-3940256099942544/1712485313" : adsIDs.iOSIDs.RewardedId;
        appOpenAdUnitId = adsIDs.isTestAd? "ca-app-pub-3940256099942544/5662855259" : adsIDs.iOSIDs.appOpenId;
#else
        smallBannerAdUnitId = reactBannerAdUnitId = interstitialAdUnitId = interstitialNonVideoAdUnitId = rewardedInterstitialAdUnitId = rewardedAdUnitId = appOpenAdUnitId = "unexpected_platform";
        //smallBannerAdUnitId = interstitialAdUnitId = rewardedInterstitialAdUnitId = rewardedAdUnitId = appOpenAdUnitId = "unexpected_platform";
#endif
        }

        #endregion

        #region UNITY MONOBEHAVIOR METHODS

        public void Start()
        {
            //DontDestroyOnLoad(this);
            SetIDs();//Akaash

            MobileAds.RaiseAdEventsOnUnityMainThread = true;
#if UNITY_IPHONE
            MobileAds.SetiOSAppPauseOnBackground(true);
#endif
            List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };

/*
            /// Add some test device IDs (replace with your own device IDs).
#if UNITY_ANDROID
            deviceIds.Add("75EF8D155528C04DACBBA6F36F433035");
#elif UNITY_IPHONE
                        deviceIds.Add("96e23e80653bb28980d3f40beb58915c");
#endif
*/

            /// Configure TagForChildDirectedTreatment and test device IDs.
            RequestConfiguration requestConfiguration =
                new RequestConfiguration();//.Builder().build();
                //.SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
                //.SetTestDeviceIds(deviceIds).build();
            MobileAds.SetRequestConfiguration(requestConfiguration);

            /// Initialize the Google Mobile Ads SDK.
            //MobileAds.Initialize(HandleInitCompleteAction);

            /// Listen to application foreground / background events.
            //AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
            //SetCanvasBG();

            #region CMP Screen
            var deviceID = SystemInfo.deviceUniqueIdentifier;
            Debug.Log($"DeviceID: {deviceID}");
            var debugSettings = new ConsentDebugSettings
            {
                // Geography appears as in EEA for debug devices.
                DebugGeography = DebugGeography.EEA,
                TestDeviceHashedIds = new List<string> {
                     "7296285ef2f8169c1f5658a7d8747296",//Akaash
                     //"630666A576E262A5726DE983712C46CB",
                     deviceID.ToUpper()

                }
            };

            if (HaveInternet)
            {
                // Set tag for under age of consent.
                ConsentRequestParameters request = new ConsentRequestParameters
                {
                    TagForUnderAgeOfConsent = false,
                    ConsentDebugSettings = debugSettings,//Comment For Live Build
                };
                // Check the current consent information status.
                ConsentInformation.Update(request, OnConsentInfoUpdated);
            }
            else
            {
                InitializeAds();
                //MobileAds.Initialize(HandleInitCompleteAction);
            }

            #endregion
        }

        #region CMP Screen
        public bool HaveInternet = true;
        void OnConsentInfoUpdated(FormError consentError)
        {
            if (consentError != null)
            {
                ///Handle the error
                Debug.LogError(consentError);

                InitializeAds();
                //MobileAds.Initialize(HandleInitCompleteAction);
                return;
            }
            if (ConsentInformation.PrivacyOptionsRequirementStatus == PrivacyOptionsRequirementStatus.Required &&
                ConsentInformation.ConsentStatus != ConsentStatus.Obtained)
            {
                Debug.Log("Obtaining Consent");
                /// If the error is null, the consent information state was updated.
                /// You are now ready to check if a form is available.
                ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
                {
                    PlayerPrefs.SetInt("Privacy", 6676);
                    Debug.Log($"Privacy: {PlayerPrefs.GetInt("Privacy")}");
                    InitializeAds();
                    //MobileAds.Initialize(HandleInitCompleteAction);
                    if (formError != null)
                    {
                        // Consent gathering failed.
                        Debug.LogError(formError);
                        return;
                    }
                });
                // Consent has been gathered.
                //if (ConsentInformation.CanRequestAds())
                //{
                //    // TODO: Request an ad.
                //    PlayerPrefs.SetInt("Privacy", 6676);
                //    Debug.Log($"Privacy: {PlayerPrefs.GetInt("Privacy")}");
                //    MobileAds.Initialize(HandleInitCompleteAction);
                //}
            }
            else
            {
                Debug.Log("Consent Not Required");
                InitializeAds();
                //MobileAds.Initialize(HandleInitCompleteAction);
            }
        }

        void InitializeAds()
        {
            #region CMP
            if (!PlayerPrefs.HasKey("Privacy"))
            {
                Debug.Log("Privacy Required");
               // if (sceneLoader != null) sceneLoader.enabled = true;
            }
            else
            {
               // if (sceneLoader != null) sceneLoader.enabled = true;
            }
            #endregion

            MobileAds.Initialize((InitializationStatus initstatus) => {
                MobileAdsEventExecutor.ExecuteInUpdate(() => {
                    if (HaveInternet)
                    {
                        //if (initstatus == null) return;
                        PrintStatus("Initialization complete.");
                        StartCoroutine(RequestDelay());
                    }
                });
            });
        }

        #endregion

        private void HandleInitCompleteAction(InitializationStatus initstatus)
        {
            //Debug.Log("Initialization complete.");

            /// Callbacks from GoogleMobileAds are not guaranteed to be called on
            /// the main thread.
            /// In this example we use MobileAdsEventExecutor to schedule these calls on
            /// the next Update() loop.
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {

                if (!HaveInternet)
                    return;
                //statusText.text = "Initialization complete.";
                PrintStatus("Initialization complete.");

                StartCoroutine(RequestDelay());//Akaash
            });
        }

#endregion

        #region HELPER METHODS

        private AdRequest CreateAdRequest()
        {
            return new AdRequest();
                //.AddKeyword("unity-admob-sample")
                //.Build();
        }

        #endregion

        #region BANNER ADS

        public void RequestBannerAd(AdPosition adPosition)
        {
            if (smallBannerAdUnitId.Equals(string.Empty))
                return;

            PrintStatus("Requesting Banner ad.");

            /// Clean up banner before reusing
            if (bannerView != null)
            {
                //bannerView.Hide();//Akaash
                bannerView.Destroy();
                GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
                {
                    if (canvas != null) canvas.gameObject.SetActive(false);
                });
            }

            /// Create a 320x50 banner at top of the screen
            //bannerView = new BannerView(smallBannerAdUnitId, AdSize.Banner, adPosition);
            bannerView = new BannerView(smallBannerAdUnitId, AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth), adPosition);

            /// Add Event Handlers
            bannerView.OnBannerAdLoaded += () =>
            {
                PrintStatus("Banner ad loaded.");
                IsBannerLoaded = true;
                //OnAdLoadedEvent.Invoke();
            };
            bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                PrintStatus("Banner ad failed to load with error: " + error.GetMessage());
                IsBannerLoaded = false;
                //OnAdFailedToLoadEvent.Invoke();
                RequestBannerAd(adPosition); //Akaash ///Load Banner
            };
            /*bannerView.OnAdImpressionRecorded += () =>
            {
                PrintStatus("Banner ad recorded an impression.");
            };
            bannerView.OnAdClicked += () =>
            {
                PrintStatus("Banner ad recorded a click.");
            };
            bannerView.OnAdFullScreenContentOpened += () =>
            {
                PrintStatus("Banner ad opening.");
                //OnAdOpeningEvent.Invoke();
            };
            bannerView.OnAdFullScreenContentClosed += () =>
            {
                PrintStatus("Banner ad closed.");
                //OnAdClosedEvent.Invoke();
                //Akaash ///Load Banner
            };
            bannerView.OnAdPaid += (AdValue adValue) =>
            {
                string msg = string.Format("{0} (currency: {1}, value: {2}",
                                            "Banner ad received a paid event.",
                                            adValue.CurrencyCode,
                                            adValue.Value);
                PrintStatus(msg);
            };*/

            /// Load a banner ad
            bannerView.LoadAd(CreateAdRequest());
            //HideBannerAd();
            //bannerView.SetPosition(adPosition);//Akaash
            HideReactBannerAd();
            //Debug.Log(bannerView.GetHeightInPixels() + ":" + bannerView.GetWidthInPixels());

        }
        //bool IsBannerLoaded()
        //{
        //    return value;
        //}

        void SetCanvasBG()
        {
            if (canvas != null)
            {
                CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                //canvasScaler.referenceResolution = new Vector2(1280, 800);
                canvasScaler.referenceResolution = new Vector2(1920, 1080);
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                canvasScaler.matchWidthOrHeight = 1;
            }

        }

        bool IsBannerLoaded = false;
        public void ShowBanner(AdPosition adPosition) //Akaash
        {
            if (!HaveInternet)
                return;

            if (!EnableAds) {
                DestroyBannerAd();
                return;
            }
            if (bannerView != null && IsBannerLoaded) {

                //bannerView.SetPosition(adPosition);
                bannerView.Show();
                if (canvas != null)
                {
                    RectTransform bgRectTransform = canvas.GetComponentInChildren<Image>().rectTransform;
                    bgRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bannerView.GetWidthInPixels() + 5);
                    bgRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bannerView.GetHeightInPixels() + 5);
                    canvas.gameObject.SetActive(true);
                }
            }
            else {
                //if (canvas != null) canvas.gameObject.SetActive(false);
                RequestBannerAd(adPosition);
            }
        }
        public void HideBannerAd()
        {
            if (bannerView != null)
            {
                bannerView.Hide();
                GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
                {
                    if (canvas != null) canvas.gameObject.SetActive(false);
                });
            }
        }
        public void DestroyBannerAd()
        {
            if (bannerView != null)
            {
                bannerView.Destroy();
                GoogleMobileAds.Common.MobileAdsEventExecutor.ExecuteInUpdate(() =>
                {
                    if (canvas != null) canvas.gameObject.SetActive(false);
                });
            }
            bannerView = null;
            //IsBannerLoaded = false;
        }

        #endregion

        #region REACT_BANNER ADS

        public void RequestReactBannerAd(AdPosition adPosition)
        {
            if (reactBannerAdUnitId.Equals(string.Empty))
                return;

            PrintStatus("Requesting React Banner ad.");

            /// Clean up banner before reusing
            if (reactBannerView != null)
            {
                //reactbannerView.Hide();//Akaash
                reactBannerView.Destroy();
            }

            /// Create a 320x50 banner at top of the screen
            reactBannerView = new BannerView(reactBannerAdUnitId, AdSize.MediumRectangle, adPosition);

            /// Add Event Handlers
            reactBannerView.OnBannerAdLoaded += () =>
        {
            PrintStatus("React Banner ad loaded.");
            //OnAdLoadedEvent.Invoke();
        };
            reactBannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            PrintStatus("React Banner ad failed to load with error: " + error.GetMessage());
            //OnAdFailedToLoadEvent.Invoke();
            //Akaash ///Load Banner
        };
            reactBannerView.OnAdImpressionRecorded += () =>
        {
            PrintStatus("React Banner ad recorded an impression.");
        };
            reactBannerView.OnAdClicked += () =>
        {
            PrintStatus("React Banner ad recorded a click.");
        };
            reactBannerView.OnAdFullScreenContentOpened += () =>
        {
            PrintStatus("React Banner ad opening.");
            //OnAdOpeningEvent.Invoke();
        };
            reactBannerView.OnAdFullScreenContentClosed += () =>
        {
            PrintStatus("React Banner ad closed.");
            //OnAdClosedEvent.Invoke();
            ///Akaash ///Load Banner
        };
            reactBannerView.OnAdPaid += (AdValue adValue) =>
        {
            string msg = string.Format("{0} (currency: {1}, value: {2}",
                                        "React Banner ad received a paid event.",
                                        adValue.CurrencyCode,
                                        adValue.Value);
            PrintStatus(msg);
        };

            /// Load a banner ad
            reactBannerView.LoadAd(CreateAdRequest());
            reactBannerView.SetPosition(adPosition);//Akaash
            reactBannerView.Hide();
        }
        public void showbigbanner()
        {
            ShowReactBanner(AdPosition.BottomLeft);
        }
        public void ShowReactBanner(AdPosition adPosition) //Akaash
        {
            if (!HaveInternet)
                return;
            if (!EnableAds)
            {
                DestroyBannerAd();
                return;
            }
            if (reactBannerView != null)
            {
                reactBannerView.SetPosition(adPosition);
                reactBannerView.Show();
            }
            else
            {
                RequestReactBannerAd(adPosition);
            }
        }
        public void HideReactBannerAd()
        {
            if (reactBannerView != null)
            {
                reactBannerView.Hide();
            }
        }
        public void DestroyReactBannerAd()
        {
            if (reactBannerView != null)
            {
                reactBannerView.Destroy();
            }
        }

        #endregion

        #region INTERSTITIAL ADS Video

        public void RequestAndLoadInterstitialAd()
        {
            if (interstitialAdUnitId.Equals(string.Empty))
                return;
            PrintStatus("Requesting Interstitial ad.");

            /// Clean up interstitial before using it
            if (interstitialAd != null)
            {
                interstitialAd.Destroy();
            }

            /// Load an interstitial ad
            InterstitialAd.Load(interstitialAdUnitId, CreateAdRequest(),
                (InterstitialAd ad, LoadAdError loadError) =>
                {
                    if (loadError != null)
                    {
                        PrintStatus("Interstitial ad failed to load with error: " +
                            loadError.GetMessage());
                        //RequestAndLoadInterstitialAd();//Akaash ///Load Interstitial
                        return;
                    }
                    else if (ad == null)
                    {
                        PrintStatus("Interstitial ad failed to load.");
                        //RequestAndLoadInterstitialAd();//Akaash ///Load Interstitial
                        return;
                    }

                    PrintStatus("Interstitial ad loaded.");
                    interstitialAd = ad;

                    /*ad.OnAdFullScreenContentOpened += () =>
                    {
                        PrintStatus("Interstitial ad opening.");
                        //OnAdOpeningEvent.Invoke();
                    };*/
                    ad.OnAdFullScreenContentClosed += () =>
                    {
                        PrintStatus("Interstitial ad closed.");
                        //OnAdClosedEvent.Invoke();
                        this.appOpenExpireTime = DateTime.Now + APPOPEN_TIMEOUT;//Akaash
                        //appOpenExpireTime = APPOPEN_TIMEOUT;
                        RequestAndLoadInterstitialAd();//Akaash ///Load Interstitial
                    };
                    /*ad.OnAdImpressionRecorded += () =>
                    {
                        PrintStatus("Interstitial ad recorded an impression.");
                    };
                    ad.OnAdClicked += () =>
                    {
                        PrintStatus("Interstitial ad recorded a click.");
                    };
                    ad.OnAdFullScreenContentFailed += (AdError error) =>
                    {
                        PrintStatus("Interstitial ad failed to show with error: " +
                                    error.GetMessage());
                    };
                    ad.OnAdPaid += (AdValue adValue) =>
                    {
                        string msg = string.Format("{0} (currency: {1}, value: {2}",
                                                   "Interstitial ad received a paid event.",
                                                   adValue.CurrencyCode,
                                                   adValue.Value);
                        PrintStatus(msg);
                    };*/
                });
        }

        [Range(30, 120)]
        [SerializeField] private float showThreshHold = 120f;
        float _threshHoldTimer = 0f;
        private void Update()
        {
            _threshHoldTimer += Time.deltaTime;
        }
        public void ShowInterstitialAd(bool forceShow = false)
        {
            if (forceShow == false)
            {
                if (_threshHoldTimer < showThreshHold) return;
            }

            if (!HaveInternet)
                return;
            if (!EnableAds)
                return;
            if (interstitialAd != null && interstitialAd.CanShowAd())
            {
                _threshHoldTimer = 0f;
                interstitialAd.Show();
            }
            else
            {
                PrintStatus("Interstitial ad is not ready yet.");
                RequestAndLoadInterstitialAd();//Akaash
            }
        }

        public void DestroyInterstitialAd()
        {
            if (interstitialAd != null)
            {
                interstitialAd.Destroy();
            }
        }

        #region Akaash

        /*private void RegisterReloadHandler(InterstitialAd ad)
        {
            /// Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
        {
            PrintStatus("Interstitial Ad full screen content closed.");

            /// Reload the ad so that we can show another as soon as possible.
            RequestAndLoadInterstitialAd();
        };
            /// Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Interstitial ad failed to open full screen content " +
                               "with error : " + error);

                /// Reload the ad so that we can show another as soon as possible.
                RequestAndLoadInterstitialAd();
            };
        }*/

        #endregion Akaash

        #endregion
        
        #region INTERSTITIAL ADS Non Video

        public void RequestAndLoadNonVideoInterstitialAd()
        {
            if (interstitialNonVideoAdUnitId.Equals(string.Empty))
                return;
            PrintStatus("Requesting NonVideo Interstitial ad.");

            /// Clean up interstitial before using it
            if (interstitialAdNonVideo != null)
            {
                interstitialAdNonVideo.Destroy();
            }

            /// Load an interstitial ad
            InterstitialAd.Load(interstitialNonVideoAdUnitId, CreateAdRequest(),
                (InterstitialAd ad, LoadAdError loadError) =>
                {
                    if (loadError != null)
                    {
                        PrintStatus("Interstitial NonVideo ad failed to load with error: " +
                            loadError.GetMessage());
                        //RequestAndLoadNonVideoInterstitialAd();//Akaash ///Load Interstitial
                        return;
                    }
                    else if (ad == null)
                    {
                        PrintStatus("Interstitial NonVideo ad failed to load.");
                        //RequestAndLoadNonVideoInterstitialAd();//Akaash ///Load Interstitial
                        return;
                    }

                    PrintStatus("Interstitial NonVideo ad loaded.");
                    interstitialAdNonVideo = ad;

                    ad.OnAdFullScreenContentOpened += () =>
                    {
                        PrintStatus("Interstitial NonVideo ad opening.");
                        //OnAdOpeningEvent.Invoke();
                    };
                    ad.OnAdFullScreenContentClosed += () =>
                    {
                        PrintStatus("Interstitial NonVideo ad closed.");
                        //OnAdClosedEvent.Invoke();
                        this.appOpenExpireTime = DateTime.Now + APPOPEN_TIMEOUT;//Akaash
                        //appOpenExpireTime = APPOPEN_TIMEOUT;
                        RequestAndLoadNonVideoInterstitialAd();//Akaash ///Load Interstitial
                    };
                    ad.OnAdImpressionRecorded += () =>
                    {
                        PrintStatus("Interstitial NonVideo ad recorded an impression.");
                    };
                    ad.OnAdClicked += () =>
                    {
                        PrintStatus("Interstitial NonVideo ad recorded a click.");
                    };
                    ad.OnAdFullScreenContentFailed += (AdError error) =>
                    {
                        PrintStatus("Interstitial NonVideo ad failed to show with error: " +
                                    error.GetMessage());
                    };
                    ad.OnAdPaid += (AdValue adValue) =>
                    {
                        string msg = string.Format("{0} (currency: {1}, value: {2}",
                                                   "Interstitial NonVideo ad received a paid event.",
                                                   adValue.CurrencyCode,
                                                   adValue.Value);
                        PrintStatus(msg);
                    };
                });
        }

        public void ShowInterstitialNonVideoAd()
        {
            if (!HaveInternet)
                return;
            if (!EnableAds)
                return;
            if (interstitialAdNonVideo != null && interstitialAdNonVideo.CanShowAd())
            {
                interstitialAdNonVideo.Show();
            }
            else
            {
                PrintStatus("Interstitial NonVideo ad is not ready yet.");
                RequestAndLoadNonVideoInterstitialAd();//Akaash
            }
        }

        public void DestroyInterstitialNonVideoAd()
        {
            if (interstitialAdNonVideo != null)
            {
                interstitialAdNonVideo.Destroy();
            }
        }

        #endregion

        #region REWARDED ADS

        private Action<AdsResult> mRewardCallback;
        private Action<AdsResult> mRewardInterstitialCallback;

        public void RequestAndLoadRewardedAd()
        {
            if (rewardedAdUnitId.Equals(string.Empty))
                return;
            PrintStatus("Requesting Rewarded ad.");

            /// create new rewarded ad instance
            RewardedAd.Load(rewardedAdUnitId, CreateAdRequest(),
                (RewardedAd ad, LoadAdError loadError) =>
                {
                    if (loadError != null)
                    {
                        PrintStatus("Rewarded ad failed to load with error: " +
                                    loadError.GetMessage());
                        //RequestAndLoadRewardedInterstitialAd(); //Akaash ///Load Rewarded
                        return;
                    }
                    else if (ad == null)
                    {
                        PrintStatus("Rewarded ad failed to load.");
                        //RequestAndLoadRewardedInterstitialAd(); //Akaash ///Load Rewarded
                        return;
                    }

                    PrintStatus("Rewarded ad loaded.");
                    rewardedAd = ad;

                    /*ad.OnAdFullScreenContentOpened += () =>
                    {
                        PrintStatus("Rewarded ad opening.");
                        //OnAdOpeningEvent.Invoke();
                    };*/
                    ad.OnAdFullScreenContentClosed += () =>
                    {
                        PrintStatus("Rewarded ad closed.");
                        //OnAdClosedEvent.Invoke();
                        this.appOpenExpireTime = DateTime.Now + APPOPEN_TIMEOUT;//Akaash
                        //appOpenExpireTime = APPOPEN_TIMEOUT;
                        RequestAndLoadRewardedAd(); //Akaash ///Load Rewarded
                    };
                    /*ad.OnAdImpressionRecorded += () =>
                    {
                        PrintStatus("Rewarded ad recorded an impression.");
                    };
                    ad.OnAdClicked += () =>
                    {
                        PrintStatus("Rewarded ad recorded a click.");
                    };
                    ad.OnAdFullScreenContentFailed += (AdError error) =>
                    {
                        PrintStatus("Rewarded ad failed to show with error: " +
                                   error.GetMessage());
                    };
                    ad.OnAdPaid += (AdValue adValue) =>
                    {
                        string msg = string.Format("{0} (currency: {1}, value: {2}",
                                                   "Rewarded ad received a paid event.",
                                                   adValue.CurrencyCode,
                                                   adValue.Value);
                        PrintStatus(msg);
                    };*/
                });
        }

        public void ShowRewardedAd(Action<AdsResult> callback)
        {
            if (!HaveInternet)
                return;
            if (rewardedAd != null)
            {
                mRewardCallback = callback;
                rewardedAd.Show((Reward reward) =>
                {
                PrintStatus("Rewarded ad granted a reward: " + reward.Amount);                
                    if (mRewardCallback != null)
                    {
                        mRewardCallback(AdsResult.Finished);
                        mRewardCallback = null;
                    }
                });
            }
            else
            {
                PrintStatus("Rewarded ad is not ready yet.");
                RequestAndLoadRewardedAd(); //Akaash
                if (callback != null)
                {
                    callback(AdsResult.Failed);
                }
            }
        }

        public void RequestAndLoadRewardedInterstitialAd()
        {
            PrintStatus("Requesting Rewarded Interstitial ad.");

            // Create a rewarded interstitial.
            RewardedInterstitialAd.Load(rewardedAdUnitId, CreateAdRequest(),
                (RewardedInterstitialAd ad, LoadAdError loadError) =>
                {
                    if (loadError != null)
                    {
                        PrintStatus("Rewarded interstitial ad failed to load with error: " +
                                    loadError.GetMessage());
                        return;
                    }
                    else if (ad == null)
                    {
                        PrintStatus("Rewarded interstitial ad failed to load.");
                        return;
                    }

                    PrintStatus("Rewarded interstitial ad loaded.");
                    rewardedInterstitialAd = ad;

                    ad.OnAdFullScreenContentOpened += () =>
                    {
                        PrintStatus("Rewarded interstitial ad opening.");
                        //OnAdOpeningEvent.Invoke();
                    };
                    ad.OnAdFullScreenContentClosed += () =>
                    {
                        PrintStatus("Rewarded interstitial ad closed.");
                        //OnAdClosedEvent.Invoke();
                        RequestAndLoadRewardedInterstitialAd();//Akaash
                    };
                    ad.OnAdImpressionRecorded += () =>
                    {
                        PrintStatus("Rewarded interstitial ad recorded an impression.");
                    };
                    ad.OnAdClicked += () =>
                    {
                        PrintStatus("Rewarded interstitial ad recorded a click.");
                    };
                    ad.OnAdFullScreenContentFailed += (AdError error) =>
                    {
                        PrintStatus("Rewarded interstitial ad failed to show with error: " +
                                    error.GetMessage());
                    };
                    ad.OnAdPaid += (AdValue adValue) =>
                    {
                        string msg = string.Format("{0} (currency: {1}, value: {2}",
                                                    "Rewarded interstitial ad received a paid event.",
                                                    adValue.CurrencyCode,
                                                    adValue.Value);
                        PrintStatus(msg);
                    };
                });
        }

        public void ShowRewardedInterstitialAd(Action<AdsResult> callback)
        {
            if (rewardedInterstitialAd != null)
            {
                mRewardInterstitialCallback = callback;
                rewardedInterstitialAd.Show((Reward reward) =>
                {
                    PrintStatus("Rewarded interstitial granded a reward: " + reward.Amount);
                    if (mRewardInterstitialCallback != null)
                    {
                        mRewardInterstitialCallback(AdsResult.Finished);
                        mRewardInterstitialCallback = null;
                    }
                });
            }
            else
            {
                PrintStatus("Rewarded interstitial ad is not ready yet.");
                RequestAndLoadRewardedInterstitialAd();//Akaash
                if (callback != null)
                {
                    callback(AdsResult.Failed);
                }
            }
        }

        #region Akaash

        /*private void RegisterReloadHandler(RewardedAd ad)
        {
            /// Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
        {
            PrintStatus("Rewarded Ad full screen content closed.");

            /// Reload the ad so that we can show another as soon as possible.
            RequestAndLoadRewardedAd();
        };
            /// Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Rewarded ad failed to open full screen content " +
                               "with error : " + error);

                /// Reload the ad so that we can show another as soon as possible.
                RequestAndLoadRewardedAd();
            };
        }*/

        #endregion Akaash

        #endregion

        #region APPOPEN ADS

        public bool IsAppOpenAdAvailable
        {
            get
            {
                PrintStatus("IsAppOpenAdAvailable_" + (appOpenAd != null && appOpenAd.CanShowAd() && DateTime.Now > appOpenExpireTime));
                return (appOpenAd != null
                        && appOpenAd.CanShowAd()
                        //&& appOpenExpireTime <= 0);//Akaash                
                        && DateTime.Now > appOpenExpireTime);//Akaash                
            }
        }

        /*public void OnAppStateChanged(AppState state)
        {
            /// Display the app open ad when the app is foregrounded.
            UnityEngine.Debug.Log("App State is " + state);

            /// OnAppStateChanged is not guaranteed to execute on the Unity UI thread.
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                if (state == AppState.Foreground)
                {
                    //UnityEngine.Debug.Log("ZZZ App State is " + state);
                    ShowAppOpenAd();
                }
            });
        }*/

        private void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                /// OnAppStateChanged is not guaranteed to execute on the Unity UI thread.
                MobileAdsEventExecutor.ExecuteInUpdate(() =>
                {
                    ShowAppOpenAd();
                //Invoke(nameof(ShowAppOpenAd), 0.2f);
                });
            }
        }
        public void RequestAndLoadAppOpenAd()
        {
            if (appOpenAdUnitId.Equals(string.Empty))
                return;
            PrintStatus("Requesting App Open ad.");

            /// destroy old instance.
            if (appOpenAd != null)
            {
                //DestroyAppOpenAd();
            }

            /// Create a new app open ad instance.
            //AppOpenAd.Load(appOpenAdUnitId, ScreenOrientation.Landscape, CreateAdRequest(),
            AppOpenAd.Load(appOpenAdUnitId, CreateAdRequest(),
                (AppOpenAd ad, LoadAdError loadError) =>
                {
                    /*if (loadError != null)
                    {
                        PrintStatus("App open ad failed to load with error: " +
                            loadError.GetMessage());
                        //RequestAndLoadAppOpenAd(); //Akaash
                        return;
                    }
                    else if (ad == null)
                    {
                        PrintStatus("App open ad failed to load.");
                        //RequestAndLoadAppOpenAd(); //Akaash
                        return;
                    }*/

                    PrintStatus("App Open ad loaded. Please background the app and return.");
                    this.appOpenAd = ad;
                    ////this.appOpenExpireTime = DateTime.Now + APPOPEN_TIMEOUT;//Akaash

                    ad.OnAdFullScreenContentOpened += () =>
                    {
                        PrintStatus("App open ad opened.");
                        //OnAdOpeningEvent.Invoke();
                    };
                    ad.OnAdFullScreenContentClosed += () =>
                    {
                        Time.timeScale = 1f;
                        PrintStatus("App open ad closed.");
                        //OnAdClosedEvent.Invoke();
                        this.appOpenExpireTime = DateTime.Now + APPOPEN_TIMEOUT;//Akaash
                        RequestAndLoadAppOpenAd(); //Akaash
                    };
                    /*ad.OnAdImpressionRecorded += () =>
                    {
                        PrintStatus("App open ad recorded an impression.");
                    };
                    ad.OnAdClicked += () =>
                    {
                        PrintStatus("App open ad recorded a click.");
                    };
                    ad.OnAdFullScreenContentFailed += (AdError error) =>
                    {
                        PrintStatus("App open ad failed to show with error: " +
                            error.GetMessage());
                    };
                    ad.OnAdPaid += (AdValue adValue) =>
                    {
                        string msg = string.Format("{0} (currency: {1}, value: {2}",
                                                   "App open ad received a paid event.",
                                                   adValue.CurrencyCode,
                                                   adValue.Value);
                        PrintStatus(msg);
                    };*/
                });
        }

        public void DestroyAppOpenAd()
        {
            if (this.appOpenAd != null)
            {
                this.appOpenAd.Destroy();
                this.appOpenAd = null;
            }
        }

        public void ShowAppOpenAd()
        {
            if (!HaveInternet)
                return;
            if (!EnableAds)
                return;
            if (IsAppOpenAdAvailable)
            {
                //appOpenExpireTime = APPOPEN_TIMEOUT;
                PrintStatus("App Open Ad Called");
                appOpenAd.Show();
            }
            else
            {
                RequestAndLoadAppOpenAd();
            }
        }

        #endregion

        #region AD INSPECTOR

        public void OpenAdInspector()
        {
            PrintStatus("Opening Ad inspector.");

            MobileAds.OpenAdInspector((error) =>
            {
                if (error != null)
                {
                    PrintStatus("Ad inspector failed to open with error: " + error);
                }
                else
                {
                    PrintStatus("Ad inspector opened successfully.");
                }
            });
        }

        #endregion

        #region Utility

        ///<summary>
        /// Log the message and update the status text on the main thread.
        ///<summary>
        //[System.Diagnostics.Conditional("DEVELOPMENT_BUILD"), System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void PrintStatus(string message)
        {
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                Debug.Log(message);
                if (infoText != null)
                    infoText.text = message;
            });
        }

        #endregion
    }
}