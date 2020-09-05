using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlAnim : MonoBehaviour
{

private Animator anim;

void Start()
{
    anim = GetComponent<Animator>();
}

void Update()
{
    if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
    {
        anim.SetBool("isRun", true);
    }
    else
    {
        anim.SetBool("isRun", false);
    }
    
    }
}
