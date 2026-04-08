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


	[Property] float Health = 1024f;

	[Property] float MaxHealth = 1024f;

	[Property] float MaxSpeed = 512f;

	[Property] float Acceleration = 256f;

	[Property] float Traction = 1f;

	[Property] public List<Wheel> DrivingWheels;

	[Property] public LaneComponent Lane;

	public bool IsAccelerating = false;

	public Chunk Chunk;

	public bool IsBreaking = false;

	public float CurrentSpeed = 0f;


	private bool IsDormant = false;

	private float CurrentSplineDistance = 0f;

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
		//Move( 500 );
		//SteerTowardPos( new Vector3( 256, -1024, 0 ) );
		CalculateMoveTowardSpline();
		LaneUpdate();

	}

	private void LaneUpdate()
	{
		var Spline = Lane.Spline.Spline;
		if ( Spline.Length - 10 <= CurrentSplineDistance )
		{
			Log.Info( "LaneEnd" );
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

	private void SearchForNewLane()
	{

		var myPos = this.WorldPosition;

		

	}

	/// <summary>
	/// Moves the Vehicle cheaply to this target speed. Negative values make it move in reverse. In this state, it ignores physical forces unless they are big enough to deter the vehicle.
	/// </summary>
	/// 

	public void MoveDormant( float Speed )
	{
		this.WorldPosition += this.WorldRotation.Left * (Speed * 0.0001f);
	}




	// Thanks Bop32 for this method. Exactly what I needed with some improvisations.
	public void CalculateMoveTowardSpline()
	{
		var splinePath = Lane.Spline;
		Spline spline = splinePath.Spline;

		float frameDistanceDelta = Time.Delta * 500f * TravelDirection;
		float nextSplineDistance = CurrentSplineDistance + frameDistanceDelta;

		Spline.Sample currentSample = spline.SampleAtDistance( nextSplineDistance );
		Spline.Sample lookAheadSample = spline.SampleAtDistance( nextSplineDistance + 15 );

		Vector3 travelDirectionVector = (lookAheadSample.Position - currentSample.Position).Normal;

		if ( travelDirectionVector == Vector3.Zero ) return;

		CurrentSplineDistance = nextSplineDistance;

		Rotation targetRotation = Rotation.LookAt( travelDirectionVector, Vector3.Up );

		DebugOverlay.Line( WorldPosition, WorldPosition + travelDirectionVector * 500, Color.Orange );

		
		WorldRotation = Rotation.Difference(Rotation.FromYaw(90), targetRotation );
		WorldPosition = currentSample.Position + splinePath.WorldPosition;
		
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
			DestroyGameObject();
		}

	}



	/*
	 * I can't get this to work, I'll leave this out for now.
	 * 
	public void CalculateMoveTowardSpline()
	{
		RigidBody.PhysicsBody.MotionEnabled = false;
		if ( CurrentLane.IsValid )
		{
			var MyPos = this.WorldPosition;

			var LaneSpline = CurrentLane.Spline;

			var Sample = LaneSpline.Spline.SampleAtClosestPosition( this.WorldPosition );
			var DistSample = LaneSpline.Spline.SampleAtDistance( Sample.Distance + 64 );

			var Pos = DistSample.Position;


			var PosOffset =  (Pos + LaneSpline.WorldPosition) - MyPos;

			//if ( PosOffset.LengthSquared < 0.0001f )
			//return;

			var LookAtRot = Rotation.LookAt( PosOffset.Normal, Sample.Up );
			var NewRot = Rotation.FromYaw( LookAtRot.Yaw() - 90 );

			DebugOverlay.Line( this.WorldPosition, this.WorldPosition + NewRot.Left * 1000, Color.Blue );
			if ( NewRot.Yaw() != 0f )
			{
				this.WorldRotation = NewRot;
			}

			this.WorldPosition += LookAtRot.Forward * 1;

			DebugOverlay.Line( this.WorldPosition, this.WorldPosition + (Vector3.Up * 512), Color.Yellow );
			DebugOverlay.Line( LaneSpline.WorldPosition + Sample.Position, LaneSpline.WorldPosition + Sample.Position + (Vector3.Up * 128), Color.Green );
			DebugOverlay.Line( LaneSpline.WorldPosition + DistSample.Position, LaneSpline.WorldPosition + DistSample.Position + (Vector3.Up * 128), Color.Red );
		}

	}
	*/
	public void Steer( float Steer )
	{
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

		this.WorldRotation *= Rotation.FromYaw(Steer);

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
