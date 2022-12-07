using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform Prefab;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(Prefab, new Vector3(2.0F, 0, 0), Quaternion.identity);
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
