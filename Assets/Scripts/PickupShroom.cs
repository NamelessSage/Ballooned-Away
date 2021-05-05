using System.Collections;
using UnityEngine;
// ReSharper disable All

public class PickupShroom : MonoBehaviour
{
    private GameObject shroom;
    private Animator animator;
    private MeshRenderer mesh;
    private ParticleSystem particle;
    private AudioSource audio;
    private Collider coll;
    public int force = 100;

    private void Start()
    {
        shroom = transform.gameObject;
        mesh = shroom.GetComponent<MeshRenderer>();
        particle = shroom.GetComponentInChildren<ParticleSystem>();
        audio = shroom.GetComponent<AudioSource>();
        coll = shroom.GetComponent<Collider>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("Redshroom"))
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
        UpdateShroom();
        
    }

    private void UpdateShroom()
    {
        Destroy(shroom);
    }
    private IEnumerator Explosion()
    {
        yield return new WaitForSeconds(0.6f);
        mesh.enabled = false;
        coll.enabled = false;
        particle.Play();
        audio.Play();
        Collider[] collider = Physics.OverlapSphere(transform.position, 5);
        foreach (Collider temp in collider)
        {
            Rigidbody rb = temp.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force,transform.position,5);
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
