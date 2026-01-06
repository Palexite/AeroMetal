using Sandbox;
using Sandbox.Diagnostics;
using Sandbox.game.utility;
using Sandbox.Utility.BBox2D;
using System.Runtime.ConstrainedExecution;

public sealed class Chunk : Component, ISceneMetadata
{
	[Property] public int Difficulty { get; set; } = 1;
	[Property] public float Weight { get; set; } = 1;
	[Property] public List<Rect> BBoxes { get; set; } = new List<Rect>();
	

	public ChunkPoint LastChunkPoint { get; set; }

	private StageMain StageMain;

	private StageSettings StageSettings;




	// Set to a high number so when checked against the first player in the index determining the closest distance, their value is guarenteed to override.
	private float ClosestPlayerDistance = 2147483648;
	private byte CurrentPlayerCheckIndex { get; set; } = 0;

	private int ChunkGOHeight = 0;




	public Dictionary<string, string> GetMetadata()
	{
		var str = Rect_Utils.RectListToString( BBoxes );
		return new Dictionary<string, string>
		{

			{"Chunk_Weight", Weight.ToString() },
			{"Chunk_BBounds", str},
			{"Chunk_Difficulty",  Difficulty.ToString()}

		};

	}

	// Checking to see if we're too far away from any player to the point where we should cull ourselves.
	private void PlayerDistanceCheckFinal()
	{
		if (ClosestPlayerDistance > (StageSettings.ChunkGCDistance * StageSettings.ChunkGCDistance))
		{

			this.GameObject.Destroy();

		}
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
	protected override void OnStart()
	{
		StageMain = Scene.Directory.FindByName("StageMain").First().GetComponent<StageMain>();
		StageSettings = Scene.Directory.FindByName( "StageSettings" ).First().GetComponent<StageSettings>();
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
				Gizmo.Draw.LineBBox( new BBox( new Vector3(bounds.BottomLeft.x, bounds.BottomLeft.y, 0 ), new Vector3( bounds.TopRight.x, bounds.TopRight.y, ChunkGOHeight ) ) );
			}

		}
	}
}
