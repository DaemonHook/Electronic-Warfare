using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainLayer : BaseLayer
{
    public static TerrainLayer Instance;

    private void Awake()
    {
        Instance = this;
    }

    public override void AddObject(int x, int y, GameObject obj)
    {
        base.AddObject(x, y, obj);
        obj.transform.position = new Vector2(x, y);
        obj.GetComponent<SpriteRenderer>().sortingLayerName = "Terrain";
    }

    
}
