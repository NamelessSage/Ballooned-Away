using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI : MonoBehaviour
{
    public Text wood_txt;
    public Text rock_txt;
    // Start is called before the first frame update
    void Start()
    {
        wood_txt.text = "Wood: 0";
        rock_txt.text = "Rock: 0";
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void printAmountOfWood(int amountOfWood) 
    {
        wood_txt.text = "Wood: " + amountOfWood;
    }

    public void printAmountOfRock(int amountOfRock)
    {
        rock_txt.text = "Rock: " + amountOfRock;
    }
 
}
