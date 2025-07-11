using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CraftyCode
{
#if DEBUG
	public class Token
#else
	internal class Token
#endif
	{
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
