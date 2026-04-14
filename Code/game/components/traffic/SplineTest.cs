using Sandbox;

public sealed class SplineTest : Component
{
	[Property] public SplineComponent SplineComp;
	protected override void OnStart()
	{
		var Samp = SplineComp.Spline.SampleAtClosestPosition((this.WorldPosition - SplineComp.WorldPosition).RotateAround(Vector3.Zero, SplineComp.WorldRotation.Inverse) );
		this.WorldPosition = (Samp.Position + SplineComp.WorldPosition).RotateAround( SplineComp.WorldPosition, SplineComp.WorldRotation );
	}
}
