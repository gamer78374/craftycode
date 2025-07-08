using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CraftyCode
{
#if DEBUG
	public
#endif
	class CraftyStack
	{
		public CraftyStack ( int capacity )
		{
			if ( capacity > 0 )
			{
				for ( int i = 0; i < capacity; i++ )
				{
					stack.Add( new StackValue( ) );
				}
			}
		}

		public CraftyStack() : this( 24 ) { }

		int count = 0;
		public int Count
		{
			get { return count; }
			private set
			{
				count = value; /*topValue = Count > 0 ? stack[Count - 1] : null;*/
			}
		}

		StackValue topValue
		{
			get { return stack[count - 1]; }
		}

		List<StackValue> stack = new List<StackValue>( );

		public void Push ( float f )
		{
			if (Count == stack.Count){
				for ( int i = 0; i < 10; i++ )
				{
					stack.Add( new StackValue( ) );
				}
			}
			stack[Count].FloatValue = f;
			Count++;
		}

		public void Push ( bool b )
		{
			if ( Count == stack.Count )
			{
				for ( int i = 0; i < 10; i++ )
				{
					stack.Add( new StackValue( ) );
				}
			}
			stack[Count].BooleanValue = b ;
			Count++;
		}

		public void Push ( string s )
		{
			if ( count == stack.Count )
			{
				for ( int i = 0; i < 10; i++ )
				{
					stack.Add( new StackValue( ) );
				}
			}
			stack[Count].StringValue = s;
			count++;
		}

		public void Push( Symbol s )
		{
			if (count == stack.Count)
			{
				for (int i = 0; i < 10; i++)
				{
					stack.Add(new StackValue());
				}
			}
			//StackValue v = stack[Count];
			
			if (s.IsNumeric)
			{
				stack[count].FloatValue = s.FloatValue;
			}
			else if (s.IsBool)
			{
				stack[count].BooleanValue = s.BooleanValue;
			}
			else if (s.IsString)
			{
				stack[count].StringValue = s.StringValue;
			}
			
			/*
			stack[count].FloatValue = s.FloatValue;
			stack[count].BooleanValue = s.BooleanValue;
			stack[count].StringValue = s.StringValue;
			*/
			//stack[Count] = v;
			count++;
		}

		public void PeekSymbol ( Symbol s )
		{
			if (count == 0)
			{
				throw new CraftyException("Nothing on the stack.", null, s);
			}
			if ( s.IsBool )
			{
				s.BooleanValue = stack[count - 1].BooleanValue;
			}
			else if ( s.IsNumeric )
			{
				s.FloatValue = stack[count - 1].FloatValue;
			}
			else if (s.IsString)
			{
				s.StringValue = stack[count - 1].StringValue;
			}
			else
			{
				throw new CraftyException("Can't peek at symbol.");
			}
		}

		public float PeekFloat ( )
		{
			if ( count <= 0 )
			{
				throw new IndexOutOfRangeException( "No items on the stack." );
			}
			return stack[count - 1].FloatValue;
		}

		public string PeekString ( )
		{
			if ( count <= 0 )
			{
				throw new IndexOutOfRangeException( "No items on the stack." );
			}
			return stack[count - 1].StringValue;
		}

		public bool PeekBoolean ( )
		{
			if ( count <= 0 )
			{
				throw new IndexOutOfRangeException( "No items on the stack." );
			}
			return stack[count - 1].BooleanValue;
		}

		public float PopFloat ( )
		{
			if ( count <= 0 )
			{
				throw new IndexOutOfRangeException( "No items on the stack." );
			}
			float f = stack[count - 1].FloatValue;
			count--;
			return f;
		}

		public bool PopBoolean ( )
		{
			if ( count <= 0 )
			{
				throw new IndexOutOfRangeException( "No items on the stack." );
			}
			bool b = stack[count - 1].BooleanValue;
			count--;
			return b;
		}

		public string PopString ( )
		{
			if ( count <= 0 )
			{
				throw new IndexOutOfRangeException( "No items on the stack." );
			}
			string s = stack[count - 1].StringValue;
			count--;
			return s;
		}

		public void NegateBoolean ( )
		{
			if (count <= 0)
			{
				throw new IndexOutOfRangeException("No items on the stack.");
			}
			stack[count - 1].BooleanValue = !stack[count - 1].BooleanValue;
		}

		public void IncrementFloat ( )
		{
			if (count <= 0)
			{
				throw new IndexOutOfRangeException("No items on the stack.");
			}
			stack[count - 1].FloatValue++;
		}

		public void DecrementFloat ( )
		{
			if (count <= 0)
			{
				throw new IndexOutOfRangeException("No items on the stack.");
			}
			stack[count - 1].FloatValue--;
		}

		public void Multiply ( )
		{
			if (count <= 0)
			{
				throw new IndexOutOfRangeException("No items on the stack.");
			}
			float f2 = PopFloat( );
			float f1 = PopFloat( );
			Push( f1 * f2 );
		}

		public void Divide ( )
		{
			if (count <= 0)
			{
				throw new IndexOutOfRangeException("No items on the stack.");
			}
			float f2 = PopFloat( );
			float f1 = PopFloat( );
			Push( f1 / f2 );
		}

		public void Add ( )
		{
			if (count <= 0)
			{
				throw new IndexOutOfRangeException("No items on the stack.");
			}
			float f2 = PopFloat( );
			float f1 = PopFloat( );
			Push( f1 + f2 );
		}

		public void Subtract ( )
		{
			if (count <= 0)
			{
				throw new IndexOutOfRangeException("No items on the stack.");
			}
			float f2 = PopFloat( );
			float f1 = PopFloat( );
			Push( f1 - f2 );
		}

		public StackValue this[int index]
		{
			get
			{
				if ( index >= stack.Count || index < 0 )
				{
					throw new IndexOutOfRangeException( );
				}
				return stack[index];
			}
		}

		public void Discard ( )
		{
			if (count <= 0)
			{
				throw new IndexOutOfRangeException("No items on the stack.");
			}
			count--;
		}

		public void Clear ( )
		{
			count = 0;
		}
	}

	public class ReadonlyCraftyStack 
	{
		public ReadonlyCraftyStack ( CraftyStack stack )
		{
			theStack = stack;
		}

		public StackValue this[int index]
		{
			get
			{
				return theStack[index];
			}
		}

		public float PeekFloat ( )
		{
			return theStack.PeekFloat( );
		}

		public bool PeekBool ( )
		{
			return theStack.PeekBoolean( );
		}

		public string PeekString ( )
		{
			return theStack.PeekString( );
		}

		public int Count
		{
			get { return theStack.Count; }
		}

		CraftyStack theStack = null;
	}

#if DEBUG
	public
#endif
	class StackValue
	{
		public float FloatValue;
		public bool BooleanValue;
		public string StringValue;
	}
}
