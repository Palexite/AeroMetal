using Sandbox;

public static class MathExtended
{
	public static Rotation GetRotationBetweenVectors( in Vector3 Vec1, in Vector3 Vec2 )
	{
		Vector3 travelDirVector = (Vec2 - Vec1).Normal;

		if ( travelDirVector == Vector3.Zero ) return Rotation.Identity;

		Rotation FinalRotation = Rotation.LookAt( travelDirVector, Vector3.Up );

		return Rotation.Difference( Rotation.FromYaw( 90 ), FinalRotation );
	}
}
