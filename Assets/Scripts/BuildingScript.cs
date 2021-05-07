using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class BuildingScript : MonoBehaviour
{
    public GameObject DepsitColliderObj;
    public GameObject WithdrawObj;

    public Slider progressBar;


    public string What_I_Take;
    public string What_I_Give;

    public int WaitTime;
    public int ConversionRatio;

    private GameControllerScript Controller;
    private int TotalAmountOfDeposited;
    private int TotalAmountOfMadeResource;
    private bool Operating = false;
    private bool FinishedProcessingMaterials = true;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BuildTheBuilding());
    }

    // Update is called once per frame
    void Update()
    {
        if (TotalAmountOfDeposited > 0)
        {
            if (FinishedProcessingMaterials == true)
            {
                transform.GetChild(4).gameObject.SetActive(true);
                FinishedProcessingMaterials = false;
            }
            if (Operating == false)
            {
                Operating = true;
                StartCoroutine(ProcessBarLoader(WaitTime/10, 1, 20)); 
                StartCoroutine(Convert(WaitTime));
            }
        }
        else
        {
            if (FinishedProcessingMaterials == false)
            {
                FinishedProcessingMaterials = true;
                transform.GetChild(4).gameObject.SetActive(false);
            }
        }

    }

    public void ColorProcessBar(int i)
    {
        float coverPercentage = (i*1.0f) /ConversionRatio;
        progressBar.value = coverPercentage; //(coverPercentage);
    }

    public void SetController(GameControllerScript script)
    {
        Controller = script;
    }

    public void Deposit(int aow)
    {
        TotalAmountOfDeposited += aow;
        
    }

    public int Take(int amountToTake = 0)
    {
        int availableAmount = TotalAmountOfMadeResource;
        if (amountToTake == 0)
        {
            TotalAmountOfMadeResource = 0;
            return availableAmount;
        }
        if (TotalAmountOfMadeResource < amountToTake)
        {
            TotalAmountOfMadeResource = 0;
            return availableAmount;
        }
        else
        {
            TotalAmountOfMadeResource -= amountToTake;
            return amountToTake;
        }
    }
    
    private IEnumerator Convert(int waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        TotalAmountOfMadeResource += ConversionRatio;
        TotalAmountOfDeposited--;
        Operating = false;
    }
    
    private IEnumerator ProcessBarLoader(int waitTime, int i, int count)
    {
        yield return new WaitForSeconds(waitTime);
        ColorProcessBar(i);
        if (count > 0)
        {
            StartCoroutine(ProcessBarLoader(waitTime, i + 1, count - 1)); 
        }
    }
    
    private IEnumerator BuildTheBuilding()
    {
        yield return new WaitForSeconds(2);
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(4).gameObject.GetComponent<ParticleSystem>().Play();
        transform.GetChild(4).gameObject.GetComponents<AudioSource>()[1].Play();
        Destroy(transform.GetChild(3).gameObject);
    }

}
