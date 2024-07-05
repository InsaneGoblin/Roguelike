using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : Consumable
{
    [SerializeField] private int damage = 20;
    [SerializeField] private int maximumRange = 5;

    public int Damage { get => damage; set => damage = value; }
    public int MaximumRange { get => maximumRange; set => maximumRange = value; }

    public override bool Activate(Actor consumer)
    {
        consumer.GetComponent<Inventory>().SelectedConsumable = this;
        consumer.GetComponent<Player>().ToggleTargetMode();
        UIManager.instance.AddMessage($"Select a target to strike", "light green");
        return false;
    }

    public override bool Cast(Actor consumer, Actor target)
    {
        UIManager.instance.AddMessage($"A lightning bolt strikes the {target.name} with a loud thunder, for {damage} damage!", "white");
        target.GetComponent<Fighter>().HP -= damage;
        Consume(consumer);
        consumer.GetComponent<Player>().ToggleTargetMode();
        return true;
    }
}
