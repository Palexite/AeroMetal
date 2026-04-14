using Sandbox;
using System;

public sealed class TrafficSystem : Component
{

	public static List<LaneComponent> Lanes { get; set; }

	public static Random TrafficRandom = new Random();
	protected override void OnUpdate()
	{

	}
	/// <summary>
	/// Resolves lanes between two different chunks via chunkpoints.
	/// </summary>
	/// <param name="Chunk"></param>
	public static void ResolveChunkLaneRoutes( Chunk Chunk)
	{
		
		var PreceedingLanes = Chunk.LaneComponents;

		foreach ( var PreceedLane in PreceedingLanes )
		{
			var ResChunkPoint = PreceedLane.ResolvingChunkPoint;
			
			if ( ResChunkPoint != null )
			{
				var ChunkObject = ResChunkPoint.GeneratedChunk;
				var ChunkComp = ChunkObject.GetComponent<Chunk>();

				foreach ( var SucceedLane in ChunkComp.LaneComponents )
				{
					if ( SucceedLane.IsStartingRoute && SucceedLane.LaneIndex == PreceedLane.LaneIndex )
					{
						PreceedLane.Routes.Add( SucceedLane );
						break;
					}

				}
			}
		}
	}

	protected override void OnStart()
	{

	}
}

