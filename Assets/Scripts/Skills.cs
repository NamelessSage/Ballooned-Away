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
    public int skillPoints = 100;
    public int totalChop = 0;
    private int _currentChop = 0;
    private bool _skillTreeActive;
    public int loot_reward;
    public int chop_power;
    public int maxhealth = 100;
    public int currentHealth = 100;
    public Text scoretext;
    private int score = 0;
    public Text pointText;
    private Text movePointsText;
    private Text powerPointsText;
    private Text fortunePointsText;
    private Text healthPointsText;
    
    private int movepoints = 0;
    private int powerpoints = 0;
    private int fortunepoints = 0;
    private int healthpoints = 0;
    
    private float TimeAlive = 0;

    void Update()
    {
        TimeAlive += Time.deltaTime;
    }

    public float[] GetStats()
    {
        float [] a = {score, TimeAlive};
        return a;
    }

    public void SetPlayer(WorldInteractorTool p)
    {
        _player = p;
        loot_reward = _player.loot_reward;
        chop_power = _player.chop_power;
        maxhealth = (int)healthSlider.maxValue;
        currentHealth = (int) healthSlider.value;
        pointText.text = "Skill points left: " + skillPoints;
        
        movePointsText = GameObject.Find("MovementText").GetComponent<Text>();
        movePointsText.text = "Movement Speed: \n" + movepoints + "/5";
        
        powerPointsText = GameObject.Find("PowerText").GetComponent<Text>();
        powerPointsText.text = "Gathering power: \n" + powerpoints + "/5";
        
        fortunePointsText = GameObject.Find("FortuneText").GetComponent<Text>();
        fortunePointsText.text = "Fortune: \n" + fortunepoints + "/5";
        
        healthPointsText = GameObject.Find("HealthText").GetComponent<Text>();
        healthPointsText.text = "Health: \n" + healthpoints + "/20";
    }

    public void AddDistance(float distance)
    {
        totalDistance += distance;
        _currentDistance += distance;
        if (_currentDistance > 10)
        {
            skillPoints += 1;
            pointText.text = "Skill points left: " + skillPoints;
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
            pointText.text = "Skill points left: " + skillPoints;
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
            if (flag == 0  && movepoints < 5)
            {
                IncreaseMoveSpeed();
                skillPoints--;
                pointText.text = "Skill points left: " + skillPoints;
            }
            
            else if (flag==1  && powerpoints < 5)
            {
                IncreaseChopPower();
                skillPoints--;
                pointText.text = "Skill points left: " + skillPoints;
            }
            
            else if (flag==2  && fortunepoints < 5)
            {
                IncreaseResourceReward();
                skillPoints--;
                pointText.text = "Skill points left: " + skillPoints;
            }
            
            else if (flag==3  && healthpoints < 20)
            {
                int health = (int) healthSlider.maxValue + 5;
                SetMaxHealth(health);
                skillPoints--;
                pointText.text = "Skill points left: " + skillPoints;
            }
        }

    }

    private void IncreaseResourceReward()
    {
        _player.loot_reward += 1;
        loot_reward += 1;
        fortunepoints += 1;
        fortunePointsText.text = "Fortune: \n" + fortunepoints + "/5";

    }

    private void IncreaseMoveSpeed()
    {
        _player.player_Speed = (float) Math.Round(_player.player_Speed + 0.5f, 1);
        movepoints += 1;
        movePointsText.text = "Movement Speed: \n" + movepoints + "/5";

    }

    private void IncreaseChopPower()
    {
        _player.chop_power += 1;
        chop_power += 1;
        powerpoints += 1;
        powerPointsText.text = "Gathering power: \n" + powerpoints + "/5";

    }
    
    public void SetHealth(int health)
    {
        healthSlider.value = health;
    }
    public void SetMaxHealth(int health)
    {
        healthSlider.maxValue = health;
        maxhealth = health;
        healthpoints += 1;
        healthPointsText.text = "Health: \n" + healthpoints + "/20";

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

    public void addScore(int points)
    {
        score += points;
        scoretext.text = "Score: " + score;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Projectile"))
        {
            takeDamage(25);
        }
    }
}