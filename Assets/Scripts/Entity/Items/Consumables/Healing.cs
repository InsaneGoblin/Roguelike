using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Consumable;
using static UnityEditor.Progress;

public class Healing : Consumable
{

    [SerializeField] private int amount = 0;
    public int Amount { get { return amount; } set { amount = value; } }

    public override bool Activate(Actor consumer)
    {
        int amountRecovered = consumer.GetComponent<Fighter>().Heal(amount);

        if (amountRecovered > 0)
        {
            UIManager.instance.AddMessage($"You consume the {name}, and recover {amountRecovered} HP!", "lime");
            Consume(consumer);
            return true;
        }
        else
        {
            UIManager.instance.AddMessage($"Your health is already full.", "grey");
            return false;
        }
        
    }

}
