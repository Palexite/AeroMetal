using Sandbox;

public class TrafficNode : Component
{
	[Property] int LaneIndex { get; set; }
	[Property] GameObject NextNode { get; set; }
	[Property] float FinalSpeedMultiplier { get; set; } = 1f;
}
