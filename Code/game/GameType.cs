using Sandbox;
using System.Runtime.Serialization;

public class GameType : Component
{
	public virtual void OnGameStart()
	{
	}

	public virtual void OnGameEnd()
	{
	}

	public virtual void OnPlayerDeath()
	{
	}

	public virtual void OnPlayerHurt()
	{
	}
}

