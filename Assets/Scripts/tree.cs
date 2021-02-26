
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class tree : MonoBehaviour
{
    public GameObject controller;

    GameObject ThisTree;
    BoxCollider ThisCollider;

    public int treeHealth = 100;
    private bool isFallen = false;

    private void Start()
    {
        ThisTree = transform.gameObject;

        // Find gamecontroller in the game
        controller = GameObject.Find("GameController");
        ThisCollider = ThisTree.GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (treeHealth <= 0 && isFallen == false)
        {
            isFallen = true;

            Rigidbody rb = ThisTree.AddComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(Vector3.forward,ForceMode.Impulse);

            // Removes tree from the grid, so area where ThisTree was standing is now walkable
            controller.GetComponent<GameControllerScript>().ChopDownTreeAtPosition(ThisTree.transform.position);

            StartCoroutine(dropTree());
        }
    }

    public void change(Text script)
    {
        treeHealth--;
        script.text = "Health:" + treeHealth;
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
}
