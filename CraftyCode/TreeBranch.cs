using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CraftyCode
{
#if DEBUG
	public class TreeBranch
#else 
	internal class TreeBranch
#endif
	{
		public override string ToString ( )
		{
			if ( IsRuleBranch )
			{
				return "RuleBranch [" + BranchRule.Name + "]";
			}
			else if ( IsTokenBranch )
			{
				return "TokenBranch [" + BranchToken.Name + "]";
			}
			else
			{
				return base.ToString( );
			}
		}

		readonly Parser ParentParser = null;
		TreeBranch ParentBranch = null;
		/// <summary>
		/// Returns the rule the branch represents.
		/// </summary>
		public readonly ParsingRule BranchRule = null;
		/// <summary>
		/// Returns the token the branch represents.
		/// </summary>
		public Token BranchToken = null;
		public readonly int RuleOptionNumber;
		public readonly int RuleStepNumber;

		public ParsingStep BranchRuleParsingStep
		{
			get
			{
				return IsRuleBranch ? BranchRule[RuleOptionNumber, RuleStepNumber] : ParentBranch.BranchRule[RuleOptionNumber, RuleStepNumber];
			}
		}

		/// <summary>
		/// Gets the branch at the root of the tree.
		/// </summary>
		public TreeBranch TreeRoot
		{
			get
			{
				return ParentBranch == null ? this : ParentBranch.TreeRoot;
			}
		}

		/// <summary>
		/// Returns whether this treebranch is the root branch for the program.
		/// </summary>
		public bool IsRoot
		{
			get { return ParentBranch == null; }
		}

		Dictionary<Symbol, TreeBranch> functionBranches = new Dictionary<Symbol, TreeBranch>( );
		int subResult = 0;
		int currentOptionNumber = 0;
		int tokenOffset = 0;
		int repeatstep = 0;
		readonly int StartToken = 0;
		string currentToken = null;
		//ParsingStep[] optionSteps = null;
		private int level;

		public int Level
		{
			get { return level; }
			set
			{
				level = value;
				for ( int i = 0; i < branches.Count; i++ )
				{
					branches[i].Level = Level + 1;
				}
			}

		}

		List<TreeBranch> branches = new List<TreeBranch>( );

		protected IList<TreeBranch> Branches
		{
			get { return branches; }
		}

		public IEnumerable<TreeBranch> GetAllBranches ( )
		{
			for ( int i = 0; i < branches.Count; i++ )
			{
				yield return branches[i];
			}
		}

		Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>( );

		public bool HasSymbolList
		{
			get { return IsBlock || IsRoot; }
		}

		public bool IsBlock
		{
			get { return IsRuleBranch && BranchRule.IsBlock; }
		}

		public bool IsIsolated
		{
			get { return IsRuleBranch && BranchRule.Name == "functiondeclare"; }
		}

		protected IDictionary<string, Symbol> SymbolList
		{
			get
			{
				if ( IsRoot )
				{
					return symbols;
				}
				/*
				if ( IsBlock )
				{
					return symbols;
				}
				else
				{
					return null;
				}
				*/
				if ( IsIsolated || IsBlock )
				{
					return symbols;
				}
				else
				{
					return ParentBranch.SymbolList;
				}
			}
		}

		private TreeBranch ( Parser p, TreeBranch t )
		{
			ParentParser = p;
			ParentBranch = t;
			Success = false;
		}

		public TreeBranch ( Parser p, TreeBranch t, Token k, int optionNum, int stepNum, int level )
			: this( p, t )
		{
			BranchToken = k;
			Success = true;
			TokenAdvance = 1;
			StartToken = BranchToken.TokenNumber;
			Level = level + 1;
			RuleOptionNumber = optionNum;
			RuleStepNumber = stepNum;
			if ( t.IsTokenBranch && t.BranchToken.Name == "FLOAT" )
			{

			}
		}

		public TreeBranch ( Parser p, TreeBranch t, ParsingRule rule, int optionNum, int stepnum, int tokenStartAt, int level )
			: this( p, t )
		{
			StartToken = tokenStartAt;
			Level = level + 1;
			BranchRule = rule;
			RuleOptionNumber = optionNum;
			RuleStepNumber = stepnum;
		}

		public bool IsTokenBranch
		{
			get { return BranchToken != null; }
		}

		public bool IsRuleBranch
		{
			get { return BranchRule != null; }
		}

		protected IEnumerable<KeyValuePair<string, Symbol>> GetRawSymbols ( )
		{
			foreach ( KeyValuePair<string, Symbol> kvp in SymbolList )
			{
				yield return kvp;
			}

			if ( ParentBranch != null && !IsIsolated )
			{
				foreach ( KeyValuePair<string, Symbol> x in ParentBranch.GetRawSymbols( ) )
				{
					yield return x;
				}
			}
		}

		public IEnumerable<Symbol> GetSymbols ( )
		{
			List<Symbol> list = new List<Symbol>( );

			foreach ( KeyValuePair<string, Symbol> kvp in GetRawSymbols( ) )
			{
				if ( !list.Contains( kvp.Value ) )
				{
					list.Add( kvp.Value );
				}
			}

			foreach ( Symbol s in list )
			{
				yield return s;
			}
		}

		protected Symbol GetSymbol ( Token token )
		{
			if ( token.IsIdentifier )
			{
				return GetSymbol( token.StringValue );
			}
			else
			{
				throw new CraftyException( "No symbol available for token.", token );
			}
		}

		protected Symbol GetSymbol ( string name )
		{
			if ( SymbolList.ContainsKey( name ) )
			{
				return SymbolList[name];
			}
			else
			{
				if ( ParentBranch != null )
				{
					return ParentBranch.GetSymbol( name );
				}
				else
				{
					return null; //throw new CraftyException( string.Format( "Symbol by name {0} could not be found.", name ) );
				}
			}
		}

		public IEnumerable<TreeBranch> GetBranches ( bool children )
		{
			foreach ( TreeBranch b in branches )
			{
				yield return b;
				if ( children )
				{
					foreach ( TreeBranch bb in b.GetBranches( children ) )
					{
						yield return bb;
					}
				}
			}
		}

		public TreeBranch GetBranchAt ( int index )
		{
			return branches[index];
		}

		private bool SymbolDeclared ( string n )
		{
			if ( SymbolList.ContainsKey( n ) )
			{
				return true;
			}
			else
			{
				if ( ParentBranch == null || ( IsIsolated ) )
				{
					return false;
				}
				else
				{
					return ParentBranch.SymbolDeclared( n );
				}
			}
			//return ( ParentBranch != null && ParentBranch.SymbolDeclared( n ) ) || symbols.ContainsKey( n );
		}

		public bool IsLocalSymbol ( Symbol s )
		{
			foreach ( KeyValuePair<string, Symbol> kvp in SymbolList )
			{
				if ( kvp.Value == s ) { return true; }
			}
			return false;
		}

		public bool IsLocalSymbol ( string s )
		{
			return SymbolList.ContainsKey( s );
		}

		public void ReplaceLiteral ( Token t )
		{
			if ( t.Name == "FLOAT" )
			{
				int autoCount = 0;
				string symbolName = "_auto" + autoCount.ToString( );

				while ( TreeRoot.symbols.ContainsKey( symbolName ) )
				{
					autoCount++;
					symbolName = "_auto" + autoCount.ToString( );
				}

				Symbol symbol = new Symbol( t.FloatValue );
				Token token = new Token( "IDENTIFIER", t.TokenNumber, t.Line, symbolName );

				ParentParser.ReplaceToken( t, token );
				this.BranchToken = token;
				TreeRoot.symbols.Add( symbolName, symbol );
			}
		}

		/// <summary>
		/// Adds a function symbol. Added to the tree root.
		/// </summary>
		/// <param name="returntype"></param>
		/// <param name="signature"></param>
		public void AddFunctionSymbol ( string returntype, string signature )
		{
			if ( !signature.StartsWith( "_func_" ) )
			{
				signature = "_func_" + signature;
			}

			Symbol function = new Symbol( signature, returntype );
			function.Name = signature;
			IDictionary<string, Symbol> symbolList = TreeRoot.SymbolList;

			if ( TreeRoot.SymbolDeclared( signature ) )
			{
				throw new CraftyException( string.Format( "A function with the signature {0} has already been declared.", signature.Substring( 6 ) ), function );
			}

			symbolList.Add( signature, function );

			//GetSymbol( signature ).Name = signature;
			TreeRoot.functionBranches.Add( function, this );
		}

		public void AddSymbol ( Token currentTypeToken, Token currentIDToken )
		{
			if ( currentTypeToken.StringValue == "int" || currentTypeToken.StringValue == "float" )
			{
				SymbolList.Add( currentIDToken.StringValue, new Symbol( 0f ) );
			}
			else if ( currentTypeToken.StringValue == "string" )
			{
				SymbolList.Add( currentIDToken.StringValue, new Symbol( "" ) );
			}
			else if ( currentTypeToken.StringValue == "bool" )
			{
				SymbolList.Add( currentIDToken.StringValue, new Symbol( false ) );
			}
			else if ( currentTypeToken.StringValue == "event" )
			{
				SymbolList.Add( currentIDToken.StringValue, new Symbol( (Symbol) null, currentIDToken.StringValue ) );
			}
			else
			{
				throw new CraftyException( string.Format( "Unknown variable type {0}.", currentTypeToken.StringValue ), currentTypeToken );
			}

			GetSymbol( currentIDToken.StringValue ).Name = currentIDToken.StringValue;

			foreach ( TreeBranch bb in branches )
			{
				if ( bb.IsTokenBranch && bb.BranchToken.Name == "IDENTIFIER" && bb.BranchToken.StringValue == currentIDToken.StringValue )
				{
					bb.BranchToken.IdentifierSymbol = GetSymbol( bb.BranchToken.StringValue );
				}
			}
		}

		public const int MaxNumberOfFunctionArguments = 16;

		/// <summary>
		/// Traverses the branch for functions.
		/// </summary>
		public void DoFunctions ( )
		{
			string signature = null;
			StringBuilder sb = new StringBuilder( );
			TreeBranch declareBranch = null;
			List<string> argTypes = new List<string>( );
			int totalArguments = 0;
			// Investigate all branches.
			for ( int i = 0; i < branches.Count; i++ )
			{
				totalArguments = 0;
				// If it's a function declaration... process.
				if ( branches[i].IsRuleBranch && branches[i].BranchRule.Name == "functiondeclare" )
				{
					declareBranch = branches[i];
					//sb = new StringBuilder( );
					//sb.Append( declareBranch.branches[0].BranchToken.StringValue ); sb.Append("_");
					// Append the function's name to the signature.
					//sb.Append( declareBranch.branches[1].BranchToken.StringValue );
					// Loop through the argument declaration.

					for ( int x = 0; x < declareBranch.branches[3].branches.Count; x++ )
					{
						// This replaces any function_argument_declaration_more with its children (which should all be tokens).
						if ( declareBranch.branches[3].branches[x].IsRuleBranch )
						{
							declareBranch.branches[3].ReplaceBranchWithChildren( declareBranch.branches[3].branches[x] );
						}
					}

					/*
					for ( int x = 0; x < declareBranch.branches[3].branches.Count; x++ )
					{
						// This replaces any function_argument_declaration_more with its children (which should all be tokens).
						if ( declareBranch.branches[3].branches[x].IsRuleBranch )
						{
							declareBranch.branches[3].ReplaceBranchWithChildren( declareBranch.branches[3].branches[x] );
						}

						// Skips over comma and identifiers since we don't want either of those to be part of the signature.
						if ( declareBranch.branches[3].branches[x].BranchToken.Name == "COMMA" || 
							declareBranch.branches[3].branches[x].BranchToken.Name == "IDENTIFIER" )
						{
							continue;
						} // If we find a close bracket token, we've obviously reached the end of the list.
						else if ( declareBranch.branches[3].branches[x].BranchToken.Name == "CLOSEBRACKET" )
						{
							break;
						}

						// At this point, if there's anything but a type identifier, we should break. Though this should never return true.
						if (// declareBranch.branches[3].branches[x].BranchToken.Name != "IDENTIFIER" &&
							declareBranch.branches[3].branches[x].BranchToken.Name != "TYPEIDENTIFIER" )
						{
							break;
						}

						//sb.Append( "_" );
						//sb.Append( declareBranch.branches[3].branches[x].BranchToken.StringValue );
						argTypes.Add( declareBranch.branches[3].branches[x].BranchToken.StringValue );
						// Check if the number of arguments is above the fixed maximum. 
						if ( ++totalArguments > MaxNumberOfFunctionArguments )
						{
							throw new CraftyException( "Too many arguments in function declaration.", declareBranch.branches[1].BranchToken );
						}
					}*/

					argTypes = new List<string>( GetFunctionArgumentTypes( declareBranch.branches[3] ) );

					if ( ++totalArguments > MaxNumberOfFunctionArguments )
					{
						throw new CraftyException( "Too many arguments in function declaration.", declareBranch.branches[1].BranchToken );
					}

					//signature = sb.ToString( );
					signature = GenerateFunctionSignature( declareBranch.branches[1].BranchToken.StringValue, argTypes.ToArray( ) );
					// declareBranch.branches[1].BranchToken.StringValue
					AddFunctionSymbol( declareBranch.branches[0].BranchToken.StringValue, signature );
				}
				else
				{
					// Oh so it's not a function declaration... but who knows about its children. 
					branches[i].DoFunctions( );
				}
			}
		}
		protected string[] GetFunctionArgumentTypes ( TreeBranch treeBranch )
		{
			return GetFunctionArgumentTypes( treeBranch, 0 );
		}

		protected string[] GetFunctionArgumentTypes ( TreeBranch treeBranch, int skip )
		{
			for ( int i = skip; i < branches.Count; i++ )
			{
				if ( branches[i].IsRuleBranch && branches[i].BranchRule.Name.StartsWith( "function_call_args" ) )
				{
					ReplaceBranchWithChildren( branches[i] );
				}
			}

			List<string> argTypes = new List<string>( );
			TreeBranch thisTreeBranch = null;
			bool skipNext = false;
			string returnType = null;
			for ( int x = skip; x < treeBranch.branches.Count; x++ )
			{
				if ( skipNext )
				{
					skipNext = false;
					continue;
				}

				thisTreeBranch = treeBranch.branches[x];

				if ( thisTreeBranch.IsTokenBranch )
				{
					if ( thisTreeBranch.BranchToken.IsTypeIdentifier )
					{
						skipNext = true;
					}

					if ( thisTreeBranch.BranchToken.Name == "COMMA" || thisTreeBranch.BranchToken.Name == "OPENBRACKET" )
					{
						continue;
					}

					if ( thisTreeBranch.BranchToken.Name == "CLOSEBRACKET" )
					{
						break;
					}
				}

				if ( thisTreeBranch.IsTokenBranch && thisTreeBranch.BranchToken.Name == "TYPEIDENTIFIER" )
				{
					returnType = thisTreeBranch.BranchToken.StringValue;
				}
				else
				{
					returnType = thisTreeBranch.GetReturnType( );
				}

				argTypes.Add( returnType );
				/*
				if ( thisTreeBranch.IsRuleBranch )
				{
					argTypes.Add( thisTreeBranch.GetReturnType( ) );
				}
				if ( thisTreeBranch.IsTokenBranch )
				{
					argTypes.Add( thisTreeBranch.GetReturnType( ) );
				}*/
				/*
				// Skips over comma and identifiers since we don't want either of those to be part of the signature.
				if ( treeBranch.branches[x].BranchToken.Name == "COMMA" ||
					treeBranch.branches[x].BranchToken.Name == "IDENTIFIER" )
				{
					continue;
				} // If we find a close bracket token, we've obviously reached the end of the list.
				else if ( treeBranch.branches[x].BranchToken.Name == "CLOSEBRACKET" )
				{
					break;
				}

				// At this point, if there's anything but a type identifier, we should break. Though this should never return true.
				if ( treeBranch.branches[x].BranchToken.Name != "TYPEIDENTIFIER" )
				{
					break;
				}

				//sb.Append( "_" );
				//sb.Append( declareBranch.branches[3].branches[x].BranchToken.StringValue );
				argTypes.Add( treeBranch.branches[x].BranchToken.StringValue );
				 * */
			}

			return argTypes.ToArray( );
		}

		protected string GetFunctionSignature ( TreeBranch branch )
		{
			if ( branch.IsRuleBranch )
			{
				if ( branch.BranchRule.Name == "functioncall" )
				{
					return GenerateFunctionSignature( branch.branches[0].BranchToken.StringValue, GetFunctionArgumentTypes( branch ) );
				}
				else if ( branch.BranchRule.Name == "functiondeclare" )
				{
					return GenerateFunctionSignature( branch.branches[1].BranchToken.StringValue, GetFunctionArgumentTypes( branch, 3 ) );
				}
			}
			throw new ArgumentException( "Branch must be a functioncall or functiondeclare." );
		}

		protected string GenerateFunctionSignature ( string name, params string[] argTypes )
		{
			string signature = "_func";
			signature += "_" + name.ToLower( );
			for ( int i = 0; i < argTypes.Length; i++ )
			{
				signature += "_" + argTypes[i];
			}

			return signature;
		}

		/// <summary>
		/// Generates the symbol table.
		/// </summary>
		public void GenerateSymbolTable ( )
		{
			Token currentTypeToken = null;
			Token currentIDToken = null;

			if ( IsIsolated )
			{
				bool typeDeclaration = false;

				for ( int i = 0; i < branches.Count; i++ )
				{
					if ( branches[i].IsRuleBranch && ( branches[i].BranchRule.Name == "statementlist" || branches[i].BranchRule.Name == "functionbody" ) )
					{
						branches[i].GenerateSymbolTable( );
					}

					if ( branches[i].IsRuleBranch &&
						( branches[i].BranchRule.Name == "function_argument_declaration" ||
						branches[i].BranchRule.Name == "function_argument_declaration_more" ) )
					{
						ReplaceBranchWithChildren( branches[i] );
						typeDeclaration = true;
					}

					if ( typeDeclaration )
					{
						if ( branches[i].IsTokenBranch )
						{
							if ( branches[i].BranchToken.Name == "TYPEIDENTIFIER" )
							{
								currentTypeToken = branches[i].BranchToken;
							}
							else if ( branches[i].BranchToken.Name == "IDENTIFIER" )
							{
								currentIDToken = branches[i].BranchToken;

								AddSymbol( currentTypeToken, currentIDToken );

								currentIDToken = null;
								currentTypeToken = null;

							}
							else if ( branches[i].BranchToken.Name == "COMMA" )
							{

							}
							else
							{
								typeDeclaration = false;
							}
						}
					}
				}

				return;
			}

			if ( IsRuleBranch && BranchRule.Name == "functioncall" )
			{
				List<string> argTypes = new List<string>( );

				argTypes = new List<string>( GetFunctionArgumentTypes( this, 2 ) );

				string sig = GenerateFunctionSignature( branches[0].BranchToken.StringValue, argTypes.ToArray( ) );

				/*
				if ( branches[i].IsTokenBranch && branches[i].BranchToken.Name == "CLOSEBRACKET" )
				{
					break;
				}

				if ( branches[i].IsTokenBranch )
				{
					if ( branches[i].BranchToken.Name == "IDENTIFIER" )
					{
						//argSymbol = GetSymbol( branches[i].BranchToken.StringValue );
						//argTypes.Add( branches[i].BranchToken.ReturnType );
						argTypes.Add( GetSymbol( branches[i].BranchToken.StringValue ).Type );
					}
					else if ( branches[i].BranchToken.Name == "FLOAT" )
					{
						argTypes.Add( "float" );
					}
					else if ( branches[i].BranchToken.Name == "TRUE" )
					{
						argTypes.Add( "bool" );
					}
					else if ( branches[i].BranchToken.Name == "FALSE" )
					{
						argTypes.Add( "bool" );
					}
				}

				if ( branches[i].IsRuleBranch )
				{
					argTypes.Add( branches[i].GetReturnType( ) );
				}
			}*/


				if ( !TreeRoot.SymbolDeclared( sig ) )
				{
					throw new CraftyException( string.Format( "Function with signature {0} was never declared.", sig ), branches[0].BranchToken );
				}
			}

			int branchNumber = 0;
			foreach ( TreeBranch b in GetBranches( false ) )
			{
				if ( b.IsTokenBranch )
				{
					if ( b.BranchToken.Name == "IDENTIFIER" && SymbolDeclared( b.BranchToken.StringValue ) )
					{
						b.BranchToken.IdentifierSymbol = GetSymbol( b.BranchToken.StringValue );
						continue;
					}

					currentIDToken = b.BranchToken;
					currentIDToken = b.BranchToken;
					if ( currentIDToken.Name == "IDENTIFIER" && !( branchNumber == 0 && b.ParentBranch.IsRuleBranch && b.ParentBranch.BranchRule.Name == "functioncall" ) )
					{
						currentIDToken = b.BranchToken;
						// Check if the symbol has already been declared.
						if ( !SymbolDeclared( currentIDToken.StringValue ) )
						{
							throw new CraftyException( string.Format( "Variable {0} was not declared before use.", currentIDToken.StringValue ), currentIDToken );
						}
					}
				}
				else
				{
					if ( b.BranchRule.Name == "variable_declaration_statement" )
					{
						currentTypeToken = b.GetBranchAt( 0 ).BranchToken;
						currentIDToken = b.GetBranchAt( 1 ).BranchToken;
						if ( !currentTypeToken.IsTypeIdentifier )
						{
							throw new CraftyException( "Token should be type identifier." );
						}

						if ( SymbolDeclared( currentIDToken.StringValue ) )
						{
							throw new CraftyException( string.Format( "Symbol was already declared." ), currentIDToken, GetSymbol( currentIDToken.StringValue ) );
							//throw new CraftyException( string.Format( "Variable {0} was not declared before use.", currentIDToken.StringValue ), currentIDToken );
							/*
							if ( currentTypeToken.StringValue != GetSymbol(currentIDToken.StringValue).Type )
							{
								throw new CraftyException( string.Format( "Variable {0} cannot be redeclared as a different type.", currentIDToken.StringValue ), currentIDToken );
							}
							 * */
						}
						/*
						if ( b.BranchRule.Name == "functiondeclare" )
						{
							SymbolList.Add( currentIDToken.StringValue, new Symbol( currentIDToken.StringValue, currentTypeToken.StringValue ) );
						}
						else if ( currentTypeToken.StringValue == "int" || currentTypeToken.StringValue == "float" )
						{
							SymbolList.Add( currentIDToken.StringValue, new Symbol( 0f ) );
						}
						else if ( currentTypeToken.StringValue == "string" )
						{
							SymbolList.Add( currentIDToken.StringValue, new Symbol( "" ) );
						}
						else if ( currentTypeToken.StringValue == "bool" )
						{
							SymbolList.Add( currentIDToken.StringValue, new Symbol( false ) );
						}
						else if ( currentTypeToken.StringValue == "event" )
						{
							SymbolList.Add( currentIDToken.StringValue, new Symbol( (Symbol) null, currentIDToken.StringValue ) );
						}
						else
						{
							throw new CraftyException( string.Format( "Unknown variable type {0}.", currentTypeToken.StringValue ), currentTypeToken );
						}

						GetSymbol( currentIDToken.StringValue ).Name = currentIDToken.StringValue;

						foreach ( TreeBranch bb in branches )
						{
							if ( bb.IsTokenBranch && bb.BranchToken.Name == "IDENTIFIER" && bb.BranchToken.StringValue == currentIDToken.StringValue )
							{
								bb.BranchToken.IdentifierSymbol = GetSymbol( bb.BranchToken.StringValue );
							}
						}*/

						AddSymbol( currentTypeToken, currentIDToken );
					}
				}
				b.GenerateSymbolTable( );

				branchNumber++;
			}

			foreach ( KeyValuePair<string, Symbol> kvp in symbols )
			{
				break;
				kvp.Value.Name = kvp.Key;
				foreach ( TreeBranch b in branches )
				{
					if ( b.IsTokenBranch && b.BranchToken.Name == "IDENTIFIER" )
					{
						b.BranchToken.IdentifierSymbol = GetSymbol( b.BranchToken.StringValue );
					}
				}
			}

			return;
			if ( !IsRoot )
			{
				throw new CraftyException( "This method can only be called on the root of the syntax tree." );
			}


			IDictionary<string, Symbol> symbolList = null;

			if ( IsRoot )
			{
				symbolList = (IDictionary<string, Symbol>) symbols;
			}
			else
			{
				symbolList = (IDictionary<string, Symbol>) TreeRoot.symbols;
				//symbolList = (IDictionary<string, Symbol>) symbols;
			}

			// Search all branches for identifiers.
			foreach ( TreeBranch b in GetBranches( true ) )
			{
				string returnType = null;
				// If it's a token branch it might be an identifier.
				if ( b.IsTokenBranch )
				{
					currentIDToken = b.BranchToken;
					if ( currentIDToken.Name == "IDENTIFIER" && b.ParentBranch.BranchRule.Name != "variables" )
					{
						currentIDToken = b.BranchToken;
						// Check if the symbol has already been declared.
						if ( !SymbolDeclared( currentIDToken.StringValue ) )
						{
							throw new CraftyException( string.Format( "Variable {0} was not declared before use.", currentIDToken.StringValue ), currentIDToken );
						}
					}
				}
				else
				{
					if ( b.BranchRule.Name == "variable_declaration_statement" || b.BranchRule.Name == "functiondeclare" )
					{
						currentTypeToken = b.GetBranchAt( 0 ).BranchToken;
						currentIDToken = b.GetBranchAt( 1 ).BranchToken;
						if ( !currentTypeToken.IsTypeIdentifier )
						{
							throw new CraftyException( "Token should be type identifier." );
						}
						else
						{
							if ( SymbolDeclared( currentIDToken.StringValue ) )
							{
								//throw new CraftyException( string.Format( "Variable {0} already declared.", currentIDToken.StringValue ), currentIDToken );

								if ( currentTypeToken.StringValue != symbolList[currentIDToken.StringValue].Type )
								{
									throw new CraftyException( string.Format( "Variable {0} cannot be redeclared as a different type.", currentIDToken.StringValue ), currentIDToken );
								}
							}
							else
							{

								if ( b.BranchRule.Name == "functiondeclare" )
								{
									symbolList.Add( currentIDToken.StringValue, new Symbol( currentIDToken.StringValue, currentTypeToken.StringValue ) );
								}
								else if ( currentTypeToken.StringValue == "int" || currentTypeToken.StringValue == "float" )
								{
									symbolList.Add( currentIDToken.StringValue, new Symbol( 0f ) );
								}
								else if ( currentTypeToken.StringValue == "string" )
								{
									symbolList.Add( currentIDToken.StringValue, new Symbol( "" ) );
								}
								else if ( currentTypeToken.StringValue == "bool" )
								{
									symbolList.Add( currentIDToken.StringValue, new Symbol( false ) );
								}
								else if ( currentTypeToken.StringValue == "event" )
								{
									symbolList.Add( currentIDToken.StringValue, new Symbol( (Symbol) null, currentIDToken.StringValue ) );
								}
								else
								{
									throw new CraftyException( string.Format( "Unknown variable type {0}.", currentTypeToken.StringValue ), currentTypeToken );
								}
							}

							if ( b.branches.Count == 3 && false )
							{
								returnType = b.GetBranchAt( 2 ).GetReturnType( );
								if ( returnType != symbols[currentIDToken.StringValue].Type )
								{
									throw new CraftyException( string.Format( "{0} was initiated as a {1} but is being assigned a {2} value.", currentIDToken.StringValue, symbols[currentIDToken.StringValue].Type, returnType ), currentIDToken, symbols[currentIDToken.StringValue] );
								}
							}
						}
					}

					if ( b.ParentBranch == this )
					{

					}
				}
			}

			foreach ( KeyValuePair<string, Symbol> kvp in symbols )
			{
				kvp.Value.Name = kvp.Key;
			}

			string comptype = null;
			TreeBranch idBranch = null;
			TreeBranch assignBranch = null;
			foreach ( TreeBranch b in GetBranches( true ) )
			{
				if ( idBranch == null && b.IsTokenBranch && b.BranchToken.Name == "IDENTIFIER" )
				{
					idBranch = b;
				}
				else if ( idBranch != null && b.IsRuleBranch && b.BranchRule.Name == "assign" )
				{
					assignBranch = b;
				}
				else
				{
					idBranch = null;
					assignBranch = null;
				}

				if ( idBranch != null && assignBranch != null )
				{
					comptype = assignBranch.GetReturnType( );
					if (/*
						(	 symbols[idBranch.BranchToken.StringValue].IsFunction && 
							 symbols[idBranch.BranchToken.StringValue].ReturnType != comptype ) ||
						(	!symbols[idBranch.BranchToken.StringValue].IsFunction &&
							 symbols[idBranch.BranchToken.StringValue].Type != comptype 
						)*/
						symbols[idBranch.BranchToken.StringValue].Type != comptype
						)
					{
						throw new CraftyException(
							string.Format( "{0} is a {1} but is being assigned a {2}.",
								idBranch.BranchToken.StringValue,
								symbols[idBranch.BranchToken.StringValue].Type,
								comptype
							),
							idBranch.BranchToken,
							symbols[idBranch.BranchToken.StringValue]
						);
					}
				}
			}

			foreach ( TreeBranch t in GetBranches( true ) )
			{
				if ( !IsRoot ) { break; }
				if ( t.IsTokenBranch && t.BranchToken.IsIdentifier )
				{
					t.BranchToken.IdentifierSymbol = symbols[t.BranchToken.StringValue];
				}
			}
		}

		/*public string GetReturnType ( )
		{
			return GetReturnType( null );
		}*/

		public string GetReturnType ( )//string expectedType
		{
			string returnType = null;
			//Debug.Print( "".PadRight( Level * 4, ' ' ) + "New tree for {0}.", ( IsRuleBranch ? "branch rule " + BranchRule.Name : "branch token " + BranchToken.Name ) );
			if ( IsRuleBranch )
			{
				if ( BranchRule.Name == "functioncall" )
				{
					string functionCallReturnType = GetSymbol( GenerateFunctionSignature( this.Branches[0].BranchToken.StringValue, GetFunctionArgumentTypes( this, 2 ) ) ).FunctionReturnType;
					return functionCallReturnType;
				}
				else if ( BranchRule.HasReturnType )
				{
					//Debug.Print( "".PadRight( Level * 4, ' ' ) + "Branch rule is type {0}.", BranchRule.ReturnType );
					return BranchRule.ReturnType;
				}
				else
				{
					return branches[0].GetReturnType( );
					/*throw new InvalidOperationException( "What are you doing?" );
					foreach ( TreeBranch b in GetBranches( false ) )
					{
						if ( b.GetReturnType( ) != null )
						{
							//Debug.Print( "".PadRight( Level * 4, ' ' ) + "Sub branch type is {0}.", b.GetReturnType( ) );
							if ( returnType == null )
							{
								returnType = b.GetReturnType( ); // expectedType
							}
							else
							{
								if ( b.GetReturnType( ) != returnType ) //expectedType
								{
									//throw new CraftyException( string.Format( "Was expecting another {0} but found {1} instead.", returnType, b.GetReturnType( ) ) );
									return null;
								}
							}
							//return b.ReturnType( );
						}
					}*/
					/*
					if ( expectedType != null && returnType != expectedType )
					{
						//throw new CraftyException( string.Format( "Found {0} but was expecting {1}.", returnType, expectedType ) );
					}*/
					//return returnType;
				}
			}
			else if ( IsTokenBranch )
			{
				if ( BranchToken.IsTypeIdentifier )
				{
					returnType = BranchToken.StringValue;
					//Debug.Print( "".PadRight( Level * 4, ' ' ) + "Branch token type is {0}.", BranchToken.StringValue );
					/*if ( expectedType != null && returnType != expectedType )
					{
						//throw new CraftyException( string.Format( "Found {0} but was expecting {1}.", returnType, expectedType ), BranchToken );
					}*/

					return returnType;
				}
				else if ( BranchToken.ReturnType != null )
				{
					returnType = BranchToken.ReturnType;
					/*
					if ( expectedType != null && returnType != expectedType )
					{
						//	throw new CraftyException( string.Format( "Found {0} but was expecting {1}.", returnType, expectedType ), BranchToken );
					}*/
					return returnType;
				}
				else
				{
					throw new CraftyException( "Token has no return type.", BranchToken );
				}
			}

			return null;
		}

		public Symbol GetSymbolFromID ( Token t )
		{
			return TreeRoot.symbols[t.StringValue];
		}

		public void ReplaceLiterals ( )
		{
			for ( int i = 0; i < branches.Count; i++ )
			{
				if ( branches[i].IsRuleBranch )
				{
					branches[i].ReplaceLiterals( );
				}
				else
				{
					if ( branches[i].IsTokenBranch && branches[i].BranchToken.Name == "FLOAT" )
					{
						ReplaceLiteral( branches[i].BranchToken );
					}
				}
			}
		}

		public void Traverse ( )
		{
			if ( Level > 2000 )
			{
				throw new CraftyException( "Possible stack overflow detected!" );
			}

			//List<ParsingRule> currentOption = null;
			//List<string> stepParts = null;

			// Option loop

			Debug.Print( "".PadRight( ( Level - 1 ) * 4, ' ' ) + "New branch for {0}.", BranchRule.Name );

			ParsingStep[] optionSteps = null;

			while ( true )
			{
				if ( currentOptionNumber == BranchRule.NumberOfOptions )
				{
					break;
				}

				optionSteps = BranchRule.GetOptionSteps( currentOptionNumber );

				for ( int stepNumber = 0; stepNumber < optionSteps.Length; )
				{
					if ( ParentParser.RulesContains( optionSteps[stepNumber].Value ) )
					{
						TreeBranch branch = null;
						/*
						if ( optionSteps[stepNumber].Value == "ifrootstatement" )
						{
							branch = new IfRootBranch( ParentParser, this, ParentParser.GetRuleAt( optionSteps[stepNumber].Value ), currentOptionNumber, stepNumber, StartToken + tokenOffset, Level );
						}
						else if ( optionSteps[stepNumber].Value == "ifstatement" ||
									optionSteps[stepNumber].Value == "elseifstatement" ||
									optionSteps[stepNumber].Value == "elsestatement"
							)
						{
							branch = new IfBranch( ParentParser, this, ParentParser.GetRuleAt( optionSteps[stepNumber].Value ), currentOptionNumber, stepNumber, StartToken + tokenOffset, Level );
							if ( optionSteps[stepNumber].Value == "elsestatement" )
							{
								( (IfBranch) branch ).IsElse = true;
							}
						}
						else if ( optionSteps[stepNumber].Value == "booleanexpression" )
						{
							branch = new BooleanExpressionBranch( ParentParser, this, ParentParser.GetRuleAt( optionSteps[stepNumber].Value ), currentOptionNumber, stepNumber, StartToken + tokenOffset, Level );
						}
						else if ( optionSteps[stepNumber].Value == "booleanexpression" )
						{ 
							
						}
						else
						{
							branch = new TreeBranch( ParentParser, this, ParentParser.GetRuleAt( optionSteps[stepNumber].Value ), currentOptionNumber, stepNumber, StartToken + tokenOffset, Level );
						}
						*/
						branch = new TreeBranch( ParentParser, this, ParentParser.GetRuleAt( optionSteps[stepNumber].Value ), currentOptionNumber, stepNumber, StartToken + tokenOffset, Level );

						branch.Traverse( );

						subResult = branch.TokenAdvance;

						if ( branch.TokenAdvance > 0 )
						{
							branches.Add( branch );
						}

						/*
						if ( !branch.Success )
						{
							subResult = 0;
						}
						else
						{
							subResult = branch.TokenAdvance;
						}*/
						// If this statement did not evaluate and is not optional, we go to the next option.
						if ( subResult == 0 && !optionSteps[stepNumber].Optional && !optionSteps[stepNumber].Repeat )
						{
							// next option
							tokenOffset = 0;
							TokenAdvance = 0;

							goto NextOption;
						}
						else if ( subResult == 0 && optionSteps[stepNumber].Repeat )
						{
							if ( repeatstep > 0 || optionSteps[stepNumber].Optional )
							{
								repeatstep = 0;
								stepNumber++;
							}
							else
							{
								tokenOffset = 0;
								TokenAdvance = 0;

								goto NextOption;
							}
						}
						else
						{
							// next token
							tokenOffset += subResult;

							if ( !optionSteps[stepNumber].Repeat )
							{
								stepNumber++;
							}
							else
							{
								repeatstep++;
							}
						}
					}
					else
					{
						if ( StartToken + tokenOffset >= ParentParser.NumberOfTokens )
						{
							tokenOffset = 0;
							TokenAdvance = 0;

							goto NextOption;
						}
						else
						{
							currentToken = ParentParser.GetTokenAt( StartToken + tokenOffset ).Name;
						}

						//currentToken == optionSteps[stepNumber].Value
						if ( currentToken == optionSteps[stepNumber].Value )
						{
							//tokenOffset = 0;
							//goto NextOption;
							if ( optionSteps[stepNumber].HasReturnTypes )
							{

							}

							if ( ParentParser.GetTokenAt( StartToken + tokenOffset ).Name == "FLOAT" )
							{
								//ReplaceLiteral( ParentParser.GetTokenAt( StartToken + tokenOffset ) );
								//currentToken = ParentParser.GetTokenAt( StartToken + tokenOffset ).Name;
							}

							branches.Add( new TreeBranch( ParentParser, this, ParentParser.GetTokenAt( StartToken + tokenOffset ), currentOptionNumber, stepNumber, Level ) );

							tokenOffset++;
							TokenAdvance++;

							stepNumber++;
							repeatstep = 0;
						}
						else
						{
							// next option
							tokenOffset = 0;
							TokenAdvance = 0;

							Debug.Print( "".PadRight( Level * 4, ' ' ) + "Current token {0} is not equal to option step {1}.", currentToken, optionSteps[stepNumber].Value );

							goto NextOption;
						}

					}
				}
				break;
			NextOption:
				currentOptionNumber++;
				repeatstep = 0;
			}

			//return tokenOffset;
			TokenAdvance = tokenOffset;
			Success = TokenAdvance > 0;
			//if ( true || Success ) {
			List<Token> tokens = new List<Token>( );

			int total = Math.Max( 3, TokenAdvance );
			for ( int i = StartToken; i < StartToken + total; i++ )
			{
				if ( i == ParentParser.NumberOfTokens ) { break; }
				tokens.Add( ParentParser.GetTokenAt( i ) );
			}

			StringBuilder sb = new StringBuilder( Level );
			StringBuilder whiteSpaceBuilder = new StringBuilder( Level * 2 );
			for ( int i = 0; i < Level; i++ )
			{
				whiteSpaceBuilder.Append( "    " );
			}
			foreach ( Token t in tokens )
			{
				sb.Append( t.Name + " " );
			}
			if ( TokenAdvance == 0 )
			{
				//sb.Append( ParentParser.GetTokenAt( StartToken ).Name );
			}
			Debug.Print( "{0}'{1}' is{2} a {3} ({4})", whiteSpaceBuilder.ToString( ), sb.ToString( ).TrimEnd( ), Success ? "" : " not", BranchRule.Name, BranchRule.OriginalInput );
			//}
			//Debug.Print( "{0} token is {1}: {2}", ParentParser.GetTokenAt( StartToken ).Name, BranchRule.Name, Success );
		}

		int tokenAdvance = -1;
		public int TokenAdvance
		{
			get { return tokenAdvance; }
			private set { tokenAdvance = value; }
		}

		LinkedList<Operation> ops = new LinkedList<Operation>( );

		public List<Operation> Operations = new List<Operation>( );

		public ICollection<Operation> OpCodes
		{
			get { return ops; }
		}

		public void DoOpCodes ( )
		{
			if ( !IsRoot )
			{
				throw new CraftyException( "Must be root." );
			}

			foreach ( TreeBranch t in Branches )
			{
				foreach ( Operation o in t.GetOperations( ) )
				{
					ops.AddLast( o );
				}
			}

			int instructionNum = 0;
			foreach ( Operation o in ops )
			{
				o.Number = instructionNum;
				instructionNum++;
			}

			foreach ( Operation o in ops )
			{
				if ( o.Code == OpCode.Dummy )
				{
					continue;
				}
				Operations.Add( o );
			}

			for ( int i = 0; i < Operations.Count; i++ )
			{
				if ( i + 1 < Operations.Count )
				{
					Operations[i].Next = Operations[i + 1];

				}
				if ( i - 1 >= 0 )
				{
					Operations[i].Previous = Operations[i - 1];
				}
			}

			Operations[Operations.Count - 1].Next = null;
		}

		protected void ReplaceBranchWithChildren ( TreeBranch tb )
		{
			if ( !Branches.Contains( tb ) )
			{
				throw new CraftyException( "Must pass a treebranch whose parent is this treebranch." );
			}

			int lastBranchAt = branches.IndexOf( tb );

			foreach ( TreeBranch t in tb.GetBranches( false ) )
			{
				branches.Insert( lastBranchAt + 1, t );
				lastBranchAt = branches.IndexOf( t );
				t.ParentBranch = this;
				t.Level = Level + 1;
			}

			branches.Remove( tb );
		}

		private Dictionary<string, Operation> namedOpCodes = new Dictionary<string, Operation>( );

		protected Operation GetNamedOpcode ( string name )
		{
			return namedOpCodes[name];
		}

		protected void SetNamedOpcode ( string k, Operation op )
		{
			if ( namedOpCodes.ContainsKey( k ) )
			{
				namedOpCodes[k] = op;
			}
			else
			{
				namedOpCodes.Add( k, op );
			}
		}

		protected LinkedListNode<Operation> GetNodeForOperation ( Operation o )
		{
			return ops.Find( o );
		}

		protected IEnumerable<Operation> GetOperations ( )
		{
			if ( IsRuleBranch && ( BranchRule.Name == "functionbody" || BranchRule.Name == "statementlist" || BranchRule.Name == "statement" ) )
			{
				if ( BranchRule.Name == "statementlist" )
				{
					ops.AddFirst( new Operation( OpCode.Dummy, "(Statement List)" ) );
				}

				if ( BranchRule.Name == "functionbody" )
				{
					ops.AddFirst( new Operation( OpCode.Dummy, "(Function Body)" ) );
				}

				foreach ( TreeBranch t in Branches )
				{
					foreach ( Operation o in t.GetOperations( ) )
					{
						ops.AddLast( o );
					}
				}

				if ( BranchRule.Name == "statement" )
				{
					ops.AddLast( new Operation( OpCode.ClearStack ) );
				}

			}
			else if ( IsRuleBranch && BranchRule.Name == "assign" )
			{
				//ReplaceBranchWithChildren( Branches[0] );// replace assign with its children
				ReplaceBranchWithChildren( Branches[0] );// replace assignoperators with ASSIGN
				TreeBranch identifierBranch = null;
				bool validAssign = false;
				if ( ParentBranch.BranchRule.Name == "assignstatement" )
				{
					identifierBranch = ParentBranch.branches[0];
				}
				else if ( ParentBranch.BranchRule.Name == "variable_declaration_statement" )
				{
					//identifierBranch = ParentBranch.branches[1];
					validAssign = branches[0].BranchToken.Name == "ASSIGN";
				}
				else
				{
					throw new InternalCraftyException( string.Format( "Parent branch is {0}. Must be assignmentstatement or variable_declaration_statement.", ParentBranch.BranchRule.Name ) );
				}

				LinkedList<Operation> addAfter = new LinkedList<Operation>( );
				if ( branches[0].IsTokenBranch && identifierBranch != null )
				{
					if ( branches[0].BranchToken.Name == "ASSIGN" )
					{
						validAssign = true;
					}
					else if ( branches[0].BranchToken.Name == "PLUSEQUAL" )
					{
						//( identifierBranch.BranchToken.ReturnType == "float" || identifierBranch.BranchToken.ReturnType == "string" )
						if ( identifierBranch.BranchToken.ReturnType == "float" )
						{
							addAfter.AddLast( new Operation( OpCode.Add ) );
							validAssign = true;
						}
						else if ( identifierBranch.BranchToken.ReturnType == "string" )
						{
							addAfter.AddLast( new Operation( OpCode.Concecrate ) );
							validAssign = true;
						}

						if ( validAssign )
						{
							addAfter.AddFirst( new Operation( OpCode.PushSymbol, identifierBranch.BranchToken.IdentifierSymbol ) );
						}
					}
					else if ( branches[0].BranchToken.Name == "MINUSEQUAL" )
					{
						if ( identifierBranch.BranchToken.ReturnType == "float" )
						{
							addAfter.AddLast( new Operation( OpCode.PushSymbol, identifierBranch.BranchToken.IdentifierSymbol ) );
							addAfter.AddLast( new Operation( OpCode.SubtractReverse ) );
							validAssign = true;
						}
					}
					else if ( branches[0].BranchToken.Name == "TIMESEQUAL" )
					{
						if ( identifierBranch.BranchToken.ReturnType == "float" )
						{
							addAfter.AddLast( new Operation( OpCode.PushSymbol, identifierBranch.BranchToken.IdentifierSymbol ) );
							addAfter.AddLast( new Operation( OpCode.Multiply ) );
							validAssign = true;
						}
					}
					else if ( branches[0].BranchToken.Name == "DIVIDEEQUAL" )
					{
						if ( identifierBranch.BranchToken.ReturnType == "float" )
						{
							addAfter.AddLast( new Operation( OpCode.PushSymbol, identifierBranch.BranchToken.IdentifierSymbol ) );
							addAfter.AddLast( new Operation( OpCode.DivideReverse ) );

							validAssign = true;
						}
					}

					if ( !validAssign )
					{
						throw new CraftyException( "Invalid assignment operator.", branches[0].BranchToken );
					}
				}
				if ( Branches[1].IsRuleBranch )
				{
					foreach ( Operation o in Branches[1].GetOperations( ) )
					{
						ops.AddLast( o );
					}
				}
				else if ( Branches[1].IsTokenBranch && Branches[1].BranchToken.Name == "STRING" )
				{
					ops.AddLast( new Operation( OpCode.PushString, Branches[1].BranchToken.StringValue ) );
				}

				foreach ( Operation oo in addAfter )
				{
					ops.AddLast( oo );
				}
			}
			else if ( IsRuleBranch && BranchRule.Name == "assignstatement" )
			{
				/*
				assignstatement
					IDENTIFIER
					assign
						assignoperators
							ASSIGN
						expression
							arithmatic
								float_arithmatic_expression
									float_arithmatic_atomic
										FLOAT
									float_arithmatic_expression_more
										float_arithmatic_joiners
											DIVIDE
										float_arithmatic_atomic
											FLOAT
				ENDSTATEMENT
				*/
				Symbol symbol = Branches[0].BranchToken.IdentifierSymbol;
				foreach ( Operation o in branches[1].GetOperations( ) )
				{
					ops.AddLast( o );
				}

				ops.AddLast( new Operation( OpCode.Store, symbol ) );
				ops.AddLast( new Operation( OpCode.Discard ) );
			}
			else if ( IsRuleBranch && BranchRule.Name == "booleanexpression" )
			{
				Debug.Print( "Flattening boolean expression." );

				for ( int jj = 0; jj < branches.Count; jj++ )
				{
					while ( !branches[jj].IsTokenBranch )
					{
						ReplaceBranchWithChildren( branches[jj] );
					}
				}

				IntermediateExpression ie = new IntermediateExpression(
					new Operator( "AND", 3, OpCode.LogicalAnd ), new Operator( "OR", 3, OpCode.LogicalOr ),
					new Operator( "TRUE", true ), new Operator( "FALSE", true ), new Operator( "IDENTIFIER", true ),
					new Operator( "FLOAT", true ), new Operator( "NOT", 10, OpCode.NegateBoolean ),
					new Operator( "LESSTHAN", 9, OpCode.FloatLessThan ), new Operator( "GREATERTHAN", 9, OpCode.FloatGreaterThan ),
					new Operator( "NOTEQUAL", 9, OpCode.FloatNotEqualTo ), new Operator( "EQUITY", 9, OpCode.FloatEqualTo ),
					new Operator( "LESSTHANOREQUAL", 9, OpCode.FloatLessOrEqualTo ), new Operator( "GREATERTHANOREQUAL", 9, OpCode.FloatGreaterOrEqualTo ),
					new Operator( "MOD", 105, OpCode.Modulus ), new Operator( "TIMES", 105, OpCode.Multiply ), new Operator( "DIVIDE", 105, OpCode.Divide ),
					new Operator( "PLUS", 103, OpCode.Add ), new Operator( "MINUS", 103, OpCode.Subtract ) );

				ie.Evaluate( branches );

				foreach ( Operation o in ie.GetOperations( ) )
				{
					ops.AddLast( o );
				}
			}
			else if ( IsRuleBranch && ( BranchRule.Name == "incrementexpression" || BranchRule.Name == "decrementexpression" ) )
			{
				if ( branches[0].IsTokenBranch && branches[0].BranchToken.Name == "INCREMENT" )
				{
					// Pre-increment
					ops.AddLast( new Operation( OpCode.PushSymbol, branches[1].BranchToken.IdentifierSymbol ) );
					ops.AddLast( new Operation( OpCode.Increment ) );
					ops.AddLast( new Operation( OpCode.Store, branches[1].BranchToken.IdentifierSymbol ) );
				}
				else if ( branches[0].IsTokenBranch && branches[0].BranchToken.Name == "DECREMENT" )
				{
					// Pre-decrement
					ops.AddLast( new Operation( OpCode.PushSymbol, branches[1].BranchToken.IdentifierSymbol ) );
					ops.AddLast( new Operation( OpCode.Decrement ) );
					ops.AddLast( new Operation( OpCode.Store, branches[1].BranchToken.IdentifierSymbol ) );
				}
				else if ( branches[1].IsTokenBranch && branches[1].BranchToken.Name == "INCREMENT" )
				{
					// Post-increment
					ops.AddLast( new Operation( OpCode.PushSymbol, branches[0].BranchToken.IdentifierSymbol ) );
					ops.AddLast( new Operation( OpCode.PushSymbol, branches[0].BranchToken.IdentifierSymbol ) );
					ops.AddLast( new Operation( OpCode.Increment ) );
					ops.AddLast( new Operation( OpCode.Store, branches[0].BranchToken.IdentifierSymbol ) );
					ops.AddLast( new Operation( OpCode.Discard ) );
				}
				else if ( branches[1].IsTokenBranch && branches[1].BranchToken.Name == "DECREMENT" )
				{
					// Post-decrement
					ops.AddLast( new Operation( OpCode.PushSymbol, branches[0].BranchToken.IdentifierSymbol ) );
					ops.AddLast( new Operation( OpCode.PushSymbol, branches[0].BranchToken.IdentifierSymbol ) );
					ops.AddLast( new Operation( OpCode.Decrement ) );
					ops.AddLast( new Operation( OpCode.Store, branches[0].BranchToken.IdentifierSymbol ) );
					ops.AddLast( new Operation( OpCode.Discard ) );
				}
			}
			else if ( IsRuleBranch && BranchRule.Name == "arithmatic" )
			{
				ReplaceBranchWithChildren( branches[0] );
				//ops.AddLast( new Operation( OpCode.Dummy, "Arithmatic expression." ) );
				Debug.Print( "Flattening arithmatic expression." );

				for ( int jj = 0; jj < branches.Count; jj++ )
				{
					while ( !branches[jj].IsTokenBranch )
					{
						ReplaceBranchWithChildren( branches[jj] );
					}
				}

				IntermediateExpression ie = IntermediateExpression.GetMathematicalExpression( );

				ie.Evaluate( branches );

				foreach ( Operation o in ie.GetOperations( ) )
				{
					ops.AddLast( o );
				}
			}
			else if ( IsRuleBranch && BranchRule.Name == "expression" )
			{
				TreeBranch subBranch = Branches[0];
				if ( Branches[0].IsRuleBranch )
				{
					foreach ( Operation o in subBranch.GetOperations( ) )
					{
						ops.AddLast( o );
					}
				}
				else
				{
					throw new CraftyException( "Expression is a token." );
				}
			}
			else if ( IsRuleBranch && BranchRule.Name == "variable_declaration_statement" )
			{
				// push the type onto the stack...
				ops.AddLast( new Operation( OpCode.PushString, Branches[0].BranchToken.StringValue ) );
				// then add the operation to init the symbol.
				ops.AddLast( new Operation( OpCode.InitSymbol, Branches[1].BranchToken.StringValue ) );
				if ( branches.Count > 2 )
				{
					Symbol s = Branches[1].BranchToken.IdentifierSymbol;
					foreach ( Operation o in branches[2].GetOperations( ) )
					{
						ops.AddLast( o );
					}

					ops.AddLast( new Operation( OpCode.Store, s ) );
					ops.AddLast( new Operation( OpCode.Discard ) );
				}
			}
			else if ( IsRuleBranch && BranchRule.Name == "solidblock" )
			{
				for ( int i = 0; i < branches.Count; i++ )
				{
					if ( branches[i].IsRuleBranch && branches[i].BranchRule.Name == "statementlist" )
					{
						foreach ( Operation o in branches[i].GetOperations( ) )
						{
							ops.AddLast( o );
						}
					}
				}
				ops.AddFirst( new Operation( OpCode.StartSolidBlock ) );
				ops.AddLast( new Operation( OpCode.EndSolidBlock ) );
			}
			else if ( IsRuleBranch && BranchRule.Name == "forstatement" )
			{
				TreeBranch tb = null;
				int step = 0;

				List<Operation> expressionOps = new List<Operation>( );

				ops.AddLast( new Operation( OpCode.NoOperation ) );

				for ( int i = 0; i < branches.Count; i++ )
				{
					tb = branches[i];

					if ( step == 0 )
					{
						if ( tb.IsRuleBranch && ( tb.BranchRule.Name == "assignstatement" || tb.BranchRule.Name == "variable_declaration_statement" ) )
						{
							foreach ( Operation o in tb.GetOperations( ) )
							{
								ops.AddLast( o );
							}
						}
						else if ( tb.IsTokenBranch && tb.BranchToken.Name == "ENDSTATEMENT" )
						{
							step++;

							ops.AddLast( new Operation( OpCode.NoOperation, "for boolean expression" ) );
							SetNamedOpcode( "forstart", ops.Last.Value );
							ops.AddLast( new Operation( OpCode.NoOperation, "for loop end" ) );
							SetNamedOpcode( "endofthisif", ops.Last.Value );
						}
					}
					else if ( step == 1 )
					{
						if ( tb.IsRuleBranch && tb.BranchRule.Name == "booleanexpression" )
						{
							foreach ( Operation o in tb.GetOperations( ) )
							{
								//ops.AddBefore( o );
								ops.AddBefore( ops.Last, o );
							}
							ops.AddBefore( ops.Last, new Operation( OpCode.JumpIfFalse, ops.Last.Value ) ); // elseOp.Value
						}
						else if ( tb.IsTokenBranch && tb.BranchToken.Name == "ENDSTATEMENT" )
						{
							step++;
						}
					}
					else if ( step == 2 )
					{
						if ( tb.IsRuleBranch && tb.BranchRule.Name == "forstatement_third" )
						{
							foreach ( Operation o in tb.GetOperations( ) )
							{
								ops.AddBefore( ops.Last, o );
							}
						}
						if ( tb.IsRuleBranch && tb.BranchRule.Name == "expression" )
						{
							foreach ( Operation o in tb.GetOperations( ) )
							{
								//ops.AddBefore( ops.Last, o );
								expressionOps.Add( o );
							}
							step++;
						}
						else if ( tb.IsTokenBranch && tb.BranchToken.Name == "CLOSEBRACKET" )
						{
							step++;
						}
					}
					else if ( step == 3 )
					{
						if ( tb.IsRuleBranch && tb.BranchRule.Name == "statementlist" )
						{
							foreach ( Operation o in tb.GetOperations( ) )
							{
								ops.AddBefore( ops.Last, o );
							}

							foreach ( Operation op in expressionOps )
							{
								ops.AddBefore( ops.Last, op );
							}

							ops.AddBefore( ops.Last, new Operation( OpCode.Jump, GetNamedOpcode( "forstart" ) ) );
							break;


						}
					}
				}
			}
			else if ( IsRuleBranch && BranchRule.Name == "forstatement_third" )
			{
				for ( int i = 0; i < branches.Count; i++ )
				{
					foreach ( Operation o in branches[i].GetOperations( ) )
					{
						ops.AddLast( o );
					}
				}

				ops.AddLast( new Operation( OpCode.ClearStack ) );
			}
			else if ( IsRuleBranch && BranchRule.Name == "whilestatement" )
			{
				TreeBranch tb = null;

				ops.AddFirst( new Operation( OpCode.NoOperation ) );
				ops.AddLast( new Operation( OpCode.NoOperation, "endofthisif" ) );

				if ( namedOpCodes.ContainsKey( "endofthisif" ) )
				{
					namedOpCodes.Remove( "endofthisif" );
				}
				namedOpCodes.Add( "endofthisif", ops.Last.Value );

				for ( int i = 0; i < branches.Count; i++ )
				{
					tb = branches[i];

					if ( tb.IsRuleBranch && tb.BranchRule.Name == "booleanexpression" )
					{
						foreach ( Operation o in tb.GetOperations( ) )
						{
							//ops.AddBefore( o );
							ops.AddBefore( ops.Last, o );
						}
						ops.AddBefore( ops.Last, new Operation( OpCode.JumpIfFalse, ops.Last.Value ) ); // elseOp.Value
					}
					else if ( tb.IsRuleBranch && tb.BranchRule.Name == "statementlist" )
					{
						//ops.AddBefore( ops.Last, new Operation( OpCode.Dummy, "While statementlist start." ) );
						foreach ( Operation o in tb.GetOperations( ) )
						{
							ops.AddBefore( ops.Last, o );
						}

						ops.AddBefore( ops.Last, new Operation( OpCode.Jump, ops.First.Value ) );

						break;
					}
				}
			}
			else if ( IsRuleBranch && BranchRule.Name == "ifrootstatement" )
			{
				Operation oper = null;
				LinkedListNode<Operation> elseOp = null;
				for ( int i = 0; i < branches.Count; i++ )
				{
					if ( branches[i].IsRuleBranch &&
						( branches[i].BranchRule.Name == "ifstatement" ||
						branches[i].BranchRule.Name == "elseifstatement" ||
						branches[i].BranchRule.Name == "elsestatement" ) )
					{
						if ( branches[i].BranchRule.Name == "elsestatement" )
						{
							ops.AddLast( new Operation( OpCode.NoOperation, "ElseStart" ) );
							elseOp = ops.Last;
						}
						else
						{
							ReplaceBranchWithChildren( branches[i] );
						}
					}
				}

				//int step = 0;

				//Operation oper = null;

				LinkedListNode<Operation> endOp = null;
				ops.AddLast( new Operation( OpCode.NoOperation, "EndIf" ) );
				endOp = ops.Last;

				if ( elseOp == null )
				{
					elseOp = endOp;
				}

				namedOpCodes.Add( "elseOp", elseOp.Value );

				TreeBranch tb = null;

				int mode = 0;

				LinkedList<Operation> thisIf = null;

				LinkedList<LinkedList<Operation>> ifList = new LinkedList<LinkedList<Operation>>( );

				for ( int i = 0; i < branches.Count; i++ )
				{
					tb = branches[i];
					if ( mode == 1 )
					{
						if ( tb.IsRuleBranch && tb.BranchRule.Name == "booleanexpression" )
						{
							foreach ( Operation o in tb.GetOperations( ) )
							{
								//ops.AddBefore( o );
								thisIf.AddBefore( thisIf.Last, o );
							}
							thisIf.AddBefore( thisIf.Last, new Operation( OpCode.JumpIfFalse, GetNamedOpcode( "endofthisif" ) ) ); // elseOp.Value
						}
						else if ( tb.IsTokenBranch && tb.BranchToken.Name == "CURLYOPEN" )
						{
							mode = 2;
						}
					}
					else if ( mode == 2 )
					{
						if ( tb.IsRuleBranch && tb.BranchRule.Name == "statementlist" )
						{
							foreach ( Operation o in tb.GetOperations( ) )
							{
								thisIf.AddBefore( thisIf.Last, o );
							}
						}
						else if ( tb.IsTokenBranch && tb.BranchToken.Name == "CURLYCLOSE" )
						{
							thisIf.AddFirst( new Operation( OpCode.StartBlock ) );
							thisIf.AddBefore( thisIf.Last, new Operation( OpCode.EndBlock ) );
							thisIf.AddBefore( thisIf.Last, new Operation( OpCode.Jump, endOp.Value ) );
							mode = 0;
							ifList.AddLast( thisIf );
							//thisIf = new LinkedList<Operation>( );
						}
					}
					else
					{
						if ( tb.IsTokenBranch && tb.BranchToken.Name == "IF" )
						{
							mode = 1;
							thisIf = new LinkedList<Operation>( );
							oper = new Operation( OpCode.NoOperation );
							thisIf.AddLast( oper );
							if ( namedOpCodes.ContainsKey( "endofthisif" ) )
							{
								namedOpCodes.Remove( "endofthisif" );
							}
							namedOpCodes.Add( "endofthisif", oper );
							continue;
						}
						else if ( tb.IsRuleBranch && tb.BranchRule.Name == "elsestatement" )
						{
							for ( int j = 0; j < tb.Branches.Count; j++ )
							{
								if ( tb.Branches[j].IsRuleBranch && tb.Branches[j].BranchRule.Name == "statementlist" )
								{
									thisIf = new LinkedList<Operation>( );
									ops.Remove( elseOp );
									thisIf.AddFirst( elseOp );
									thisIf.AddFirst( new Operation( OpCode.StartBlock ) );
									foreach ( Operation o in tb.Branches[j].GetOperations( ) )
									{
										thisIf.AddLast( o );
									}

									ifList.AddLast( thisIf );
									thisIf.AddLast( new Operation( OpCode.EndBlock ) );
									break;
								}
							}
							break;
						}
					}
				}

				LinkedList<Operation> finalList = new LinkedList<Operation>( );

				foreach ( LinkedList<Operation> list in ifList )
				{
					foreach ( Operation o in list )
					{
						finalList.AddLast( o );
					}
				}

				foreach ( Operation o in ops )
				{
					finalList.AddLast( o );
				}

				ops.Clear( );

				foreach ( Operation o in finalList )
				{
					ops.AddLast( o );
				}

				foreach ( TreeBranch t in Branches )
				{
					break;
					/*
					if ( t.IsRuleBranch && t.BranchRule.Name == "booleanexpression" )
					{
						foreach ( Operation o in t.GetOperations( ) )
						{
							//ops.AddBefore( o );
							ops.AddBefore( ops.Last, o );
						}
						ops.AddBefore( ops.Last, new Operation( OpCode.JumpIfFalse, ops.Last.Value ) );
						//ops.AddBefore( ops.Last, new Operation( OpCode.Discard ) );
					}
					else if ( t.IsRuleBranch && t.BranchRule.Name == "statementlist" )
					{
						foreach ( Operation o in t.GetOperations( ) )
						{
							ops.AddBefore( ops.Last, o );
						}
					}*/
				}
			}
			else if ( IsRuleBranch && BranchRule.Name == "ifstatement" )
			{

			}
			else if ( IsRuleBranch &&
				  ( BranchRule.Name == "booleanexpression" || BranchRule.Name == "compoundbooleanexpression" ) )
			{
				ops.AddLast( new Operation( OpCode.NoOperation ) );
				TreeBranch tt = null;
				for ( int i = 0; i < branches.Count; i++ )
				{
					tt = branches[i];
					if ( tt.IsRuleBranch && tt.BranchRule.Name == "booleanexpression_more" )
					{
						ReplaceBranchWithChildren( tt );
						i = 0;
					}
				}
				bool negate = false;
				foreach ( TreeBranch t in Branches )
				{
					if ( t.IsRuleBranch && t.BranchRule.Name == "not" )
					{
						negate = true;
					}
					else if ( t.IsRuleBranch && ( t.BranchRule.Name == "boolean_atomic" ) )
					{
						if ( t.Branches[0].IsTokenBranch &&
							( t.Branches[0].BranchToken.Name == "TRUE" || t.Branches[0].BranchToken.Name == "FALSE" ) )
						{
							ops.AddBefore( ops.Last, new Operation( OpCode.PushBoolean, t.Branches[0].BranchToken.Name == "TRUE" ) );
						}
						else if ( t.Branches[0].IsRuleBranch &&
							( t.Branches[0].BranchRule.Name == "boolean_string_comparison" ||
							t.Branches[0].BranchRule.Name == "boolean_float_comparison" ||
							t.Branches[0].BranchRule.Name == "compoundbooleanexpression" ) )
						{
							foreach ( Operation o in t.Branches[0].GetOperations( ) )
							{
								ops.AddBefore( ops.Last, o );
							}
						}
						else if ( t.Branches[0].IsTokenBranch && t.Branches[0].BranchToken.Name == "IDENTIFIER" )
						{
							ops.AddBefore( ops.Last, new Operation( OpCode.PushSymbol, t.Branches[0].BranchToken.IdentifierSymbol ) );
						}
					}
					else if ( t.IsTokenBranch && t.BranchToken.Name == "AND" )
					{
						if ( negate ) { ops.AddBefore( ops.Last, new Operation( OpCode.NegateBoolean ) ); negate = false; }
						ops.AddBefore( ops.Last, new Operation( OpCode.JumpIfFalse, ParentBranch.GetNamedOpcode( "endofthisif" ) ) );
					}
					else if ( t.IsTokenBranch && t.BranchToken.Name == "OR" )
					{
						if ( negate ) { ops.AddBefore( ops.Last, new Operation( OpCode.NegateBoolean ) ); negate = false; }
						ops.AddBefore( ops.Last, new Operation( OpCode.JumpIfTrue, ops.Last.Value ) );
					}
				}
			}
			else if ( IsRuleBranch && BranchRule.Name == "boolean_string_comparison" )
			{
				if ( Branches[0].BranchToken.Name == "IDENTIFIER" )
				{
					ops.AddLast( new Operation( OpCode.PushSymbol, Branches[0].BranchToken.IdentifierSymbol ) );
				}
				else if ( Branches[0].BranchToken.Name == "STRING" )
				{
					ops.AddLast( new Operation( OpCode.PushString, Branches[0].BranchToken.StringValue ) );
				}

				if ( Branches[2].BranchToken.Name == "IDENTIFIER" )
				{
					ops.AddLast( new Operation( OpCode.PushSymbol, Branches[2].BranchToken.IdentifierSymbol ) );
				}
				else if ( Branches[2].BranchToken.Name == "STRING" )
				{
					ops.AddLast( new Operation( OpCode.PushString, Branches[2].BranchToken.StringValue ) );
				}

				ops.AddLast( new Operation( OpCode.StringEqualTo ) );
			}
			else if ( IsRuleBranch && BranchRule.Name == "boolean_float_comparison" )
			{
				if ( Branches[0].Branches[0].BranchToken.Name == "IDENTIFIER" )
				{
					ops.AddLast( new Operation( OpCode.PushSymbol, Branches[0].Branches[0].BranchToken.IdentifierSymbol ) );
				}
				else if ( Branches[0].Branches[0].BranchToken.Name == "FLOAT" )
				{
					ops.AddLast( new Operation( OpCode.PushFloat, Branches[0].Branches[0].BranchToken.FloatValue ) );
				}

				if ( Branches[2].Branches[0].BranchToken.Name == "IDENTIFIER" )
				{
					ops.AddLast( new Operation( OpCode.PushSymbol, Branches[2].Branches[0].BranchToken.IdentifierSymbol ) );
				}
				else if ( Branches[2].Branches[0].BranchToken.Name == "FLOAT" )
				{
					ops.AddLast( new Operation( OpCode.PushFloat, Branches[2].Branches[0].BranchToken.FloatValue ) );
				}
				while ( !Branches[1].IsTokenBranch )
				{
					ReplaceBranchWithChildren( Branches[1] );
				}
				if ( Branches[1].IsTokenBranch )
				{
					if ( Branches[1].BranchToken.Name == "EQUITY" )
					{
						ops.AddLast( new Operation( OpCode.FloatEqualTo ) );
					}
					else if ( Branches[1].BranchToken.Name == "LESSTHAN" )
					{
						ops.AddLast( new Operation( OpCode.FloatLessThan ) );
					}
					else if ( Branches[1].BranchToken.Name == "NOTEQUAL" )
					{
						ops.AddLast( new Operation( OpCode.FloatNotEqualTo ) );
					}
					else if ( Branches[1].BranchToken.Name == "GREATERTHAN" )
					{
						ops.AddLast( new Operation( OpCode.FloatGreaterThan ) );
					}
					else if ( Branches[1].BranchToken.Name == "GREATERTHANOREQUAL" )
					{
						ops.AddLast( new Operation( OpCode.FloatGreaterOrEqualTo ) );
					}
					else if ( Branches[1].BranchToken.Name == "LESSTHANOREQUAL" )
					{
						ops.AddLast( new Operation( OpCode.FloatLessOrEqualTo ) );
					}
				}
			}
			else if ( IsRuleBranch && BranchRule.Name == "returnstatement" )
			{
				if ( branches.Count > 1 && branches[1].IsRuleBranch && branches[1].BranchRule.Name == "expression" )
				{
					foreach ( TreeBranch t in Branches )
					{
						foreach ( Operation o in t.GetOperations( ) )
						{
							ops.AddLast( o );
						}
					}
				}

				//ops.AddLast(new Operation(OpCode.Jump,
			}
			else if ( IsRuleBranch && BranchRule.Name == "functiondeclare" )
			{
				string functionSignature = GenerateFunctionSignature( branches[1].BranchToken.StringValue, GetFunctionArgumentTypes( this, 3 ) );
				Symbol function = GetSymbol( functionSignature );
				function.FunctionStartOp = new Operation( OpCode.NoOperation );
				//ops.AddFirst( function.FunctionStartOp );
				int startBody = 4;

				ops.AddLast( new Operation( OpCode.StartFunctionBlock ) );

				List<string> arguments = new List<string>( );

				for ( int f = 3; f < branches.Count; f++ )
				{
					if ( branches[f].IsTokenBranch && branches[f].BranchToken.Name == "CLOSEBRACKET" )
					{
						startBody = f + 1;
						break;
					}
					else
					{
						if ( branches[f].IsTokenBranch )
						{
							if ( branches[f].BranchToken.IsTypeIdentifier )
							{
								// push the type onto the stack...
								ops.AddLast( new Operation( OpCode.PushString, branches[f].BranchToken.StringValue ) );
							}
							else if ( branches[f].BranchToken.IsIdentifier )
							{
								ops.AddLast( new Operation( OpCode.InitSymbol, branches[f].BranchToken.StringValue ) );
								arguments.Add( branches[f].BranchToken.StringValue );
							}
						}
					}
				}

				while ( arguments.Count > 0 )
				{
					ops.AddLast( new Operation( OpCode.Store, GetSymbol( arguments[arguments.Count - 1] ) ) );
					arguments.RemoveAt( arguments.Count - 1 );
				}

				ops.AddLast( new Operation( OpCode.ClearStack ) );

				for ( int f = startBody; f < branches.Count; f++ )
				{
					foreach ( Operation operation in branches[f].GetOperations( ) )
					{
						ops.AddLast( operation );
					}
				}

				ops.AddLast( new Operation( OpCode.EndFunctionBlock ) );
				ops.AddLast( new Operation( OpCode.NoOperation ) );// end

				ops.AddFirst( function.FunctionStartOp );
				ops.AddFirst( new Operation( OpCode.Jump, ops.Last.Value ) );
			}
			else if ( IsRuleBranch && BranchRule.Name == "functioncall" )
			{
				string functionSignature = GenerateFunctionSignature( branches[0].BranchToken.StringValue, GetFunctionArgumentTypes( this, 2 ) );
				Symbol function = GetSymbol( functionSignature );

				for ( int a = branches.Count - 1; a > 2; a-- )
				{
					foreach ( Operation operation in branches[a].GetOperations( ) )
					{
						ops.AddLast( operation );
					}
				}

				ops.AddLast( new Operation( OpCode.DogEarCallStack ) );
				ops.AddLast( new Operation( OpCode.Jump, function.FunctionStartOp ) );
				ops.AddLast( new Operation( OpCode.NoOperation ) );
			}

			if ( IsRuleBranch && BranchRule.IsBlock )
			{
				ops.AddFirst( new Operation( OpCode.StartBlock ) );
				ops.AddLast( new Operation( OpCode.EndBlock ) );
			}

			foreach ( Operation o in ops )
			{
				yield return o;
			}
		}

		public bool IsPrevalent ( TreeBranch t )
		{
			if ( !t.IsTokenBranch || !IsTokenBranch )
			{
				throw new CraftyException( "Must be token branches." );
			}

			if ( BranchToken.Name == "OPENBRACKET" )
			{
				return !true;
			}

			if ( t.BranchToken.Name == "OPENBRACKET" )
			{
				return !false;
			}

			if ( BranchToken.Name == "DIVIDE" || BranchToken.Name == "TIMES" )
			{
				if ( t.BranchToken.Name == "PLUS" || t.BranchToken.Name == "MINUS" )
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Melts down this branch to its atomic operations.
		/// </summary>
		public void Melt ( )
		{
			//List<TreeBranch> branches = null;
			List<TreeBranch> soFar = new List<TreeBranch>( );
			List<TreeBranch> toRemove = new List<TreeBranch>( );

			soFar.AddRange( branches.ToArray( ) );

			foreach ( TreeBranch b in Branches )
			{
				if ( b.IsRuleBranch )
				{
					if ( b.BranchRule.DoMelt ) { toRemove.Add( b ); }
					foreach ( TreeBranch twig in b.Branches )
					{
						if ( b.BranchRule.DoMelt )
						{
							soFar.Insert( branches.IndexOf( b ) + 1, twig );
						}
						twig.Melt( );
					}
				}
			}

			foreach ( TreeBranch b in toRemove )
			{
				soFar.Remove( b );
			}

			branches.Clear( );
			branches.AddRange( soFar.ToArray( ) );

			return;
			/*
			for ( int i = 0; i < Branches.Count; i++ )
			{
				if ( !Branches[i].IsRuleBranch || !Branches[i].BranchRule.DoMelt )
				{
					continue;
				}
				if ( Branches[i].Branches.Count == 1 && Branches[i].IsRuleBranch && !Branches[i].BranchRule.DoMelt )
				{
					//Branches[i].Melt( );
					Branches[i] = Branches[i].Branches[0];
				}
			}

			if ( this.IsRuleBranch && BranchRule.Name == "booleanexpression" )
			{
				//booleanexpression_more
				soFar = new List<TreeBranch>( );
				for ( int i = 0; i < Branches.Count; i++ )
				{
					if ( Branches[i].IsRuleBranch && Branches[i].BranchRule.Name == "booleanexpression_more" )
					{
						//branches = new List<TreeBranch>( );
						//branches.AddRange( Branches[i].Branches );
						Branches[i].Melt( );
						soFar.AddRange( Branches[i].Branches );
					}
					else if ( Branches[i].IsRuleBranch && Branches[i].BranchRule.Name == "boolean_atomic" )
					{
						Branches[i].Melt( );
						Branches[i] = Branches[i].Branches[0];
						soFar.Add( Branches[i] );
					}
					else
					{
						Branches[i].Melt( );
						soFar.Add( Branches[i] );
					}
				}
				branches = soFar;

				Debug.Print( "Melted {0} branch.", BranchRule.Name );
			}
			else if ( this.IsRuleBranch && BranchRule.Name == "booleanexpression_more" )
			{
				Branches[0] = Branches[0];
				Branches[1] = Branches[1].Branches[0];
				//Branches[1].Melt( );
			}
			else if ( IsRuleBranch && BranchRule.Name == "boolean_float_comparison" )
			{
				for ( int i = 0; i < Branches.Count; i++ )
				{
					if ( Branches[i].IsRuleBranch && Branches[i].BranchRule.Name == "float_comparison_operators" )
					{
						Branches[i] = Branches[i].Branches[0];
					}
					else if ( Branches[i].IsRuleBranch && Branches[i].BranchRule.Name == "float_or_id" )
					{
						Branches[i].Melt( );
						Branches[i] = Branches[i].Branches[0];
					}
					else
					{
						Branches[i].Melt( );
					}
					//float_or_id
				}
			}
			else if ( IsRuleBranch && BranchRule.Name == "float_arithmatic_expression" )
			{
				soFar = new List<TreeBranch>( );
				for ( int i = 0; i < Branches.Count; i++ )
				{
					if ( Branches[i].IsRuleBranch && Branches[i].BranchRule.Name == "float_arithmatic_expression_more" )
					{
						Branches[i].Melt( );
						soFar.AddRange( Branches[i].Branches );
					}
					else {
						Branches[i].Melt( );
						soFar.Add( Branches[i] );
					}
				}
				branches = soFar;
			}
			else
			{
				for ( int i = 0; i < Branches.Count; i++ )
				{
					Branches[i].Melt( );
				}
			}

			for ( int i = 0; i < branches.Count; i++ )
			{
				branches[i].Level = Level + 1;
			}*/
		}

		public void FixLevels ( )
		{
			if ( IsRoot ) { Level = 1; }
			for ( int i = 0; i < Branches.Count; i++ )
			{
				Branches[i].FixLevels( Level + 1 );
			}
		}

		private void FixLevels ( int yourlevel )
		{
			Level = yourlevel;
			for ( int i = 0; i < Branches.Count; i++ )
			{
				Branches[i].FixLevels( Level + 1 );
			}


		}

		public bool Success { private set; get; }

		internal void CheckBranchTypes ( )
		{
			Symbol symbol = null;
			IDictionary<string, Symbol> symbolList = null;
			symbolList = this.SymbolList;//TreeRoot.symbols;
			if ( IsRuleBranch && ( BranchRule.Name == "variable_declaration_statement" || BranchRule.Name == "assignstatement" ) )
			{
				string typeA = null;
				string typeB = null;
				Symbol symbolInQuestion = null;
				Token tokenInQuestion = null;
				if ( BranchRule.Name == "variable_declaration_statement" )
				{
					if ( branches.Count > 2 )
					{
						symbolInQuestion = GetSymbol( branches[1].BranchToken );
						tokenInQuestion = branches[1].BranchToken;
						typeA = branches[2].branches[1].GetReturnType( );
						typeB = branches[1].GetReturnType( );
					}
					else
					{
						typeA = null;
						typeB = null;
					}
				}
				else
				{
					symbolInQuestion = GetSymbol( branches[0].BranchToken );
					tokenInQuestion = branches[0].BranchToken;
					typeA = branches[0].GetReturnType();
					typeB = branches[1].branches[1].GetReturnType();
				}
				
				if ( typeA != typeB )
				{
					throw new CraftyException( string.Format( "Value being assigned to symbol is of an incompatible type: {0}.", typeB ), tokenInQuestion, symbolInQuestion );
				}
			}
			else
			{
				foreach ( TreeBranch branch in GetBranches( false ) )
				{
					if ( branch.IsRuleBranch && branch.BranchRule.Name == "functiondeclare" )
					{
						continue;
					}

					if ( ( branch.IsTokenBranch && branch.BranchToken.Name == "IDENTIFIER" ) || ( branch.IsRuleBranch && branch.BranchRule.Name == "functioncall" ) )
					{
						//s = symbolList[b.BranchToken.StringValue];
						if ( branch.IsTokenBranch )
						{
							symbol = GetSymbol( branch.BranchToken.StringValue );
						}
						else
						{
							symbol = GetSymbol( GenerateFunctionSignature( branch.branches[0].BranchToken.StringValue, GetFunctionArgumentTypes( branch, 1 ) ) );
						}

						//Debug.Print( "Type of {0} is {1}.", s.Name, s.Type );
						if ( branch.BranchRuleParsingStep.HasReturnTypes )
						{
							if ( symbol == null ) { continue; }
							//Debug.Print( "Type {0} required.", b.BranchRuleParsingStep.GetReturnType( ) );
							if ( symbol.Type != branch.BranchRuleParsingStep.GetReturnType( ) )
							{
								Debug.Print( "{0}", branch.ParentBranch.BranchRule.Name );
								throw new CraftyException( string.Format( "Symbol is type {0}. A symbol of type {1} was expected.", symbol.Type, branch.BranchRuleParsingStep.GetReturnType( ) ), branch.BranchToken, symbol );
							}
						}
					}
					else
					{
						branch.CheckBranchTypes( );
					}
				}
			}
		}
	}
}