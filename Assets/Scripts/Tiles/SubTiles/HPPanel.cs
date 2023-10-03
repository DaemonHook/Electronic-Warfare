using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPPanel : MonoBehaviour
{
    public Sprite[] HPSprites;

    public void SetHPDisplay(int hp)
    {
        Debug.Assert(hp <= 10 && hp >= 0);
        var sprdr = GetComponent<SpriteRenderer>();
        sprdr.sprite = HPSprites[hp];
    }
}