using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Player
{
    public bool invinclible = false;
    public float invincibilityTime = 0.5f;

    public float Health;
    public float MaxHealth;
    public float attackDamage;
    public float attackCooldown;
    public float attackCooldownTimer;
    public float score;
}
