using Sandbox.Utility.BBox2D;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;




namespace Sandbox.Utility.BBox2D
{
	public struct BBox2D
	{
		 public Vector2 Mins { get; set; }
		 public Vector2 Maxs { get; set; }

		public BBox2D( float MnX, float MnY, float MxX, float MxY )
		{
			Mins = new Vector2( MnX, MnY );
			Maxs = new Vector2( MxX, MxY );
		}

		public BBox2D( Vector2 Mn, Vector2 Mx )
		{
			Mins = Mn;
			Maxs = Mx;
		}

		public static BBox2D operator +(BBox2D BBox, Vector3 Vect)
		{
			var Vect3To2 = new Vector2( Vect.x, Vect.y );

			BBox.Mins += Vect3To2;
			BBox.Maxs += Vect3To2;
			return BBox;
		}

		
		public void AddVector3( Vector3 Vect )
		{
			this.Mins += Vect.x;
			this.Mins += Vect.y;
			this.Maxs += Vect.x;
			this.Maxs += Vect.y;
		}
	}
	public static class BBox2D_Utils
	{
		public static string BBox2DArrayToString( BBox2D[] BBoxes )
		{
			StringBuilder BBoxArrayString = new StringBuilder();
			foreach ( BBox2D bBox in BBoxes )
			{
				BBoxArrayString.AppendFormat( "{0}, {1}, {2}, {3}, ", bBox.Mins.x, bBox.Mins.y, bBox.Maxs.x, bBox.Maxs.y );
			}
			BBoxArrayString.Remove( BBoxArrayString.Length - 2 , 1 );
			return BBoxArrayString.ToString();
		}

		public static BBox2D[] StringToBBox2DArray(string str )
		{
			var numbers = str.Split(",");
			var FinalBBox2DArray = new BBox2D[(numbers.Count() / 2)];
			for ( int i = 0; i < (numbers.Length / 4); i++ )
			{
				var num1 = numbers[i].ToFloat(0);
				var num2 = numbers[i + 1].ToFloat( 0 );
				var num3 = numbers[i + 2].ToFloat( 0 );
				var num4 = numbers[i + 3].ToFloat( 0 );
				var newbbox = new BBox2D( num1, num2, num3, num4 );
				var newbbox2 = new BBox2D( num1 + 99999, num2, num3, num4 );
				FinalBBox2DArray[i] = newbbox;
				FinalBBox2DArray[i + 1] = newbbox2;
			}
			;
			return FinalBBox2DArray;
		}
	}

	}
