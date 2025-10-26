using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CraftyCode
{
	public class SymbolTableCollection
	{
		public Stack<List<Symbol>> SymbolTables = new Stack<List<Symbol>>( );
		public Stack<Symbol> BlockMarkers = new Stack<Symbol>( );
		public IList<Symbol> CurrentSymbolTable
		{
			get { return SymbolTables.Count > 0 ? SymbolTables.Peek( ) : null; }
		}

		public void MarkSymbolTable ( )
		{
			Symbol mark = new Symbol( );
			mark.Name = "[mark]";
			CurrentSymbolTable.Add( mark );

			BlockMarkers.Push( mark );
		}

		public bool IsScopeMark ( Symbol s )
		{
			return !s.IsActive && s.Type == "__scope";
		}

		public bool IsSymbolInTable ( string name )
		{
			for ( int i = CurrentSymbolTable.Count - 1; i > 0; i-- )
			{
				if ( IsScopeMark( CurrentSymbolTable[ i ] ) ) { break; }
				if ( CurrentSymbolTable[ i ].Name == name )
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
			for ( int i = CurrentSymbolTable.Count - 1; i > 0; i-- )
			{
				if ( currentScope && IsScopeMark( CurrentSymbolTable[ i ] ) ) { break; }
				if ( CurrentSymbolTable[ i ].Name == name )
				{
					return i;
				}
			}

			return -1;
		}

		public void PushNewSymbolTable ( )
		{
			SymbolTables.Push( new List<Symbol>( ) );
		}

		public void DiscardSymbolTable ( )
		{
			SymbolTables.Pop( );
		}

		public void InitSymbol ( string name, string type )
		{
			Symbol symbol = null;
			int symbolIndex = SymbolIndex( name );
			if ( symbolIndex < 0 )
			{
				symbol = new Symbol( );
				symbol.Name = name;
				symbol.Type = type;
				CurrentSymbolTable.Add( symbol );
			}
			else
			{
				symbol = CurrentSymbolTable[ symbolIndex ];
				if ( symbolIndex != CurrentSymbolTable.Count - 1 )
				{
					Symbol temp = CurrentSymbolTable[ symbolIndex ];
					CurrentSymbolTable.RemoveAt( symbolIndex );
					CurrentSymbolTable.Add( temp );
				}
			}

			if ( symbol.IsActive )
			{
				throw new CraftyException( string.Format( "Symbol of name {0} already initialized.", name ), CurrentSymbolTable[ symbolIndex ] );
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

		public Symbol GetSymbolByName ( string name, bool activeOnly )
		{
			for ( int i = CurrentSymbolTable.Count - 1; i >= 0; i-- )
			{
				if ( IsScopeMark( CurrentSymbolTable[ i ] ) ) { break; }
				if ( CurrentSymbolTable[ i ].Name == name )
				{
					if ( activeOnly && !CurrentSymbolTable[ i ].IsActive )
					{
						throw new CraftyException( "Attempt to access a deactivated symbol.", CurrentSymbolTable[ i ] );
					}
					return CurrentSymbolTable[ i ];
				}
			}

			throw new CraftyException( string.Format( "Cannot find symbol by name {0}.", name ) );
		}

		public Symbol GetSymbolByName ( string name )
		{
			return GetSymbolByName( name, false );
		}

		public void ClearSymbolTable ( )
		{
			for ( int i = 0; i < CurrentSymbolTable.Count; i++ )
			{
				CurrentSymbolTable[ i ].IsActive = false;
			}
		}

		public void ClearSymbol ( string name )
		{
			int symbolIndex = SymbolIndex( name, false );
			if ( symbolIndex < 0 || !CurrentSymbolTable[ symbolIndex ].IsActive )
			{
				throw new CraftyException( string.Format( "Failed to deactivate symbol by name of {0}.", name ) );
			}

			CurrentSymbolTable[ symbolIndex ].IsActive = false;
		}

		public void AddScopeMark ( )
		{
			Symbol scopeMark = new Symbol( );
			scopeMark.Type = "__scope";
			CurrentSymbolTable.Add( scopeMark );
		}

		public void DropScope ( )
		{
			for ( int i = CurrentSymbolTable.Count - 1; i > 0; i-- )
			{
				if ( IsScopeMark( CurrentSymbolTable[ i ] ) )
				{
					CurrentSymbolTable.RemoveAt( i );
					break;
				}
				else
				{
					CurrentSymbolTable.RemoveAt( i );
				}
			}
		}

		public void ClearSymbolTableToMark ( )
		{
			Symbol last = BlockMarkers.Pop( );

			for ( int i = CurrentSymbolTable.Count - 1; i > 0; i-- )
			{
				if ( IsScopeMark( CurrentSymbolTable[ i ] ) )
				{
					break;
					//throw new CraftyException( "Found scope mark while clearing symbol table to mark." );
				}
				if ( CurrentSymbolTable[ i ] == last )
				{
					CurrentSymbolTable[ i ].IsActive = false;
					CurrentSymbolTable.RemoveAt( i );
					break;
				}
				CurrentSymbolTable[ i ].IsActive = false;
			}
		}
	}

}
