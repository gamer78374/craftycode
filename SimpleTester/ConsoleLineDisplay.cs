using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleTester
{
	class ConsoleLineDisplay
	{
		public static void PrintLines ( params string[] lines )
		{
			int displayed = 0;
			for ( int i = 0; i < lines.Length; i++ )
			{
				Console.WriteLine( lines[ i ] );
				displayed++;
				if ( displayed >= Console.WindowHeight - 1 )
				{
					Console.Write( "More..." );
					Console.ReadLine( );
					displayed = 0;
				}
			}
			Console.WriteLine( "End." );
		}

		public ConsoleLineDisplay ( )
		{

		}

		public ConsoleLineDisplay ( params string[] startlines )
		{
			lines.AddRange( startlines );
		}

		List<string> lines = new List<string>( );

		public IList<string> Lines
		{
			get { return lines; }
		}

		public void PrintLines ( )
		{
			int displayed = 0;
			for ( int i = 0; i < lines.Count; i++ )
			{
				Console.WriteLine( lines[ i ] );
				displayed++;
				if ( displayed >= Console.WindowHeight - 1 )
				{
					Console.Write( "More..." );
					Console.ReadLine( );
					displayed = 0;
				}
			}
			Console.WriteLine( "End." );
		}
	}
}