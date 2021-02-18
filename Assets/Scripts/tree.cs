
using System;
using System.Collections;
using UnityEngine;
public class tree : MonoBehaviour
{
    GameObject ThisTree;
    public int treeHealth = 5;
    private bool isFallen = false;

    private void Start()
    {
        ThisTree = transform.gameObject;
    }

    private void Update()
    {
        if (treeHealth <= 0 && isFallen == false)
        {
            Rigidbody rb = ThisTree.AddComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(Vector3.forward,ForceMode.Impulse);
            StartCoroutine(destroyTree());
            isFallen = true;
        }
    }



    private IEnumerator destroyTree()
    {
        yield return new WaitForSeconds(60);
        Destroy(ThisTree);
    }
}
