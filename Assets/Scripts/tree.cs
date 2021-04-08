
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class tree : MonoBehaviour
{
    public GameObject controller;

    GameObject ThisTree;
    
    BoxCollider ThisCollider;

    public int treeHealth = 5;
    private bool isFallen = false;
    public int progress = 0;

    private Canvas thisTreeCanvas;
    private Text thisTreeHealthbar;
    private ParticleSystem Particle;
    private MeshRenderer mesh;
    private AudioSource audio;
    private bool showUI = false;
    private bool wasInteracted = false;

    private void Start()
    {
        ThisTree = transform.gameObject;
        thisTreeHealthbar = transform.gameObject.GetComponentInChildren<Text>();
        thisTreeCanvas = transform.gameObject.GetComponentInChildren<Canvas>();
        Particle = transform.gameObject.GetComponentInChildren<ParticleSystem>();
        mesh = transform.gameObject.GetComponent<MeshRenderer>();
        audio = transform.gameObject.GetComponent<AudioSource>();
        thisTreeCanvas.enabled = false;
        // Find gamecontroller in the game
        controller = GameObject.Find("GameController");
        ThisCollider = ThisTree.GetComponent<BoxCollider>();
    }


    public void Perform_Chop()
    {
        treeHealth--;
        progress++;
        UpdateTree();

    }

    private void UpdateTree()
    {
        wasInteracted = true;
        if (treeHealth <= 0 && isFallen == false)
        {
            //Destroy(thisTreeCanvas);
            thisTreeCanvas.enabled = false;
            isFallen = true;

            Rigidbody rb = ThisTree.AddComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(Vector3.forward, ForceMode.Impulse);

            // Removes tree from the grid, so area where ThisTree was standing is now walkable
            controller.GetComponent<GameControllerScript>().ChopDownTreeAtPosition(ThisTree.transform.position);

            StartCoroutine(dropTree());
        }
        else
        {
            thisTreeHealthbar.text = "Health: " + treeHealth;
            if (showUI == false)
            {
                showUI = true;
                thisTreeCanvas.enabled = true;

                StartCoroutine(TurnOffUI());
            }
        }

        if (progress >= 10)
        {
            progress = progress - 10;
            controller.GetComponent<GameControllerScript>().UpdateWoodAmount();
        }
    }

    private IEnumerator dropTree()
    {
        yield return new WaitForSeconds(3);
        ThisCollider.enabled = true;
        StartCoroutine(destroyTree());
        audio.Play();
        Particle.Play();
        mesh.enabled = false;
        
    }

    private IEnumerator destroyTree( )
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(ThisTree);

    }



    private IEnumerator TurnOffUI()
    {
        yield return new WaitForSeconds(1);
        
        if (wasInteracted == true)
        {
            wasInteracted = false;
            StartCoroutine(TurnOffUI());
        }
        else
        {
            showUI = false;
            thisTreeCanvas.enabled = false;
            wasInteracted = false;
        }

    }


}
