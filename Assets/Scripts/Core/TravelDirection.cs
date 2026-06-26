using UnityEngine;

public enum TravelDirection
{
    Up,
    Left,
    Right
}

public static class TravelDirectionExtensions
{
    public static Vector2 ToVector2(this TravelDirection direction)
    {
        return direction switch
        {
            TravelDirection.Up => Vector2.up,
            TravelDirection.Left => Vector2.left,
            TravelDirection.Right => Vector2.right,
            _ => Vector2.up
        };
    }
}
