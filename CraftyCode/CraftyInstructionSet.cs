namespace CraftyCode
{
	public class CraftyInstructionSet : InstructionSet
	{
		public CraftyInstructionSet ( ):base()
		{
			

			Operations[ (int) OpCode.NoOperation ] = ( state ) =>
			{
				return true;
			};

			Operations[ (int) OpCode.PushBoolean ] = ( state ) =>
			{
				if ( state.ExecuteDebug )
				{
					state.DebugString = state.CurrentOp.BoolValue.ToString( );
				}
				state.VariableStack.Push( state.CurrentOp.BoolValue );
				return true;
			};

			Operations[ (int) OpCode.PushString ] = ( state ) =>
			{	
				//if ( executeDebug ) { debugString = currentOp.StringValue; }
				state.VariableStack.Push( state.CurrentOp.StringValue );
			
				return true;
			};

			Operations[ (int) OpCode.PushFloat ] = ( state ) =>
			{
			/*
				if ( executeDebug )
				{
					debugString = currentOp.FloatValue.ToString( );
				}
				*/
				state.VariableStack.Push( state.CurrentOp.FloatValue );
					
				return true;
			}; 
			
			Operations[ (int) OpCode.PushSymbol ] = ( state ) =>
			{
			/*
				if ( executeDebug )
				{
					debugString = string.Format( "{0}", currentOp.Symbol.Name );
				}
				*/
				state.VariableStack.Push( state.SymbolTableCollection.GetSymbolByName( state.CurrentOp.SymbolName, true ) );
				
				return true;
			}; 
			
			Operations[ (int) OpCode.Store ] = ( state ) =>
			{
			/*
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
				*/
				state.VariableStack.PeekSymbol(state.SymbolTableCollection.GetSymbolByName( state.CurrentOp.SymbolName, true ) );

				/*
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
				*/

				return true;
			}; 
			
			Operations[ (int) OpCode.GetTop ] = ( state ) =>
			{
				throw new CraftyException( "Invalid OpCode GetTop." );
				//return true;
			}; 
			
			Operations[ (int) OpCode.Discard ] = ( state ) =>
			{
				state.VariableStack.Discard( );
				return true;
			}; 
			
			Operations[ (int) OpCode.Jump ] = ( state ) =>
			{
				
				state.NextOp = state.CurrentOp.JumpToOperation;
				/*
				if ( executeDebug )
				{
					debugString = string.Format( "Jumping from {0} ({1}) to {2} ({3}).", currentOp.Code, currentOp.Number, nextOp.Code, nextOp.Number );
				}
				*/
				if ( state.NextOp == null )
				{
					throw new CraftyException( "Jump instruction points to an instruction that does not exist!" );
				}

				
				return true;
			}; 
			
			Operations[ (int) OpCode.JumpIfFalse ] = ( state ) =>
			{
				
				if ( !state.VariableStack.PopBoolean( ) )
				{
					state.NextOp = state.CurrentOp.JumpToOperation;
					/*
					if ( executeDebug )
					{
						debugString = string.Format( "Jumping from {0} ({1}) to {2} ({3}).", currentOp.Code, currentOp.Number, nextOp.Code, nextOp.Number );
					}
					*/
					if ( state.NextOp == null )
					{
						throw new CraftyException( "Jump instruction points to an instruction that does not exist!" );
					}
				}
				else
				{
					/*
					if ( executeDebug )
					{
						debugString = string.Format( "Not jumping from {0} ({1}).", currentOp.Code, currentOp.Number );
					}
					*/
				}
				
				return true;
			};

			Operations[ (int) OpCode.JumpIfTrue ] = ( state ) =>
			{
				if ( state.VariableStack.PopBoolean( ) )
				{
					state.NextOp = state.CurrentOp.JumpToOperation;
					
					/*
					if ( executeDebug )
					{
						debugString = string.Format( "Jumping from {0} ({1}) to {2} ({3}).", currentOp.Code, currentOp.Number, nextOp.Code, nextOp.Number );
					}*/

					if ( state.NextOp == null )
					{
						throw new CraftyException( "Jump instruction points to an instruction that does not exist!" );
					}
				}
				else
				{
					/*
					if ( executeDebug )
					{
						debugString = string.Format( "Not jumping from {0} ({1}).", currentOp.Code, currentOp.Number );
					}*/
				}
				
				return true;
			};

			Operations[ (int) OpCode.FloatEqualTo ] = ( state ) =>
			{
				
				float floatRegisterOne = state.VariableStack.PopFloat( );
				float floatRegisterTwo = state.VariableStack.PopFloat( );
				state.VariableStack.Push( floatRegisterOne == floatRegisterTwo );
				
				//if ( executeDebug ) { debugString = string.Format( "{0} != {1}: {2}", floatRegisterOne, floatRegisterTwo, state.VariableStack.PeekBoolean( ) ); }
				
				return true;
			};

			Operations[ (int) OpCode.FloatGreaterThan ] = ( state ) =>
			{
				float floatRegisterTwo = state.VariableStack.PopFloat( );
				float floatRegisterOne = state.VariableStack.PopFloat( );
				state.VariableStack.Push( floatRegisterOne > floatRegisterTwo );
				//if ( executeDebug ) { debugString = string.Format( "{0} > {1}: {2}", floatRegisterOne, floatRegisterTwo, variableStack.PeekBoolean( ) ); }
				
				return true;
			};

			Operations[ (int) OpCode.FloatLessThan ] = ( state ) =>
			{
				
				float floatRegisterTwo = state.VariableStack.PopFloat( );
				float floatRegisterOne = state.VariableStack.PopFloat( );
				state.VariableStack.Push( floatRegisterOne < floatRegisterTwo );
				//if ( executeDebug ) { debugString = string.Format( "{0} < {1}: {2}", floatRegisterOne, floatRegisterTwo, variableStack.PeekBoolean( ) ); }
				
				return true;
			};

			Operations[ (int) OpCode.FloatGreaterOrEqualTo ] = ( state ) =>
			{
				
				float floatRegisterTwo = state.VariableStack.PopFloat( );
				float floatRegisterOne = state.VariableStack.PopFloat( );
				state.VariableStack.Push( floatRegisterOne >= floatRegisterTwo );
				//if ( executeDebug ) { debugString = string.Format( "{0} >= {1}: {2}", floatRegisterOne, floatRegisterTwo, variableStack.PeekBoolean( ) ); }
				
				return true;
			};

			Operations[ (int) OpCode.FloatLessOrEqualTo ] = ( state ) =>
			{
				
				float floatRegisterTwo = state.VariableStack.PopFloat( );
				float floatRegisterOne = state.VariableStack.PopFloat( );
				state.VariableStack.Push( floatRegisterOne <= floatRegisterTwo );
				if ( state.ExecuteDebug ) { state.DebugString = string.Format( "{0} <= {1}: {2}", floatRegisterOne, floatRegisterTwo, state.VariableStack.PeekBoolean( ) ); }
				
				return true;
			};

			Operations[ (int) OpCode.FloatNotEqualTo ] = ( state ) =>
			{   
				float floatRegisterTwo = state.VariableStack.PopFloat( );
				float floatRegisterOne = state.VariableStack.PopFloat( );
				state.VariableStack.Push( floatRegisterOne != floatRegisterTwo );
				if ( state.ExecuteDebug ) { state.DebugString = string.Format( "{0} != {1}: {2}", floatRegisterOne, floatRegisterTwo, state.VariableStack.PeekBoolean( ) ); }
				
				return true;
			};

			Operations[ (int) OpCode.StringEqualTo ] = ( state ) =>
			{
				state.VariableStack.Push( state.VariableStack.PopString( ) == state.VariableStack.PopString( ) );
				return true;
			};

			Operations[ (int) OpCode.JumpTarget ] = ( state ) =>
			{
				// Do nothing
				return true;
			}; 
			
			Operations[ (int) OpCode.NegateBoolean ] = ( state ) =>
			{
				state.VariableStack.NegateBoolean( );
				return true;
			}; 
			
			Operations[ (int) OpCode.Increment ] = ( state ) =>
			{
				state.VariableStack.IncrementFloat( );
					return true;
			}; 
			
			Operations[ (int) OpCode.Decrement ] = ( state ) =>
			{
				state.VariableStack.DecrementFloat( );
				return true;
			}; 
			
			Operations[ (int) OpCode.Multiply ] = ( state ) =>
			{
				state.VariableStack.Push( state.VariableStack.PopFloat( ) * state.VariableStack.PopFloat( ) );
				return true;
			}; 
			
			Operations[ (int) OpCode.Add ] = ( state ) =>
			{
				state.VariableStack.Push( state.VariableStack.PopFloat( ) + state.VariableStack.PopFloat( ) );
				return true;
			}; 
			
			Operations[ (int) OpCode.SubtractReverse ] = ( state ) =>
			{
				// Used for cases where the values on the stack are in reverse order.
				float floatRegisterOne = state.VariableStack.PopFloat( );
				float floatRegisterTwo = state.VariableStack.PopFloat( );
				state.VariableStack.Push( floatRegisterOne - floatRegisterTwo );
				
				return true;
			};

			Operations[ (int) OpCode.Subtract ] = ( state ) =>
			{
				
				float floatRegisterTwo = state.VariableStack.PopFloat( );
				float floatRegisterOne = state.VariableStack.PopFloat( );
				state.VariableStack.Push( floatRegisterOne - floatRegisterTwo );
				
				return true;
			};

			Operations[ (int) OpCode.DivideReverse ] = ( state ) =>
			{
				// Used for cases where the values on the stack are in reverse order.
				float floatRegisterOne = state.VariableStack.PopFloat( );
				float floatRegisterTwo = state.VariableStack.PopFloat( );
				state.VariableStack.Push( floatRegisterOne / floatRegisterTwo );
				return true;
			};

			Operations[ (int) OpCode.Divide ] = ( state ) =>
			{
				float floatRegisterTwo = state.VariableStack.PopFloat( );
				float floatRegisterOne = state.VariableStack.PopFloat( );
				state.VariableStack.Push( floatRegisterOne / floatRegisterTwo );
				return true;
			};

			Operations[ (int) OpCode.ModulusReverse ] = ( state ) =>
			{
				// Used for cases where the values on the stack are in reverse order.
				float floatRegisterOne = state.VariableStack.PopFloat( );
				float floatRegisterTwo = state.VariableStack.PopFloat( );
				state.VariableStack.Push( floatRegisterOne % floatRegisterTwo );
				
				return true;
			};

			Operations[ (int) OpCode.Modulus ] = ( state ) =>
			{
				float floatRegisterTwo = state.VariableStack.PopFloat( );
				float floatRegisterOne = state.VariableStack.PopFloat( );
				state.VariableStack.Push( floatRegisterOne % floatRegisterTwo );
				return true;
			};

			Operations[ (int) OpCode.SwapFloatRegisters ] = ( state ) =>
			{	// No-op?
				/*
				float temp = floatRegisterOne;
				floatRegisterOne = floatRegisterTwo;
				floatRegisterTwo = temp;
				*/
				return true;
			};

			Operations[ (int) OpCode.InvertFloat ] = ( state ) =>
			{
				state.VariableStack.Push( -state.VariableStack.PopFloat( ) );
				return true;
			};

			Operations[ (int) OpCode.ClearStack ] = ( state ) =>
			{
				state.VariableStack.Clear( );
				return true;
			};

			Operations[ (int) OpCode.LogicalAnd ] = ( state ) =>
			{
				state.VariableStack.Push( state.VariableStack.PopBoolean( ) && state.VariableStack.PopBoolean( ) );
				return true;
			}; 
			
			Operations[ (int) OpCode.LogicalOr ] = ( state ) =>
			{
				state.VariableStack.Push( state.VariableStack.PopBoolean( ) || state.VariableStack.PopBoolean( ) );
				return true;
			};

				
			Operations[ (int) OpCode.DHook ] = ( state ) =>
			{
					return true;
			}; 

			Operations[ (int) OpCode.StartBlock ] = ( state ) =>
			{
				state.SymbolTableCollection.MarkSymbolTable( );
				return true;
			};

			Operations[ (int) OpCode.EndBlock ] = ( state ) =>
			{
				state.SymbolTableCollection.ClearSymbolTableToMark( );
				return true;
			};

			Operations[ (int) OpCode.StartSolidBlock ] = ( state ) =>
			{
				state.InterruptsAllowed++;
				return true;
			};

			Operations[ (int) OpCode.EndSolidBlock ] = ( state ) =>
			{
				state.InterruptsAllowed--;
				return true;
			};

			Operations[ (int) OpCode.InitSymbol ] = ( state ) =>
			{
				state.SymbolTableCollection.InitSymbol( state.CurrentOp.StringValue, state.VariableStack.PopString( ) );
				return true;
			};

			Operations[ (int) OpCode.StartFunctionBlock ] = ( state ) =>
			{
				state.SymbolTableCollection.PushNewSymbolTable( );
				return true;
			};

			Operations[ (int) OpCode.EndFunctionBlock ] = ( state ) =>
			{
				state.SymbolTableCollection.DiscardSymbolTable( );
				state.NextOp = state.CallStack.Pop( );

				return true;
			};

			Operations[ (int) OpCode.DogEarCallStack ] = ( state ) =>
			{
				state.CallStack.Push( state.CurrentOp.Next.Next );
				return true;
			};
		}
	}
}
