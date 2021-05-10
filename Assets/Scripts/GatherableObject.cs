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
    public bool isResource = true;

    public string rareDropName;
    public bool rareDropIsItem = false;
    public bool hasRareDrop = false;
    public int rareAmount = 1;

    private Canvas thisCanvas;
    private Text thisHealthBar;
    private Slider slider;
    private ParticleSystem Particle;
    private MeshRenderer mesh;
    [SerializeField]
    private AudioSource audio;
    [SerializeField]
    private AudioSource audio2;
    private bool showUI = false;
    private bool wasInteracted = false;
    

    private void Start()
    {
        thisObject = transform.gameObject;
        thisHealthBar = transform.gameObject.GetComponentInChildren<Text>();
        slider = transform.gameObject.GetComponentInChildren<Slider>();
        thisCanvas = transform.gameObject.GetComponentInChildren<Canvas>();
        Particle = transform.gameObject.GetComponentInChildren<ParticleSystem>();
        mesh = transform.gameObject.GetComponent<MeshRenderer>();
        //audio = transform.gameObject.GetComponent<AudioSource>();
        thisCanvas.enabled = false;
        // Find gamecontroller in the game
        controller = GameObject.Find("GameController");
        ThisCollider = thisObject.GetComponent<BoxCollider>();
        setmaxHealt(objectHealth);
    }


    public void Perform_Chop(int chop_power, int loot_reward)
    {
        audio2.Play();
        objectHealth -= chop_power;
        setHealt(objectHealth);
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

            audio.Play();
            Particle.Play();
            StartCoroutine(DropObject());
            if (hasRareDrop)
            {
                int i = Random.Range(1, 100);
                if (i >= 50)
                {
                    if (!rareDropIsItem) controller.GetComponent<GameControllerScript>().AddResourceToPlayer(rareDropName, 1);
                    else controller.GetComponent<GameControllerScript>().AddItemToPlayer(rareDropName, rareAmount);
                }

            }
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
            if (isResource)
                controller.GetComponent<GameControllerScript>().AddResourceToPlayer(resourceName, loot_reward);
            else
                controller.GetComponent<GameControllerScript>().AddItemToPlayer(resourceName, loot_reward);

        }
        else if (objectHealth == 0)
        {
            if (isResource)
                controller.GetComponent<GameControllerScript>().AddResourceToPlayer(resourceName, loot_reward);
            else
                controller.GetComponent<GameControllerScript>().AddItemToPlayer(resourceName, loot_reward);
        }
    }

    private void setmaxHealt(int healt)
    {
        slider.value = healt;
        slider.maxValue = healt;
    }
    private void setHealt(int healt)
    {
        slider.value = healt;
    }
    private IEnumerator DropObject()
    {
        yield return new WaitForSeconds(3);
        ThisCollider.enabled = true;
        StartCoroutine(destroyObject());
        
        if (mesh != null) mesh.enabled = false;

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
