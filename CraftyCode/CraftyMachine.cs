using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CraftyCode
{
	/// <summary>
	/// A single running instance of a <see cref="CraftyInstance"/>.
	/// </summary>
	public class CraftyMachine
	{
		List<CraftyContext> contexts = new List<CraftyContext>( );

		public CraftyMachine ( params Operation[] ops )
		{
			operationsList = new List<Operation>( );
			for ( int i = 0; i < ops.Length; i++ )
			{
				operations.AddLast( ops[i] );
				operationsList.Add( ops[i] );
			}

			//ClearSymbolTable( );
		}

		public CraftyMachine ( CraftyInstance instance ) : this( instance.Parser ) { }
		public CraftyMachine ( Parser parser ) : this( parser.ProgramRoot.Operations.ToArray( ) ) { }

		protected LinkedList<Operation> operations = new LinkedList<Operation>( );
		protected List<Operation> operationsList { get; private set; }

		protected bool executeDebug = false;
		public bool DebugMode
		{
			get { return executeDebug; }
			set { executeDebug = value; }
		}

		/**************************************
		 ********* CraftyContext code ***********
		 **************************************/

		protected Operation currentOp;
		protected Operation nextOp;
		//LinkedList<Operation> operations = null;

		protected CraftyStack variableStack = new CraftyStack( );
		protected Stack<Symbol> blockMarkers = new Stack<Symbol>( );
		protected float floatRegisterOne = 0f;
		protected float floatRegisterTwo = 0f;

		protected Stack<Operation> callStack = new Stack<Operation>( );

		protected Stack<List<Symbol>> symbolTables = new Stack<List<Symbol>>( );
		protected IList<Symbol> currentSymbolTable
		{
			get { return symbolTables.Count > 0 ? symbolTables.Peek( ) : null; }
		}

		/// <summary>
		/// If positive, interrupts are not allowed. If 0, interrupts are allowed.
		/// </summary>
		int interruptsAllowed = 0;

		public int Run ( )
		{
			return Run( -2 );
		}

		private void MarkSymbolTable ( )
		{
			Symbol mark = new Symbol( );
			mark.Name = "[mark]";
			currentSymbolTable.Add( mark );
			blockMarkers.Push( mark );
		}

		private bool SymbolInTable ( string name )
		{
			for ( int i = currentSymbolTable.Count - 1; i > 0; i-- )
			{
				if ( IsScopeMark( currentSymbolTable[i] ) ) { break; }
				if ( currentSymbolTable[i].Name == name )
				{
					return true;
				}
			}
			return false;
		}

		private int SymbolIndex ( string name )
		{
			return SymbolIndex( name, true );
		}

		private int SymbolIndex ( string name, bool currentScope )
		{
			for ( int i = currentSymbolTable.Count - 1; i > 0; i-- )
			{
				if ( currentScope && IsScopeMark( currentSymbolTable[i] ) ) { break; }
				if ( currentSymbolTable[i].Name == name )
				{
					return i;
				}
			}
			/*
			for ( int i = 0; i < symbols.Count; i++ )
			{
				if ( symbols[i].Name == name )
				{
					return i;
				}
			}*/
			return -1;
		}

		private void PushNewSymbolTable ( )
		{
			symbolTables.Push( new List<Symbol>( ) );
		}

		private void DiscardSymbolTable ( )
		{
			symbolTables.Pop( );
		}

		private void InitSymbol ( string name, string type )
		{
			Symbol symbol = null;
			int symbolIndex = SymbolIndex( name );
			if ( symbolIndex < 0 )
			{
				symbol = new Symbol( );
				symbol.Name = name;
				symbol.Type = type;
				currentSymbolTable.Add( symbol );
			}
			else
			{
				symbol = currentSymbolTable[symbolIndex];
				if ( symbolIndex != currentSymbolTable.Count - 1 )
				{
					Symbol temp = currentSymbolTable[symbolIndex];
					/*for ( int i = symbolIndex; i < symbols.Count; i++ )
					{
						if ( i + 1 == symbols.Count ) { break; }
						symbols[i] = symbols[i + 1];
					}
					symbols[symbols.Count - 1] = temp;*/
					currentSymbolTable.RemoveAt( symbolIndex );
					currentSymbolTable.Add( temp );
				}
			}

			if ( symbol.IsActive )
			{
				throw new CraftyException( string.Format( "Symbol of name {0} already initialized.", name ), currentSymbolTable[symbolIndex] );
			}
			else
			{
				symbol.IsActive = true;
				symbol.FunctionName = null;
				if ( type == "float" || type == "int" )
				{
					symbol.IsNumeric = true;
					symbol.IsString = false;
					symbol.IsBool = false;
				}
				else if ( type == "bool" )
				{
					symbol.IsNumeric = false;
					symbol.IsString = false;
					symbol.IsBool = true;
				}
				else if ( type == "string" )
				{
					symbol.IsBool = false;
					symbol.IsNumeric = false;
					symbol.IsString = true;
				}
				else
				{
					throw new CraftyException( string.Format( "Unknown type {0} for symbol.", type ), symbol );
				}
			}
		}

		private bool IsScopeMark ( Symbol s )
		{
			return !s.IsActive && s.Type == "__scope";
		}

		private Symbol GetSymbolByName ( string name, bool activeOnly )
		{
			for ( int i = currentSymbolTable.Count - 1; i >= 0; i-- )
			{
				if ( IsScopeMark( currentSymbolTable[i] ) ) { break; }
				if ( currentSymbolTable[i].Name == name )
				{
					if ( activeOnly && !currentSymbolTable[i].IsActive )
					{
						throw new CraftyException( "Attempt to access a deactivated symbol.", currentSymbolTable[i] );
					}
					return currentSymbolTable[i];
				}
			}

			throw new CraftyException( string.Format( "Cannot find symbol by name {0}.", name ) );
		}

		private Symbol GetSymbolByName ( string name )
		{
			return GetSymbolByName( name, false );
		}

		private void ClearSymbolTable ( )
		{
			for ( int i = 0; i < currentSymbolTable.Count; i++ )
			{
				currentSymbolTable[i].IsActive = false;
			}

			foreach ( Operation o in operations )
			{
				o.Symbol = null;
			}
		}

		private void ClearSymbol ( string name )
		{
			int symbolIndex = SymbolIndex( name, false );
			if ( symbolIndex < 0 || !currentSymbolTable[symbolIndex].IsActive )
			{
				throw new CraftyException( string.Format( "Failed to deactivate symbol by name of {0}.", name ) );
			}

			currentSymbolTable[symbolIndex].IsActive = false;
		}

		private void AddScopeMark ( )
		{
			Symbol scopeMark = new Symbol( );
			scopeMark.Type = "__scope";
			currentSymbolTable.Add( scopeMark );
		}

		private void DropScope ( )
		{
			for ( int i = currentSymbolTable.Count - 1; i > 0; i-- )
			{
				if ( IsScopeMark( currentSymbolTable[i] ) )
				{
					currentSymbolTable.RemoveAt( i );
					break;
				}
				else
				{
					currentSymbolTable.RemoveAt( i );
				}
			}
		}

		private void ClearSymbolTableToMark ( )
		{
			Symbol last = blockMarkers.Pop( );

			for ( int i = currentSymbolTable.Count - 1; i > 0; i-- )
			{
				if ( IsScopeMark( currentSymbolTable[i] ) )
				{
					break;
					//throw new CraftyException( "Found scope mark while clearing symbol table to mark." );
				}
				if ( currentSymbolTable[i] == last )
				{
					currentSymbolTable[i].IsActive = false;
					currentSymbolTable.RemoveAt( i );
					break;
				}
				currentSymbolTable[i].IsActive = false;
			}
		}

		public bool IsMidExecution { get { return nextOp != null; } }

		public int Run ( int steps )
		{
			if ( currentSymbolTable == null )
			{
				symbolTables.Push( new List<Symbol>( ) );
			}

			if ( nextOp == null )
			{
				ClearSymbolTable( );
				nextOp = operations.First.Value;
			}

			string debugString = "";
			if ( steps < -1 )
			{
				steps = -1;
			}
			
			int totalSteps = steps;
			while ( steps != 0 )
			{
				//try
				//{
				debugString = null;

				currentOp = nextOp;

				if ( currentOp != null )
				{
					nextOp = currentOp.Next;
					switch ( currentOp.Code )
					{
						case OpCode.NoOperation:
							break;
						case OpCode.PushBoolean:
							if ( executeDebug )
							{
								debugString = currentOp.BoolValue.ToString( );
							}
							variableStack.Push( currentOp.BoolValue );
							break;
						case OpCode.PushString:
							if ( executeDebug ) { debugString = currentOp.StringValue; }
							variableStack.Push( currentOp.StringValue );
							break;
						case OpCode.PushFloat:
							if ( executeDebug )
							{
								debugString = currentOp.FloatValue.ToString( );
							}
							variableStack.Push( currentOp.FloatValue );
							break;
						case OpCode.PushSymbol:
							if ( executeDebug )
							{
								debugString = string.Format( "{0}", currentOp.Symbol.Name );
							}

							/*
							if ( currentOp.Symbol == null )
							{
								currentOp.Symbol = GetSymbolByName( currentOp.SymbolName, true );
							}

							stack.Push( currentOp.Symbol );
							 */
							variableStack.Push( GetSymbolByName( currentOp.SymbolName, true ) );
							break;
						case OpCode.Store:
							if ( executeDebug )
							{
								debugString = string.Format( "Setting symbol {0} to ", currentOp.Symbol.Name );
								if ( currentOp.Symbol.IsBool )
								{
									debugString += variableStack.PeekBoolean( );
								}
								else if ( currentOp.Symbol.IsNumeric )
								{
									debugString += variableStack.PeekFloat( );
								}
								else if ( currentOp.Symbol.IsString )
								{
									debugString += variableStack.PeekString( );
								}
								debugString += ".";
							}

							/*
							if ( currentOp.Symbol == null )
							{
								currentOp.Symbol = GetSymbolByName( currentOp.SymbolName, true );
							}

							stack.PeekSymbol( currentOp.Symbol );
							*/
							variableStack.PeekSymbol( GetSymbolByName( currentOp.SymbolName, true ) );

							if ( executeDebug )
							{
								if ( currentOp.Symbol.IsBool )
								{
									debugString += string.Format( " Set to {0}.", currentOp.Symbol.BooleanValue );
								}
								else if ( currentOp.Symbol.IsNumeric )
								{
									debugString += string.Format( " Set to {0}.", currentOp.Symbol.FloatValue );
								}
								else if ( currentOp.Symbol.IsString )
								{
									debugString += string.Format( " Set to {0}.", currentOp.Symbol.StringValue );
								}
							}
							break;
						case OpCode.GetTop:
							throw new CraftyException( "Invalid OpCode GetTop." );
						case OpCode.Discard:
							variableStack.Discard( );
							break;
						case OpCode.Jump:
							nextOp = currentOp.JumpToOperation;

							if ( executeDebug )
							{
								debugString = string.Format( "Jumping from {0} ({1}) to {2} ({3}).", currentOp.Code, currentOp.Number, nextOp.Code, nextOp.Number );
							}

							if ( nextOp == null )
							{
								throw new CraftyException( "Jump instruction points to an instruction that does not exist!" );
							}
							break;
						case OpCode.JumpIfFalse:
							if ( !variableStack.PopBoolean( ) )
							{
								nextOp = currentOp.JumpToOperation;
								if ( executeDebug )
								{
									debugString = string.Format( "Jumping from {0} ({1}) to {2} ({3}).", currentOp.Code, currentOp.Number, nextOp.Code, nextOp.Number );
								}

								if ( nextOp == null )
								{
									throw new CraftyException( "Jump instruction points to an instruction that does not exist!" );
								}
							}
							else
							{
								if ( executeDebug )
								{
									debugString = string.Format( "Not jumping from {0} ({1}).", currentOp.Code, currentOp.Number );
								}
							}
							break;
						case OpCode.JumpIfTrue:
							if ( variableStack.PopBoolean( ) )
							{
								nextOp = currentOp.JumpToOperation;
								if ( executeDebug )
								{
									debugString = string.Format( "Jumping from {0} ({1}) to {2} ({3}).", currentOp.Code, currentOp.Number, nextOp.Code, nextOp.Number );
								}
								if ( nextOp == null )
								{
									throw new CraftyException( "Jump instruction points to an instruction that does not exist!" );
								}
							}
							else
							{
								if ( executeDebug )
								{
									debugString = string.Format( "Not jumping from {0} ({1}).", currentOp.Code, currentOp.Number );
								}
							}
							break;
						case OpCode.FloatEqualTo:
							floatRegisterOne = variableStack.PopFloat( );
							floatRegisterTwo = variableStack.PopFloat( );
							variableStack.Push( floatRegisterOne == floatRegisterTwo );
							if ( executeDebug ) { debugString = string.Format( "{0} != {1}: {2}", floatRegisterOne, floatRegisterTwo, variableStack.PeekBoolean( ) ); }
							break;
						case OpCode.FloatGreaterThan:
							floatRegisterTwo = variableStack.PopFloat( );
							floatRegisterOne = variableStack.PopFloat( );
							variableStack.Push( floatRegisterOne > floatRegisterTwo );
							if ( executeDebug ) { debugString = string.Format( "{0} > {1}: {2}", floatRegisterOne, floatRegisterTwo, variableStack.PeekBoolean( ) ); }
							break;
						case OpCode.FloatLessThan:
							floatRegisterTwo = variableStack.PopFloat( );
							floatRegisterOne = variableStack.PopFloat( );
							variableStack.Push( floatRegisterOne < floatRegisterTwo );
							if ( executeDebug ) { debugString = string.Format( "{0} < {1}: {2}", floatRegisterOne, floatRegisterTwo, variableStack.PeekBoolean( ) ); }
							break;
						case OpCode.FloatGreaterOrEqualTo:
							floatRegisterTwo = variableStack.PopFloat( );
							floatRegisterOne = variableStack.PopFloat( );
							variableStack.Push( floatRegisterOne >= floatRegisterTwo );
							if ( executeDebug ) { debugString = string.Format( "{0} >= {1}: {2}", floatRegisterOne, floatRegisterTwo, variableStack.PeekBoolean( ) ); }
							break;
						case OpCode.FloatLessOrEqualTo:
							floatRegisterTwo = variableStack.PopFloat( );
							floatRegisterOne = variableStack.PopFloat( );
							variableStack.Push( floatRegisterOne <= floatRegisterTwo );
							if ( executeDebug ) { debugString = string.Format( "{0} <= {1}: {2}", floatRegisterOne, floatRegisterTwo, variableStack.PeekBoolean( ) ); }
							break;
						case OpCode.FloatNotEqualTo:
							floatRegisterTwo = variableStack.PopFloat( );
							floatRegisterOne = variableStack.PopFloat( );
							variableStack.Push( floatRegisterOne != floatRegisterTwo );
							if ( executeDebug ) { debugString = string.Format( "{0} != {1}: {2}", floatRegisterOne, floatRegisterTwo, variableStack.PeekBoolean( ) ); }
							break;
						case OpCode.StringEqualTo:
							variableStack.Push( variableStack.PopString( ) == variableStack.PopString( ) );
							break;
						case OpCode.JumpTarget:
							// Do nothing.
							break;
						case OpCode.NegateBoolean:
							variableStack.NegateBoolean( );
							break;
						case OpCode.Increment:
							variableStack.IncrementFloat( );
							break;
						case OpCode.Decrement:
							variableStack.DecrementFloat( );
							break;
						case OpCode.Multiply:
							//floatRegisterTwo = stack.PopFloat( );
							//floatRegisterOne = stack.PopFloat( );
							variableStack.Push( variableStack.PopFloat( ) * variableStack.PopFloat( ) );
							break;
						case OpCode.Add:
							variableStack.Push( variableStack.PopFloat( ) + variableStack.PopFloat( ) );
							break;
						case OpCode.SubtractReverse:
							// Used for cases where the values on the stack are in reverse order.
							floatRegisterOne = variableStack.PopFloat( );
							floatRegisterTwo = variableStack.PopFloat( );
							variableStack.Push( floatRegisterOne - floatRegisterTwo );
							break;
						case OpCode.Subtract:
							floatRegisterTwo = variableStack.PopFloat( );
							floatRegisterOne = variableStack.PopFloat( );
							variableStack.Push( floatRegisterOne - floatRegisterTwo );
							break;
						case OpCode.DivideReverse:
							// Used for cases where the values on the stack are in reverse order.
							floatRegisterOne = variableStack.PopFloat( );
							floatRegisterTwo = variableStack.PopFloat( );
							variableStack.Push( floatRegisterOne / floatRegisterTwo );
							break;
						case OpCode.Divide:
							floatRegisterTwo = variableStack.PopFloat( );
							floatRegisterOne = variableStack.PopFloat( );
							variableStack.Push( floatRegisterOne / floatRegisterTwo );
							break;
						case OpCode.ModulusReverse:
							// Used for cases where the values on the stack are in reverse order.
							floatRegisterOne = variableStack.PopFloat( );
							floatRegisterTwo = variableStack.PopFloat( );
							variableStack.Push( floatRegisterOne % floatRegisterTwo );
							break;
						case OpCode.Modulus:
							floatRegisterTwo = variableStack.PopFloat( );
							floatRegisterOne = variableStack.PopFloat( );
							variableStack.Push( floatRegisterOne % floatRegisterTwo );
							break;
						case OpCode.SwapFloatRegisters:
							float temp = floatRegisterOne;
							floatRegisterOne = floatRegisterTwo;
							floatRegisterTwo = temp;
							break;
						case OpCode.InvertFloat:
							variableStack.Push( -variableStack.PopFloat( ) );
							break;
						case OpCode.LogicalOr:
							variableStack.Push( variableStack.PopBoolean( ) || variableStack.PopBoolean( ) );
							break;
						case OpCode.LogicalAnd:
							variableStack.Push( variableStack.PopBoolean( ) && variableStack.PopBoolean( ) );
							break;
						case OpCode.ClearStack:
							variableStack.Clear( );
							break;
						case OpCode.Dummy:
							// Do nothing.
							break;
						case OpCode.StartBlock:
							MarkSymbolTable( );
							break;
						case OpCode.EndBlock:
							ClearSymbolTableToMark( );
							break;
						case OpCode.StartSolidBlock:
							interruptsAllowed++;
							break;
						case OpCode.EndSolidBlock:
							interruptsAllowed--;
							break;
						case OpCode.InitSymbol: 
							InitSymbol( currentOp.StringValue, variableStack.PopString( ) );
							break;
						case OpCode.StartFunctionBlock:
							PushNewSymbolTable( );
							
							break;
						case OpCode.EndFunctionBlock:
							DiscardSymbolTable( );
							nextOp = callStack.Pop( );
							break;
						case OpCode.DogEarCallStack:
							callStack.Push( currentOp.Next.Next );
							break;
						default:
							throw new CraftyException( string.Format( "Unknown opcode {0}.", currentOp.Code.ToString( ) ) );
					}

					if ( executeDebug )
					{
						string stackString = "(" + variableStack.Count.ToString( ) + ") ";

						for ( int i = 0; i < variableStack.Count; i++ )
						{
							stackString += "[(bool) " + variableStack[i].BooleanValue.ToString( ) + " (string) " + variableStack[i].StringValue + " (float) " + variableStack[i].FloatValue.ToString( ) + "] ";
						}

						if ( debugString == null )
						{
							debugString = "";
						}
						else
						{
							debugString = ": " + debugString;
						}
						Debug.Print( string.Format( "{0} {1}{2}", currentOp.Number.ToString( ).PadRight( 8 ), currentOp.Code, debugString ) );
						Debug.Print( "Stack: {0}", stackString );
					}
				}
				else
				{
					if ( executeDebug ) { Debug.Print( "Ended execution." ); }
					currentSymbolTable.Clear( );
					break;
				}

				if ( !operations.Contains( nextOp ) && nextOp != null )
				{
					throw new CraftyException( "Operation is not a part of the program." );
				}

				steps--;
				/*}
				catch ( CraftyException ke )
				{
					Console.WriteLine( string.Format( "A runtime CraftyException occured: {0}", ke.Message ) );
					throw ke;
					break;
				}
				catch ( Exception e )
				{
					Console.WriteLine( string.Format( "An exception occured at runtime: {0}", e.Message ) );
					throw e;
					break;
				}*/
			}

			return totalSteps > 0 ? totalSteps - steps : -( steps + 1 );
		}
	}

	public class DebugCraftyMachine : CraftyMachine
	{
		public DebugCraftyMachine ( params Operation[] ops ) : base( ops ) { }
		public DebugCraftyMachine ( CraftyInstance instance ) : base( instance ) { }
		public DebugCraftyMachine ( Parser parser ) : base( parser ) { }

		ReadonlyCraftyStack theStack = null;
		public ReadonlyCraftyStack Stack
		{
			get
			{
				if ( theStack == null )
				{
					theStack = new ReadonlyCraftyStack( variableStack );
				}
				return theStack;
			}
		}

		public Operation CurrentOperation
		{
			get { return currentOp; }
		}

		public Operation NextOperation
		{
			get { return nextOp; }
		}

		IList<Operation> readonlyOperationsList = null;
		public IList<Operation> Operations
		{
			get
			{
				if ( readonlyOperationsList == null )
				{
					readonlyOperationsList = operationsList.AsReadOnly( );
				}
				return readonlyOperationsList;
			}
		}

		IList<Symbol> symbolList = null;
		public IList<Symbol> SymbolList
		{
			get
			{
				if ( symbolList == null )
				{
					symbolList = currentSymbolTable;
				}
				return symbolList;
			}
		}

        
	}
}
