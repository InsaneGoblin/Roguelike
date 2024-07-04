using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]

public class Consumable : MonoBehaviour
{
    public enum ConsumableType
    {
        Healing,
    }

    [SerializeField] private ConsumableType consumableType;
    [SerializeField] private int amount = 0;

    public ConsumableType _ConsumableType { get { return consumableType; } set { consumableType = value; } }
    public int Amount { get { return amount; } set { amount = value; } }

    public bool Activate(Actor actor, Item item)
    {
        switch (consumableType)
        {
            case ConsumableType.Healing:
                return Healing(actor, item);
            default:
                return false;
        }
    }

    private bool Healing (Actor actor, Item item)
    {
        int amountRecovered = actor.GetComponent<Fighter>().Heal(amount);

        if (amountRecovered > 0)
        {
            UIManager.instance.AddMessage($"You consume the {item.name}, and recover {amountRecovered} HP!", "lime");
            Consume(actor, item);
            return true;
        }
        else
        {
            UIManager.instance.AddMessage($"Your health is already full.", "grey");
            return false;
        }
    }

    private void Consume(Actor actor, Item item)
    {
        actor.Inventory.Items.Remove(item);
        Destroy(item.gameObject);
    }
}
