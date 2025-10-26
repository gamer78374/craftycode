using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CraftyCode
{
	/// <summary>
	/// Represents an opcode.
	/// </summary>
	public class Operation
	{
		public OpCode Code;
		public Operation JumpToOperation;
		public Operation Next;
		public Operation Previous;

		public Symbol Symbol = null;
		public string SymbolName = null;

		public int Number = -1;

		protected string stringValue = null;
		public string StringValue
		{
			get
			{
				return Symbol == null ? stringValue : Symbol.StringValue;
			}
		}
		
		protected bool boolValue;
		public bool BoolValue
		{
			get
			{
				return Symbol == null ? boolValue : Symbol.BooleanValue;
			}
		}
		
		protected float floatValue;
		public float FloatValue
		{
			get
			{
				return Symbol == null ? floatValue : Symbol.FloatValue;
			}
		}

		public Operation ( OpCode c )
		{
			Code = c;
		}

		public Operation ( OpCode c, string s )
			: this( c )
		{
			stringValue = s;
		}

		public Operation ( OpCode c, Operation operation )
			: this( c )
		{
			if ( c == OpCode.Jump && operation.Code != OpCode.JumpTarget )
			{
				//throw new CraftyException( "Jump opcode must point to a jump target." );
			}
			JumpToOperation = operation;
		}

		public Operation ( OpCode c, bool b )
			: this( c )
		{
			boolValue = b;
		}

		public Operation ( OpCode c, float f )
			: this( c )
		{
			floatValue = f;
		}

		public Operation ( OpCode c, Symbol s )
			: this( c )
		{
			Symbol = s;
			SymbolName = Symbol.Name;
		}
	}

	/// <summary>
	/// Virtual machine operations.
	/// </summary>
	public enum OpCode : byte
	{
		/// <summary>
		/// Perform no operation.
		/// </summary>
		NoOperation,
		
		/// <summary>
		/// Pushes a boolean literal onto the stack.
		/// </summary>
		PushBoolean,
	
		/// <summary>
		/// Pushes a string literal onto the stack.
		/// </summary>
		PushString,
	
		/// <summary>
		/// Pushes a float literal onto the stack.
		/// </summary>
		PushFloat,
		
		/// <summary>
		/// Push a symbol onto the stack.
		/// </summary>
		PushSymbol,
		
		/// <summary>
		/// Stores the value at the top of the stack in the symbol.
		/// </summary>
		Store,
		
		/// <summary>
		/// Gets the value at the top of the stack. Useless?
		/// </summary>
		GetTop,
	
		/// <summary>
		/// Discards the value at the top of the stack.
		/// </summary>
		Discard,
	
		/// <summary>
		/// Unconditionally jump to the specified Operation.
		/// </summary>
		Jump,
		
		/// <summary>
		/// Jump if the value at the top of the stack is false.
		/// </summary>
		JumpIfFalse,
		
		/// <summary>
		/// Jump if the value at the top of the stack is true.
		/// </summary>
		JumpIfTrue,
		
		/// <summary>
		/// Marks a place for return from an <see cref="EndFunctionBlock"/>.
		/// </summary>
		DogEarCallStack,

		/// <summary>
		/// Takes two float values off the top of the stack and compares them, then pops the result onto the stack.
		/// </summary>
		FloatEqualTo,
		
		/// <summary>
		/// Takes two float values off the top of the stack and compares them, then pops the result onto the stack.
		/// </summary>
		FloatGreaterThan,
		
		/// <summary>
		/// Takes two float values off the top of the stack and compares them, then pops the result onto the stack.
		/// </summary>
		FloatLessThan,
		
		/// <summary>
		/// Takes two float values off the top of the stack and compares them, then pops the result onto the stack.
		/// </summary>
		FloatGreaterOrEqualTo,
		
		/// <summary>
		/// Takes two float values off the top of the stack and compares them, then pops the result onto the stack.
		/// </summary>
		FloatLessOrEqualTo,
		
		/// <summary>
		/// Takes two float values off the top of the stack and compares them, then pops the result onto the stack.
		/// </summary>
		FloatNotEqualTo,
		
		/// <summary>
		/// Takes two float values off the top of the stack and compares them, then pops the result onto the stack.
		/// </summary>
		StringEqualTo,
		
		/// <summary>
		/// A pseudo code used to designate a jump target.
		/// </summary>
		JumpTarget,
		
		/// <summary>
		/// Discards the boolean at the top of the stack and pushes its negated value onto the stack.
		/// </summary>
		NegateBoolean,
		
		/// <summary>
		/// Increments the value at the top of the stack.
		/// </summary>
		Increment,
		
		/// <summary>
		/// Decrements the value at the top of the stack.
		/// </summary>
		Decrement,
		
		/// <summary>
		/// Glues to strings together.
		/// </summary>
		Concecrate, 
		
		/// <summary>
		/// Pops two floats from the top of the stack and multiples them, and pushes the result onto the stack.
		/// </summary>
		Multiply,
		
		/// <summary>
		/// Pops two floats from the top of the stack and adds them, and pushes the result onto the stack.
		/// </summary>
		Add,
		
		/// <summary>
		/// Pops two floats from the top of the stack and subtracts them, and pushes the result onto the stack.
		/// </summary>
		Subtract,
		SubtractReverse,
	
		/// <summary>
		/// Pops two floats from the top of the stack and divides them, and pushes the result onto the stack.
		/// </summary>
		Divide,
		DivideReverse,
		

		Modulus, 
		ModulusReverse,
		
		/// <summary>
		/// Makes a negative float positive, or a postive float negative.
		/// </summary>
		InvertFloat,
		
		/// <summary>
		/// Swaps the first and second float registers.
		/// </summary>
		SwapFloatRegisters,
		
		/// <summary>
		/// Pops two booleans from the stack and pushes true if both are true.
		/// </summary>
		LogicalAnd,
		
		/// <summary>
		/// Pops two booleans from the stack and pushes true if either are true.
		/// </summary>
		LogicalOr,
		
		/// <summary>
		/// Clears the stack.
		/// </summary>
		ClearStack, 
		
		/// <summary>
		/// Declares a new symbol.
		/// </summary>
		InitSymbol,
		
		/// <summary>
		/// Marks the start of a block.
		/// </summary>
		StartBlock,
		
		/// <summary>
		/// Marks the end of a block.
		/// </summary>
		EndBlock,
		
		/// <summary>
		/// Instructs the runtime environment that the following block may not be interrupted.
		/// </summary>
		StartSolidBlock,
		
		/// <summary>
		/// Instructs te runtime environment that the solid block as ended.
		/// </summary>
		EndSolidBlock,
		
		/// <summary>
		/// Marks the start of a function body.
		/// </summary>
		StartFunctionBlock,
		
		/// <summary>
		/// Marks the end of a function body.
		/// </summary>
		EndFunctionBlock,

		/// <summary>
		/// A pseudo operation. Used for debugging.
		/// </summary>
		DHook
	}
}
