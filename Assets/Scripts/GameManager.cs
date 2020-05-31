using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Usual game manager things
    public static GameManager Instance { get; private set; }
    private GameManager instance;
   
    public delegate void GamePauseToggle(bool paused);
    public event GamePauseToggle GamePauseEvent;
    public delegate void GameRestart();
    public event GameRestart GameRestartEvent;

    public GameObject pausePanel;
    public GameObject gameFinishedPanel;

    public bool paused { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            instance = this;
            Instance = this;
        }
        else
            Destroy(this);


        paused = false;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        gameFinishedPanel.SetActive(false);
    }
    public void TogglePause(bool pause)
    {
        paused = pause;

        if (paused)
        {
            Time.timeScale = 0;
            pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pausePanel.SetActive(false);
        }

        if (GamePauseEvent != null)
            GamePauseEvent(pause);
    }
    
    public void GameFinished()
    {
        //Pause the game and activate the restart panel
        Time.timeScale = 0;
        paused = true;
        if (GamePauseEvent != null)
            GamePauseEvent(true);
        gameFinishedPanel.SetActive(true);
    }

    public void Restart()
    {
        if (GameRestartEvent != null)
            GameRestartEvent();
        
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }

}
