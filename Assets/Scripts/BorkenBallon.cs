using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class BorkenBallon : MonoBehaviour
{
    //-------------------------------------------------------
    public GameObject model;
    public GameObject ballon;

    public AudioSource soundEffect;
    public ParticleSystem particles;

    private PlayerGuiController player_UI;
    private Inventory PlrInventory;

    //-------------------------------------------------------

    private int WaitTime = 5;
    private float PassedTime = 0;

    private bool arrived = false;

    void Start()
    {
        PassedTime = WaitTime;
    }

    void Update()
    {
        PassedTime = PassedTime - Time.deltaTime;
        if (PassedTime <= 0)
        {

            PassedTime = WaitTime;

        }
    }

    public void ArriveBallon()
    {
        Destroy(model);
        particles.Play();
        soundEffect.Play();
        ballon.SetActive(true);
        Destroy(gameObject.GetComponent<Collider>());
    }

    public void SetPlayerUI(PlayerGuiController gui)
    {
        player_UI = gui;
    }

    public void SetPlayerInventory(Inventory inv)
    {
        PlrInventory = inv;
    }



}
