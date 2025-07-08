using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CraftyCode
{
	class IntermediateExpression
	{
		public static IntermediateExpression GetMathematicalExpression ( )
		{
			return new IntermediateExpression(
					new Operator( "MOD", 5, OpCode.Modulus ), new Operator( "TIMES", 5, OpCode.Multiply ), new Operator( "DIVIDE", 5, OpCode.Divide ),
					new Operator( "PLUS", 3, OpCode.Add ), new Operator( "MINUS", 3, OpCode.Subtract ),
					new Operator( "FLOAT", true ), new Operator( "IDENTIFIER", true ) );
		}
		
		List<TreeBranch> branches = new List<TreeBranch>( );
		LinkedList<Operation> ops = new LinkedList<Operation>( );
		List<Operator> operatoringTokens = new List<Operator>( );
		List<string> legalOperands = new List<string>( );

		public IntermediateExpression ( params Operator[] list )
			: this( "OPENBRACKET", "CLOSEBRACKET", list )
		{

		}

		public IntermediateExpression ( IntermediateExpression mixer, params Operator[] list )
			: this( "OPENBRACKET", "CLOSEBRACKET", mixer, list )
		{

		}

		public IntermediateExpression ( string openBracket, string closeBracket, params Operator[] list )
		{
			SetOperators( openBracket, closeBracket, null, list.ToArray( ) );
		}

		public IntermediateExpression ( string openBracket, string closeBracket, IntermediateExpression mixer, params Operator[] list )
		{
			SetOperators( openBracket, closeBracket, mixer, list.ToArray( ) );
		}

		public void SetOperators ( string openBracket, string closeBracket, IntermediateExpression mixer, params Operator[] list )
		{
			operatoringTokens.Clear( );
			legalOperands.Clear( );

			operatoringTokens.Add( Operator.GetOpenBracket( openBracket ) );
			operatoringTokens.Add( Operator.GetCloseBracket( closeBracket ) );

			for ( int i = 0; i < list.Length; i++ )
			{
				if ( HasOperatorOrOperand( list[i].Name ) ) { continue; }
				if ( list[i].Operand )
				{
					legalOperands.Add( list[i].Name );
				}
				else
				{
					operatoringTokens.Add( list[i] );
				}
			}
			
			if ( mixer != null )
			{
				for ( int i = 0; i < mixer.legalOperands.Count; i++ )
				{
					if ( HasOperatorOrOperand( mixer.legalOperands[i] ) ) { continue; }
					legalOperands.Add( mixer.legalOperands[i] );
				}

				for ( int i = 0; i < mixer.operatoringTokens.Count; i++ )
				{
					if ( HasOperatorOrOperand( mixer.operatoringTokens[i].Name ) ) { continue; }
					operatoringTokens.Add( mixer.operatoringTokens[i] );
				}
			}
		}

		public bool HasOperatorOrOperand ( string t )
		{
			for ( int i = 0; i < legalOperands.Count; i++ )
			{
				if ( legalOperands[i] == t )
				{
					return true;
				}
			}

			for ( int i = 0; i < operatoringTokens.Count; i++ )
			{
				if ( operatoringTokens[i].Name == t )
				{
					return true;
				}
			}

			return false;
		}

		private bool IsPrevalent ( Operator x, Operator y )
		{
			return x.Priority > y.Priority;
		}

		public Operator GetOperator ( Token t )
		{
			for ( int i = 0; i < operatoringTokens.Count; i++ )
			{
				if ( operatoringTokens[i].Name == t.Name )
				{
					return operatoringTokens[i];
				}
			}

			throw new CraftyException( "No operator found for token.", t );
		}

		private bool IsOperator ( TreeBranch t )
		{
			return IsOperator( t.BranchToken );
		}

		private bool IsOperator ( Token t )
		{
			for ( int i = 0; i < operatoringTokens.Count; i++ )
			{
				if ( t.Name == operatoringTokens[i].Name )
				{
					return true;
				}
			}

			return false;
		}

		private bool IsOperand ( TreeBranch t )
		{
			return IsOperand( t.BranchToken );
		}

		private bool IsOperand ( Token t )
		{
			for ( int i = 0; i < legalOperands.Count; i++ )
			{
				if ( legalOperands[i] == t.Name )
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Transforms the token stream into operations.
		/// </summary>
		/// <param name="tbs">A list of tokens.</param>
		public void Evaluate ( IList<TreeBranch> tbs )
		{
			ops.Clear( );

			branches.Clear( );
			branches.AddRange( tbs.ToArray( ) );

			for ( int i = 0; i < branches.Count; i++ )
			{
				if ( !branches[i].IsTokenBranch )
				{
					throw new InternalCraftyException( new ArgumentException( "TBS must be a list of tokens." ) );
				}
			}

			List<TreeBranch> operators = new List<TreeBranch>( );
			List<TreeBranch> listOfTokens = new List<TreeBranch>( );
			//List<Operation> listOfTokensOperations = new List<Operation>( );
			//List<Operation> operatorOperations = new List<Operation>( );
			bool prevalent = false;
			bool openBracketFound = false;
			TreeBranch currentTreeBranch = null;
			bool foundOperator = false;
			bool foundOperand = false;
			ops.AddLast( new Operation( OpCode.Dummy, "IntermediateExpression." ) );

			for ( int j = 0; j < branches.Count; j++ )
			{
				foundOperator = false;
				foundOperand = false;
				prevalent = false;
				openBracketFound = false;
				currentTreeBranch = branches[j];

				foundOperand = IsOperand( currentTreeBranch );
				foundOperator = IsOperator( currentTreeBranch );

				if ( foundOperand && foundOperator )
				{
					throw new CraftyException( "Token is both operator and operand.", currentTreeBranch.BranchToken );
				}
				else if ( !foundOperator && !foundOperand )
				{
					throw new CraftyException( "Token is not a valid operator or operand.", currentTreeBranch.BranchToken );
				}

				if ( foundOperand )
				{
					listOfTokens.Add( currentTreeBranch );
				}
				else
				{
					if ( GetOperator( currentTreeBranch.BranchToken ).CloseBracket )
					{
						openBracketFound = false;
						while ( operators.Count > 0 )
						{
							if ( GetOperator( operators[operators.Count - 1].BranchToken ).OpenBracket )
							{
								operators.RemoveAt( operators.Count - 1 );
								openBracketFound = true;
								break;
							}
							listOfTokens.Add( operators[operators.Count - 1] );
							operators.RemoveAt( operators.Count - 1 );
						}
						if ( !openBracketFound )
						{
							throw new CraftyException( "A matching opening bracket was not found while parsing the arithmatic expression.", currentTreeBranch.BranchToken );
						}
					}
					else if ( GetOperator( currentTreeBranch.BranchToken ).OpenBracket )
					{
						operators.Add( currentTreeBranch );
					}
					else
					{
						for ( int x = operators.Count - 1; x >= 0; )
						{
							if ( IsPrevalent( GetOperator( currentTreeBranch.BranchToken ), GetOperator( operators[x].BranchToken ) ) )
							{
								operators.Add( currentTreeBranch );
								prevalent = true;
								break;
							}
							else
							{
								if ( !GetOperator( operators[x].BranchToken ).CloseBracket )
								{
									listOfTokens.Add( operators[x] );
								}
								operators.RemoveAt( x );
								x--;
							}
						}

						if ( !prevalent )
						{
							operators.Add( currentTreeBranch );
						}
					}
				}
			}

			while ( operators.Count > 0 )
			{
				listOfTokens.Add( operators[operators.Count - 1] );
				operators.RemoveAt( operators.Count - 1 );
			}

			while ( listOfTokens.Count > 0 )
			{
				currentTreeBranch = listOfTokens[0];

				if ( IsOperand( currentTreeBranch ) )
				{
					if ( currentTreeBranch.BranchToken.Name == "IDENTIFIER" )
					{
						ops.AddLast( new Operation( OpCode.PushSymbol, currentTreeBranch.BranchToken.IdentifierSymbol ) );
					}
					else if ( currentTreeBranch.BranchToken.Name == "FLOAT" )
					{
						ops.AddLast( new Operation( OpCode.PushFloat, currentTreeBranch.BranchToken.FloatValue ) );
					}
					else if ( currentTreeBranch.BranchToken.Name == "TRUE" )
					{
						ops.AddLast( new Operation( OpCode.PushBoolean, true ) );
					}
					else if ( currentTreeBranch.BranchToken.Name == "FALSE" )
					{
						ops.AddLast( new Operation( OpCode.PushBoolean, false ) );
					}
					else if ( currentTreeBranch.BranchToken.Name == "STRING" )
					{
						ops.AddLast( new Operation( OpCode.PushString, currentTreeBranch.BranchToken.StringValue ) );
					}
					else
					{
						throw new CraftyException( string.Format( "Unknown type {0}.", currentTreeBranch.BranchToken.Name ), currentTreeBranch.BranchToken );
					}
				}
				else
				{
					ops.AddLast( new Operation( GetOperator( currentTreeBranch.BranchToken ).OperationCode ) );
				}

				listOfTokens.Remove( currentTreeBranch );
			}
		}

		public IEnumerable<Operation> GetOperations ( )
		{
			foreach ( Operation o in ops )
			{
				yield return o;
			}
		}
	}

	class Operator
	{
		/// <summary>
		/// Creates a new operator.
		/// </summary>
		/// <param name="n">The operator's token.</param>
		/// <param name="priority">It's priority.</param>
		/// <param name="code">The opcode of the operator.</param>
		public Operator ( string n, int priority, OpCode code )
		{
			Name = n;
			Priority = priority;
			OperationCode = code;
		}

		/// <summary>
		/// Creates a new operand.
		/// </summary>
		/// <param name="n">The token.</param>
		public Operator ( string n, bool operand )
		{
			Name = n;
			Operand = true;
		}

		private Operator ( string n, bool openBracket, bool closeBracket )
		{
			if ( openBracket == closeBracket )
			{
				throw new InternalCraftyException( "Operator cannot be both opening and closing bracket." );
			}
			OpenBracket = openBracket;
			CloseBracket = closeBracket;
			Name = n;
		}

		public static Operator GetOpenBracket ( string name )
		{
			Operator e = new Operator( name, true, false );
			return e;
		}

		public static Operator GetCloseBracket ( string name )
		{
			Operator e = new Operator( name, false, true );
			return e;
		}

		public readonly string Name = "";
		public readonly int Priority = 0;
		public readonly int NumberOfOperands = 2;
		public readonly OpCode OperationCode = OpCode.NoOperation;
		public readonly bool Operand = false;
		public readonly bool OpenBracket = false;
		public readonly bool CloseBracket = false;
	}
}
