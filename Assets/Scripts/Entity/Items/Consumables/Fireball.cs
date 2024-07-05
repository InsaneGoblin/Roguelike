using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Consumable
{
    [SerializeField] private int damage = 12;
    [SerializeField] private int radius = 3;

    public int Damage { get => damage; set => damage = value; }
    public int Radius { get => radius; set => radius = value; }

    public override bool Activate(Actor consumer)
    {
        consumer.GetComponent<Inventory>().SelectedConsumable = this;
        consumer.GetComponent<Player>().ToggleTargetMode(true, radius);
        UIManager.instance.AddMessage($"Select a location to throw a Fireball", "light green");
        return false;
    }

    public override bool Cast(Actor consumer, List<Actor> targets)
    {
        foreach (Actor target in targets)
        {
            UIManager.instance.AddMessage($"The {target.name} is engulfed in a fiery explosion, taking {damage} damage!", "red");
            target.GetComponent<Fighter>().HP -= damage;
        }

        Consume(consumer);
        consumer.GetComponent<Player>().ToggleTargetMode();
        return true;
    }
}
