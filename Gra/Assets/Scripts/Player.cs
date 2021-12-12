using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Player
{
    public float invincibilityTime = 0.5f;
    public float invincibilityTimer = 0f;

    public float Health;
    public float MaxHealth;
    public float attackDamage;
    public float attackCooldown;
    public float attackCooldownTimer;
    public float score;

    public int MaxHPincreasedCounter = 0;
    public int ASincreasedCounter = 0;
    public int ADincreasedCounter = 0;
}
