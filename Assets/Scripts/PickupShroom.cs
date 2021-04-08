using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupShroom : MonoBehaviour
{
    private GameObject shroom;
    private Animator animator;
    private void Start()
    {
        shroom = transform.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("Redshroom") )
        {
            if (other.CompareTag("Player"))
            {
                animator = GetComponent<Animator>();
                animator.Play("Boom");
                StartCoroutine(Explosion());
            }
        }
    }

    public void pickup()
    {
        shroom = transform.gameObject;
        UpdateShroom();
        
    }

    private void UpdateShroom()
    {
        Destroy(shroom);
    }
    private IEnumerator Explosion()
    {
        yield return new WaitForSeconds(0.6f);
        shroom.GetComponent<MeshRenderer>().enabled = false;
        shroom.GetComponentInChildren<ParticleSystem>().Play();
        Collider[] collider = Physics.OverlapSphere(transform.position, 5);
        foreach (Collider temp in collider)
        {
            Rigidbody rb = temp.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(100,transform.position,5);
            }
        }
        StartCoroutine(Destroyitem());
    }
    private IEnumerator Destroyitem()
    {
        yield return new WaitForSeconds(1);
        Destroy(shroom);
    }
}
