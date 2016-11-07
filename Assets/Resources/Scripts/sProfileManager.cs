using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//////////////////////////////
/// TRUCK CITY!
//////////////////////////////
/// ProfileManager [DONTDESTROYONLOAD] Persistente
//////////////////////////////
/// Este script carga o crea nuevo perfil, e inicializa el juego
//////////////////////////////

public class sProfileManager : Singleton<sProfileManager> {
    

  
    [SerializeField]
    bool ForceNewProfile;
    [SerializeField]
    bool DebugVersion = false;
    [SerializeField]
    int targetFrameRateHigh = 60;
    [SerializeField]
    int targetFrameRateLow = 30;
    [SerializeField]
    int RecommendedVideoMemory = 1024;

    static Profile _singleton = null;
    public static Profile ProfileSingleton
    {
        get
        {
            if (_singleton == null)
            {
                if (sProfileManager.instance == null) return null;
                if (sProfileManager.instance.ForceNewProfile)
                {
                    _singleton = NewProfile();
                }
                else
                {
                    Profile pf = sSaveLoad.LoadProfile();
                    if (pf == null || pf.version == "" || pf.version != sProfileManager.instance.defaultProfile.version)
                    {
                        _singleton = NewProfile();
                    }
                    else
                    {
                        _singleton = pf;
                        sSaveLoad.savedProfile = _singleton;                        
                        Debug.Log("Profile Loaded");
                    }
                }
            }
            return _singleton;


        }
        set
        {
            _singleton = value;
        }
    }



    public Profile defaultProfile;

    public List<LevelConditions> levelconditions;



    static Profile NewProfile()
    {
        Debug.Log("NEW PROFILE!");
        
        sSaveLoad.savedProfile = sProfileManager.instance.defaultProfile;
        sSaveLoad.savedProfile.LanguageSelected = ChooseLanguage();
        //sSaveLoad.savedProfile.GlobalGraphicQualitySettings = CheckSystem(out sSaveLoad.savedProfile.GraphicMemory);
        return sSaveLoad.savedProfile;
    }

    static GraphicQualitySettings CheckSystem(out float graphicMemory)
    {
        float mem = SystemInfo.graphicsMemorySize;
        graphicMemory = mem;
        Debug.Log(mem);
        if (mem >= sProfileManager.s.RecommendedVideoMemory) {            
            return GraphicQualitySettings.High;
        }else
        {
            return GraphicQualitySettings.Low;
        }
    }

    static string ChooseLanguage()
    {
        string s = "English";
        switch (Application.systemLanguage)
        {           
            case SystemLanguage.Spanish:
                s = "Spanish";
                break;           
            default:
                s = "English";
                break;
        }
        return s;
    }


    void Start()
    {
        
     //Esto solo sera ejecutado cuando se crea este objeto.
     //Gracias a que hereda de Singleton<> y "Is Peristent" es true

        //DEVELOPER!
        //sSaveLoad.DeleteSavedGame(); //
        //---DEVELOPER


        //Inicializa el Perfil   (Se hace en el GET)
#pragma warning disable 0219
        Profile pf = sProfileManager.ProfileSingleton;
#pragma warning restore 0219
        //pf no vale para nada realmente! (Por ahora)

        //Comprueba si hay una partida guardada... estabamos jugando
        //pf.currentlyPlayingLevel = sSaveLoad.CheckIfSavedGame();


        //Si estabamos jugando:
        
        Localization.language = pf.LanguageSelected;

        switch (sProfileManager.ProfileSingleton.GlobalGraphicQualitySettings)
        {
            case GraphicQualitySettings.Low:
                Application.targetFrameRate = targetFrameRateLow;
                break;
            case GraphicQualitySettings.Medium:
                Application.targetFrameRate = targetFrameRateHigh;
                break;
            case GraphicQualitySettings.High:
                Application.targetFrameRate = targetFrameRateHigh;
                break;
            default:
                Application.targetFrameRate = targetFrameRateLow;
                break;
        }

        //pplication.targetFrameRate = 5;
        QualitySettings.SetQualityLevel((int)_singleton.GlobalGraphicQualitySettings);
        GameConfig.s.MusicState = pf.MusicState;
        GameConfig.s.SoundState = pf.SoundState;
        if (!DebugVersion)StartCoroutine(ChangeScene(1)); //Vamos al menu

        //if (GameConfig.s.MusicState) MusicStore.s.PlayMusicByAlias("Menu", 1.5f, GameConfig.s.MusicVolume, true, 5f);




    }

    IEnumerator ChangeScene(int i)
    {
        yield return new WaitForSeconds(3f);
        //        SceneManager.LoadScene(i);
        LoadingScreenManager.LoadScene(i);
    }

    public void ChangeLevel(int levelIndex)
    {
        SoundSystem.s.FadeOutMusic(0.5f, () =>
         {

             sProfileManager.ProfileSingleton.newLevelIndex = levelIndex;
             sProfileManager.ProfileSingleton.ChangingLevel = true;
             LoadingScreenManager.LoadScene(levelIndex + 3);
         });
    }
    



}






