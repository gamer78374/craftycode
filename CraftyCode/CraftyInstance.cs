using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Collections;

namespace CraftyCode
{
	public class CraftyInstance
	{
		public CraftyInstance ( )
		{

		}

		List<Token> tokens = null;

		public void Compile ( string input, string outputname )
		{
			tokens = new List<Token>( Lex( input ) );
			Parse( tokens.ToArray( ) );

			FileInfo info = new FileInfo( outputname );

			using ( StreamWriter re = info.CreateText( ) )
			{
				re.AutoFlush = true;
				re.Write( input );
			}
		}

		public void OutputRules ( string folder )
		{
			throw new NotImplementedException( );
			folder = Path.Combine( folder, "CraftyRules" );
			if ( !Directory.Exists( folder ) )
			{
				Directory.CreateDirectory( folder );
			}
			foreach ( KeyValuePair<string, ParsingRule> kvp in parser.GetRules( ) )
			{

			}
		}

		public void PrintRules ( )
		{
			if ( parser != null )
			{
				int displayed = 0;
				foreach ( KeyValuePair<string, ParsingRule> kvp in parser.GetRules( ) )
				{
					Console.WriteLine( "{0}", kvp.Key.PadRight( 24, ' ' ) );
					displayed++;
					for ( int i = 0; i < kvp.Value.NumberOfOptions; i++ )
					{
						Console.Write( "".PadLeft( 4 ) );
						foreach ( ParsingStep ps in kvp.Value.GetOptionSteps( i ) )
						{
							Console.Write( "{0}{1}{2}{3}{4} ", ps.Value, ps.HasReturnTypes ? "." + ps.OfTypesString : "", ( ps.Repeat && ps.Optional ? "*" : "" ), ( ps.Repeat && !ps.Optional ? "+" : "" ), ( ps.Optional && !ps.Repeat ? "?" : "" ) );
						}
						if ( i == kvp.Value.NumberOfOptions - 1 )
						{
							Console.WriteLine( );
						}
						else
						{
							//Console.WriteLine( "|" );
							Console.WriteLine( );
						}
						displayed++;
						//Console.WriteLine("".PadLeft(4)+"{0}", kvp.Value.GetOptionSteps(
					}
					//Console.WriteLine("".PadLeft(4)+"{0}", kvp.Value.OriginalInput );
					Console.WriteLine( );
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

		/*
		public TreeBranch ProgramRoot
		{
			get { return parser != null ? parser.ProgramRoot : null; }
		}
		*/
		public void PrintOpCodes ( ) {
			if ( parser != null )
			{
				int displayed = 0;
				int currentinstruction = 0;
				int opcodeLength = 0;
				foreach ( Operation o in parser.GetOpCodes( ) )
				{
					if ( o.Code.ToString( ).Length > opcodeLength )
					{
						opcodeLength = o.Code.ToString( ).Length;
					}
				}

				opcodeLength += 6;

				foreach ( Operation o in parser.GetOpCodes( ) )
				{
					if ( o == null )
					{
						Console.Write( "?\t(NULL)" );
					}
					else
					{
						Console.Write( o.Number + "\t" );
						Console.Write( o.Code.ToString( ).PadRight(opcodeLength) );
						//Console.Write( "\t" );
						if ( o.Code == OpCode.PushSymbol || o.Code == OpCode.Store)
						{
							Console.Write( o.Symbol.Name + " (" + o.Symbol.Type + ")" );
						}
						else if ( o.Code == OpCode.PushBoolean )
						{
							Console.Write( o.BoolValue );
						}
						else if ( o.Code == OpCode.PushFloat )
						{
							Console.Write( o.FloatValue );
						}
						else if ( o.StringValue != null && (o.Code == OpCode.PushString || o.Code == OpCode.Dummy || o.Code == OpCode.NoOperation ))
						{
							Console.Write( o.StringValue );
						}
						else if ( o.Code == OpCode.Jump || o.Code == OpCode.JumpIfFalse || o.Code == OpCode.JumpIfTrue )
						{
							if ( o.JumpToOperation == null ) {
								Console.Write( "(Unknown Instruction)" );
							}
							else
							{
								Console.Write( o.JumpToOperation.Number );
							}
						}
					}
					Console.WriteLine( );
					displayed++;
					if ( displayed >= Console.WindowHeight - 1 )
					{
						Console.Write( "More..." );
						Console.ReadLine( );
						displayed = 0;
					}
					currentinstruction++;
				}
				Console.WriteLine( "End." );
			}
		}

		public void PrintSymbolTable ( )
		{
			if ( parser != null )
			{
				int displayed = 0;
				foreach ( Symbol s in parser.ProgramRoot.GetSymbols( ) )
				{
					Console.Write( "".PadRight( Math.Max( 12 - s.Type.Length, 0 ), ' ' ) + s.Type + "\t" + s.Name + "".PadRight( Math.Max( 16 - s.Name.Length, 0 ), ' ' ) + "\t" );
					if ( s.IsBool )
					{
						Console.WriteLine( s.BooleanValue );
					}
					else if ( s.IsFunction )
					{
						Console.WriteLine( s.FunctionReturnType );
					}
					else if ( s.IsNumeric )
					{
						Console.WriteLine( s.FloatValue );
					}
					else if ( s.IsString )
					{
						Console.WriteLine( "\"" + s.StringValue + "\"" );
					}
					else if ( s.IsVoid )
					{
						Console.WriteLine( "void" );
					}
					else
					{
						Console.WriteLine( "unknown" );
					}
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

		public void PrintSyntaxTree ( )
		{
			if ( parser != null )
			{
				int displayed = 0;
				foreach ( TreeBranch b in parser.ProgramRoot.GetBranches( true ) )
				{
					if ( b.BranchRule != null )
					{
						Console.WriteLine( "".PadRight( b.Level * 2, ' ' ) + "{0}", b.BranchRule.Name );
						displayed++;
					}
					else
					{
						Console.WriteLine( "".PadRight( b.Level * 2, ' ' ) + "{0}", b.BranchToken.Name );
						displayed++;
					}
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

		

		public void PrintTokens ( )
		{
			if ( tokens != null )
			{
				int displayed = 0;
				foreach ( Token t in tokens )
				{
					if ( t.Line < 1 ) { continue; }
					Console.Write( "{0}\t{1}\t {2}", t.Line, t.TokenNumber, t.Name );
					if ( t.Name == "STRING" || t.IsIdentifier || t.IsTypeIdentifier )
					{
						Console.Write( ":  {0}", t.StringValue );
					}
					else if ( t.Name == "FLOAT" )
					{
						Console.Write( ":  {0}", t.FloatValue );
					}
					Console.WriteLine( );
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

		private static class TokenCollection
		{
			static TokenCollection ( )
			{
				/*
				TokenCollection.AddToken( new Token( "NEWLINE", @"\r\n", TokenOptions.DontEmitToken ) );
				TokenCollection.AddToken( new Token( "NEWLINE", @"\n", TokenOptions.DontEmitToken ) );
				TokenCollection.AddToken( new Token( "WHITESPACE", @" \t", TokenOptions.DontEmitToken ) );
				TokenCollection.AddToken( new Token( "if" ) );
				TokenCollection.AddToken( new Token( "else" ) );
				TokenCollection.AddToken( new Token( "while" ) );
				TokenCollection.AddToken( new Token( "do" ) );
				TokenCollection.AddToken( new Token( "foreach" ) );
				TokenCollection.AddToken( new Token( "for" ) );
				TokenCollection.AddToken( new Token( "increment", "++" ) );
				TokenCollection.AddToken( new Token( "decrement", "--" ) );
				TokenCollection.AddToken( new Token( "NOTEQUAL", "!=" ) );
				TokenCollection.AddToken( new Token( "EQUITY", "==" ) );
				TokenCollection.AddToken( new Token( "LESSTHANOREQUAL", "<=" ) );
				TokenCollection.AddToken( new Token( "GREATERTHANOREQUAL", ">=" ) );
				TokenCollection.AddToken( new Token( "LESSTHAN", "<" ) );
				TokenCollection.AddToken( new Token( "GREATERTHAN", ">" ) );
				TokenCollection.AddToken( new Token( "ASSIGN", "=" ) );
				TokenCollection.AddToken( new Token( "NOT", "!" ) );
				TokenCollection.AddToken( new Token( "LINECOMMENT", "//", TokenOptions.DontEmitToken ) );
				TokenCollection.AddToken( new Token( "TIMES", "*" ) );
				TokenCollection.AddToken( new Token( "PLUS", "+" ) );
				TokenCollection.AddToken( new Token( "DIVIDE", "/" ) );
				TokenCollection.AddToken( new Token( "MINUS", "-" ) );

				TokenCollection.AddToken( new Token( "CURLYOPEN", "{" ) );
				TokenCollection.AddToken( new Token( "CURLYCLOSE", "}" ) );
				TokenCollection.AddToken( new Token( "OPENBRACKET", "(" ) );
				TokenCollection.AddToken( new Token( "CLOSEBRACKET", ")" ) );

				TokenCollection.AddToken( new Token( "\"" ) );
				TokenCollection.AddToken( new Token( "'" ) );

				TokenCollection.AddToken( new Token( "FLOAT", "[0-9]+(.[0-9]+)?", TokenOptions.DontEscapePattern | TokenOptions.EmitMatch ) );
				*/
				/*
				patterns.Add( @"\r\n", "NEWLINE" );
				patterns.Add( @"\n", "NEWLINE" );
				patterns.Add( @"[ \t]", "WHITESPACE" );
				//patterns.Add( "[\r\n]", "WHITESPACE" );
				patterns.Add( "if", "IF" );
				patterns.Add( "else", "ELSE" );
				patterns.Add( "while", "WHILE" );
				patterns.Add( "do", "DO" );
				patterns.Add( "for", "FOR" );
				patterns.Add( "foreach", "FOREACH" );
				patterns.Add( @"\+\+", "INCREMENT" );
				patterns.Add( @"\-\-", "DECREMENT" );
				patterns.Add( "!=", "NOTEQUAL" );
				patterns.Add( "==", "EQUITY" );
				patterns.Add( "<=", "LESSTHANOREQUAL" );
				patterns.Add( ">=", "GREATERTHANOREQUAL" );
				patterns.Add( "<", "LESSTHAN" );
				patterns.Add( ">", "GREATERTHAN" );
				patterns.Add( "=", "ASSIGN" );
				patterns.Add( "!", "NOT" );
				patterns.Add( "//", "LINECOMMENT" );
				patterns.Add( @"\+", "PLUS" );
				patterns.Add( @"\-", "MINUS" );
				patterns.Add( @"\/", "DIVIDE" );
				patterns.Add( @"\*", "TIMES" );
				patterns.Add( @"\{", "CURLYOPEN" );
				patterns.Add( @"\}", "CURLYCLOSE" );
				patterns.Add( @"\(", "OPENBRACKET" );
				patterns.Add( @"\)", "CLOSEBRACKET" );
				patterns.Add( "\"", "\"" );
				patterns.Add( "'", "\'" );
				patterns.Add( @"[0-9]+(\.[0-9]+)?", "FLOAT" );
				 * */
			}

			static List<Token> tokens = new List<Token>( );

			private static void AddToken ( Token t )
			{
				tokens.Add( t );
			}

			public static bool IsValidToken ( string value )
			{
				for ( int i = 0; i < tokens.Count; i++ )
				{
					//if ( tokens[i].Value == value ) { return true; }
				}
				return false;
			}
		}

		[Flags]
		enum TokenOptions { None = 0, DontEmitToken = 1, EmitMatch = 2, FixedLengthMatch = 4, DontEscapePattern = 8 }

		List<string> validTokens = new List<string>( );
		Dictionary<int, int> tokenOnLine = new Dictionary<int, int>( );

		private Token[] Lex ( string input )
		{
			Console.Write( "Performating lexicon operation... " );
			//StringBuilder sb = new StringBuilder( );
			List<Token> tokens = new List<Token>( );
			tokens.Add( new Token( "ENDSTATEMENT", 0, 0 ) );
			Dictionary<string, string> patterns = new Dictionary<string, string>( );
			validTokens = new List<string>( );
			patterns.Add( @"\r\n", "NEWLINE" );
			patterns.Add( @"\n", "NEWLINE" );
			patterns.Add( @"[ \t]", "WHITESPACE" );
			patterns.Add( @"\-?[a-z][a-z0-9]*", "IDENTIFIER" );
			patterns.Add( @"\-?[0-9]+(\.[0-9]+)?", "FLOAT" );
			patterns.Add( @"\(", "OPENBRACKET" );
			patterns.Add( @"\)", "CLOSEBRACKET" );
			patterns.Add( @"\/\*", "LONGCOMMENT" );
			patterns.Add( "and", "AND" );
			patterns.Add( "or", "OR" );
			patterns.Add( "&&", "AND" );
			patterns.Add( @"\|\|", "OR" );
			patterns.Add( "true", "TRUE" );
			patterns.Add( "false", "FALSE" );
			patterns.Add( "if", "IF" );
			patterns.Add( "when", "WHEN" );
			patterns.Add( "solid", "SOLID" );
			patterns.Add( "return", "RETURN" );
			patterns.Add( "else", "ELSE" );
			patterns.Add( "while", "WHILE" );
			patterns.Add( "in", "IN" );
			patterns.Add( "throw", "THROW" );
			patterns.Add( "catch", "CATCH" );
			patterns.Add( "try", "TRY" );
			patterns.Add( "finally", "FINALLY" );
			patterns.Add( "new", "NEW" );
			patterns.Add( "do", "DO" );
			patterns.Add( "foreach", "FOREACH" );
			patterns.Add( "for", "FOR" );
			patterns.Add( @"\+?infinity", "POSITIVEINFINITY" );
			patterns.Add( @"\-infinity", "NEGATIVEINFINITY" );
			patterns.Add( "∞", "POSITIVEINFINITY" );
			patterns.Add( Regex.Escape( "-∞" ), "NEGATIVEINFINITY" );
			patterns.Add( Regex.Escape( "+∞" ), "POSITIVEINFINITY" );
			patterns.Add( "NaN", "NAN" );

			patterns.Add( @"\+\+", "INCREMENT" );
			patterns.Add( @"\-\-", "DECREMENT" );
			patterns.Add( Regex.Escape( @"->" ), "POINTRIGHT" );
			patterns.Add( "!=", "NOTEQUAL" );
			patterns.Add( "==", "EQUITY" );
			patterns.Add( "<=", "LESSTHANOREQUAL" );
			patterns.Add( ">=", "GREATERTHANOREQUAL" );
			patterns.Add( "<", "LESSTHAN" );
			patterns.Add( ">", "GREATERTHAN" );
			patterns.Add( "=", "ASSIGN" );
			patterns.Add( ",", "COMMA" );
			patterns.Add( "!", "NOT" );
			patterns.Add( Regex.Escape( "&" ), "AMPERSAND" );
			patterns.Add( Regex.Escape( "^" ), "CARET" );
			patterns.Add( "//", "LINECOMMENT" );
			patterns.Add( @"\+=", "PLUSEQUAL" );
			patterns.Add( @"\-=", "MINUSEQUAL" );
			patterns.Add( @"\/=", "DIVIDEEQUAL" );
			patterns.Add( @"\*=", "TIMESEQUAL" );
			patterns.Add( @"%=", "MODEQUAL" );
			patterns.Add( @"\+", "PLUS" );
			patterns.Add( @"\-", "MINUS" );
			patterns.Add( @"\/", "DIVIDE" );
			patterns.Add( @"\*", "TIMES" );
			patterns.Add( @"%", "MOD" );
			patterns.Add( @"\{", "CURLYOPEN" );
			patterns.Add( @"\}", "CURLYCLOSE" );

			patterns.Add( "\"", "\"" );
			patterns.Add( "'", "\'" );

			//patterns.Add( @"[0-9]+[^\.]", "INTEGER" );
			//patterns.Add( @"\$[a-z][a-z0-9]*", "VID" );
			patterns.Add( ";", "ENDSTATEMENT" );
			patterns.Add( "*", "ILLEGAL" );

			//patterns.

			foreach ( KeyValuePair<string, string> kvp in patterns )
			{
				validTokens.Add( kvp.Value );
			}

			validTokens.Add( "STRING" );
			validTokens.Add( "TYPEIDENTIFIER" );
			
			Dictionary<int, int> tokenOnLine = new Dictionary<int, int>( );

			int start = 0;
			int current = 0;
			string tryString = null;
			int length = 0;
			//int matchLength = 0;
			int lineNumber = 1;
			float tryFloat = 0f;
			int tryInt = 0;
			string tryMatch = null;
			bool tryResult = false;
			int commentLevel = 0;
			ASCIIEncoding encoder = new ASCIIEncoding( );

			IEnumerable<KeyValuePair<string, string>> patternQuery = from kvp in patterns select kvp; //orderby kvp.Key.Length descending

			while ( true )
			{
				tryMatch = input.Substring( start );
				foreach ( KeyValuePair<string, string> kvp in patternQuery )
				{
					if ( Regex.IsMatch( tryMatch, "^" + kvp.Key, RegexOptions.IgnoreCase ) )
					{
						tryString = Regex.Match( tryMatch, kvp.Key, RegexOptions.IgnoreCase ).Value;

						if ( kvp.Value == "ILLEGAL" )
						{
							throw new CraftyException( string.Format( "Illegal string \"{0}\" on line number {1}.", tryString, lineNumber ) );
						}

						if ( kvp.Value == "IDENTIFIER" )
						{
							foreach ( KeyValuePair<string, string> subKvp in patterns )
							{
								if ( Regex.IsMatch( subKvp.Key, "[^a-z0-9]", RegexOptions.IgnoreCase ) )
								{
									continue;
								}
								if ( tryString == subKvp.Key )
								{
									goto InvalidID;
								}
							}
							goto ValidID;
						InvalidID:
							continue;
						ValidID:
							//tokens.Add( "IDENTIFIER" );
							tokens.Add( new Token( "IDENTIFIER", tokens.Count + 1, lineNumber, tryString ) );
							//sb.Append( "IDENTIFIER " );
							//tokens.Add( tryString );
							//sb.Append( tryString + "" );
						}
						else if ( kvp.Value == "INTEGER" )
						{
							tryResult = int.TryParse( tryString, out tryInt);
							if ( !tryResult )
							{
								throw new CraftyException( "Malformed integer." );
							}
							tokens.Add( new Token( "FLOAT", tokens.Count + 1, lineNumber, (int) tryInt ) );
							
							//tokens.Add( "INTEGER" );
							//tokens.Add( new Token( "INTEGER", tokens.Count + 1, lineNumber ) );
							//sb.Append( tryString + "" );
						}
						else if ( kvp.Value == "FLOAT" )
						{
							tryResult = float.TryParse( tryString, out tryFloat );
							if ( !tryResult )
							{
								throw new CraftyException( "Malformed float." );
							}
							tokens.Add( new Token( "FLOAT", tokens.Count + 1, lineNumber, tryFloat ) );
							//sb.Append( tryString + "" );
						}
						else if ( kvp.Value == "\"" || kvp.Value == "\'" )
						{
							//tokens.Add( new Token( "STRING", lineNumber, kvp.Value ) );
							//tokens.Add( "STRING" + kvp.Value );
							//sb.Append( tryString + "" );
						}
						else if ( kvp.Value == "LINECOMMENT" )
						{
							//lineNumber++;
						}
						else if ( kvp.Value == "WHITESPACE" || kvp.Value == "NEWLINE" )
						{
							if ( kvp.Value == "NEWLINE" )
							{
								lineNumber++;
							}
						}
						else if ( kvp.Value == "LONGCOMMENT" )
						{
							commentLevel++;
						}
						else
						{
							//tokens.Add( kvp.Value );
							tokens.Add( new Token( kvp.Value, tokens.Count + 1, lineNumber ) );
							/*if ( kvp.Value == "FOR" )
							{
								int kkk = 0;
							}*/
						}

						start += tryString.Length;
						length = 0;
						current = start;
						if ( kvp.Value == "LONGCOMMENT" )
						{
							while ( commentLevel > 0 )
							{
								current++;
								length++;
								if ( input.Substring( current, 2 ) == "/*" )
								{
									commentLevel++;
								}
								else if ( input.Substring( current, 2 ) == "*/" )
								{
									commentLevel--;
								}
							}
							start++;
							start += length;
							start++;
						}
						else if ( kvp.Key == "\"" || kvp.Key == "\'" )
						{
							while ( !Regex.IsMatch( input.Substring( current, 1 ), kvp.Key, RegexOptions.IgnoreCase ) )
							{
								current++;
								length++;
								if ( current + 1 >= input.Length )
								{
									break;
								}
							}
							tokens.Add( new Token( "STRING", tokens.Count + 1, lineNumber, input.Substring( start, length ) ) );
							//sb.Append( input.Substring( start, length ) );
							start += length;
							start++;
							//sb.Append( kvp.Value + "" );
						}
						else if ( kvp.Value == "LINECOMMENT" )
						{
							while ( !Regex.IsMatch( input.Substring( current, 1 ), "[\r\n]" ) )
							{
								current++;
								length++;
								if ( current + 1 >= input.Length )
								{
									break;
								}
							}

							//sb.Append( input.Substring( start, length ) );
							start += length;
							start++;
						}

						/*if ( sb.Length > 0 && sb[sb.Length - 1] != ' ' )
						{
							sb.Append( " " );
						}
						*/
						break;
					}
				}
				if ( !( start < input.Length ) )
				{
					break;
				}
			}

			Console.WriteLine( "done." );

			for ( int i = 0; i < tokens.Count; i++ )
			{

			}

			return tokens.ToArray( );
		}

#if DEBUG
		public Parser Parser { get { return parser; } }
#endif
		Parser parser = null;

		private void Parse ( Token[] input )
		{
			Console.Write( "Parsing... " );

			try
			{
				parser = new Parser( input, validTokens );
				parser.Parse( );
			}
			catch ( CraftyException exception )
			{
				parser = null;
				throw exception;
			}

			Console.WriteLine( "done." );
		}
	}
}