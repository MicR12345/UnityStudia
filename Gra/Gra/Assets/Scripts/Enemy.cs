using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Enemy
{
    public string name;
    public float health;
    public float armor;
    public float contactDamage;
    public float attackDamage;
    public float scoreValue;
    public bool canDropItems;
    public List<string> tags;

    public float invincibilityTime = 1f;
    public float invincibilityTimer;

    public Sprite sprite;
    public BoxCollider2D collider;

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}
