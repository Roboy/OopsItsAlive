using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevel : MonoBehaviour {

    public void ReloadCurrentLevel()
    {
        LoadLevelNow(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void LoadLevelNow(string name)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(name);
    }
}
