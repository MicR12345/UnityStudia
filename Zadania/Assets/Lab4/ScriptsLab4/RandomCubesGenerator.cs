using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomCubesGenerator : MonoBehaviour
{
    List<Vector3> positions = new List<Vector3>();
    public float delay = 3.0f;
    [SerializeField]
    int objectCounter = 0;
    // obiekt do generowania
    public GameObject block;
    public int maxObjectCount = 10;
    public List<Material> materials;
    void Start()
    {
        int _this_x = Mathf.FloorToInt(this.gameObject.transform.position.x);
        int _this_z = Mathf.FloorToInt(this.gameObject.transform.position.z);
        MeshRenderer renderer = this.GetComponent<MeshRenderer>();
        //Vector3 size = renderer.
        // w momecie uruchomienia generuje 10 kostek w losowych miejscach
        List<int> pozycje_x = new List<int>(Enumerable.Range(_this_x, _this_x+maxObjectCount).OrderBy(x => Guid.NewGuid()).Take(maxObjectCount));
        List<int> pozycje_z = new List<int>(Enumerable.Range(_this_z, _this_z + maxObjectCount).OrderBy(x => Guid.NewGuid()).Take(maxObjectCount));

        for (int i = 0; i < maxObjectCount; i++)
        {
            this.positions.Add(new Vector3(pozycje_x[i], 5, pozycje_z[i]));
        }
        foreach (Vector3 elem in positions)
        {
            Debug.Log(elem);
        }
        // uruchamiamy coroutine
        StartCoroutine(GenerujObiekt());
    }

    void Update()
    {

    }

    IEnumerator GenerujObiekt()
    {
        Debug.Log("wywołano coroutine");
        foreach (Vector3 pos in positions)
        {
            if (objectCounter >= maxObjectCount)
            {
                break;
            }
            else
            {
                GameObject go = Instantiate(this.block, this.positions.ElementAt(this.objectCounter++), Quaternion.identity);
                go.GetComponent<MeshRenderer>().material 
                    = materials[Mathf.FloorToInt(UnityEngine.Random.Range(0,materials.Count))];
                yield return new WaitForSeconds(this.delay);
            }
        }
        // zatrzymujemy coroutine
        StopCoroutine(GenerujObiekt());
    }
}