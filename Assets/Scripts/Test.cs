using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var text = Resources.Load<TextAsset>("SamplePackage/Maps/sampleMap.tmx");
        var tileMap = new TiledMap(text.text);
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
