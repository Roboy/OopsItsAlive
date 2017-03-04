using UnityEngine;
using System.Collections;
using System;

public class GoalManager : MonoBehaviour
{

    public enum STATUS {NONE, WON, LOST };

    public static STATUS gameState = STATUS.NONE;
    public String nextLevel = "level2";

    public KeyCode winKey = KeyCode.O;
    public KeyCode loseKey = KeyCode.I;

    void Start()
    {
        gameState = STATUS.NONE;
    }

    void Update()
    {
        if (Input.GetKeyDown(winKey))
        {
            Win();
        }

        if (Input.GetKeyDown(loseKey))
        {
            Lose();
        }

    }
    
    public void LoadLevel(string levelName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
    }
    
    public void Lose()
    {
        Debug.Log("YOU LOST!!!");
        gameState = STATUS.LOST;
        GameObject.FindObjectOfType<UI>().losePanel.SetActive(true);
        //throw new NotImplementedException();
    }
    
    public void Win()
    {
        if (gameState != STATUS.NONE)
            return;

        StopAllCoroutines();
        StartCoroutine(WinCor());
    }

    private IEnumerator WinCor()
    {
        Debug.Log("WIN!!!");
        gameState = STATUS.WON;
        Fade fade = GameObject.FindObjectOfType<Fade>();
        if(fade)
        {
            fade.FadeInNow();
        }
        yield return new WaitForSeconds(1);
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevel);
        yield return null;
    }
    
}
