using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mushroom"))
        {
            MeshRenderer mesh = other.gameObject.GetComponent<MeshRenderer>();
            ParticleSystem Particle = other.gameObject.GetComponentInChildren<ParticleSystem>();
            Particle.Play();
            mesh.enabled = false;
            other.enabled = false;
            StartCoroutine(Destroy_shroom(other));

        }
    }

    private IEnumerator Destroy_shroom(Collider other)
    {
        yield return new WaitForSeconds(2.5f);
        Destroy(other.gameObject);
        

    }

}
