using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CraftyCode
{
#if DEBUG
	public class Token
#else
	internal class Token
#endif
	{
		public static List<TokenPattern> _patterns = null;
		
		public static List<TokenPattern> TokenPatterns 
		{
			get
			{
				if ( _patterns == null )
				{
					_patterns = new List<TokenPattern>( GetPatterns( ) );
				}

				return _patterns;
			}
		}

		public static bool IsTokenValid(string token_name)
		{
			foreach (TokenPattern patterns in TokenPatterns)
			{
				if ( patterns.TokenText == token_name ) { return true; }
			}

			return false;
		}

		private static TokenPattern[] GetPatterns ( )
		{
			List<TokenPattern> patterns = new List<TokenPattern>( );

			patterns.Add( new TokenPattern( @"\r\n", "NEW_LINE" ) );
			patterns.Add( new TokenPattern( @"\n", "NEW_LINE" ));
			patterns.Add( new TokenPattern( @"[ \t]", "WHITESPACE") );
			patterns.Add( new TokenPattern( @"\-?[a-z][a-z0-9]*", "IDENTIFIER" ) );
			patterns.Add( new TokenPattern( @"\-?[0-9]+(\.[0-9]+)?", "FLOAT" ) );
			patterns.Add( new TokenPattern( @"\(", "BRACKET_OPEN" ) );
			patterns.Add( new TokenPattern( @"\)", "BRACKET_CLOSE" ) );
			patterns.Add( new TokenPattern( @"\/\*", "LONG_COMMENT" ) );
			patterns.Add( new TokenPattern( "and", "AND" ) );
			patterns.Add( new TokenPattern( "or", "OR" ) );
			patterns.Add( new TokenPattern( "&&", "AND" ) );
			patterns.Add( new TokenPattern( @"\|\|", "OR" ) );
			patterns.Add( new TokenPattern( "true", "TRUE" ) );
			patterns.Add( new TokenPattern( "false", "FALSE") );
			patterns.Add( new TokenPattern( "if", "IF" ) );
			patterns.Add( new TokenPattern( "when", "WHEN") );
			patterns.Add( new TokenPattern( "solid", "SOLID") );
			patterns.Add( new TokenPattern( "return", "RETURN") );
			patterns.Add( new TokenPattern( "else", "ELSE" ) );
			patterns.Add( new TokenPattern( "while", "WHILE" ) );
			patterns.Add( new TokenPattern( "in", "IN" ) );
			patterns.Add( new TokenPattern( "throw", "THROW") );
			patterns.Add( new TokenPattern( "catch", "CATCH" ) );
			patterns.Add( new TokenPattern( "try", "TRY" ) );
			patterns.Add( new TokenPattern( "finally", "FINALLY" ) );
			patterns.Add( new TokenPattern( "new", "NEW" ) );
			patterns.Add( new TokenPattern( "do", "DO" ) );
			patterns.Add( new TokenPattern( "foreach", "FOREACH") );
			patterns.Add( new TokenPattern( "for", "FOR" ) );
			patterns.Add( new TokenPattern( @"\+?infinity", "POSITIVE_INFINITY") );
			patterns.Add( new TokenPattern( @"\-infinity", "NEGATIVE_INFINITY") );
			patterns.Add( new TokenPattern( "∞", "POSITIVEINFINITY" ) );
			patterns.Add( new TokenPattern( Regex.Escape( "-∞" ), "NEGATIVE_INFINITY" ) );
			patterns.Add( new TokenPattern( Regex.Escape( "+∞" ), "POSITIVE_INFINITY" ) );
			patterns.Add( new TokenPattern( "NaN", "NAN" ) );
			patterns.Add( new TokenPattern( @"\+\+", "INCREMENT" ) );
			patterns.Add( new TokenPattern( @"\-\-", "DECREMENT" ) );
			patterns.Add( new TokenPattern( Regex.Escape( @"->" ), "POINT_RIGHT" ) );
			patterns.Add( new TokenPattern( "!=", "NOT_EQUAL" ) );
			patterns.Add( new TokenPattern( "==", "EQUALITY" ) );
			patterns.Add( new TokenPattern( "<=", "LESS_THAN_OR_EQUAL" ) );
			patterns.Add( new TokenPattern( ">=", "GREATER_THAN_OR_EQUAL") );
			patterns.Add( new TokenPattern( "<", "LESS_THAN" ) );
			patterns.Add( new TokenPattern( ">", "GREATER_THAN") );
			patterns.Add( new TokenPattern( "=", "ASSIGN" ) );
			patterns.Add( new TokenPattern( ",", "COMMA" ) );
			patterns.Add( new TokenPattern( "!", "NOT" ) );
			patterns.Add( new TokenPattern( Regex.Escape( "&" ), "AMPERSAND" ) );
			patterns.Add( new TokenPattern( Regex.Escape( "^" ), "CARET" ) );
			patterns.Add( new TokenPattern( "//", "LINE_COMMENT" ) );
			patterns.Add( new TokenPattern( @"\+=", "PLUS_EQUAL" ) );
			patterns.Add( new TokenPattern( @"\-=", "MINUS_EQUAL") );
			patterns.Add( new TokenPattern( @"\/=", "DIVIDE_EQUAL") );
			patterns.Add( new TokenPattern( @"\*=", "TIMES_EQUAL") );
			patterns.Add( new TokenPattern( @"%=", "MOD_EQUAL") );
			patterns.Add( new TokenPattern( @"\+", "PLUS" ) );
			patterns.Add( new TokenPattern( @"\-", "MINUS" ) );
			patterns.Add( new TokenPattern( @"\/", "DIVIDE") );
			patterns.Add( new TokenPattern( @"\*", "TIMES") );
			patterns.Add( new TokenPattern( @"%", "MOD" ) );
			patterns.Add( new TokenPattern( @"\{", "CURLY_OPEN" ) );
			patterns.Add( new TokenPattern( @"\}", "CURLY_CLOSE" ) );
			patterns.Add( new TokenPattern( "\"", "\"" ) );
			patterns.Add( new TokenPattern( "'", "\'" ) );
			patterns.Add( new TokenPattern( ";", "END_STATEMENT") );
			patterns.Add( new TokenPattern( "*", "ILLEGAL" ) );
			patterns.Add( new TokenPattern( "", "STRING" ) );
			patterns.Add( new TokenPattern( "", "TYPE_IDENTIFIER" ) );

			return patterns.ToArray( );
		}

		private Token ( string tokenName, int tnumber, int line, float? fvalue, string svalue )
		{
			Name = tokenName;
			Line = line;
			IsTypeIdentifier = false;
			IsIdentifier = false;
			TokenNumber = tnumber;

			if ( fvalue.HasValue )
			{
				FloatValue = fvalue.Value;
			}
			if ( svalue != null )
			{
				StringValue = svalue;
			}

			if ( Name == "IDENTIFIER" )
			{
				if ( Token.IsValidVariableType( StringValue ) != null )
				{
					IsTypeIdentifier = true;
					Name = "TYPE_IDENTIFIER";
					StringValue = Token.IsValidVariableType( StringValue );
				}
				if ( !IsTypeIdentifier )
				{
					IsIdentifier = true;
				}
			}

			if ( Name == "TRUE" || Name == "FALSE" )
			{
				literalReturnType = "bool";
			}
			else if ( Name == "FLOAT" || Name == "INT" )
			{
				literalReturnType = "float";
			}
			else if ( Name == "STRING" )
			{
				literalReturnType = "string";
			}
		}

		public string ReturnType
		{
			get
			{
				if ( IsIdentifier )
				{
					return IdentifierSymbol == null ? null : IdentifierSymbol.Type;
				}
				else
				{
					return literalReturnType;
				}
			}
		}
		readonly string literalReturnType = null;
		Symbol symbol;
		public Symbol IdentifierSymbol
		{
			get { return symbol; }
			set
			{
				symbol = value;
			}
		}

		public Token ( string tokenName, int tnumber, int line )
			: this( tokenName, tnumber,  line, null, null )
		{

		}

		public Token ( string tokenName,int tnumber, int line, float value )
			: this( tokenName, tnumber,  line, value, null )
		{

		}

		public Token ( string tokenName,int tnumber, int line, string value )
			: this( tokenName, tnumber, line, null, value )
		{

		}

		public readonly int TokenNumber;
		public readonly int Line;
		public readonly string Name;
		public readonly float FloatValue;
		public readonly string StringValue;
		public readonly bool IsTypeIdentifier;
		public readonly bool IsIdentifier;
		//private readonly bool isSymbol;
		
		public bool IsSymbol
		{
			get
			{//isSymbol || 
				return IsTypeIdentifier || IsIdentifier;
			}
		}

		private const string VariableTypes = "int float string void bool event";
		private static Dictionary<string, string> pseudoTypes = null;
		public static string IsValidVariableType ( string t )
		{
			if ( pseudoTypes == null )
			{
				pseudoTypes = new Dictionary<string, string>( );
				pseudoTypes.Add( "int", "float" );
			}

			string[] matches = VariableTypes.Split( ' ' );
			for ( int i = 0; i < matches.Length; i++ )
			{
				if ( matches[i] == t )
				{
					if ( pseudoTypes.ContainsKey( t ) )
					{
						return pseudoTypes[t];
					}
					else
					{
						return t;
					}
				}
			}
			return null;
		}
	}
}
