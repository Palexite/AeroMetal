using Sandbox;
using static Sandbox.Component;
using Sandbox.game.interfaces;

public class NPCController : Component, IMortal
{

	public float MaxHealth = 100;
	public float Health = 100;
	public bool isDead = false;
	protected override void OnUpdate()
	{

	}

	public void OnDamage(in DamageInfo DMGInfo)
	{

	}

	public void OnHeal( int Amount)
	{

	}

	public void OnDeath()
	{

	}
}
