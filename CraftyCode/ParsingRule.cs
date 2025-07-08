using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CraftyCode
{
#if DEBUG
	public class ParsingRule
#else 
		internal class ParsingRule
#endif
	{
		public static ParsingRule CreatePlainRule ( string input )
		{
			return new ParsingRule( input );
		}

		public static ParsingRule CreatePlainRule ( string input, string returnType )
		{
			return new ParsingRule( input, returnType, false, false );
		}

		public static ParsingRule CreateBlockRule ( string input )
		{
			return new ParsingRule( input, null, true, false );
		}

		public static ParsingRule CreateWildCardBlock ( string input )
		{
			return new ParsingRule( input, null, false, true );
		}

		public static ParsingRule CreateWildCardBlock ( string input, string returnType )
		{
			return new ParsingRule( input, returnType, false, true );
		}
		
		/*
		public ParsingRule ( string input, string compatibleTypes, bool domelt )
			: this( input, compatibleTypes )
		{
			DoMelt = domelt;
		}

		public ParsingRule ( string input, bool domelt )
			: this( input )
		{
			DoMelt = domelt;
		}
		*/

		private ParsingRule ( string input, string returnType, bool blockRule, bool wildCard )
			: this( input )
		{
			if ( returnType != null )
			{
				if ( Token.IsValidVariableType( returnType ) == null )
				{
					throw new InternalCraftyException( string.Format( "{0} is not a valid variable type.", returnType ) );
				}
				else
				{
					ReturnType = returnType;
				}
			}
			
			IsBlock = blockRule;
			IsWildCard = wildCard;
		}
		
		public ParsingRule ( string input, string returnType )
			: this( input, returnType, false, false )
		{

		}

		public ParsingRule ( string input )
		{
			OriginalInput = input;
			//Parent = parent;

			if ( Regex.IsMatch( input, @"[^ a-z0-9_\.\+\*\?|]", RegexOptions.IgnoreCase ) )
			{
				throw new CraftyException(string.Format( "Invalid parsing rule string '{0}'.", input ));
			}
			//input = Regex.Replace(input, "["
			List<string> parts = new List<string>( input.Split( ' ' ) );

			//int current = 0;
			List<ParsingStep> currentGroup = new List<ParsingStep>( );
			//bool doingOrSub = false;

			for ( int i = 0; i < parts.Count; i++ )
			{
				if ( parts[i] == "|" )
				{
					options.Add( currentGroup );
					currentGroup = new List<ParsingStep>( );
				}
				else
				{
					currentGroup.Add( new ParsingStep( parts[i] ) );
				}
			}

			options.Add( currentGroup );
		}

		public bool DoMelt { get; private set; }

		public bool IsBlock { get; private set; }
		public bool isWildCard = false;
		public bool IsWildCard
		{
			get { return isWildCard; }
			private set
			{
				isWildCard = value;
				if ( isWildCard )
				{
					DoMelt = true;
				}
				else
				{
					DoMelt = false;
				}
			}
		}

		string returnType = null;
		public string ReturnType
		{
			get { return returnType; }
			set { returnType = value; }
		}

		public bool ReturnsType ( string t )
		{
			return ReturnType == t;
		}

		public bool HasReturnType
		{
			get { return ReturnType != null; }
		}

		public string Name = "";
		public readonly string OriginalInput = null;

		public ParsingStep[] GetOptionSteps ( int optionNum )
		{
			return options[optionNum].ToArray( );
		}

		public ParsingStep GetOptionStep ( int optionNum )
		{
			return options[optionNum][0];
		}

		public bool IsStepSinglePart ( int stepnum )
		{
			return options[stepnum].Count == 1;
		}

		public int NumberOfOptions
		{
			get { return options.Count; }
		}

		public ParsingStep this[int groupNumber, int itemNumber]
		{
			get { return options[groupNumber][itemNumber]; }
		}

		List<List<ParsingStep>> options = new List<List<ParsingStep>>( );
	}

#if DEBUG
	public class ParsingStep
#else
	internal class ParsingStep
#endif
	{
		public ParsingStep ( string step )
		{
			step = step.Trim( );
			string value = null;
			//string type = null;
			if ( step.Contains( '.' ) )
			{
				List<string> parts = new List<string>( step.Split( '.' ) );
				value = parts[0];
				parts.RemoveAt( 0 );
				OfTypes = parts.ToArray( );
				StringBuilder sb = new StringBuilder( );
				for ( int i = 0; i < OfTypes.Length; i++ )
				{
					sb.Append( OfTypes[i] );
					if ( i < OfTypes.Length - 1 )
					{
						sb.Append( '.' );
					}
				}
				OfTypesString = sb.ToString( );
				step = value;
			}
			else
			{
				value = step;
			}

			if ( step.EndsWith( "+" ) )
			{
				Value = step.Substring( 0, step.Length - 1 );
				Repeat = true;
			}
			else if ( step.EndsWith( "?" ) )
			{
				Value = step.Substring( 0, step.Length - 1 );
				Optional = true;
			}
			else if ( step.EndsWith( "*" ) )
			{
				Value = step.Substring( 0, step.Length - 1 );
				Optional = true;
				Repeat = true;
			}
			else
			{
				Value = step;
			}
		}

		public readonly string Value;
		public readonly bool Repeat;
		public readonly bool Optional;
		readonly string[] OfTypes;

		public readonly string OfTypesString;
		
		public bool HasReturnType ( string t )
		{
			return OfTypes.Contains( t.ToLower( ) );
		}

		public string GetReturnType ( )
		{
			return OfTypes[0];
		}

		public bool HasReturnTypes
		{
			get { return OfTypes != null && OfTypes.Length > 0; }
		}
	}
}
