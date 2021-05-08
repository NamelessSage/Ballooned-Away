using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour
{
    private GameObject game;
    public int force = 500;
    private AudioSource audio;
    private Animator animator;
    private Skills skills;
    private bool push = false;
    private void Start()
    {
        game = transform.gameObject;
        animator = GetComponent<Animator>();
        audio = transform.gameObject.GetComponent<AudioSource>();
        
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
                    audio.Play();
                    
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
                skills = rb.GetComponent<Skills>();
                if (skills != null)
                    skills.takeDamage(20);
            }
        }
        yield return new WaitForSeconds(2f);
        push = false;
    }
}
