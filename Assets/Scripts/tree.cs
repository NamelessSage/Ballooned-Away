
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class tree : MonoBehaviour
{
    public GameObject controller;

    GameObject ThisTree;
    BoxCollider ThisCollider;

    public int treeHealth = 5;
    private bool isFallen = false;

    private Canvas thisTreeCanvas;
    private Text thisTreeHealthbar;
    private bool showUI = false;
    private bool wasInteracted = false;

    private void Start()
    {
        ThisTree = transform.gameObject;
        thisTreeHealthbar = transform.gameObject.GetComponentInChildren<Text>();
        thisTreeCanvas = transform.gameObject.GetComponentInChildren<Canvas>();
        thisTreeCanvas.enabled = false;

        // Find gamecontroller in the game
        controller = GameObject.Find("GameController");
        ThisCollider = ThisTree.GetComponent<BoxCollider>();
    }


    public void Perform_Chop()
    {
        treeHealth--;
        UpdateTree();

    }

    private void UpdateTree()
    {
        wasInteracted = true;
        if (treeHealth <= 0 && isFallen == false)
        {
            Destroy(thisTreeCanvas);
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
    }

    private IEnumerator dropTree()
    {
        yield return new WaitForSeconds(3);
        ThisCollider.enabled = false;

        StartCoroutine(destroyTree());
    }

    private IEnumerator destroyTree()
    {
        yield return new WaitForSeconds(1);
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
