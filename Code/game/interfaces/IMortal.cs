using Sandbox;

namespace Sandbox.game.interfaces
{
	public interface IMortal
	{
		void OnDamage( in DamageInfo DamageInfo );
		void OnHeal( int amount );
		void OnDeath();
	}
}
