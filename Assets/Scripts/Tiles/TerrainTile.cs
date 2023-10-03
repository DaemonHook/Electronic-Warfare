using UnityEngine;

public class TerrainTile : GameTile
{
    public TerrainType TerrainType { get; private set; }

    public void Init(int x, int y, TerrainType terrainType)
    {
        Init(x, y);
        TerrainType = terrainType;
    }
}