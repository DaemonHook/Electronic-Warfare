using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    public GameObject EffectLayer;

    public List<GameObject> Effects;

    public void ShowEffect(int effectNumber, Vector2 effectPosition)
    {
        var effectGO = Instantiate(Effects[effectNumber], EffectLayer.transform);
        effectGO.transform.position = effectPosition;
        effectGO.GetOrAddComponent<EffectTile>();
    }

    private void Awake()
    {
        Instance = this;
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
