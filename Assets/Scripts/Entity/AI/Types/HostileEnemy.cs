using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Fighter))]
public class HostileEnemy : AI
{
    [SerializeField] private Fighter fighter;
    [SerializeField] private Actor actor;
    [SerializeField] private bool isFighting;
    [SerializeField] private float attackRange;

    public float AttackRange { get { return attackRange; } set { attackRange = value; } }

    private void OnValidate()
    {
        fighter = GetComponent<Fighter>();
        actor = GetComponent<Actor>();
        AStar = GetComponent<AStar>();
    }

    public void RunAI()
    {
        if (!fighter.Target)
            fighter.Target = GameManager.instance.Actors[0];
        else if (fighter.Target && !fighter.Target.IsAlive)
            fighter.Target = null;

        Vector3Int targetPosition = MapManager.instance.FloorMap.WorldToCell(fighter.Target.transform.position);
        float targetDistance = Vector3.Distance(transform.position, fighter.Target.transform.position);

        if (fighter.Target && targetDistance < actor.FieldOfViewRange)
        {

            if (!isFighting) isFighting = true;
            
            //Debug.Log("Found target " + fighter.Target + " at distance: " + targetDistance);

            if (targetDistance <= attackRange) // is in attack range?
            {
                Action.MeleeAction(GetComponent<Actor>(), fighter.Target);
                return;
            }
            else // if not, move to range
            {
                MoveAlongPath(targetPosition);
                return;
            }
        }

        Action.SkipAction();
    }
}
