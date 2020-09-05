using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private bool reloading;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!reloading)
        {
            Debug.Log("Hit: " + other.name);

            IDamageable hit = other.GetComponent<IDamageable>();

            if (hit != null)
            {
                hit.Damage();
                reloading = true;
                StartCoroutine(AttackReload());
            }

        }

    }

    IEnumerator AttackReload()
    {
        yield return new WaitForSeconds(.6f);
        reloading = false;
    }
}
