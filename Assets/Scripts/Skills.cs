using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Skills : MonoBehaviour
{
    private WorldInteractorTool player;
    public float totalDistance = 0f;
    private float currentDistance = 0f;
    public int skillPoints = 0;
    public int totalChop = 0;
    private int currentChop = 0;

    public void SetPlayer(WorldInteractorTool p)
    {
        player = p;
    }

    public void AddDistance(float distance)
    {
        totalDistance += distance;
        currentDistance += distance;
        if (currentDistance > 10)
        {
            skillPoints += 1;
            currentDistance -= 10;
            if (player.player_Speed < 4)
            {
                CheckSkillPoints(0);
            }
        }
    }
    
    public void AddChop()
    {
        currentChop += 1;
        totalChop += 1;
        if (currentChop>10)
        {
            skillPoints += 1;
            currentChop = 0;
            if (player.chop_power < 5)
            {
                CheckSkillPoints(1);
                CheckSkillPoints(2);
            }
        }
    }
    
/// <summary>
/// flag 0 - distance
/// flag 1 - chop
/// </summary>
/// <param name="flag"></param>
    private void CheckSkillPoints(int flag)
    {
        if (flag == 0 && skillPoints > 0)
        {
            IncreaseMoveSpeed();
        }

        if (flag==1 && skillPoints > 0)
        {
            IncreaseChopPower();
        }
        
        if (flag==2 && skillPoints > 0)
        {
            IncreaseResourceReward();
        }
    }

    private void IncreaseResourceReward()
    {
        player.loot_reward += 1;
    }

    private void IncreaseMoveSpeed()
    {
        player.player_Speed = (float) Math.Round(player.player_Speed + 0.5f, 1);
    }

    private void IncreaseChopPower()
    {
        player.chop_power += 1;
    }
}