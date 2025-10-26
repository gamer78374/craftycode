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
		public CraftyMachine ( InstructionSet exeset, params Operation[] ops )
		{
			operationsList = new List<Operation>( );
			for ( int i = 0; i < ops.Length; i++ )
			{
				operations.AddLast( ops[i] );
				operationsList.Add( ops[i] );
			}

			State.SymbolTableCollection = new SymbolTableCollection( );
			State.CallStack = new Stack<Operation>( );
			State.VariableStack = new CraftyStack( );
			//ClearSymbolTable( );
			InstructionSet = exeset;
		}

		public CraftyMachine ( InstructionSet exeset, CraftyInstance instance ) : this( exeset, instance.Parser ) { }
		public CraftyMachine ( InstructionSet exeset, Parser parser ) : this( exeset, parser.ProgramRoot.Operations.ToArray( ) ) { }

		public InstructionSet InstructionSet = null;
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

		public InstructionState State = new InstructionState( );
	
		/*
		protected Operation currentOp;
		protected Operation nextOp;

		protected CraftyStack variableStack = new CraftyStack( );
		protected Stack<Symbol> blockMarkers = new Stack<Symbol>( );
		protected float floatRegisterOne = 0f;
		protected float floatRegisterTwo = 0f;

		protected Stack<Operation> callStack = new Stack<Operation>( );

		public SymbolTableCollection SymbolCollection = new SymbolTableCollection( );*/

		/*
		protected Stack<List<Symbol>> symbolTables = new Stack<List<Symbol>>( );
		protected IList<Symbol> currentSymbolTable
		{
			get { return symbolTables.Count > 0 ? symbolTables.Peek( ) : null; }
		}
		*/
		/// <summary>
		/// If positive, interrupts are not allowed. If 0, interrupts are allowed.
		/// </summary>
		int interruptsAllowed = 0;

		public int Run ( )
		{
			return Run( -2 );
		}
		/*
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
		*/
		public bool IsMidExecution { get { return State.NextOp != null; } }

		public int Run ( int steps )
		{
			if ( State.SymbolTableCollection.CurrentSymbolTable == null )
			{
				State.SymbolTableCollection.SymbolTables.Push( new List<Symbol>( ) );
			}

			if ( State.NextOp == null )
			{
				State.SymbolTableCollection.ClearSymbolTable( );
				foreach ( Operation o in operations )
				{
					o.Symbol = null;
				}
				State.NextOp = operations.First.Value;
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

				State.CurrentOp = State.NextOp;

				if ( State.CurrentOp != null )
				{
					State.NextOp = State.CurrentOp.Next;

					if ( InstructionSet.Operations[ (int) State.CurrentOp.Code ]( State ) )
					{
						// Success
					}

					if ( executeDebug )
					{
						string stackString = "(" + State.VariableStack.Count.ToString( ) + ") ";

						for ( int i = 0; i < State.VariableStack.Count; i++ )
						{
							stackString += "[(bool) " + State.VariableStack[ i].BooleanValue.ToString( ) + " (string) " + State.VariableStack[i].StringValue + " (float) " + State.VariableStack[i].FloatValue.ToString( ) + "] ";
						}

						if ( debugString == null )
						{
							debugString = "";
						}
						else
						{
							debugString = ": " + debugString;
						}
						Debug.Print( string.Format( "{0} {1}{2}", State.CurrentOp.Number.ToString( ).PadRight( 8 ), State.CurrentOp.Code, debugString ) );
						Debug.Print( "Stack: {0}", stackString );
					}
				}
				else
				{
					if ( executeDebug ) { Debug.Print( "Ended execution." ); }
					State.SymbolTableCollection.CurrentSymbolTable.Clear( );
					break;
				}

				if ( !operations.Contains( State.NextOp ) && State.NextOp != null )
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
		public DebugCraftyMachine ( InstructionSet exeset, params Operation[] ops ) : base(exeset, ops ) { }
		public DebugCraftyMachine ( InstructionSet exeset, CraftyInstance instance ) : base( exeset, instance ) { }
		public DebugCraftyMachine ( InstructionSet exeset, Parser parser ) : base( exeset, parser ) { }

		ReadonlyCraftyStack theStack = null;
		public ReadonlyCraftyStack Stack
		{
			get
			{
				if ( theStack == null )
				{
					theStack = new ReadonlyCraftyStack( State.VariableStack );
				}
				return theStack;
			}
		}

		public Operation CurrentOperation
		{
			get { return State.CurrentOp; }
		}

		public Operation NextOperation
		{
			get { return State.NextOp; }
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
					symbolList = State.SymbolTableCollection.CurrentSymbolTable;
				}
				return symbolList;
			}
		}

        
	}
}
