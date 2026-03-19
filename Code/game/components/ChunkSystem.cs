using Sandbox;
using Sandbox.Internal;
using System;
using System.Runtime.InteropServices;
using Sandbox.Utility.BBox2D;

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
		Log.Info("done");
		StageMain = Scene.Directory.FindByName( "Scene Information" ).First().GetComponent<StageMain>();

		Log.Info("StageMain Located.");
	}


	// Creates a starting Chunk for the stage to branch off from. From here, it's chunk point will take over and generate other chunks. For that reason, All chunks should have a chunk point and if they don't, nothing else will generate.
#nullable enable

	public void GetChunkPrefabFilesOfLevel()
	{

	}

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
		}
#nullable disable
		Chunks.Prepend( Chunk );
		AddChunkBBoundsForQuery(ChunkObj.GetComponent<Chunk>());

		return ChunkComp;
	}

	/// <summary>
	/// Adds a chunk's Boundary Boxes for predictive collision querying when creating newer chunks. This is how we prevent cycling chunks.
	/// </summary>
	public void AddChunkBBoundsForQuery(Chunk Chunk)
	{
		var WorldPos = Chunk.GameObject.WorldPosition;
		foreach ( var ChunkBounds in Chunk.BBoxes )
		{
			Log.Info( ChunkBBoxes );
			ChunkBBoxes.Append( ChunkBounds );
		}
	}
}
