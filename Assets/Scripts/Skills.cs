using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Skills : MonoBehaviour
{
    private WorldInteractorTool _player;
    public float totalDistance = 0f;
    private float _currentDistance = 0f;
    public int skillPoints = 0;
    public int totalChop = 0;
    private int _currentChop = 0;
    private bool _skillTreeActive;
    public int loot_reward;
    public int chop_power;
    
    public void SetPlayer(WorldInteractorTool p)
    {
        _player = p;
        loot_reward = _player.loot_reward;
        chop_power = _player.chop_power;
    }

    public void AddDistance(float distance)
    {
        totalDistance += distance;
        _currentDistance += distance;
        if (_currentDistance > 10)
        {
            skillPoints += 1;
            _currentDistance -= 10;
        }
    }
    
    public void AddChop()
    {
        _currentChop += 1;
        totalChop += 1;
        if (_currentChop>10)
        {
            skillPoints += 1;
            _currentChop = 0;
        }
    }
    
/// <summary>
/// flag 0 - distance
/// flag 1 - chop
/// </summary>
/// <param name="flag"></param>
    public void CheckSkillPoints(int flag)
    {
        if (flag == 0 && skillPoints > 0 && _player.player_Speed < 5)
        {
            IncreaseMoveSpeed();
            skillPoints--;
        }

        if (flag==1 && skillPoints > 0 && _player.chop_power < 5)
        {
            IncreaseChopPower();
            skillPoints--;
        }
        
        if (flag==2 && skillPoints > 0 && _player.loot_reward < 5)
        {
            IncreaseResourceReward();
            skillPoints--;
        }
    }

    private void IncreaseResourceReward()
    {
        _player.loot_reward += 1;
        loot_reward += 1;
    }

    private void IncreaseMoveSpeed()
    {
        _player.player_Speed = (float) Math.Round(_player.player_Speed + 0.5f, 1);
    }

    private void IncreaseChopPower()
    {
        _player.chop_power += 1;
        chop_power += 1;
    }
}