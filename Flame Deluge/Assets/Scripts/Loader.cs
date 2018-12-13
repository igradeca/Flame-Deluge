using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour {

    public GameObject gameManager;
    public GameObject pauseCanvas;

    // Use this for initialization
    void Awake () {

        if (GameManager.instance == null)
            Instantiate(gameManager);	
	}

    public void Pause()
    {
        pauseCanvas.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void Resume()
    {
        pauseCanvas.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void ReturnToMainMenu()
    {
        GameObject.Find("GameManager(Clone)").GetComponent<GameManager>().endFromPause = true;
        pauseCanvas.SetActive(false);
        Time.timeScale = 1.0f;
    }

}
