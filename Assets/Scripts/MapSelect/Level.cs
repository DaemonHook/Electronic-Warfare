
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    public string mapName;
    public Sprite mapImage;

    public void Init(string mapName, Sprite mapImage)
    {
        this.mapName = mapName;
        this.mapImage = mapImage;
        transform.Find("MapImage").GetComponent<Image>().sprite = mapImage;
        transform.Find("MapImage").GetComponent<ButtonController>().mapName = mapName;
        transform.Find("MapName").GetComponent<Text>().text = mapName;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
