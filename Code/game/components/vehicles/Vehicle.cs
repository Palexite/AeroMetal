using Sandbox;
using Sandbox.Modals;

public class Vehicle : PhysicsObject, Component.IDamageable, IRecycle
{
	public float health = 1024f;

	private bool isDormant = false;

	private Collider collider;

	public List<VehiclePart> Parts;

	protected override void OnUpdate()
	{
		
	}

	public void Recycle()
	{

	}

	public void Construct()
	{

	}

	public void SetDormant(bool state)
	{
		isDormant = state;

		
	}

	public void OnDamage( in DamageInfo damage )
	{

	}
}
