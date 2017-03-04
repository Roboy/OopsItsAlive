using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;


public class ConditionedTriggerPlayerData : MonoBehaviour
{


    public enum AllONE { All, One }

    public bool applyOnStart = false;
    public bool destroyOnTrigger = false;
    public bool multipleTrigger = false;

    [Header("Time between checks")]
    public float updateRate = 3;

    [Header("What should happen?")]
    public UnityEvent OnConditionMatch;


    [Header("== Conditions ==")]
    public int neededATP = 0;
    public Condition.EQUALMINMAX mode = Condition.EQUALMINMAX.GreaterEqual;

    void Start()
    {
        StartCoroutine(SlowUpdate());
        if (applyOnStart)
        {
            TriggerEvent();
        }
    }

    IEnumerator SlowUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateRate);

            if (GoalManager.gameState != GoalManager.STATUS.NONE)
                break;

            CheckConditions();

        }
    }

    private void CheckConditions()
    {
        if ((Player.atp >= neededATP && mode == Condition.EQUALMINMAX.GreaterEqual)
            || (Player.atp < neededATP && mode == Condition.EQUALMINMAX.Less)
            || (Player.atp == neededATP && mode == Condition.EQUALMINMAX.Exact))
        {
            TriggerEvent();
        }
    }

    private void TriggerEvent()
    {
        // Stop the check coroutine.
        if (!multipleTrigger) StopAllCoroutines();

        // Trigger Event
        if (OnConditionMatch != null)
        {
            OnConditionMatch.Invoke();
        }

        if (destroyOnTrigger)
        {
            Destroy(this);
        }
    }

}
