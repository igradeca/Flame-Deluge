using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public float levelStartDelay = 2f;
    public float turnDelay = 0.02f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerHealth = 100;
    public int playerFoodPoints = 80;
    public int playerAmmo = 15;
    [HideInInspector]
    public bool playersTurn = true;

    public bool endFromPause = false;

    private Text levelText;
    private GameObject levelImage;
    private int level = 0;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;

	void Awake () {

        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }            

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();        

        boardScript = GetComponent<BoardManager>();
        //InitGame();	
	}

    private void OnLevelWasLoaded (int index)
    {
        level++;        

        InitGame();
    }

    void InitGame()
    {
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver()
    {
        levelText.text = "After " + level + " days, you died.\nPress any key\n to return to main menu.";
        levelImage.SetActive(true);
    }
	
	void Update () {

        if (!levelImage.activeSelf) {
            if (endFromPause) {
                ReturnToMainMenu();
            } else if (playersTurn || enemiesMoving || doingSetup) {
                return;
            }              
            StartCoroutine(MoveEnemies());
        } else if (!doingSetup && Input.anyKey) {
            Invoke("ReturnToMainMenu", levelStartDelay);            
        }
	
	}

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;

        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
            yield return new WaitForSeconds(turnDelay);

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;

    }

}
