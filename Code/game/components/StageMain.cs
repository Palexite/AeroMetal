using Sandbox;
using Sandbox.Diagnostics;
using System;

public sealed class StageMain : Component
{

	[Property] public PrefabFile[] ChunkPrefabs { get; set; } = Array.Empty<PrefabFile>();
	[Property] public PrefabFile[] StartingChunks { get; set; }= Array.Empty<PrefabFile>();

	[Property] public string GameType { get; set; } = "GameType_DeathMatch";
	[Property] public float DifficultyScale { get; set; } = 1.0f;
	[Property] public int MaxVehicles { get; set; } = 64;

	[Property] public int TrafficMPH { get; set; } = 64;
	[Property] public float TrafficDensity { get; set; } = 1;
	[Property] public int ChunkProcDistance { get; set; } = 8192;

	[Property] public int ChunkGCDistance { get; set; } = 16384;

	[Property] public GameObject[] PlayerObjects { get; set; }

	public ChunkSystem ChunkSystem;
	public IEnumerable<PlayerController> Players { get; set; }
	 private FastTimer PlayerScanTimer { get; set; }
	protected override void OnStart()
	{

		//await GameTask.DelaySeconds(.1f );
		ChunkSystem = Scene.Directory.FindByName( "ChunkSystem" ).First().GetComponent<ChunkSystem>();
		Log.Info( " Chunk System Located" );

		var gt = Components.Create( TypeLibrary.GetType(this.GameType));
		Log.Info( "Creating Starting Chunk" );

		Log.Info( ChunkSystem.ChunkRandom );

		var RandChunk = this.StartingChunks[ChunkSystem.ChunkRandom.Next( 0, this.StartingChunks.Length - 1 )];


		//Creating the first chunk
		var NewChunk = ChunkSystem.CreateChunk(RandChunk, null);

		PlayerScanTimer = FastTimer.StartNew();

		Log.Info( "Starting Chunk " + NewChunk +" successfully created" );
	}

	// Scanning for players that we're acknowledging within the current stage. This method is subject to change.
	private void PlayerObjectCheck()
	{
		if ( PlayerScanTimer.ElapsedSeconds > 2 )
		{
			Players = Scene.GetAll<PlayerController>();
			PlayerScanTimer = FastTimer.StartNew();
		}
	}

	protected override void OnUpdate()
	{
		PlayerObjectCheck();
	}
}
