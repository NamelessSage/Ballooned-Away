using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BuildingScript : MonoBehaviour
{
    public GameObject DepsitColliderObj;
    public GameObject WithdrawObj;

    public string What_I_Take;
    public string What_I_Give;

    public int WaitTime;
    public int ConversionRatio;

    private GameControllerScript Controller;
    private int TotalAmountOfDeposited;
    private int TotalAmountOfMadeResource;
    private bool Operating = false;
    
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
            if (Operating == false)
            {
                Operating = true;
                StartCoroutine(Convert(WaitTime));
            }
        }
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
    
    private IEnumerator BuildTheBuilding()
    {
        yield return new WaitForSeconds(2);
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(4).gameObject.GetComponent<ParticleSystem>().Play();
        transform.GetChild(4).gameObject.GetComponents<AudioSource>()[1].Play();
        Destroy(transform.GetChild(3).gameObject);
    }

}
