
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour
{

    public AudioSource stoneStep;
    public AudioSource grassStep;
    
    

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Rock"))
        {
            PlayStone();
        }
        if (other.collider.CompareTag("Grass"))
        {
            PlayGrass();
        }
    }

    private void PlayGrass()
    {
        if (!grassStep.isPlaying)
        {
            float rn = Random.Range(2f, 2.3f);
            grassStep.pitch = rn;
            grassStep.Play();
        }
    }

    private void PlayStone()
    {
        if (!stoneStep.isPlaying)
        {
            float rn = Random.Range(2f, 2.3f);
            stoneStep.pitch = rn;
            stoneStep.Play();
        }
    }
 
}
