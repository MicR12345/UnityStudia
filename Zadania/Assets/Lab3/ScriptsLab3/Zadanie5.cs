using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zadanie5 : MonoBehaviour
{
    public GameObject toBePlacedObject;
    int counter = 0;
    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < 10; i++)
        {
            while (!AttemptGenerate(5f, 5f)) ;
        }
    }
    bool AttemptGenerate(float rangeX,float rangeZ)
    {
        Vector3 newPosition = new Vector3(Random.Range(-rangeX, rangeX),1.5f, Random.Range(-rangeZ, rangeZ));
        newPosition = newPosition + this.gameObject.transform.position;
        if (IsThereSomething(newPosition) == false)
        {
            Instantiate(toBePlacedObject, newPosition, Quaternion.identity);
            return true;
        }
        else
        {
            counter++;
            if (counter >= 100)
            {
                counter = 0;
                return true;
            }
            return false;
        }
    }
    bool IsThereSomething(Vector3 pos)
    {
        Collider[] Colliders = Physics.OverlapSphere(pos, 1);
        if (Colliders.Length > 0) return true;
        else return false;
    }
}
