using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mushroom"))
        {
            AudioSource audio = other.GetComponent<AudioSource>();
            MeshRenderer[] mesh = other.gameObject.GetComponentsInChildren<MeshRenderer>();
            ParticleSystem[] Particle = other.gameObject.GetComponentsInChildren<ParticleSystem>();
            audio.Play();
            mesh[0].enabled = false;
            mesh[1].enabled = false;
            Particle[0].Play();
            Particle[1].Play();
            other.enabled = false;
            StartCoroutine(Destroy_shroom(other));

        }
    }

    private IEnumerator Destroy_shroom(Collider other)
    {
        yield return new WaitForSeconds(2);
        Destroy(other.gameObject);
        

    }

}
