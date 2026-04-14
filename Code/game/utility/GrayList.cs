using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq.Expressions;
using System.Text;

namespace Sandbox.game.utility
{
	/// <summary>
	/// List that encompasses whitelist and blacklist behaviors.
	/// </summary>
	public sealed class GrayList<T> : List<T> 
	{
		public List<T> Parent;
		public bool IsWhitelist = true;
		public T SelectRandomPrefab( Random Random )
		{
			List<T> ValidList = new();
			if ( Parent.Count() > 0 ) {
				if ( IsWhitelist )
				{
					Log.Info( "fefefefe" );
						ValidList = Parent.Intersect( this ).ToList();
				}
				else
				{
					ValidList = Parent.Except( this ).ToList();

				}

				if ( ValidList.Count() > 0 )
				{
					return ValidList[Random.Next( ValidList.Count() - 1 )];
				}

				Log.Info( "Failed to retrieve item from graylist " + this + ", fallbacking to Parent Graylist." );

				try
				{
					return Parent[Random.Next( ValidList.Count() - 1 )];
				}
				catch ( IndexOutOfRangeException )
				{
					Log.Warning( "parent Graylist " + Parent + " is empty. Returning default." );
					return default( T );
				}
			} 

			// No parent items so just ignore
			else
			{
				if ( IsWhitelist )
				{
					return this[Random.Next( ValidList.Count() - 1 )];
				}
				else
				{

					Log.Info( "Trying to blacklist items in an empty list. Returning Default" );
					return default( T );
				}
			}
		}
	

	public bool IsValid()
		{


			return true;
		}
	}
}
