using Sandbox;
using System.ComponentModel.Design;
using System.Data;

public class AeroCharacter : Component
{

	[Property, Group("Stats")] public float Health = 100f;
	[Property, Group( "Attributes" )] public float MaxHealth = 100f;
	[Property, Group( "Stats" )] public float Composure = 100f;
	[Property, Group( "Attributes" )] public float MaxComposure = 100f;
[RequireComponent] public PlayerController PController { get; set; }
//[RequireComponent] public FixedJoint Constrainer { get; set; }
public Collider PCollider;
public Rigidbody PBody;
public Vector3 LastPos = Vector3.Zero;

protected override void OnStart()
{
PCollider = PController.BodyCollider;
PBody = PController.Body;
}
	protected override void OnUpdate()
	{
		if ( PController.GroundObject != null && PController.GroundObject.GetComponentInChildren<Vehicle>().IsValid() )
		{
			PBody.Velocity += (PController.GroundVelocity - PBody.Velocity);

		}
	}
}
