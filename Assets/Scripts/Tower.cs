using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : GameTileContent
{
    Tower tower = default;

    TargetPoint target;

	[SerializeField] Transform turret,laserBeam;
	
    [SerializeField, Range(1.5f, 10.5f)] float targetingRange = 1.5f;
	[SerializeField, Range(1f, 100f)] float damagePerSecond = 50f;

	static Collider[] targetsBuffer = new Collider[1];

	const int enemyLayerMask = 1 << 7;

	Vector3 laserBeamScale;

    private void Awake()
    {
		laserBeamScale = laserBeam.localScale;
    }
    public override void GameUpdate()
    {
		if (TrackTarget() || AcquireTarget())
		{
			Shoot();
		}
		else laserBeam.localScale = Vector3.zero;
    }

	bool AcquireTarget()
	{
		Vector3 a = transform.localPosition;
		Vector3 b = a;

		b.y += 3f;

		int hits = Physics.OverlapCapsuleNonAlloc(
			a,b, targetingRange,targetsBuffer,enemyLayerMask
												);

		if (hits > 0)
		{
			target = targetsBuffer[0].GetComponent<TargetPoint>();

			Debug.Assert(target != null, "Targeted non-enemy!", targetsBuffer[0]);

			return true;
		}

		target = null;
		return false;
	}

	bool TrackTarget()
    {
		if(target == null)
        {
			return false;
        }

		Vector3 a = transform.localPosition;
		Vector3 b = target.Position;

		if (Vector3.Distance(a, b) > targetingRange+0.125f)
		{
			target = null;
			return false;
		}
		return true;
    }

	private void Shoot()
    {
		Vector3 point = target.Position;

		turret.LookAt(point);

		laserBeam.localRotation = turret.localRotation;

		float d = Vector3.Distance(turret.position, point);

		laserBeamScale.z = d;
		laserBeam.localScale=laserBeamScale;

		laserBeam.localPosition = turret.localPosition + 0.5f * d * laserBeam.forward;

		target.Enemy.ApplyDamage(damagePerSecond * Time.deltaTime);
	}
}
