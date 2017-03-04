using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DNA_MutationPickup : MonoBehaviour
{

    public enum MUTATION { None, Neutral, Good, Bad, SpikesOn, SpikesOff, DrainOn, DrainOff,EnergyConsumptionOn, EnergyConsumptionOff, UsePhotoSynthesisOn, UsePhotoSynthesisOff, CanUseATPOn, CanUseATPOff, CanSplitOn, CanSplitOff, IsPoisonousOn, IsPoisonousOff, DropLootOn,DropLootOff };


    public MUTATION[] statesAfterMutation;

    public string text = "";
    private Text uiText;


    void Start()
    {
        foreach (Text t in transform.GetComponentsInChildren<Text>())
        {
            t.text = text;
        }
    }

    public void ApplyMutationsTo(Cell targetCell)
    {
        if (statesAfterMutation == null)
            return;

        if (statesAfterMutation.Length < 1)
            return;

        foreach (MUTATION m in statesAfterMutation)
        {
            switch (m)
            {
                case MUTATION.CanUseATPOff:
                    targetCell.canAbsorbATP = false;
                    break;

                case MUTATION.CanUseATPOn:
                    targetCell.canAbsorbATP = true;
                    break;

                case MUTATION.DrainOff:
                    targetCell.canDrain = false;
                    break;

                case MUTATION.DrainOn:
                    targetCell.canDrain = true;
                    break;

                case MUTATION.Good:
                    targetCell.cellType = Cell.CELLTYPE.Good;
                    break;

                case MUTATION.Bad:
                    targetCell.cellType = Cell.CELLTYPE.Bad;
                    break;

                case MUTATION.Neutral:
                    targetCell.cellType = Cell.CELLTYPE.Neutral;
                    break;

                case MUTATION.SpikesOff:
                    targetCell.hasSpikes = false;
                    break;

                case MUTATION.SpikesOn:
                    targetCell.hasSpikes = true;
                    break;

                case MUTATION.UsePhotoSynthesisOff:
                    targetCell.usePhotosynthesis = false;
                    break;

                case MUTATION.UsePhotoSynthesisOn:
                    targetCell.usePhotosynthesis = true;
                    break;

                case MUTATION.EnergyConsumptionOn:
                    targetCell.consumeEnergy = true;
                    break;

                case MUTATION.EnergyConsumptionOff:
                    targetCell.consumeEnergy = false;
                    break;

                case MUTATION.CanSplitOn:
                    targetCell.cellSplits = true;
                    break;

                case MUTATION.CanSplitOff:
                    targetCell.cellSplits = false;
                    break;

                case MUTATION.IsPoisonousOn:
                    targetCell.isPoisonous = true;
                    break;

                case MUTATION.IsPoisonousOff:
                    targetCell.isPoisonous = false;
                    break;


                case MUTATION.DropLootOn:
                    targetCell.dropLoot = true;
                    break;

                case MUTATION.DropLootOff:
                    targetCell.dropLoot = false;
                    break;
            }

            targetCell.Restart();

            Destroy(this.gameObject);
        }

    }
}
