using Sandbox;
using Sandbox.Diagnostics;
using Sandbox.game.utility;
using Sandbox.Utility.BBox2D;
using System;
using System.Diagnostics;
using System.Net.Security;
using System.Runtime.InteropServices;



public sealed class ChunkPoint : Component
{
	// Leaving this list empty allows for any chunk to be used. Otherwise this is useful if the chunk model is structured in such a way that it only looks natural with a certain set of chunks along it.
	[Property] List<string> SpecificChunkList { get; set; }
	[Property] bool IsWhitelist { get; set; }

	public GameObject GeneratedChunk;

	public string SelectedChunkPrefab;

	private StageSettings StageSettings;

	private StageMain StageMain;

	private ChunkSystem ChunkSystem;

	private Random ChunkRandom;

	private FastTimer ChunkProcUpdateTick = FastTimer.StartNew();

	//Instead of a for each loop, we're simply incrementing a flag every frame to get an item out of an array. This component has minimal interaction and thus may be time and stability insensitive for a faster workflow.
	private byte CurrentPlayerCheckIndex = 0;

	private int ChunkQueryIndex = 0;

	private static readonly byte MaxCollisionQueriesPerFrame = 4;

	private bool FinishedCollisionQuery = false;

	private List<string> SafeChunks;





	protected override void OnStart()
	{
		StageMain = Scene.Directory.FindByName( "StageMain" ).First().GetComponent<StageMain>();
		StageSettings = Scene.Directory.FindByName( "StageSettings" ).First().GetComponent<StageSettings>();
		ChunkSystem = Scene.Directory.FindByName("ChunkSystem").First().GetComponent<ChunkSystem>();
		ChunkRandom = ChunkSystem.ChunkRandom;

		// May be faster and deterministic if we define this ahead of time.
		SelectedChunkPrefab = StageMain.StageObject.ChunkList[ChunkRandom.Next( 0, StageMain.StageObject.ChunkList.Count())];
	}


	//TODO: make system that predicts if a chunk will collide with another chunk within the world space using it's bounds, and exclude it from a list of available chunks to choose from.

	public void ChunkCollisionQuery()
	{
		var AvailableChunks = StageMain.StageObject.ChunkList;

		if ( IsWhitelist )
		{
			AvailableChunks = SpecificChunkList;
		}
		var finalQueryAmount = MaxCollisionQueriesPerFrame;

		if ( AvailableChunks.Count - ChunkQueryIndex < MaxCollisionQueriesPerFrame )
		{
			finalQueryAmount = (byte)(AvailableChunks.Count() - ChunkQueryIndex);
		}

		var CurrentChunkQueryIndex = ChunkQueryIndex;
		for ( int i = CurrentChunkQueryIndex; i < CurrentChunkQueryIndex + finalQueryAmount; i++ )
		{
			var chunk = AvailableChunks[i];
			var PrefFile = ResourceLibrary.Get<PrefabFile>( AvailableChunks[i] );
			var Scene = PrefFile.GetScene();
			var data = PrefFile.GetMetadata( "Chunk_BBounds" );
			var BoundaryData = Rect_Utils.StringToRectList( data );

			var ChunkPoints = Scene.GetComponentsInChildren<ChunkPoint>();

			foreach (var ChunkPoint in ChunkPoints )
			{
				var RayStruct = new Ray( this.WorldPosition, ChunkPoint.WorldRotation.Forward);

				// TODO 12.20.2025: Check against all BBoxes (Rects)
				foreach ( var Bounds in BoundaryData)
				{
					
				}
			}


			
		}
		ChunkQueryIndex = CurrentChunkQueryIndex + finalQueryAmount;
		if ( AvailableChunks.Count <= ChunkQueryIndex )
		{
			FinishedCollisionQuery = true;
		}
	}

	public void ChooseRandomValidChunk()
	{


	}

	protected override void DrawGizmos()
	{
		Gizmo.Draw.LineThickness = 16f;
		Gizmo.Draw.Color = Color.White.WithAlpha( Gizmo.IsSelected ? 1f : 0.2f );
		Gizmo.Draw.Arrow(Vector3.Zero, new Vector3(0, -86, 0), 32, 12);
		Gizmo.Draw.Color = Gizmo.Colors.Green.WithAlpha( Gizmo.IsSelected ? 1f : 0.2f );
		Gizmo.Draw.SolidBox(new BBox(new Vector3(-32, -32, -32), new Vector3(32, 32, 32)));

		if (Gizmo.IsSelected && Scene.Directory.FindByName( "Model", true ).Count() != 0)
		{
			var Model = Scene.Directory.FindByName( "Model", true ).First().GetComponent<ModelRenderer>();
			Gizmo.Draw.Model(Model.Model, new global::Transform(this.LocalPosition / 2));

		}
	}


	public void PlayerDistanceCheck()
	{
		var ChunkProcDistanceSq = StageSettings.ChunkProcDistance^2;

		if ( StageMain.Players != null)
		{
			var Players = StageMain.Players.ToList();
			var Player = Players[CurrentPlayerCheckIndex];

			// If the player is close enough, we generate a chunk
				var PlayerGO = Player.GameObject;
				var Distance = PlayerGO.WorldPosition.DistanceSquared( this.WorldPosition );
				var MyChunk = this.GameObject.Parent.GetComponent<Chunk>();

			// If we have not created a chunk yet.

			if ( GeneratedChunk == null )
			{
				if ( Distance < (ChunkProcDistanceSq * ChunkProcDistanceSq) )
				{
					ChunkSystem.CreateChunk(SelectedChunkPrefab, this);
				}
				if ( CurrentPlayerCheckIndex + 1 < Players.Count )
				{
					CurrentPlayerCheckIndex += 1;
				}
				else
					CurrentPlayerCheckIndex = 0;
			}
		}
	}
	protected override void OnUpdate()
	{
		if (GeneratedChunk == null)
		{
			if ( ChunkProcUpdateTick.ElapsedSeconds > 0.1 )
			{
				PlayerDistanceCheck();
				ChunkProcUpdateTick = FastTimer.StartNew();
			}
		}


		if (!FinishedCollisionQuery)
		{
			ChunkCollisionQuery();
		}
	}
}
