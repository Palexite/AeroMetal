using Sandbox;
using System.Data;
using System.Runtime.InteropServices;
using System.Transactions;
using Sandbox.Physics;

public class VehiclePart : Component, Component.IDamageable
{
	public float Health = 100f;
	public float MaxHealth = 100f;

	public Vehicle Vehicle;
	public PhysicsJoint Joint;

	public bool isBroken = false;


	public void OnDamage( in DamageInfo dmgInfo )
	{
		if ( dmgInfo.Damage >= Health )
		{
			Health = 0;
			Kill( dmgInfo );
		}
		Health -= dmgInfo.Damage;
	}

	public void Kill( in DamageInfo dmgInfo )
	{
		isBroken = true;

	}

	public void makeCheap()
	{

	}
	protected override void OnUpdate()
	{

	}
}
