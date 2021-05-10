using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour
{

    public AudioSource audio;
    public AudioSource audiograss;
    

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Rock"))
        {
            audio.Play();
        }
        if (other.collider.CompareTag("Grass"))
        {
            audiograss.Play();
        }
    }
 
}
