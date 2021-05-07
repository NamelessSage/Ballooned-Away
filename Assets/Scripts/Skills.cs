using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Skills : MonoBehaviour
{
    private WorldInteractorTool _player;
    public Slider healthSlider;
    public float totalDistance = 0f;
    private float _currentDistance = 0f;
    public int skillPoints = 0;
    public int totalChop = 0;
    private int _currentChop = 0;
    private bool _skillTreeActive;
    public int loot_reward;
    public int chop_power;
    public int maxhealth;
    public int currentHealth;
    
    public void SetPlayer(WorldInteractorTool p)
    {
        _player = p;
        loot_reward = _player.loot_reward;
        chop_power = _player.chop_power;
        maxhealth = (int)healthSlider.maxValue;
        currentHealth = (int) healthSlider.value;
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
        if (skillPoints > 0)
        {
            if (flag == 0  && _player.player_Speed < 5)
            {
                IncreaseMoveSpeed();
                skillPoints--;
            }
            
            else if (flag==1  && _player.chop_power < 5)
            {
                IncreaseChopPower();
                skillPoints--;
            }
            
            else if (flag==2  && _player.loot_reward < 5)
            {
                IncreaseResourceReward();
                skillPoints--;
            }
            
            else if (flag==3  && healthSlider.maxValue < 200)
            {
                int health = (int) healthSlider.maxValue + 5;
                SetMaxHealth(health);
                skillPoints--;
            }
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
        SetMaxHealth(5);
    }
    
    public void SetHealth(int health)
    {
        healthSlider.value = health;
    }
    public void SetMaxHealth(int health)
    {
        healthSlider.maxValue = health;
        maxhealth = health;
    }

    public void takeDamage(int damage)
    {
        currentHealth -= damage;
        SetHealth(currentHealth);
    }

    public void heal(int health)
    {
        if (currentHealth + health > maxhealth)
        {
            currentHealth = maxhealth;
        }
        else
        {
            currentHealth += health;
        }
        SetHealth(currentHealth);
    }
}