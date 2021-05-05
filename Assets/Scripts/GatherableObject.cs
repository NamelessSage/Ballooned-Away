using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GatherableObject : MonoBehaviour
{
    public GameObject controller;

    GameObject thisObject;
    BoxCollider ThisCollider;

    public int objectHealth = 5;
    private bool isKilled = false;
    public int progress = 0;

    public string resourceName;

    private Canvas thisCanvas;
    private Text thisHealthBar;
    private ParticleSystem Particle;
    private MeshRenderer mesh;
    private AudioSource audio;
    private bool showUI = false;
    private bool wasInteracted = false;

    private void Start()
    {
        thisObject = transform.gameObject;
        thisHealthBar = transform.gameObject.GetComponentInChildren<Text>();
        thisCanvas = transform.gameObject.GetComponentInChildren<Canvas>();
        Particle = transform.gameObject.GetComponentInChildren<ParticleSystem>();
        mesh = transform.gameObject.GetComponent<MeshRenderer>();
        audio = transform.gameObject.GetComponent<AudioSource>();
        thisCanvas.enabled = false;
        // Find gamecontroller in the game
        controller = GameObject.Find("GameController");
        ThisCollider = thisObject.GetComponent<BoxCollider>();
    }


    public void Perform_Chop(int chop_power, int loot_reward)
    {
        objectHealth -= chop_power;
        progress += chop_power;
        UpdateHealth(loot_reward);
    }

    private void UpdateHealth(int loot_reward)
    {
        wasInteracted = true;
        if (objectHealth <= 0 && isKilled == false)
        {
            //Destroy(thisTreeCanvas);
            thisCanvas.enabled = false;
            isKilled = true;

            Rigidbody rb = thisObject.AddComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(Vector3.forward, ForceMode.Impulse);

            // Removes tree from the grid, so area where ThisTree was standing is now walkable
            controller.GetComponent<GameControllerScript>().ChopDownTreeAtPosition(thisObject.transform.position);

            StartCoroutine(DropObject());
        }
        else
        {
            thisHealthBar.text = "Health: " + objectHealth;
            if (showUI == false)
            {
                showUI = true;
                thisCanvas.enabled = true;

                StartCoroutine(TurnOffUI());
            }
        }

        if (progress >= 10)
        {
            progress = progress - 10;
            controller.GetComponent<GameControllerScript>().AddResourceToPlayer(resourceName, loot_reward);
        }
    }

    private IEnumerator DropObject()
    {
        yield return new WaitForSeconds(3);
        ThisCollider.enabled = true;
        StartCoroutine(destroyObject());
        audio.Play();
        Particle.Play();
        mesh.enabled = false;

    }

    private IEnumerator destroyObject()
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(thisObject);

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
            thisCanvas.enabled = false;
            wasInteracted = false;
        }

    }


}
