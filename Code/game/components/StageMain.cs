using Sandbox;
using Sandbox.Diagnostics;
using System;

public sealed class StageSettings : Component
{
	[Property] public string Stage { get; set; } = "Stage_Dev01";
	[Property] public string GameType { get; set; } = "GameType_DeathMatch";
	[Property] public float DifficultyScale { get; set; } = 1.0f;
	[Property] public int MaxVehicles { get; set; } = 64;

	[Property] public int TrafficMPH { get; set; } = 64;
	[Property] public float TrafficDensity { get; set; } = 1;
	[Property] public int ChunkProcDistance { get; set; } = 8192;

	[Property] public int ChunkGCDistance { get; set; } = 16384;

	[Property] public GameObject[] PlayerObjects { get; set; }
}

public sealed class StageMain : Component
{
	public StageSettings StageSettings;
	public ChunkSystem ChunkSystem;
	public Stage StageObject;
	public IEnumerable<PlayerController> Players { get; set; }
	 private FastTimer PlayerScanTimer { get; set; }
	protected override void OnStart()
	{
		ChunkSystem = Scene.Directory.FindByName( "ChunkSystem" ).First().GetComponent<ChunkSystem>();
		Log.Info( " Chunk System Located" );
		StageSettings = Scene.Directory.FindByName( "StageSettings" ).First().GetComponent<StageSettings>();
		Log.Info( "Stage Settings Located" );

		// Create gametype under our own GameObject.
		StageObject = TypeLibrary.Create<Stage>(StageSettings.Stage);
		Log.Info( "Stage " + StageObject + " Created." );

		var gt = Components.Create( TypeLibrary.GetType(StageSettings.GameType));
		Log.Info( "Creating Starting Chunk." );

		var RandChunk = this.StageObject.ChunkList[ChunkSystem.ChunkRandom.Next( 0, this.StageObject.ChunkList.Count() - 1 )];

		// Creating the first chunk
		ChunkSystem.CreateChunk(RandChunk, null);

		PlayerScanTimer = FastTimer.StartNew();
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
