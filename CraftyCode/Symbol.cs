using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CraftyCode
{
#if DEBUG
	public class Symbol
#else
	internal class Symbol
#endif
	{
		public string Name;
		public string Type;

		public float FloatValue
		{
			get { return floatValue; }
			set { floatValue = value; }
		}
		public string StringValue
		{
			get { return stringValue; }
			set { stringValue = value; }
		}
		
		public string FunctionReturnType
		{
			get { return functionType; }
			set { functionType = value; }
		}

		public string FunctionName
		{
			get { return functionName; }
			set { functionName = value; }
		}

		public Operation FunctionStartOp
		{
			get;
			set;
		}

		public string EventName {
			get { return eventName; }
			set { eventName = value; }
		}

		public Symbol EventSymbol 
		{
			get { return eventParent; }
			set { eventParent = value; }
		}

		public bool BooleanValue
		{
			get { return booleanValue; }
			set { booleanValue = value; }
		}

		float floatValue;
		string stringValue;
		bool booleanValue;
		string functionType;
		string functionName;
		string eventName;
		Symbol eventParent;

		/// <summary>
		/// Declares a function symbol.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="type"></param>
		public Symbol ( string s, string type )
		{
			functionName = s;
			functionType = type;
			Type = "function";
		}

		public Symbol ( string s )
		{
			Type = "string";
			//Name = n;
			StringValue = s;
			IsString = true;
		}

		public Symbol ( bool b )
		{
			Type = "bool";
			//Name = n;
			BooleanValue = b;
			IsBool = true;
		}

		public Symbol ( float f )
		{
			Type = "float";
			//Name = n;
			FloatValue = f;
			IsNumeric = true;
		}

		public Symbol ( )
		{
			Type = "void";
		}

		/// <summary>
		/// Event symbol.
		/// </summary>
		/// <param name="s">The symbol this event belongs to.</param>
		public Symbol ( Symbol s, string name )
		{
			Type = "event";
			EventSymbol = s;
			EventName = name;
		}

		public bool IsBool;

		public bool IsNumeric;

		public bool IsString;

		public bool IsVoid
		{
			get { return Type == "void"; }
		}

		public bool IsFunction
		{
			get { return functionName != null; }
		}

		public bool IsActive;
	}
}
