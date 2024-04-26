
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
public class HudMenuManager : MonoBehaviour {
	public static HudMenuManager instance;
	public GameObject gameOverMenu;
	public GameObject pauseGame;
	public FPSPlayer player;
    public static float sensitivity;
    public GameObject Minimaap;
    public Slider sensitbitySlider;
    public GameObject loading;
	public GameObject BlackCurtain;
	public GameObject reviveMenu;
	public GameObject cameraInstruction, PvpcameraInstruction, MissionPanel;


	public GameObject okButton;
	
    public SmoothMouseLook SmoothMouseLook;
    public Gradient Gradient;
    public int RegisteredEnemyToKill, RegisteredTotalBomb = 0;
    public GameObject Radar;
    public Text textWave, Gernadecount, TotalEnemyKilled,TotalBombDefused, outOfAmmoText;
    //public FPSPlayer FPSPlayer;
    public Slider playerhealthBar;
    public GameObject[] meshesToDisable;
    public bool zoomedIn = false;
    public GameObject[] UI;
    public GameObject[] Snipers,buttosToDisAble;
    public GameObject HeadShortObj, ButtonRun;
    public GameObject[] ObjectsToDisableWhenGameOver;
    public int HeadShortCount= 0;
    public Toggle autoFireToggle;
    public Image InstrutionImage;
    public GameObject BombFillbar; 
    public DOTweenAnimation ThrowGrenadeButton;
    public Camera FpsCamera;
    public GameObject LastBulletPrefab, Croshair;
    public string BombDefuseNotification, RemaingEnemyNotification = "";
   
    [Header("PVP Mode Data")]
    public GameObject Pickupgernade;
    public int PvPEntryFee = 100;
    public float timeToWait = 5f;
    public Text countDownText;
    public Text MainplayerScore;
    public Text PlayerTeamScore;
    public Text EnemyTeamScore;
    public Text EnemyLeftCount;
    public GameObject[] PvpUi;
    public GameObject[] EndlessUi;
    public GameObject[] LevelmodeUi;
    public GameObject[] FireBtn;
    public GameObject Fadeinout;
    private bool shouldStartTime;
    private bool isTimeCompleted = true;
    [HideInInspector]
    public int EnemyTeamKillCout, PlayerTeamKillCout, MainplayerkillCount;
    [HideInInspector]
    public Level CurrentLevel;
    [HideInInspector]
    public List<GameObject> TempAiList = new List<GameObject>();
    [Header("Endless Mode")]
    public Text TotalKill;
    public Text BestKil;
    int totalenemies;
    [Header("Gameplay Sounds")]
    public AudioSource BgMusic;
    public AudioClip PvpBg,Playerwinsound;
    public void ShowHeadShort(Transform pos)
    {
        HeadShortObj.SetActive(false);
        HeadShortObj.transform.position = pos.position;
        HeadShortObj.transform.LookAt(Camera.main.transform);
        HeadShortObj.SetActive(true);
        HeadShortCount++;
        
    }

    public void SniperZoomPreCheck()
    {
        //foreach (var item in Snipers)
        //{
        //    if (item.activeSelf)
        //    {
        //        SniperZoom();
        //    }
        //}
    }
    public void SniperZoom()
    {
        //zoomedIn = !zoomedIn;
        if (zoomedIn)
        {
            foreach (var item in meshesToDisable)
            {
                item.SetActive(false);
            }
            foreach (var item in UI)
            {
                item.SetActive(true);
            }

            foreach (var item in buttosToDisAble)
            {
                item.SetActive(false);
            }
        }
        else
        {
            foreach (var item in meshesToDisable)
            {
                item.SetActive(true);
            }
            foreach (var item in UI)
            {
                item.SetActive(false);

            }
            foreach (var item in buttosToDisAble)
            {
                item.SetActive(true);
            }
        }
    }

    public void zoomOut() {
        foreach (var item in meshesToDisable)
        {
            item.SetActive(true);
        }
        foreach (var item in UI)
        {
            item.SetActive(false);

        }
        foreach (var item in buttosToDisAble)
        {
            item.SetActive(true);
        }
    }
    
    void Awake () {
        MConstants.isPlayerWin = false;
        MConstants.IslastBullet = false;
        MConstants.LastBulletTarget = null;
        MConstants.GunInstLimit=0;
        MConstants.HealtInstLimit=0;
        if (BgMusic == null)
        {
            BgMusic = this.GetComponent<AudioSource>();
        }
        if (SmoothMouseLook == null)
        {
            SmoothMouseLook = GameObject.FindObjectOfType(typeof(SmoothMouseLook)) as SmoothMouseLook;
        }
        instance = this;
		MConstants.isGameOver = false;
        MConstants.bulletsFinished = false;
       // updatePos(1);
        if (PlayerDataController.Instance)
        {
            sensitivity = PlayerDataController.Instance.playerData.SensivityValue;

        }
        else
        {
            sensitivity = 4f;
        }
       
        SmoothMouseLook.sensitivity = sensitivity;
        if (MConstants.CurrentLevelNumber == 1)
        {
            MConstants.autoFireMode = false;
        }
        else { MConstants.autoFireMode = true; }
        if (MConstants.CurrentGameMode == MConstants.GAME_MODES.ENDLESS_MODE)
        {
            MConstants.autoFireMode = true;
        }
        if (MConstants.CurrentGameMode == MConstants.GAME_MODES.PVP_MODE)
        {
            foreach (var ui in LevelmodeUi)
            {
                ui.SetActive(false);
            }
            MConstants.autoFireMode = false;
            BgMusic.clip=PvpBg;
            BgMusic.enabled = false;
        }
        if (PlayerDataController.Instance && PlayerDataController.Instance.playerData.isMusicOn == true)
        {
            BgMusic.volume = 1f;
            //toggleSound.isOn = isOn;
        }
        else
        {
            BgMusic.volume = 0;
            //toggleSound.isOn = isOn;
        }
        autoFireToggle.isOn = MConstants.autoFireMode;
    }
   
    public void EnableRadar()
    {
        Radar.SetActive(true);
    }

    void Start(){

        if (UnityAnalyticsScript.instance)
        {
            string levelString = "level_start";
            
            if (MConstants.CurrentGameMode == MConstants.GAME_MODES.COMMANDO_MODE)
            {
                levelString = "level_start_commando";
            }
            if (MConstants.CurrentGameMode == MConstants.GAME_MODES.City_Of_Sin)
            {
                levelString = "level_start_cityofsin";
            }
            if (MConstants.CurrentGameMode == MConstants.GAME_MODES.Squard)
            {
                levelString = "level_start_squard";
            }
            if (MConstants.CurrentGameMode == MConstants.GAME_MODES.PVP_MODE)
            {
                levelString = "pvp_start";

                UnityAnalyticsScript.instance.AddUnityEvent(levelString, new Dictionary<string, object>{
                    { "level_index", ""+PlayerDataController.Instance.playerData.PvPPlayCount}
                });
                // UnityAnalyticsScript.instance.AddUnityEvent(levelString, "level_index", PlayerDataController.Instance.playerData.PvPPlayCount);
            }
            else
            {
                UnityAnalyticsScript.instance.AddUnityEvent(levelString, new Dictionary<string, object>{
                    { "level_index", ""+MConstants.CurrentLevelNumber}
                });
            }
            
        }
        if (MConstants.CurrentGameMode == MConstants.GAME_MODES.ENDLESS_MODE)
        {

           

            Invoke("StartGame", 1);
        }
        else if (MConstants.CurrentGameMode == MConstants.GAME_MODES.PVP_MODE)
        {
            isTimeCompleted = false;
            //PlayerUiControlls.SetActive(true);
            countDownText.gameObject.SetActive(true);
            CurrentLevel = LevelsManager.instance.PvpMode;
            //  Invoke("SpawnPlayer", 0.2f);
            // Invoke("randomAiSpawing", 0.5f);
            Invoke("randomAiSpawing", 1f);
            
           
            Invoke("StartGame", 1);
        }
        else
        {
          
            if (TotalBombDefused != null && LevelsManager.instance.Levels[MConstants.CurrentLevelNumber - 1].TotalTimeBomb>0)
            {
                TotalBombDefused.transform.parent.gameObject.SetActive(true);
                RegisteredTotalBomb = LevelsManager.instance.Levels[MConstants.CurrentLevelNumber - 1].TotalTimeBomb;
                TotalBombDefused.text = RegisteredTotalBomb + " / " + LevelsManager.instance.Levels[MConstants.CurrentLevelNumber - 1].TotalTimeBomb;

            }
            RegisteredEnemyToKill = LevelsManager.instance.Levels[MConstants.CurrentLevelNumber - 1].RegisterEnemyToKill;
            TotalEnemyKilled.text = RegisteredEnemyToKill + " / " + LevelsManager.instance.Levels[MConstants.CurrentLevelNumber - 1].RegisterEnemyToKill;
            totalenemies = LevelsManager.instance.Levels[MConstants.CurrentLevelNumber - 1].RegisterEnemyToKill;
            Invoke("StartGame", 1);
        }

        if (AdsManager.instance != null)
        {
            AdsManager.instance.RequestBanner();
            AdmobAdListener.instance.setPosition(true);
            AdsManager.instance.RequestInterstitial();
        }


    }

    IEnumerator NotificationText()
    {

       
        yield return new WaitForSeconds(4f);
        HudMenuManager.instance.textWave.text = "";

    }
    public void GameOver(){



        //    if ( !MConstants.isPlayerWin && AdsManager.instance != null &&  AdsManager.instance.isAdReady() ){
        //
        //		MConstants.isGameOver = true;
        //
        //		reviveMenu.SetActive(true);
        //		return;
        //	}
        //
        
        Invoke ("GameFail",1);

	}

	public void GameFail(){
        //foreach (var item in ObjectsToDisableWhenGameOver)
        //{
        //    item.SetActive(false);
        //}


     
        MConstants.isGameOver = true;
		AdsManager.isRevive = false;
        
        gameOverMenu.SetActive (true);
        Minimaap.SetActive(false);
        //AI[] enemy = FindObjectsOfType<AI>();

        //for (int i = 0; i < enemy.Length; i++)
        //{
        //    AudioSource[] audiocomponet = enemy[i].GetComponents<AudioSource>();

        //    for (int j = 0; j < audiocomponet.Length; j++)
        //    {
        //        audiocomponet[j].enabled = false;
        //    }
        //}
    }
    
    public void BombDefuse()
    {
        RegisteredTotalBomb--;
        if (RegisteredEnemyToKill <= 0 && RegisteredTotalBomb <= 0)
        {
            GameComplete();
        }
        else if (RegisteredEnemyToKill > 0 && RegisteredTotalBomb <= 0)
        {
            HudMenuManager.instance.textWave.text = RemaingEnemyNotification;
            StartCoroutine("NotificationText");
        }
        TotalBombDefused.text = RegisteredTotalBomb + " / " + LevelsManager.instance.Levels[MConstants.CurrentLevelNumber - 1].TotalTimeBomb;
    }
    public void EnemyKilled()
    { if (MConstants.CurrentGameMode != MConstants.GAME_MODES.ENDLESS_MODE)
        {
            
            RegisteredEnemyToKill--;
            MConstants.enemyinstatiate = true;
            if (RegisteredEnemyToKill <= 0 && RegisteredTotalBomb<=0 )
            {
                GameComplete();
            }else if(RegisteredEnemyToKill <= 0 && RegisteredTotalBomb > 0)
            {
                HudMenuManager.instance.textWave.text = BombDefuseNotification;
                StartCoroutine("NotificationText");
            }
            TotalEnemyKilled.text = RegisteredEnemyToKill + " / " + LevelsManager.instance.Levels[MConstants.CurrentLevelNumber - 1].RegisterEnemyToKill;
            /*if(MConstants.CurrentGameMode == MConstants.GAME_MODES.Squard)
            {
                 if (totalenemies > 2 && RegisteredEnemyToKill >= 1)
                {
                    MConstants.enemyinstatiate = true;
                }
                TotalEnemyKilled.text = RegisteredEnemyToKill + " / " + LevelsManager.instance.Levels[MConstants.CurrentLevelNumber - 1].RegisterEnemyToKill;
            }
            else if(MConstants.CurrentGameMode == MConstants.GAME_MODES.City_Of_Sin 
                || MConstants.CurrentGameMode == MConstants.GAME_MODES.COMMANDO_MODE)
            {
              
                    MConstants.enemyinstatiate = true;
             
                TotalEnemyKilled.text = RegisteredEnemyToKill + " / " + LevelsManager.instance.Levels[MConstants.CurrentLevelNumber - 1].RegisterEnemyToKill;
            }*/

        }
        else
        {
            RegisteredEnemyToKill++;
            TotalEnemyKilled.text =RegisteredEnemyToKill.ToString();
            TotalKill.text = RegisteredEnemyToKill.ToString();
            WaveDataController.Instance.wavecount();
            MConstants.GunInstLimit++;
            MConstants.HealtInstLimit++;
            if(RegisteredEnemyToKill>PlayerDataController.Instance.playerData.BestKill)
            {
                
                PlayerDataController.Instance.playerData.BestKill = RegisteredEnemyToKill;
                BestKil.text = PlayerDataController.Instance.playerData.BestKill.ToString();
                PlayerDataController.Instance.Save();
            }
            else
            {
                BestKil.text = PlayerDataController.Instance.playerData.BestKill.ToString();
            }
            //if (MConstants.gunreset)
            //{
                
            //    MConstants.GunInstLimit = 0;
            //    MConstants.gunreset = false;

            //}
            //if (MConstants.healthreset)
            //{
            //    MConstants.HealtInstLimit = 0;
            //    MConstants.healthreset = false;


            //}
        }

    }
    public void GameOverByNoRevive(){

		MConstants.isGameOver = true;

		gameOverMenu.SetActive (true);
	}

	public void LapCompleteRemove(){
	}

	public void updatePos(int pos){
		
	}

	void FreezVehicle(){
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		if(player){
			player.GetComponent<Rigidbody> ().isKinematic = true;
		}
	}

	public void GameComplete(){
        BgMusic.clip = Playerwinsound;
        BgMusic.PlayOneShot(BgMusic.clip, 1.0f);
        foreach (var item in ObjectsToDisableWhenGameOver)
        {
          //      item.SetActive(false);
        }

        MConstants.isPlayerWin = true;	
		MConstants.isGameOver = true;	

		Invoke ("GameFail",1);

	}

	public void Revive(){
		AdsManager.isRevive = false;
		//refreshHealth ();
		Timer.Instance.resetTime ();
		MConstants.isGameOver = false;
		Time.timeScale = 1;
	}

    public GameObject[] HudForKidsMode;


	

	//public void showMissionInstruction(){
    //
	//	MissionInfo.SetActive (true);
	//	okButton.SetActive (false);
	//	Invoke ("PausePlayer",0.5f);
	//}


	void PausePlayer(){
		okButton.SetActive (true);

		if(player == null){
			player = GameObject.FindObjectOfType (typeof(FPSPlayer)) as FPSPlayer;
		}

		if(player != null){
			player.paused = true;
		}	
	}

	public void StartGame(){

		Time.timeScale = 1;
		MConstants.isGameStarted = true;
       
        if (cameraInstruction && MConstants.CurrentLevelNumber == 1&& MConstants.CurrentGameMode != MConstants.GAME_MODES.ENDLESS_MODE && MConstants.CurrentGameMode != MConstants.GAME_MODES.PVP_MODE)
        {
			cameraInstruction.SetActive (true);
			//Invoke ("hideCameraInstruction",3);
		}
		if(player == null){
			player = GameObject.FindObjectOfType (typeof(FPSPlayer)) as FPSPlayer;
		}

		if(player != null){
			player.paused = false;
		}
        player.PlayerWeaponsComponent.SelectWeapon();

    }

    void selectWeapon()
    {
        
    }
	public void hideCameraInstruction(){
		cameraInstruction.SetActive (false);
	}
	public void PauseGame(){
		pauseGame.SetActive (true);
		if(player == null){
			player = GameObject.FindObjectOfType (typeof(FPSPlayer)) as FPSPlayer;
		}

		if(player != null){
			player.paused = true;
		}
	}

	public void ResumeGame(){
        sensitivity = PlayerDataController.Instance.playerData.SensivityValue;
        SmoothMouseLook.sensitivity = sensitivity;
        if (player == null){
			player = GameObject.FindObjectOfType (typeof(FPSPlayer)) as FPSPlayer;
		}

		if(player != null){
			player.paused = false;
		}
	}


    void Update()
    {

        //if (slowmotion && Time.timeScale < 1f)
        //{
        //    Time.timeScale += 1.2f * Time.deltaTime;


        //}
        if (ControlFreak2.CF2Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
        if (isTimeCompleted == false)
        {

            timeToWait -= Time.deltaTime;
            countDownText.text = ((int)timeToWait).ToString();
            if (timeToWait < 0)
            {
                isTimeCompleted = true;
                CompletedCountDown();
            }
        }
        playerhealthBar.value = player.hitPoints;
    }
    ////.................. <PvpMode>................................................
    void randomAiSpawing()
    {

        for (int i = 1; i <= CurrentLevel.MaxTeamMember; i++)
        {


            PvPEnemyAiSpawing(i);
            if (i > 1)
            {



                PvpPlayerAiSpawing(i);

            }




        }

    }
    public void PvPEnemyAiSpawing(int Aispawnpoint = 0)
    {
        // Debug.LogError("Ai");
        GameObject CurrentEnemy;
        int randamSpwanPoint = Random.Range(0, CurrentLevel.PvpSpawningPoint.EnemyAiSpawnPoint.Length);
        int SpwanAiindex = Random.Range(0, LevelsManager.instance.AiEnemy.Length);

        float AiHealth = Random.Range(CurrentLevel.enemyProperties.MinHealth, CurrentLevel.enemyProperties.MaxHealth);

        float AiDamageToPlayer = Random.Range(CurrentLevel.enemyProperties.MinDamagetoplayer, CurrentLevel.enemyProperties.MaxDamagetoplayer);
        float AiShootrange = Random.Range(CurrentLevel.enemyProperties.MinShootrange, CurrentLevel.enemyProperties.MaxShootrange);
        if (Aispawnpoint > 0)
        {
            randamSpwanPoint = Aispawnpoint - 1;
        }
        Debug.Log("SpwanAiindex " + SpwanAiindex + "randamSpwanPoint" + randamSpwanPoint);
        Debug.Log("SpwanAiindex L" + LevelsManager.instance.AiEnemy.Length + "randamSpwanPoint l" + CurrentLevel.PvpSpawningPoint.EnemyAiSpawnPoint.Length);

        CurrentEnemy = Instantiate(LevelsManager.instance.AiEnemy[SpwanAiindex], CurrentLevel.PvpSpawningPoint.EnemyAiSpawnPoint[randamSpwanPoint].transform.position, CurrentLevel.PvpSpawningPoint.EnemyAiSpawnPoint[randamSpwanPoint].transform.rotation) as GameObject;
        CurrentEnemy.GetComponent<AI>().huntPlayer = false;
        CurrentEnemy.GetComponent<AI>().standWatch = true;
        CurrentEnemy.GetComponent<AI>().ignoreFriendlyFire = true;
        CurrentEnemy.GetComponent<CharacterDamage>().hitPoints = AiHealth;
        CurrentEnemy.GetComponent<NPCAttack>().damage = AiDamageToPlayer;
        CurrentEnemy.GetComponent<AI>().ChangeFaction(2);
        CurrentEnemy.GetComponent<AI>().shootRange = AiShootrange;
        CurrentEnemy.transform.GetComponent<AI>().attackRange = 1500F;


        if (isTimeCompleted == false)
        {
            CurrentEnemy.GetComponent<AI>().attackRange = 0F;
            //  CurrentEnemy.GetComponent<AI>().shootRange = 0f;
            TempAiList.Add(CurrentEnemy);
        }
    }
    public void PvpPlayerAiSpawing(int Aispawnpoint = 0)
    {
        GameObject CurrentEnemy;
        int randamSpwanPoint = Random.Range(0, CurrentLevel.PvpSpawningPoint.PlayerAiSpwanPoint.Length);
        int SpwanAiindex = Random.Range(0, LevelsManager.instance.AiPlayer.Length);
        float AiHealth = Random.Range(CurrentLevel.enemyProperties.MinHealth, CurrentLevel.enemyProperties.MaxHealth);
        float AiDamageToPlayer = Random.Range(CurrentLevel.enemyProperties.MinDamagetoplayer, CurrentLevel.enemyProperties.MaxDamagetoplayer);
        float AiShootrange = Random.Range(CurrentLevel.enemyProperties.MinShootrange, CurrentLevel.enemyProperties.MaxShootrange);
        if (Aispawnpoint > 0)
        {
            randamSpwanPoint = Aispawnpoint - 2;

        }
        CurrentEnemy = Instantiate(LevelsManager.instance.AiPlayer[SpwanAiindex], CurrentLevel.PvpSpawningPoint.PlayerAiSpwanPoint[randamSpwanPoint].transform.position, CurrentLevel.PvpSpawningPoint.PlayerAiSpwanPoint[randamSpwanPoint].transform.rotation) as GameObject;
        CurrentEnemy.GetComponent<AI>().huntPlayer = false;
        CurrentEnemy.GetComponent<AI>().standWatch = true;
        CurrentEnemy.GetComponent<AI>().ignoreFriendlyFire = true;
        CurrentEnemy.GetComponent<CharacterDamage>().hitPoints = AiHealth;
        CurrentEnemy.GetComponent<NPCAttack>().damage = AiDamageToPlayer;
        CurrentEnemy.GetComponent<AI>().ChangeFaction(1);
        CurrentEnemy.transform.GetComponent<AI>().attackRange = 1500F;

        CurrentEnemy.GetComponent<AI>().shootRange = AiShootrange;
        if (isTimeCompleted == false)
        {
            CurrentEnemy.GetComponent<AI>().attackRange = 0F;

            TempAiList.Add(CurrentEnemy);
        }
    }
    public void CompletedCountDown()
    {
        BgMusic.enabled = true;
        BgMusic.volume = 0.5f;
        countDownText.gameObject.SetActive(false);
        //GamePlayButton.SetActive(true);
        // PlayerUiControlls.SetActive(true);
        // EndlessUi[4].SetActive(false);
        foreach (var ui in PvpUi)
        {
            ui.SetActive(true);
        }
        MConstants.autoFireMode = true;
        autoFireToggle.isOn = MConstants.autoFireMode;
        foreach (GameObject tempAi in TempAiList) //the line the error is pointing to
        {
            tempAi.GetComponent<AI>().attackRange = 1500F;

        }
        ToggleAutoFire();
        if (MConstants.CurrentGameMode == MConstants.GAME_MODES.PVP_MODE && PlayerDataController.Instance.playerData.ISpvpTutorial)
        {

            PvpcameraInstruction.SetActive(true);
            PlayerDataController.Instance.playerData.ISpvpTutorial = false;
            PlayerDataController.Instance.Save();
        }
    }
    public void RefreshPvpData(int AiFaction)
    {
        EnemyTeamScore.text = EnemyTeamKillCout.ToString();
        PlayerTeamScore.text = PlayerTeamKillCout.ToString();
        MainplayerScore.text = MainplayerkillCount.ToString();
        EnemyLeftCount.text = (CurrentLevel.MaxKillInMatch - PlayerTeamKillCout).ToString() + " " + "Kills left";
        if (PlayerTeamKillCout >= CurrentLevel.MaxKillInMatch)
        {
            GameComplete();
        }
        else if (EnemyTeamKillCout >= CurrentLevel.MaxKillInMatch)
        {

            MConstants.isPlayerWin = false;
            GameOver();
        }
        else
        {
            if (AiFaction == 1)
            {
                PvpPlayerAiSpawing();
            }
            else if (AiFaction == 2)
            {
                PvPEnemyAiSpawing();
            }
            else
            {
                Invoke("PlayerRespawn", 2f);
            }
        }
    }
    public void PlayerRespawn()
    {
        int PvpplayerPostion = Random.Range(0, LevelsManager.instance.PvpMode.PvpSpawningPoint.PlayerAiSpwanPoint.Length);
        player.gameObject.transform.position = LevelsManager.instance.PvpMode.PvpSpawningPoint.PlayerAiSpwanPoint[PvpplayerPostion].transform.position;
        player.gameObject.transform.rotation = LevelsManager.instance.PvpMode.PvpSpawningPoint.PlayerAiSpwanPoint[PvpplayerPostion].transform.rotation;
       
        player.hitPoints = 100f;
        HudMenuManager.instance.Fadeinout.SetActive(false);
        player.WeoponsCamera.SetActive(true);
        PlayerWeapons.instance.gameObject.SetActive(true);
        player.Resethealth();
        if (UI[1].activeSelf)
        {
            player.zoomed = false;
            HudMenuManager.instance.zoomOut();
        }
        player.MouseLookComponent.enabled = true;
    }
    public void TimeScaleToOne()
    {
        Time.timeScale = 1;
    }

    public void ToggleAutoFire()
    {
        MConstants.autoFireMode = autoFireToggle.isOn;
        if (MConstants.CurrentGameMode == MConstants.GAME_MODES.PVP_MODE)
        {
            if (MConstants.autoFireMode)
            {
                foreach (GameObject firbtn in FireBtn)
                {
                    firbtn.SetActive(false);
                }
            }
            else
            {
                foreach (GameObject firbtn in FireBtn)
                {
                    firbtn.SetActive(true);
                }
            }
        }
    }
}
