using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManager : MonoBehaviour
{
    
   
    public GameDataManager Gamedata;
    public GameDataBase gamedatabase;
    public static GameManager Instance;
  

    public string fileName = "";
  
    void Awake()
    {
        Instance = this;
        fileName = Application.persistentDataPath + "/gameData.json";
        Load();
        DontDestroyOnLoad(this);
    }

    void LoadSaveDeafultData()
    {
        
        Save();
        

    }

   
    public void Save()
    {
        
        string json = JsonUtility.ToJson(Gamedata);

        // Write the JSON data to a file
        File.WriteAllText(fileName, json);
    }

    public void Load()
    {
        try
        {
           
            if (File.Exists(fileName))
            {
                // Read the JSON data from the file
                string jsonPath = File.ReadAllText(fileName);

                // Deserialize the JSON data to a GameData object
                Gamedata= JsonUtility.FromJson<GameDataManager>(jsonPath);
                Debug.LogError("file loded");
            }
            else
            {
                LoadSaveDeafultData();
                Debug.LogError("Default file loded");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load game data: " + e.Message);
            LoadSaveDeafultData();

        }


    }

   
}
public enum SubMenuNames
{
    NO_INTERNET_POP,
    NO_VIDEO_POPUP,
    SETTING_MENU,
    STORE_CONGRATS_POPUP,
    SPIN_POPUP,
    
    PLAYER_UNLOCK_POPUP,
    OUT_OF_CASH,
    CONGRATS_POPUP,
    LEVEL_UNLOCK_POPUP,
    EXIT,
   // STORE_POPUP,
    CONGRATS_POPUP_ForGun,
  //  DAILY_POPUP,
    LOADING,
    GUN_UNLOCK_POPUP,
    //  ENV_UNLOCK_POPUP,

}
public enum MenuNames
{

    MAIN_MENU,
    PLAYER_SELCT_MENU,
    MODE_SELCT_MENU,
    LEVEL_SELCT_MENU,
    LOADOUT_SELCT_MENU,
    GUN_SELCT_MENU,
    STORE_POPUP,


}
public enum ItemType
{
    COINS,
    ENERGY,
    MEDKIT,
    GRENADE,
    SLOWMO
}


public enum PaymentType
{
    GAME_GOLD,
    IN_APP,
    REWARDED_AD
}
public enum outcaststate
{

    Gun_State,
    Player_State,
    Enverienment_State,


}
public enum GAME_MODES
{

    Mode_A,
    City_Of_Sin,
    Squard,
    TDM_Mode,
    ENDLESS_MODE
}