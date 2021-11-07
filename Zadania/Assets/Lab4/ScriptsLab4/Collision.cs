using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.collider.tag == "Obstacle")
        {
            Debug.Log("Zderzenie z przeszkoda");
        }
    }
}
