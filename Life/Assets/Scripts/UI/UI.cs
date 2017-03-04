using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UI : MonoBehaviour
{

    public Text neutral;
    public Text good;
    public Text bad;

    public Text state;
    public Text goal;

    public Text ATP;
    public GameObject losePanel;



    // Use this for initialization
    void Awake()
    {
        Init();
        if (losePanel)
        {
            losePanel.SetActive(false);
        }
    }

    private void Init()
    {
        if (!neutral)
        {
            GameObject g = GameObject.Find("Text_Neutral");
            if (g)
                neutral = g.GetComponent<Text>();
        }

        if (!good)
        {
            GameObject g = GameObject.Find("Text_Good");
            if (g)
                good = g.GetComponent<Text>();
        }

        if (!bad)
        {
            GameObject g = GameObject.Find("Text_Bad");
            if (g)
                bad = g.GetComponent<Text>();
        }

        if (!goal)
        {
            GameObject g = GameObject.Find("Text_Goal");
            if (g)
                goal = g.GetComponent<Text>();
        }

        if (!ATP)
        {
            GameObject g = GameObject.Find("Text_Energie");
            if (g)
                ATP = g.GetComponent<Text>();
        }

        if (!losePanel)
        {
            losePanel = GameObject.Find("Panel_Lose");
        }

       
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCounts();
        UpdateATPCounter();

        if (state)
            state.text = GoalManager.gameState.ToString();
    }

    private void UpdateATPCounter()
    {
        if (!ATP)
            return;

        string s = "Energie \n [";
        for (int i = 0; i < Player.atp; i++)
        {
            s += "|";
        }
        for (int i = Player.atp; i < Player.maxATP; i++)
        {
            s += ".";
        }
        s += "]";
        ATP.text = s;
    }

    public void UpdateGoal(String goalText)
    {
        goal.text = goalText;
    }


    private void UpdateCounts()
    {
        bool hasToReInit = false;

        if (neutral)
            neutral.text = "Neutral: " + Cell.NeutralCellsCount;
        if (good)
            good.text = "Gut: " + Cell.GoodCellsCount;
        else
            hasToReInit = true;
        if (bad)
            bad.text = "Böse: " + Cell.BadCellsCount;
        else
            hasToReInit = true;

        if (hasToReInit)
            Init();
    }
}
