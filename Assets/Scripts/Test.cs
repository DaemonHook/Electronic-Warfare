using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (var s in GameApp.GetMapList())
        {
            print(s);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}