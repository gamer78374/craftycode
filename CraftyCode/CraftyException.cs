using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CraftyCode
{
	/// <summary>
	/// The exception that is thrown for a number of reasons.
	/// </summary>
	public class CraftyException : Exception
	{
		public CraftyException ( string message ) : base( message ) { }
		public CraftyException ( string message, Exception inner ) : base( message, inner ) { }

		internal CraftyException ( string message, Token t )
			: base( FormatMessageForToken( message, t ) )
		{

		}

		internal CraftyException ( string message, Symbol s ) : base( FormatMessageForToken( message, null, s ) ) { }

		internal CraftyException ( string message, Token t, Symbol s )
			: base( FormatMessageForToken( message, t, s ) )
		{

		}

		private static string FormatMessageForToken ( string m, Token t )
		{
			string message = m + string.Format( "\r\nToken {0} on line {1}.", t == null ? "???" : t.Name, t == null ? "?" : t.Line.ToString( ) );
			return message;
		}

		private static string FormatMessageForToken ( string m, Token t, Symbol s )
		{
			string message = m + string.Format( "\r\nSymbol {0} of type {1}{2}.", s.Name, s.Type, t == null ? "" : " on line " + t.Line.ToString( ) );//t.TokenNumber
			if ( s != null )
			{
				message += "\r\n";
				message += string.Format( "Type: {0}.\r\n", s.Type );
				if ( s.IsNumeric )
				{
					message += string.Format( "Value: {0}.", s.FloatValue );
				}
				else if ( s.IsBool )
				{
					message += string.Format( "Value: {0}.", s.BooleanValue );
				}
				else if ( s.IsString )
				{
					message += string.Format( "Value: {0}.", s.StringValue );
				}
				else if ( s.IsVoid )
				{

				}
				else if ( s.IsFunction )
				{
					message += string.Format( "Returns: {0}.", s.FunctionReturnType );
				}
			}
			return message;
		}
	}

	/// <summary>
	/// An exception that occurs when there is an internal failure with the parser.
	/// These exceptions should be reported to the author of CraftyCode.
	/// </summary>
	public class InternalCraftyException : CraftyException
	{
		public InternalCraftyException ( string message )
			: base( message )
		{

		}
				public InternalCraftyException ( string message, Exception inner )
			: base( message, inner )
		{

		}

		public InternalCraftyException ( Exception inner )
			: base(string.Format( "An internal error occured: {0}", inner.ToString()), inner )
		{

		}
	}
}
