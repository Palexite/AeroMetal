using Sandbox;
using Sandbox.Diagnostics;
using Sandbox.Utility.BBox2D;
using System.Runtime.ConstrainedExecution;

public sealed class Chunk : Component, ISceneMetadata
{

	/// <summary>
	/// The difficulty of this chunk for the player to move across. This will scale it's weight by the difficulty scaling.
	/// </summary>
	[Property] public float Difficulty { get; set; } = 1;

	/// <summary>
	/// The likelyhood that this chunk will spawn over other chunks.
	/// </summary>
	[Property] public float Weight { get; set; } = 1;

	/// <summary>
	/// The BBoxes we are defining for this chunk in the case of generation.
	/// </summary>
	[Property] public List<BBox> BBoxes { get; set; } = new List<BBox>();

	/// <summary>
	/// The boundary instances that we own in the game.
	/// </summary>

	public IEnumerable<BBox> BBoxInstances;
	public ChunkPoint LastChunkPoint { get; set; }

	/// <summary>
	/// References to all the chunk points in this Chunk. Needs to be filled in manually for faster lookup times. This will payoff in the long-run.
	/// </summary>
	[Property, Group("References")]
	public LaneComponent[] LaneComponents { get; set; }

	[Property, Group( "References" )]
	public ChunkPoint[] ChunkPoints { get; set; }

	private StageMain StageMain;

	private ChunkSystem ChunkSystem;


	// Set to a high number so when checked against the first player in the index determining the closest distance, their value is guarenteed to override.
	private float ClosestPlayerDistance = 2147483648;
	private byte CurrentPlayerCheckIndex { get; set; } = 0;

	private int ChunkGOHeight = 0;




	public Dictionary<string, string> GetMetadata()
	{
		return new Dictionary<string, string>
		{
			{"Chunk_Weight", Weight.ToString() },
			{"Chunk_Difficulty",  Difficulty.ToString()}

		};

	}

	// Checking to see if we're too far away from any player to the point where we should cull ourselves.

	protected override void OnStart()
	{
		StageMain = Scene.Directory.FindByName( "Scene Information" ).First().GetComponent<StageMain>();
		ChunkSystem = Scene.Directory.FindByName( "ChunkSystem" ).First().GetComponent<ChunkSystem>();
	}

	private void PlayerDistanceCheckFinal()
	{
		if (ClosestPlayerDistance > (StageMain.ChunkGCDistance * StageMain.ChunkGCDistance))
		{

			this.GameObject.Destroy();
			foreach ( var b in BBoxInstances ) {
				ChunkSystem.ChunkBBoxes.Remove( b );
			}
		}
	}


	protected override void OnDestroy()
	{
		

	}
	// Running checks regarding the player's distance. This process takes multiple ticks and takes longer depending on the number of players.
	private void PlayerDistanceCheck()
	{
	if (!LastChunkPoint.IsValid() && StageMain.Players != null)
		{
			var PlayerList = StageMain.Players.ToList();
			var PlayerGO = PlayerList[CurrentPlayerCheckIndex].GameObject;

			if ( PlayerGO != null )
			{
				var Distance = PlayerGO.WorldPosition.DistanceSquared(this.WorldPosition);
				if (Distance < ClosestPlayerDistance)
				{
					ClosestPlayerDistance = Distance;
				}

				CurrentPlayerCheckIndex += 1;

				if (CurrentPlayerCheckIndex >= PlayerList.Count)
				{
					PlayerDistanceCheckFinal();
					CurrentPlayerCheckIndex = 0;
					ClosestPlayerDistance = 2147483648;
				}

			}
		}

	}
	protected override void OnValidate()
	{
		ChunkGOHeight = this.GameObject.GetLocalBounds().Maxs.z.CeilToInt();
	}
	protected override void OnUpdate()
	{
		PlayerDistanceCheck();
	}


	protected override void DrawGizmos()
	{
		if ( Gizmo.IsSelected && BBoxes.Count() != 0 )
		{
			foreach ( var bounds in BBoxes )
			{
				Gizmo.Draw.LineThickness = 8f;
				Gizmo.Draw.Color = Color.Yellow.WithAlpha( Gizmo.IsSelected ? 1f : 0.2f );
				Gizmo.Draw.LineBBox(bounds);
			}

		}
	}
}
