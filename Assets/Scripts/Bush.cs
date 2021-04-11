using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour
{
    private GameObject game;
    public int force = 100;
    private Animator animator;
    private bool push = false;
    private void Start()
    {
        game = transform.gameObject;
        animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("Bush"))
        {
            if (other.CompareTag("Player"))
            {
                if (push == false)
                {
                    animator.Play("Bushpush");
                    StartCoroutine(Push());
                    
                }
            }
        }
    }

    private IEnumerator Push()
    {
        push = true;
        Collider[] collider = Physics.OverlapSphere(transform.position, 5);
        foreach (Collider temp in collider)
        {
            Rigidbody rb = temp.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force,transform.position,5);
            }
        }
        yield return new WaitForSeconds(2f);
        push = false;
    }
}
