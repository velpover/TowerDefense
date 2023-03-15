using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    EnemyFactory originFactory;

    GameTile tileFrom, tileTo;

    float progress,directionAngleFrom,directionAngleTo;

    Direction direction;
    DirectionChange directionChange;

    Vector3 positionFrom,positionTo;

    private float Health { get; set; } = 100f;
    public EnemyFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Wrong Factory from Enemy");
            originFactory = value;
        }
    }

    public void SpawnOn(GameTile tile)
    {
        Debug.Assert(tile.NextTileOnPath != null, "nowhere to go");

        tileFrom = tile;
        tileTo = tile.NextTileOnPath;

        progress = 0f;

        PrepareIntro();
    }

    void PrepareIntro()
    {
        positionFrom = tileFrom.transform.localPosition;
        positionTo = tileFrom.ExitPoint;

        direction = tileFrom.PathDiretion;
        directionChange = DirectionChange.None;
        directionAngleFrom = directionAngleTo = direction.GetAngle();

        transform.localRotation = direction.GetRotation();
    }
    public bool GameUpdate()
    {
        if (Health <= 0)
        {
            OriginFactory.Reclaim(this);
            return false;
        }

        progress+=Time.deltaTime;

        while(progress > 1f)
        {
            tileFrom = tileTo;
            tileTo = tileTo.NextTileOnPath;

            if(tileTo == null)
            {
                OriginFactory.Reclaim(this);
                return false;
            }

            PrepareNextState();

            progress -= 1f;
        }

        transform.localPosition = Vector3.LerpUnclamped(positionFrom,positionTo,progress);

        if (directionChange != DirectionChange.None)
        {
            float angle = Mathf.LerpUnclamped(directionAngleFrom, directionAngleTo, progress);

            transform.localRotation = Quaternion.Euler(0f, angle, 0f);
        }

        return true;
    }

    void PrepareNextState()
    {
        positionFrom = positionTo;
        positionTo = tileFrom.ExitPoint;
        directionChange = direction.GetDirectionChangeTo(tileFrom.PathDiretion);
        direction = tileFrom.PathDiretion;
        directionAngleFrom = directionAngleTo;

        switch (directionChange)
        {
            case DirectionChange.None: PrepareForward(); break;
            case DirectionChange.TurnRight: PrepareTurnRight(); break;
            case DirectionChange.TurnLeft: PrepareTurnLeft(); break;
            default: PrepareTurnAround(); break;
        }
    }

    void PrepareForward()
    {
        transform.localRotation = direction.GetRotation();
        directionAngleTo = direction.GetAngle();
    }

    void PrepareTurnRight()
    {
        directionAngleTo = directionAngleFrom + 90f;
    }

    void PrepareTurnLeft()
    {
        directionAngleTo = directionAngleFrom - 90f;
    }

    void PrepareTurnAround()
    {
        directionAngleTo = directionAngleFrom + 180f;
    }

    public void ApplyDamage(float damage)
    {
        Health -= damage;
    }
}
