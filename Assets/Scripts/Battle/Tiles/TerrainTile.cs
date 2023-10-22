using UnityEngine;

public class TerrainTile : GameTile
{
    private BlockType blockType;

    public override BlockType GetBlockType()
    {
        return blockType;
    }

    public void Init(int x, int y, BlockType blockType)
    {
        Init(x, y);
        this.blockType = blockType;
    }
}