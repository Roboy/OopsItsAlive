using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscHandler : MonoBehaviour {

    [Header("If we already are in this scene, the game is closed.")]
    public string levelToReturn = "MainMenu";

    [SerializeField]
    GameObject child;
    void Start()
    {
        if(child == null)
        {
            child = transform.GetChild(0).gameObject;
            if(child!=null)
            {
                child.SetActive(false);
            }
        }
    }

	// Update is called once per frame
	void Update () {

        // On Esc enable/Disable Child
		if(Input.GetKeyUp(KeyCode.Escape))
        {
            if (child != null)
                child.SetActive(!child.activeSelf);
        }
	}

    public void CloseEscWindow()
    {
        if (child != null)
            child.SetActive(false);
    }

    /// <summary>
    /// Quits application
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Loads the level specified in the inspector;
    /// </summary>
    public void GoToLevel()
    {
        GoToLevel(levelToReturn);
    }

    public void GoToLevel(string lToRet)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(lToRet);
    }

    public void RestartThisLevel()
    {
        GoToLevel(Application.loadedLevelName);
    }
}
