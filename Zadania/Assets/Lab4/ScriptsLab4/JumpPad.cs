using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    MoveWithCharacterController MvCtrl;
    private void Start()
    {
        MvCtrl = GetComponent<MoveWithCharacterController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "jumppad")
        {
            MvCtrl.playerVelocity.y += Mathf.Sqrt(MvCtrl.jumpHeight * -3.0f * MvCtrl.gravityValue) * 3f;
        }
    }
}
