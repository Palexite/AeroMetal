using Sandbox;
using Sandbox.Internal;
using System;
using System.Runtime.InteropServices;
using Sandbox.Utility.BBox2D;
using System.Collections.ObjectModel;

public sealed class ChunkSystem : Component
{

	[Property] public int StageSeed { get; set; }

	public List<BBox> ChunkBBoxes = new List<BBox>();

	public List<GameObject> Chunks = new List<GameObject>();

	public PrefabFile[] ChunkPrefabs = { };

	public StageMain StageMain;

	public Random ChunkRandom;


	protected override void OnStart()
	{
		ChunkRandom = new Random(StageSeed);
		StageMain = Scene.Directory.FindByName( "Scene Information" ).First().GetComponent<StageMain>();
		Log.Info("StageMain Located.");
	}

#nullable enable

	/// <summary>
	/// Creates a chunk for the game environment given the chunk's Prefab File. It can be used for both starting chunks and succeeding chunks.
	/// </summary>
	public Chunk CreateChunk( PrefabFile Prefab, ChunkPoint? ChunkPoint)
	{
		Log.Info( Prefab );
		var Chunk = GameObject.GetPrefab(Prefab.ResourcePath);

		var ChunkObj = Chunk.Clone();
		var ChunkComp = ChunkObj.GetComponent<Chunk>();

		if (ChunkPoint != null)
		{
			ChunkComp.LastChunkPoint = ChunkPoint;
			ChunkObj.WorldTransform = ChunkPoint.WorldTransform;
			ChunkPoint.GeneratedChunk = ChunkObj;

			// appending routes of old lanes with new lanes
			ResolveChunkLanes(ChunkPoint.GameObject.Root.GetComponent<Chunk>(), ChunkPoint, ChunkComp);
		}
#nullable disable
		Chunks.Prepend( Chunk );
		var BoundaryInstances = AddChunkBBoundsForQuery(ChunkObj.GetComponent<Chunk>());

		ChunkComp.BBoxInstances = BoundaryInstances;
		return ChunkComp;
	}


	/// <summary>
	/// Resolves the Lane Components between two chunks.
	/// </summary>

	public void ResolveChunkLanes(Chunk OldChunk, ChunkPoint OldChunkPoint, Chunk NewChunk)
	{
		var PreceedingLanes = OldChunk.LaneComponents;

		var SucceedingLanes = NewChunk.GameObject.GetComponents<LaneComponent>();
		NewChunk.LaneComponents = SucceedingLanes;

		var PotentialRoutes = new List<LaneComponent>();

		foreach ( var SLane in SucceedingLanes )
		{
			if ( SLane.IsStartingRoute )
			{
				PotentialRoutes.Add( SLane );
			}

		}

		foreach (var PLane in PreceedingLanes)
		{
			if (PLane.ResolvingChunkPoint == OldChunkPoint)
			{
				// Lets try adding all the starting routes as valid routes for any vehicle to take

				//LaneComponent ClosestLane = null;
				//float ClosestDist = 9999999999999f;

				foreach (var PRoute in PotentialRoutes)
				{
					//var Dist = PRoute.WorldPosition.DistanceSquared(PLane.WorldPosition);
					//if ( PRoute.LaneIndex == PLane.LaneIndex && (ClosestDist >= Dist)) {

					// Make sure they're the same Lane ofcourse.
					if ( PRoute.LaneIndex == PLane.LaneIndex)
					{
						PLane.Routes.Add( PRoute );
						//ClosestLane = PRoute;
						// = Dist;
					}
				}
				
			}
		}
	}

	/// <summary>
	/// Adds a chunk's Boundary Boxes for predictive collision querying when creating newer chunks. 
	/// </summary>
	public IEnumerable<BBox> AddChunkBBoundsForQuery(Chunk Chunk)
	{
		var WorldPos = Chunk.GameObject.WorldPosition;
		var WorldRot = Chunk.GameObject.WorldRotation;
		var results  = new List<BBox>();

		foreach ( var ChunkBounds in Chunk.BBoxes )
		{
			Log.Info( ChunkBBoxes );
			var WorldChunk = ChunkBounds.Transform(new Transform(WorldPos, WorldRot));
			//WorldChunk = WorldChunk.Rotate( Chunk.GameObject.LocalRotation );
			ChunkBBoxes.Add( WorldChunk );
			results.Add(WorldChunk);
		}
		return results;
	}
}
