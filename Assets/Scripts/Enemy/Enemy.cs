using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField]
    protected int health;


    [SerializeField]
    protected int speed;

    [SerializeField]
    protected float playerCheckDistance;
    [SerializeField]
    protected float wallCheckDistance;
    [SerializeField]
    protected float combatCheckDistance;


    protected bool isFacingRight;

    [SerializeField]
    protected Transform pointA, pointB, pointE, playerCheck, wallCheck, groundCheck;

    //public virtual void Attack()
    //{
    //Debug.Log("My name is " + this.gameObject.name);
    //}

    public abstract void Flip();

    public abstract void Update();
}
