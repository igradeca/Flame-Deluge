using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public GameObject mainCanvas;
    public GameObject howToPlayCanvas;
    public Text howToPlayText;    
    public GameObject[] imagePack;

    private string[] howToPlayTextsToDisplay;
    private int numberToDisplay = 0;

    void Awake()
    {
        howToPlayTextsToDisplay = new string[4];

        howToPlayTextsToDisplay[0] = "Move your character     by swiping\nhorizontally or vertically with your finger.\n\n" +
            "Press AIM to start aiming and then press\nthe shooting mark     where you want to\nshoot.";
        howToPlayTextsToDisplay[1] = "\nCollect health packs     to increase HP\nby 10, food cans     to descrease hunger\nby 15 and energy cells    " + 
            "to refill\nyour ammo by 2.";
        howToPlayTextsToDisplay[2] = "Beware of the enemies!\n\n"+
            "Raiders     can shoot\n\n" + "and ghouls     can scratch!";
        howToPlayTextsToDisplay[3] = "Reach the Exit sign     to survive the\nday.\n\n" +
            "Try to survive in the wastes as long as\npossible!";
    }

    public void ChangeToScene(int sceneToChangeTo)
    {
        SceneManager.LoadScene(sceneToChangeTo);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void HowToPlay()
    {
        numberToDisplay = 0;

        mainCanvas.SetActive(false);
        howToPlayCanvas.SetActive(true);
        howToPlayText.GetComponent<Text>().text = howToPlayTextsToDisplay[numberToDisplay];
        imagePack[numberToDisplay].SetActive(true);
    }

    public void NextInstruction()
    {
        if (numberToDisplay+1 > 3) {
            return;
        }

        imagePack[numberToDisplay].SetActive(false);
        numberToDisplay++;

        SetInstruction();
    }

    public void PreviousInstruction()
    {
        if (numberToDisplay-1 < 0) {
            return;
        }

        imagePack[numberToDisplay].SetActive(false);
        numberToDisplay--;

        SetInstruction();
    }

    private void SetInstruction()
    {
        howToPlayText.GetComponent<Text>().text = howToPlayTextsToDisplay[numberToDisplay];
        imagePack[numberToDisplay].SetActive(true);
    }

    public void ReturnToMenu()
    {
        mainCanvas.SetActive(true);
        howToPlayCanvas.SetActive(false);
        imagePack[numberToDisplay].SetActive(false);
    }
	
}
