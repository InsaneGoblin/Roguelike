using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Confusion : Consumable
{
    [SerializeField] private int numberOfTurns = 5;

    public int NumberOfTurns { get => numberOfTurns; set => numberOfTurns = value; }

    public override bool Activate(Actor consumer)
    {
        consumer.GetComponent<Inventory>().SelectedConsumable = this;
        consumer.GetComponent<Player>().ToggleTargetMode();
        UIManager.instance.AddMessage($"Select a target to confuse.", "light green");
        return false;
    }

    public override bool Cast(Actor consumer, Actor target)
    {
        if (target.TryGetComponent(out ConfusedEnemy confusedEnemy) )
        {
            if (confusedEnemy.TurnsRemaining > 0)
            {
                UIManager.instance.AddMessage($"The {target.name} is already confused", "red");
                consumer.GetComponent<Inventory>().SelectedConsumable = null;
                consumer.GetComponent<Player>().ToggleTargetMode();
                return false;
            }
        }
        else
        { 
            confusedEnemy = target.gameObject.AddComponent<ConfusedEnemy>(); 
        }

        confusedEnemy.PreviousAI = target.AI;
        confusedEnemy.TurnsRemaining = NumberOfTurns;

        UIManager.instance.AddMessage($"The eyes of the {target.name} look vacant, as it starts to stumble around!", "red");
        target.AI = confusedEnemy;
        Consume(consumer);
        consumer.GetComponent<Player>().ToggleTargetMode();
        return true;

    }
}
