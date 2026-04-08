using Sandbox;

public class Wheel : VehiclePart
{
	[RequireComponent] public Rigidbody Rigidbody { get; set; }

	[RequireComponent] public WheelJoint WheelJoint { get; set; }
	protected override void OnUpdate()
	{

	}
}
