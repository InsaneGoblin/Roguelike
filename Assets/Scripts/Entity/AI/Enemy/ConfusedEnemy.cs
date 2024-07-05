using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves randomly. If there's an Actor where it wants to go to, this attacks that Actor

[RequireComponent(typeof(Actor))]
public class ConfusedEnemy : AI
{
    [SerializeField] private AI previousAI;
    [SerializeField] private int turnsRemaining;

    public AI PreviousAI { get { return previousAI; } set { previousAI = value; } }
    public int TurnsRemaining { get { return turnsRemaining; } set { turnsRemaining = value; } }

    public override void RunAI()
    {
        if (turnsRemaining <= 0)
        {
            UIManager.instance.AddMessage($"The {gameObject.name} is no longer confused.", "red");
            GetComponent<Actor>().AI = previousAI;
            GetComponent<Actor>().AI.RunAI();
            Destroy(this);
        }
        else
        {
            Vector2Int direction = Random.Range(0, 8) switch
            {
                0 => new Vector2Int(0, 1),
                1 => new Vector2Int(0, -1),
                2 => new Vector2Int(1, 0),
                3 => new Vector2Int(-1, 0),
                4 => new Vector2Int(1, 1),
                5 => new Vector2Int(1, -1),
                6 => new Vector2Int(-1, 1),
                7 => new Vector2Int(-1, -1),
                _ => new Vector2Int(0, 0)
            };

            Action.BumpAction(GetComponent<Actor>(), direction);
            turnsRemaining--;
        }
    }
}
