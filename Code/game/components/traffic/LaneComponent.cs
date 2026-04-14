using Sandbox;
using Sandbox.Audio;
using Sandbox.game.utility;
using Sandbox.UI;
using System.Data;
using System.Diagnostics;
using System.Drawing;

/// <summary>
/// A classification represents a group of traffic nodes.
/// </summary>
/// 


/// <summary>
/// Act's as a cheap navigation for potentially hundreds of vehicles.
/// </summary>
/// 
public sealed class LaneComponent : Component
{
	[RequireComponent] public SplineComponent Spline {  get; set; }
	/// <summary>
	/// The lanes we can follow after a vehicle reaches the end of this one.
	/// </summary>
	[Property] public List<LaneComponent> Routes { get; set; }

	/// <summary>
	/// The chunkpoint that we listen to for chunk generation which will append our routes with it's own so we aren't a dead end.
	/// This is how vehicles continue from this chunk to another via transition of LaneComponents.
	/// This is unnecessary to fill if you only want routes that reside within the chunk of this LaneComponent.
	/// </summary>
	[Property] public ChunkPoint ResolvingChunkPoint { get; set; }

	[Property] public int LaneIndex { get; set; }

	/// <summary>
	/// Whether this route should help resolve the Lane Components of a preceeding chunk.
	/// </summary>
	/// 
	[Property] public bool IsStartingRoute { get; set; }

	/// <summary>
	/// Vehicles currently following this lane
	/// </summary>
	[Property] List<Vehicle> VehiclesOnLane { get; set; }

	/// <summary>
	/// The multiplied speed of this lane.
	/// </summary>

	[Property] float SpeedMultiplier { get; set; }
	[Property] float SpeedAcceleration { get; set; }

	[Property, ToggleGroup( "Vehicle Spawning")] 
	public bool CanSpawnVehicles { get; set; }

	[Property, ToggleGroup( "Vehicle Spawning" ), ShowIf( nameof( CanSpawnVehicles ), true )]
	public GrayList<PrefabFile> VehicleSpecialSpawnList = new();

	[Property, ToggleGroup( "Vehicle Spawning" ), ShowIf( nameof( CanSpawnVehicles ), true )]

	public bool SpawnListIsWhitelist = false;

	private static TrafficSystem TrafficSystem;


	// Cached on our own end for quicker mutual access

	protected override void OnStart()
	{
		if ( TrafficSystem == null )
		{
			TrafficSystem = Scene.GetComponent<TrafficSystem>();
		}

		//TrafficSystem.Lanes.Add( this );
		VehicleSpecialSpawnList.IsWhitelist = SpawnListIsWhitelist;
		VehicleSpecialSpawnList.Parent = StageMain.VehicleSpawnGrayList;
	
		LaneFill();
	}
	protected override void OnValidate()
	{

	}

	/*
	public Rotation GetRotationAtDistance(float Distance, float ErrorTol)
	{
		Spline.Sample currentSample = Spline.Spline.SampleAtDistance( Distance );
		Spline.Sample lookAheadSample = Spline.Spline.SampleAtDistance( Distance + ErrorTol );

		Vector3 travelDirVector = (lookAheadSample.Position.RotateAround( this.WorldPosition, this.WorldRotation ) - currentSample.Position.RotateAround( this.WorldPosition, this.WorldRotation )).Normal;

		if ( travelDirVector == Vector3.Zero ) return Rotation.Identity;

		Rotation FinalRotation = Rotation.LookAt( travelDirVector, Vector3.Up );

		return Rotation.Difference( Rotation.FromYaw(90 ), FinalRotation );
	}
	*/

	public float GetDistanceToClosestPos(Vector3 Pos, bool RootIt)
	{
		var Sample = this.Spline.Spline.SampleAtClosestPosition( (Pos - Spline.WorldPosition).RotateAround( Vector3.Zero, Spline.WorldRotation.Inverse ) );
		var FinalSamplePos = this.GetAbsoluteSamplePosition( Sample );
		
		DebugOverlay.Line( FinalSamplePos, FinalSamplePos + Vector3.Up * 256 );
		if ( RootIt )
		{
			return FinalSamplePos.Distance(Pos);

		} else
		{
			return FinalSamplePos.DistanceSquared( Pos );
		}
	}


	public Vector3 GetAbsoluteSamplePosition(Spline.Sample sample)
	{

		return (sample.Position + WorldPosition).RotateAround( WorldPosition, WorldRotation );
	}


	public Rotation GetRotationAtDistance( float Distance, float ErrorTol )
	{
		Spline.Sample currentSample = Spline.Spline.SampleAtDistance( Distance );
		Spline.Sample lookAheadSample = Spline.Spline.SampleAtDistance( Distance + ErrorTol );

		var Rot = MathExtended.GetRotationBetweenVectors( GetAbsoluteSamplePosition(currentSample), GetAbsoluteSamplePosition(lookAheadSample));
		DebugOverlay.Line( GetAbsoluteSamplePosition( currentSample ), GetAbsoluteSamplePosition( lookAheadSample ) );
		return Rot;
	}

	public void SpawnVehicle(float AtDistance)
	{
		var Random = TrafficSystem.TrafficRandom;
		Log.Info( TrafficSystem.TrafficRandom );
		var Prefab = VehicleSpecialSpawnList.SelectRandomPrefab( Random );
		
		var NewVehicle = Prefab.GetScene().Clone();

		//NewVehicle.WorldPosition = Spline.WorldPosition + Spline.Spline.SampleAtDistance( AtDistance ).Position + NewVehicle.LocalPosition;
		NewVehicle.WorldPosition = GetAbsoluteSamplePosition(Spline.Spline.SampleAtDistance( AtDistance )) + NewVehicle.LocalPosition;

		NewVehicle.WorldRotation = GetRotationAtDistance( AtDistance, 15 );
		var VehicleComp = NewVehicle.GetComponentInChildren<Vehicle>();
		VehicleComp.Lane = this;
		VehicleComp.CurrentSplineDistance = AtDistance;
	}


	public void SpawnVehicle(float AtDistance, PrefabFile Vehicle)
	{


	}


	protected override void DrawGizmos()
	{
		base.DrawGizmos();

		foreach ( var Route in Routes )
		{
			if ( Route != null && Route.IsValid()) {
				var len = this.Spline.Spline.Length;
				Gizmo.Draw.Color = Color.Cyan;
				Gizmo.Draw.LineThickness = 4;
				Gizmo.Draw.Line( this.Spline.Spline.SampleAtDistance( len ).Position, this.WorldPosition - Route.WorldPosition );
			}
		}

	}


	/// <summary>
	/// Fill this Lane with vehicles
	/// </summary>
	public void LaneFill()
	{
		var Spline = this.Spline.Spline;
		var Divisionlength = 1024;
		var Subdivisions = MathX.FloorToInt(Spline.Length / Divisionlength);
		for ( int i = 0; i < Subdivisions; i++ )
		{ 
			SpawnVehicle( i * Divisionlength );
		}

	}
}
