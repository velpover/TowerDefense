using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DirectionExtensions
{
    static Quaternion[] rotations =
    {
        Quaternion.identity,
        Quaternion.Euler(0,90f,0),
        Quaternion.Euler(0,180f,0),
        Quaternion.Euler(0,270f,0)
    };

    public static Quaternion GetRotation(this Direction direction)
    {
        return rotations[(int)direction];
    }
    public static DirectionChange GetDirectionChangeTo(
        this Direction current, Direction next
    )
    {
        if (current == next)
        {
            return DirectionChange.None;
        }
        else if (current + 1 == next || current - 3 == next)
        {
            return DirectionChange.TurnRight;
        }
        else if (current - 1 == next || current + 3 == next)
        {
            return DirectionChange.TurnLeft;
        }
        return DirectionChange.TurnAround;
    }

    public static float GetAngle(this Direction direction)
    {
        return (float)direction * 90f;
    }
}


public enum Direction
{
    Left, Right, Top, Bottom
}
public enum DirectionChange
{
    TurnLeft, TurnRight, None, TurnAround
}
