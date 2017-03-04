
using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class ConditionedTrigger : MonoBehaviour
{

    public enum AllONE { All, One }

    public bool applyOnStart = false;
    public bool destroyOnTrigger = false;

    [Header("Time between checks")]
    public float updateRate = 3;
    [Header("Delay between match and event")]
    public float delay = 1f;

    [Header("Number of conditions required")]
    public AllONE conditionsRequired = AllONE.All;


    [Header("What should happen?")]
    public UnityEvent OnConditionMatch;

    [Header("== Conditions ==")]
    public Condition[] conditions;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(SlowUpdate());
        if(applyOnStart)
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

        if (conditions.Length < 1)
            return;

        foreach (Condition wc in conditions)
        {
            if (wc.neededAmount < 0)
                continue;
            if (wc.genes.Length < 1)
                continue;

            // If the condition isn't fullfilled => no win!
            if (!CheckCondition(wc))
            {
                if (conditionsRequired == AllONE.All)
                {
                    return;
                }
            }
            else // This condition was matched
            {
                if (conditionsRequired == AllONE.One)
                {
                    // One codition matched => trigger event;
                    TriggerEvent();
                    return;
                }
            }
        }

        TriggerEvent();
    }

    private void TriggerEvent()
    {
        // You won! Stop the check coroutine.
        StopAllCoroutines();

        // Trigger Event
        if (OnConditionMatch != null)
        {
            OnConditionMatch.Invoke();
        }

        if(destroyOnTrigger)
        {
            Destroy(this);
        }
    }

    private bool CheckCondition(Condition cond)
    {
        // Check each cell if it has all requirements for the condition and add it to the count
        int count = 0;

        foreach (Cell cell in Cell.AllCells)
        {
            bool hasAllRequiredGenes = true;

            foreach (DNA_MutationPickup.MUTATION gene in cond.genes)
            {
                if (!cell.HasGene(gene))
                {
                    hasAllRequiredGenes = false;
                    break;
                }
            }

            if (hasAllRequiredGenes)
                count++;
        }

        return AmountAccordingToNeeded(count, cond);
    }

    /// <summary>
    /// Check if the counted corresponds to the required amount
    /// </summary>
    private bool AmountAccordingToNeeded(int count, Condition wc)
    {
        if (count == wc.neededAmount && wc.mode == Condition.EQUALMINMAX.Exact)
            return true;

        if (count >= wc.neededAmount && wc.mode == Condition.EQUALMINMAX.GreaterEqual)
            return true;

        if (count < wc.neededAmount && wc.mode == Condition.EQUALMINMAX.Less)
            return true;

        return false;
    }
}


[System.Serializable]
public class Condition : System.Object
{
    public enum EQUALMINMAX { Exact, GreaterEqual, Less };

    public int neededAmount = -1;
    public EQUALMINMAX mode = EQUALMINMAX.GreaterEqual;
    public DNA_MutationPickup.MUTATION[] genes = new DNA_MutationPickup.MUTATION[] { DNA_MutationPickup.MUTATION.None };

}