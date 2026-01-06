using Sandbox;
using Sandbox.Internal;
using System;
using System.Runtime.InteropServices;
using Sandbox.Utility.BBox2D;
using Sandbox.game.utility;

public sealed class ChunkSystem : Component
{

	[Property] public int StageSeed { get; set; }

	public List<Rect> ChunkBBoxes = new List<Rect>();

	public GameObject[] Chunks = {};

	public StageMain StageMain;

	public Stage StageObject;

	public Random ChunkRandom;


	protected override void OnStart()
	{
		ChunkRandom = new Random(StageSeed);

		StageMain = Scene.Directory.FindByName( "StageMain" ).First().GetComponent<StageMain>();
		StageObject = StageMain.StageObject;

		Log.Info("Stage Object "+StageMain.StageObject+" Created.");
	}


	// Creates a starting Chunk for the stage to branch off from. From here, it's chunk point will take over and generate other chunks. For that reason, All chunks should have a chunk point and if they don't, nothing else will generate.
#nullable enable

	public void CreateChunk( string Prefab, ChunkPoint? ChunkPoint)
	{
		var Chunk = GameObject.GetPrefab(Prefab);

		var ChunkObj = Chunk.Clone();
		if (ChunkPoint != null)
		{
			ChunkObj.GetComponent<Chunk>().LastChunkPoint = ChunkPoint;
			ChunkObj.WorldTransform = ChunkPoint.WorldTransform;
			ChunkPoint.GeneratedChunk = ChunkObj;
		}
#nullable disable
		Chunks.Prepend( Chunk );
		AddChunkBBoundsForQuery(ChunkObj.GetComponent<Chunk>());
	}


	public void AddChunkBBoundsForQuery(Chunk Chunk)
	{
		var WorldPos = Chunk.GameObject.WorldPosition;
		foreach ( var ChunkBounds in Chunk.BBoxes )
		{
			Log.Info( ChunkBBoxes );
			var NewChunkBounds = Rect_Utils.AddVector3ToRect( ChunkBounds, WorldPos );
			ChunkBBoxes.Append( NewChunkBounds );
		}
	}
}
