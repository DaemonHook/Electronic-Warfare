using UnityEditor.Rendering;

public class Unit
{
    public Coordinate Position { get; private set; }

    public Unit(Coordinate Position)
    {
        this.Position = Position;
    }

    public void Move(Coordinate newPosition)
    {
        Position = newPosition;
    }
}