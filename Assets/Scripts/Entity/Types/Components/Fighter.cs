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
            if (hp == 0)
                Die();
        }
    }

    public int Defense { get => defense; }
    public int Power { get => power; }
    public Actor Target { get => target; set => target = value; }

    private void Die ()
    {
        if (GetComponent<Player>())
            Debug.Log("You died!");
        else
            Debug.Log($"{name} is dead!");

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

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
