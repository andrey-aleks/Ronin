using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    protected Vector3 startAnimPoint;
    protected Vector3 endAnimPoint;
    protected float animStep = 0.05f;
    protected Vector3 currentAnimTarget;
    protected float _animSpeed = 0.002f;

    protected virtual void Start()
    {
        startAnimPoint = transform.position;
        endAnimPoint = startAnimPoint;
        endAnimPoint.y = startAnimPoint.y + animStep;
    }

    protected virtual void Update()
    {
        if (transform.position == startAnimPoint)
        {
            currentAnimTarget = endAnimPoint;
        }
        else if (transform.position == endAnimPoint)
        {
            currentAnimTarget = startAnimPoint;
        }
        transform.position = Vector3.MoveTowards(transform.position, currentAnimTarget, _animSpeed);
    }
}
