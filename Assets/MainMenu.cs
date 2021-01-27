using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameAnalyticsSDK;

public class MainMenu : MonoBehaviour
{

    void Awake()
    {
        GameAnalytics.Initialize();
    }
    public void PlayGame ()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "Play Game");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ReturnToMenu ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void CreateError ()
    {
        try{
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 3);
        }catch(System.Exception e){
            Debug.Log(e);
        }
    }

}
