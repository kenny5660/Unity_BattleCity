using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MenuController : MonoBehaviour
{
    // Start is called before the first frame update
    public Image BackgraundColorImage;
    public Color WinnerColor;
    public Color LoseColor;
    public Text endGameText;
    public string WinnerText = "You winner!";
    public string LoseText = "You lose!";
    Canvas canvas;
    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;

    }
    public void ShowEndGameMenu(bool isWin)
    {
        canvas.enabled = true;
        if (isWin)
        {
            BackgraundColorImage.color = WinnerColor;
            endGameText.text = WinnerText;
        }
        else
        {
            BackgraundColorImage.color = LoseColor;
            endGameText.text = LoseText;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnClickRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void OnClickExit()
    {
        Application.Quit();
    }
}
