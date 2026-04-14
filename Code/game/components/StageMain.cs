using Sandbox;
using Sandbox.Diagnostics;
using Sandbox.game.utility;
using System;
/// <summary>
/// Acts as a scene's information in regards to the game, as well as handling special gameplay procedures.
/// </summary>
public sealed class StageMain : Component
{

	[Property] public PrefabFile[] ChunkPrefabs { get; set; } = Array.Empty<PrefabFile>();
	[Property] public PrefabFile[] StartingChunks { get; set; } = Array.Empty<PrefabFile>();

	[Property] public string GameType { get; set; } = "GameType_DeathMatch";
	/// <summary>
	/// Difficulty of the stage.
	/// </summary>
	[Property] public float DifficultyScale { get; set; } = 1.0f;

	/// <summary>
	/// Maximum vehicles in the scene at any given point.
	/// </summary> 
	[Property] public int MaxVehicles { get; set; } = 64;

	/// <summary>
	/// Vehicles allowed in this stage. This takes authority over other vehicle whitelists that handles game-induced spawning.
	/// (e.g. If a vehicle is not whitelisted here but is in a Lane Component, it will not spawn.)
	/// </summary>

	// Literally a dummy because SBox doesn't like new datatypes in it's component panel for some reason.
	[Property, ToggleGroup("Vehicle")] public string[] VehicleSpawnCategories { get; set; }
	[Property, ToggleGroup( "Vehicle" )] public bool VehicleSpawnCategories_IsWhitelist { get; set; }
	public static GrayList<PrefabFile> VehicleSpawnGrayList = new();

	[Property] public int TrafficMPH { get; set; } = 64;
	[Property] public float TrafficDensity { get; set; } = 1;
	[Property] public int ChunkProcDistance { get; set; } = 8192;

	[Property] public int ChunkGCDistance { get; set; } = 16384;

	[Property] public GameObject[] PlayerObjects { get; set; }

	public ChunkSystem ChunkSystem;
	public IEnumerable<PlayerController> Players { get; set; }
	 private FastTimer PlayerScanTimer { get; set; }
	protected override async void OnStart()
	{
		VehicleRegistry.RegisterVehicles();

		SetupGrayList();

		// Some stuff isn't loaded on the first frame at this point, so just wait.

		await GameTask.DelaySeconds(.1f );


		ChunkSystem = Scene.Directory.FindByName( "ChunkSystem" ).First().GetComponent<ChunkSystem>();
		Log.Info( " Chunk System Located" );

		var gt = Components.Create( TypeLibrary.GetType(this.GameType));
		Log.Info( "Creating Starting Chunk" );

		var RandChunk = this.StartingChunks[ChunkSystem.ChunkRandom.Next( 0, this.StartingChunks.Length - 1 )];


		//Creating the first chunk
		var NewChunk = ChunkSystem.CreateChunk(RandChunk, null);

		PlayerScanTimer = FastTimer.StartNew();

		Log.Info( "Starting Chunk " + NewChunk +" successfully created" );
	}

	// Scanning for players that we're acknowledging within the current stage. This method is subject to change.

	/// <summary>
	/// Setup List of vehicles that this stage will use.
	/// </summary>
	private void SetupGrayList()
	{
		if(VehicleSpawnCategories.Count() != 0)
		{
			foreach ( var cat in VehicleSpawnCategories )
			{
				VehicleSpawnGrayList.AddRange( (GrayList<PrefabFile>)VehicleRegistry.GetVehiclesOfCategory( cat, false ) );
			}
		}
		else
		{
			VehicleSpawnGrayList.AddRange( VehicleRegistry.Vehicles );
		}
	}
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
