using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Block
{
    //name of block for debug
    public string name;
    //block theme
    public string type;
    //Block can be passed horizontally - doorway
    public bool allowHorizontalMovement;
    //Block can be passed vertically - platform
    public bool allowVerticalMovement;

    public bool canBeUsedAsFloor;
    public bool canBeUsedAsWall;

    public bool interactable;

    public string utility;

    public Sprite sprite;
    public BoxCollider2D collider;
}
