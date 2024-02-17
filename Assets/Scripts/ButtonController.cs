using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public string mapName;
    public void Onclick()
    {
        var button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        // var maplist = GameApp.GetMapList();
        // int map = maplist.IndexOf(mapName);
        // GameApp.EnterMap(maplist[map]);
    }
}
