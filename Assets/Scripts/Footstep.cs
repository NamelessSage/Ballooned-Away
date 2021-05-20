
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour
{

    public AudioSource stoneStep;
    public AudioSource grassStep;
    public AudioSource sandStep;
    public AudioSource plantStep;




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
        if (other.collider.CompareTag("Sand"))
        { 
            PlaySand();
        }
        if (other.collider.CompareTag("GenericGrass"))
        {
            PlayPlant();
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
    private void PlaySand()
    {
        if (!stoneStep.isPlaying)
        {
            float rn = Random.Range(2f, 2.3f);
            stoneStep.pitch = rn;
            sandStep.Play();
        }
    }
    private void PlayPlant()
    {
        if (!plantStep.isPlaying)
        {
            float rn = Random.Range(1f, 2.3f);
            plantStep.pitch = rn;
            plantStep.Play();
        }
    }

}
