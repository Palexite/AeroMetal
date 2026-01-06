using Sandbox.Modals;
using Sandbox.Utility.BBox2D;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandbox.game.utility
{
	public static class Rect_Utils
	{
		public static string RectListToString( List<Rect> Rects )
		{
			StringBuilder BBoxArrayString = new StringBuilder();
			foreach ( Rect Rect in Rects )
			{
				BBoxArrayString.AppendFormat( "{0}, {1}, {2}, {3}, ", Rect.BottomLeft.x, Rect.BottomLeft.y, Rect.TopRight.x, Rect.TopRight.y );
			}
			BBoxArrayString.Remove( BBoxArrayString.Length - 2, 1 );
			return BBoxArrayString.ToString();
		}

		public static Rect AddVector3ToRect( Rect rect, Vector3 vector )
		{
			return Rect.FromPoints(new Vector2(rect.BottomLeft.x + vector.x, rect.BottomLeft.y + vector.y), new Vector2(rect.TopRight.x + vector.x, rect.TopRight.y + vector.y));

		}

		public static List<Rect> StringToRectList( string str )
		{
			var numbers = str.Split( "," );
			var FinalBBox2DList = new List<Rect>();
			for ( int i = 0; i < (numbers.Length / 4); i++ )
			{
				var num1 = numbers[i].ToFloat( 0 );
				var num2 = numbers[i + 1].ToFloat( 0 );
				var num3 = numbers[i + 2].ToFloat( 0 );
				var num4 = numbers[i + 3].ToFloat( 0 );
				var newRect = Rect.FromPoints(new Vector2(num1, num2), new Vector2(num3, num4));
				FinalBBox2DList.Append( newRect );
			}
			;
			return FinalBBox2DList;
		}
	}
}
