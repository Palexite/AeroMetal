using Sandbox;

public class TrafficNode : Component
{
	[Property] int LaneIndex { get; set; }
	[Property] GameObject NextNode { get; set; }
	[Property] float FinalSpeedMultiplier { get; set; } = 1f;
	[Property] bool EnablePhysics { get; set; }



	protected override void DrawGizmos()
	{
		Gizmo.Draw.LineThickness = 4f;
		Gizmo.Draw.Color = Color.White.WithAlpha( Gizmo.IsSelected ? 1f : 0.2f );
		Gizmo.Draw.Color = Gizmo.Colors.Blue.WithAlpha( Gizmo.IsSelected ? 1f : 0.2f );
		Gizmo.Draw.SolidBox( new BBox( new Vector3( -32, -32, -32 ), new Vector3( 16, 16, 16 ) ) );

		if ( NextNode != null ) {
			Gizmo.Draw.Line(Vector3.Zero, NextNode.WorldPosition - this.WorldPosition );
		}
		if ( Gizmo.IsSelected)
		{
		}
	}
}
