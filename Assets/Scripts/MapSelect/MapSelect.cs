using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelect : MonoBehaviour
{
    [Header("关卡展示预制体")]
    public GameObject levelGO;

    private List<string> mapList;
    // Start is called before the first frame update
    void Start()
    {
        mapList = GameApp.GetMapList();
        foreach (var mapName in mapList)
        {
            var newGO = Instantiate(levelGO, transform);
            newGO.GetComponent<Level>().Init(mapName, 
                Resources.Load<Sprite>($"Texture/{mapName}"));
        }
    }

    //void CreateLevelPanel()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        
    }
}
