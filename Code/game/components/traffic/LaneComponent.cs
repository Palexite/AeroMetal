using Sandbox;
using Sandbox.UI;
using System.Data;

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

	/// <summary>
	/// Whether this route should help resolve the Lane Components of a preceeding chunk.
	/// </summary>
	/// 
	[Property] public int LaneIndex { get; set; }
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
	public PrefabFile[] Vehicles { get; set; }

	[Property, ToggleGroup( "Vehicle Spawning" ), ShowIf( nameof( CanSpawnVehicles ), true )]

	public bool IsWhitelist = false;

	private TrafficSystem TrafficSystem;

	protected override void OnStart()
	{
		TrafficSystem = Scene.GetComponent<TrafficSystem>();

		TrafficSystem.Lanes.Add( this );
	}
	protected override void OnValidate()
	{

	}

	public void SpawnVehicle(PrefabFile Vehicle, float AtDistance)
	{


	}

	public void SpawnVehicle(float AtDistance)
	{


	}

	/// <summary>
	/// Fill this Lane with vehicles
	/// </summary>
	public void LaneFill()
	{
		var Spline = this.Spline.Spline;
		var Divisionlength = 250;
		var Subdivisions = MathX.FloorToInt(Spline.Length / Divisionlength);

		for ( int i = 0; i < Subdivisions; i++ )
		{
			SpawnVehicle( i * Divisionlength );
		}

	}
}
