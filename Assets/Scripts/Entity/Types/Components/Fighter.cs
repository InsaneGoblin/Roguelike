using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
sealed class Fighter : MonoBehaviour
{
    [SerializeField] private int maxHP, hp, defense, power;
    [SerializeField] private Actor target;

    public int HP
    { 
        get => hp;
        
        set
        {
            hp = Mathf.Max(0, Mathf.Min(value, maxHP));

            if (GetComponent<Player>())
                UIManager.instance.SetHealth(hp, maxHP);

            if (hp <= maxHP / 4 && hp > 0 && GetComponent<Player>())
                UIManager.instance.AddMessage($"You're badly hurt!", "red");

            if (hp <= maxHP / 4 && maxHP > 15 && hp > 0 && !GetComponent<Player>())
                UIManager.instance.AddMessage($"The {name} is badly hurt!", "orange");

            if (hp == 0)
                Die();
        }
    }

    public int Defense { get => defense; }
    public int Power { get => power; }
    public Actor Target { get => target; set => target = value; }

    private void Start ()
    {
        if (GetComponent<Player>())
        {
            UIManager.instance.SetHealthMax(maxHP);
            UIManager.instance.SetHealth(hp, maxHP);
        }
    }
    private void Die ()
    {
        if (GetComponent<Player>())
            UIManager.instance.AddMessage("You died!", "dark red");
        else
            UIManager.instance.AddMessage($"{name} is dead!", "orange");

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = GameManager.instance.DeadSprite;
        spriteRenderer.color = new Color(191, 0, 0, 1);
        spriteRenderer.sortingOrder = 0;

        name = $"Remains of {name}";
        GetComponent<Actor>().BlocksMovement = false;
        GetComponent<Actor>().IsAlive = false;

        if (!GetComponent <Player>())
            GameManager.instance.RemoveActor(this.GetComponent<Actor>());
    }

    public int Heal(int amount)
    {
        if (hp == maxHP)
            return 0;

        int newHPValue = hp + amount;

        if (newHPValue > maxHP)
            newHPValue = maxHP;

        int amountRecovered = newHPValue - hp;
        HP = newHPValue;
        return amountRecovered;
    }
}
