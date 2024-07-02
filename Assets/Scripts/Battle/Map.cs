using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Map
{

    public Unit[,] Units { get; private set; }
    public Building[,] Buildings { get; private set; }

    private Dictionary<Coordinate, Unit> unitDict = new Dictionary<Coordinate, Unit>();
    private Dictionary<Coordinate, Building> buildingDict = new Dictionary<Coordinate, Building>();
}