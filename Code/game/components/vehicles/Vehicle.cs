using Sandbox;
using Sandbox.game.interfaces;
using Sandbox.Modals;
using Sandbox.Network;
using Sandbox.Services;
using System;
using System.Data;
using System.Runtime.InteropServices.Swift;
public class Vehicle : PhysicsObject, IMortal, IRecycle, Component.ICollisionListener
{

	[RequireComponent] public Rigidbody RigidBody { get; set; }
	[RequireComponent] public ModelRenderer ModelRenderer { get; set; }


	[Property, Group("Stats")] float Health = 1024f;

	[Property, Group( "Stats" )] float MaxHealth = 1024f;

	[Property, Group( "Stats" )] float MaxSpeed = 512f;

	[Property, Group( "Stats" )] float Acceleration = 256f;

	[Property, Group( "Stats" )] float Traction = 1f;

	[Property, Group( "References" )] public List<Wheel> DrivingWheels;

	[Property, Group( "References" )] public LaneComponent Lane;

	[Property, Group( "Identity" )] public string Category = "Default";

	public bool IsAccelerating = false;

	public Chunk Chunk;

	public bool IsBreaking = false;

	public float CurrentSpeed = 45f;



	private bool IsDormant = false;

	public float CurrentSplineDistance = 0f;

	private int TravelDirection = 1;

	private static HostStats HostStats;

	protected override void OnStart()
	{
		base.OnStart();
		foreach ( var Wheel in DrivingWheels )
		{
			Wheel.WheelJoint.MaxSpinTorque = MaxSpeed * 100000;
			Wheel.WheelJoint.MaxSteeringTorque = 100000000;
		}

	}
	protected override void OnUpdate()
	{
			if ( Lane.GetDistanceToClosestPos( this.WorldPosition, false ) < 128 * 128 )
			{
				MoveDormantAcrossLane();
				DebugOverlay.Line( WorldPosition, WorldPosition + Vector3.Up * 512 );
			}
			else
			{

				MoveDormantTowardLane();
			}
			//MoveDormantAcrossLane();
			LaneUpdate();

	}

	private void LaneUpdate()
	{
		var Spline = Lane.Spline.Spline;
		if ( Spline.Length - 10 <= CurrentSplineDistance )
		{
			OnEndOfLane();
		}

	}

	public void Recycle()
	{

	}

	/// <summary>
	/// Tells the vehicle to accelerate to the target speed. Negative values deaccelerate
	/// </summary>
	/// <param name="TargetSpeed"></param>
	/// 
	public virtual void Accelerate(float TargetSpeed)
	{
		var FinalSpeed = MathX.Approach( CurrentSpeed, TargetSpeed, Acceleration );
		if (IsDormant)
		{
			MoveDormant( FinalSpeed );
		}
		else
		{
			Move( FinalSpeed );
		}
	}

	public virtual void Brake()
	{

	}

	public virtual void Turn(float Angle)
	{
		if ( IsDormant )
		{
			SteerDormant( Angle );
		}
	else
		{
			Steer( Angle );
		}
	}

	/// <summary>
	/// Moves the Vehicle with this speed. Negative values make it move in reverse.
	/// </summary>
	/// <param name="Speed"></param>
	/// 
	public void Move(float Speed)
	{
		foreach ( var Wheel in DrivingWheels )
		{
			var FinalSpeed = MathX.Approach( CurrentSpeed, Speed, Acceleration );
			var WheelJoint = Wheel.WheelJoint;
			CurrentSpeed = FinalSpeed;

			WheelJoint.SpinMotorSpeed = FinalSpeed;
		}


	}

	private LaneComponent SearchForAvailableRoute()
	{
		// For now, just return a random  available route of our lane
		return Lane.Routes[TrafficSystem.TrafficRandom.Next(0, Lane.Routes.Count() - 1)];
	}

	/// <summary>
	/// Moves the Vehicle with this speed. Negative values make it move in reverse. In this state, it ignores physical forces.
	/// </summary>
	/// 

	public void MoveDormant( float Speed )
	{
		this.WorldPosition += this.WorldRotation.Left * (Speed * Time.Delta);
	}




	// Thanks Bop32 for code example.
	public void MoveDormantAcrossLane()
	{
		var splinePath = Lane.Spline;
		Spline spline = splinePath.Spline;

		float frameDistanceDelta = Time.Delta * 1000f * TravelDirection;
		float nextSplineDistance = CurrentSplineDistance + frameDistanceDelta;

		Spline.Sample currentSample = spline.SampleAtDistance( nextSplineDistance );

		CurrentSplineDistance = nextSplineDistance;

		WorldRotation = Lane.GetRotationAtDistance( nextSplineDistance, 15 );
		WorldPosition = (currentSample.Position + splinePath.WorldPosition).RotateAround( splinePath.WorldPosition, splinePath.WorldRotation ) + new Vector3(0, 0, 32);
		
	}

	public void MoveDormantTowardLane()
	{
		var splinePath = Lane.Spline;
		Spline spline = splinePath.Spline;

		var CurPos = this.WorldPosition;
		var Sample = spline.SampleAtClosestPosition((this.WorldPosition - Lane.WorldPosition ).RotateAround( Vector3.Zero, Lane.WorldRotation ));

		var FinalSamplePos = Lane.GetAbsoluteSamplePosition(Sample);

		var Rot = MathExtended.GetRotationBetweenVectors( CurPos, FinalSamplePos );

		SteerDormant( Rotation.Difference( this.WorldRotation, Rot ).Yaw() );
		MoveDormant( CurrentSpeed );


	}
	/// <summary>
	/// Called when the vehicle reaches the end of it's current Lane.
	/// </summary>
	/// 
	public void OnEndOfLane()
	{
		// There is no where to go, likely because it is either a dead-end or a succeeding chunk that resolves the lane has not been generated.
		//	Instead of tipping over into the endless void, just die.
		if ( Lane.ResolvingChunkPoint == null ||  Lane.Routes.Count() == 0) {
			this.GameObject.Root.Destroy();
		} else
		{
			this.Lane = SearchForAvailableRoute();
			Log.Info( this.Lane.Routes );
			this.CurrentSplineDistance = 0;
		}

	}

	public void Steer( float Steer )
	{
		if (!IsDormant)
		foreach ( var Wheel in DrivingWheels )
		{
			Wheel.WheelJoint.TargetSteeringAngle = Steer;
		}
	}

	public void SteerDormant( float Steer )
	{
		foreach ( var Wheel in DrivingWheels )
		{
			Wheel.WheelJoint.TargetSteeringAngle = Steer;
		}

		this.WorldRotation *= Rotation.FromYaw(Steer) * 0.01f;

	}

	public void SteerTowardPos(Vector3 Pos)
	{
		var MyPos = this.GameObject.WorldPosition;
		var MyRot = this.GameObject.WorldRotation;

		var PosOffset = Pos - MyPos;
		if ( PosOffset.LengthSquared < 0.0001f )
			return;
		var LookAtRot = Rotation.LookAt(PosOffset.Normal, Vector3.Up );

		var RotOffset = Rotation.Difference(MyRot, LookAtRot );


		foreach ( var Wheel in DrivingWheels )
		{
			//Wheel.WorldRotation = Rotation.FromYaw( RotOffset.Yaw() ) * this.WorldRotation;

			Wheel.WheelJoint.TargetSteeringAngle = RotOffset.Yaw();
		}
		DebugOverlay.Line( MyPos,  Pos, Color.White, 0.0001f );
		DebugOverlay.Line( MyPos, MyPos + (Rotation.FromYaw( RotOffset.Yaw() ) * this.WorldRotation).Forward * 1000, Color.Green, 0.0001f );
	}

	public override void OnCollisionStart( Collision Coll )
	{
		base.OnCollisionStart( Coll );
		SetDormant( false );
	}

	public void SetDormant(bool state)
	{
		IsDormant = state;
	}

	public void OnDamage( in DamageInfo damage )
	{

	}

	public void OnHeal( int Amount )
	{

	}

	public void OnDeath()
	{

	}
}
