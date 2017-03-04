using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class DialogueManager : MonoBehaviour
{

    public enum PROTAGONISTS { P0, P1, P2, P3 };

    public KeyCode[] keysToContinue = new KeyCode[] { KeyCode.Space, KeyCode.Mouse0, KeyCode.KeypadEnter };
    public int currentDialogueStep = 0;
    public string nextLevel = "Level2";

    public GameObject[] speakers;

    public DialogueComponent[] dialogueElements;


    // Use this for initialization
    void Start()
    {
        UpdateVisuals();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyCode k in keysToContinue)
        {
            if (Input.GetKeyUp(k))
            {
                Next();
            }
        }
    }
    public void Next()
    {
        currentDialogueStep++;

        if (currentDialogueStep >= dialogueElements.Length)
        {
            LoadNextLevel();
        }

        UpdateVisuals();
    }

    private void LoadNextLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevel);

    }

    void UpdateVisuals()
    {

        for (int i = 0; i < dialogueElements.Length; i++)
        {
            if (currentDialogueStep == i)
            {

                DialogueComponent dc = dialogueElements[i];

                Debug.Log("Actual typ: " + ((int)dc.Speaker));

                speakers[(int)dc.Speaker].SetActive(true);
                // Set text:
                Text t = speakers[(int)dc.Speaker].transform.GetComponentInChildren<Text>();
                if (t != null)
                {

                    t.text = dc.text;
                }
            }
            else
            {
                int tmp = (int)dialogueElements[i].Speaker;
                Debug.Log("Deactivating " + tmp);
                if (currentDialogueStep < dialogueElements.Length)
                {
                    if (tmp != ((int)dialogueElements[currentDialogueStep].Speaker))
                    {
                        speakers[tmp].SetActive(false);
                    }
                }
            }
        }
    }
}


[System.Serializable]
public class DialogueComponent
{
    public DialogueManager.PROTAGONISTS Speaker;
    public string text;

}