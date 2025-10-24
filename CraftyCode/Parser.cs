using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CraftyCode
{
#if DEBUG
	public class Parser
#else
	internal class Parser
#endif
	{
		List<Token> tokens = null;
		Dictionary<string, ParsingRule> rules = null;

		public Parser ( Token[] input)
		{
			IsCompleted = false;
			tokens = new List<Token>( input );
			SetupRules( );
		}

		public bool RulesContains ( string k )
		{
			return rules.ContainsKey( k );
		}

		public ParsingRule GetRuleAt ( string k )
		{
			return rules[k];
		}

		public int NumberOfTokens
		{
			get { return tokens.Count; }
		}

		public int NumberOfRules
		{
			get { return rules.Count; }
		}

		public IEnumerable<Operation> GetOpCodes ( )
		{
			foreach ( Operation o in ProgramRoot.OpCodes )
			{
				yield return o;
			}
		}

		public IEnumerable<KeyValuePair<string, ParsingRule>> GetRules ( )
		{
			foreach ( KeyValuePair<string, ParsingRule> kvp in rules )
			{
				yield return kvp;
			}
		}

		private void SetupRules ( )
		{
			try
			{
				rules = new Dictionary<string, ParsingRule>( );
				rules.Add( "program", new ParsingRule( "statement_list" ) );
				rules.Add( "statement_list", new ParsingRule( "statement+" ) );
				rules.Add( "statement", new ParsingRule( "END_STATEMENT | solid_block | if_root_statement | trigger_statement | foreach_statement | for_statement | while_statement | function_declare | assign_statement END_STATEMENT | variable_declaration_statement END_STATEMENT | expression END_STATEMENT" ) );

				rules.Add( "variable_declaration_statement", new ParsingRule( "TYPE_IDENTIFIER IDENTIFIER assign?" ) );
				rules.Add( "assign", new ParsingRule( "assignment_operators expression" ) );

				rules.Add( "function_declare", new ParsingRule( "TYPE_IDENTIFIER IDENTIFIER BRACKET_OPEN function_argument_declaration? BRACKET_CLOSE CURLY_OPEN function_body? CURLY_CLOSE" ) );
				rules.Add( "function_body", new ParsingRule( "return_statement+ | statement+" ) );
				rules.Add( "function_argument_declaration", new ParsingRule( "TYPE_IDENTIFIER IDENTIFIER function_argument_declaration_more*" ) );
				rules.Add( "function_argument_declaration_more", new ParsingRule( "COMMA TYPE_IDENTIFIER IDENTIFIER" ) );

				rules.Add( "assign_statement", new ParsingRule( "IDENTIFIER assign" ) );

				rules.Add( "assignment_operators", new ParsingRule( "ASSIGN | PLUS_EQUAL | MINUS_EQUAL | TIMES_EQUAL | DIVIDE_EQUAL | MOD_EQUAL" ) );

				rules.Add( "trigger_statement", new ParsingRule( "WHEN BRACKET_OPEN IDENTIFIER POINT_RIGHT IDENTIFIER.event BRACKET_CLOSE conditional_statement? CURLY_OPEN statement_list? CURLY_CLOSE" ) );
				rules.Add( "conditional_statement", new ParsingRule( "AND BRACKET_OPEN boolean_expression BRACKET_CLOSE" ) );
				rules.Add( "for_statement_third", new ParsingRule( "increment_expression | decrement_expression | assign_statement" ) );
				rules.Add( "for_statement", ParsingRule.CreateBlockRule( "FOR BRACKET_OPEN variable_declaration_statement? END_STATEMENT boolean_expression? END_STATEMENT for_statement_third? BRACKET_CLOSE CURLY_OPEN statement_list? CURLY_CLOSE" ) );
				rules.Add( "foreach_statement", ParsingRule.CreateBlockRule( "FOREACH BRACKET_OPEN TYPE_IDENTIFIER IDENTIFIER IN IDENTIFIER BRACKET_CLOSE CURLY_OPEN statement_list? CURLY_CLOSE" ) );
				rules.Add( "while_statement", ParsingRule.CreateBlockRule( "WHILE BRACKET_OPEN boolean_expression BRACKET_CLOSE CURLY_OPEN statement_list? CURLY_CLOSE" ) );
				rules.Add( "solid_block", ParsingRule.CreateBlockRule( "SOLID CURLY_OPEN statement_list? CURLY_CLOSE" ) );
				rules.Add( "if_root_statement", new ParsingRule( "if_statement elseif_statement* else_statement?" ) );
				rules.Add( "if_statement", ParsingRule.CreateBlockRule( "IF BRACKET_OPEN boolean_expression BRACKET_CLOSE CURLY_OPEN statement_list? CURLY_CLOSE" ) ); //elseif_statement* else_statement?
				rules.Add( "elseif_statement", ParsingRule.CreateBlockRule( "ELSE IF BRACKET_OPEN boolean_expression BRACKET_CLOSE CURLY_OPEN statement_list? CURLY_CLOSE" ) );
				rules.Add( "else_statement", ParsingRule.CreateBlockRule( "ELSE CURLY_OPEN statement_list? CURLY_CLOSE" ) );
				rules.Add( "return_statement", new ParsingRule( "RETURN expression? END_STATEMENT" ) );
				rules.Add( "expression", ParsingRule.CreateWildCardBlock( "function_call | increment_expression | arithmatic | boolean_expression | decrement_expression | STRING" ) ); //arithmatic 
				rules.Add( "non_boolean_expression", new ParsingRule( "arithmatic | increment_expression | decrement_expression | STRING" ) ); //arithmatic |
				//compound_expression | 
				rules.Add( "compound_expression", new ParsingRule( "BRACKET_OPEN expression BRACKET_CLOSE" ) );

				//rules.Add( "numericexpression", new ParsingRule( "arithmatic" ) );

				//rules.Add( "booleanext", new ParsingRule( "AND boolean_expression | OR boolean_expression" ) );

				rules.Add( "function_call", new ParsingRule( "IDENTIFIER BRACKET_OPEN function_call_args? BRACKET_CLOSE" ) );
				rules.Add( "function_call_args", new ParsingRule( "expression function_call_args_more*" ) );
				rules.Add( "function_call_args_more", new ParsingRule( "COMMA expression" ) );

				rules.Add( "compound_boolean_expression", new ParsingRule( "BRACKET_OPEN boolean_expression BRACKET_CLOSE" ) );
				rules.Add( "boolean_expression", new ParsingRule( "not? boolean_atomic boolean_expression_more*", "bool" ) );// booleanext*
				rules.Add( "boolean_expression_more", new ParsingRule( "boolean_joiners not? boolean_atomic" ) );// booleanext*
				rules.Add( "boolean_atomic", new ParsingRule( "boolean_string_comparison | boolean_float_comparison | compound_boolean_expression | IDENTIFIER.bool | TRUE | FALSE" ) );
				rules.Add( "boolean_joiners", ParsingRule.CreateWildCardBlock( "AND | OR" ) );

				rules.Add( "boolean_string_comparison", new ParsingRule( "string_or_id string_comparison_operators string_or_id" ) );
				rules.Add( "boolean_float_comparison", new ParsingRule( "float_or_id float_comparison_operators float_or_id" ) );

				rules.Add( "float_or_id", new ParsingRule( "FLOAT | function_call.float | IDENTIFIER.float", "float" ) );//float_arithmatic_expression
				rules.Add( "string_or_id", new ParsingRule( "STRING | function_call.string | IDENTIFIER.string", "string" ) );

				rules.Add( "float_comparison_operators", ParsingRule.CreateWildCardBlock( "NOT_EQUAL | EQUALITY | LESS_THAN | GREATER_THAN | GREATER_THAN_OR_EQUAL | LESS_THAN_OR_EQUAL" ) );
				rules.Add( "string_comparison_operators", new ParsingRule( "NOT_EQUAL | EQUALITY" ) );

				rules.Add( "compound_float_arithmatic_expression", new ParsingRule( "BRACKET_OPEN float_arithmatic_expression BRACKET_CLOSE" ) );
				rules.Add( "float_arithmatic_expression", new ParsingRule( "float_arithmatic_atomic float_arithmatic_expression_more*", "float" ) );
				rules.Add( "float_arithmatic_expression_more", new ParsingRule( "float_arithmatic_joiners float_arithmatic_atomic" ) );
				rules.Add( "float_arithmatic_atomic", new ParsingRule( "compound_float_arithmatic_expression | decrement_expression | increment_expression | IDENTIFIER.float | FLOAT" ) );
				rules.Add( "float_arithmatic_joiners", new ParsingRule( "PLUS | MINUS | DIVIDE | TIMES | MOD" ) );

				rules.Add( "increment_expression", new ParsingRule( "IDENTIFIER.float INCREMENT | INCREMENT IDENTIFIER.float" ) );
				rules.Add( "decrement_expression", new ParsingRule( "IDENTIFIER.float DECREMENT | DECREMENT IDENTIFIER.float" ) );

				// Added because tokens cannot be made optional or repeatable.
				rules.Add( "not", new ParsingRule( "NOT", "bool" ) );

				rules.Add( "arithmatic", new ParsingRule( "float_arithmatic_expression" ) );// | integerarithmatic

				foreach ( KeyValuePair<string, ParsingRule> kvp in rules )
				{
					for ( int i = 0; i < kvp.Value.NumberOfOptions; i++ )
					{
						foreach ( ParsingStep step in kvp.Value.GetOptionSteps( i ) )
						{
							if ( !rules.ContainsKey( step.Value ) && !Token.IsTokenValid( step.Value ) ) 
							{
								throw new CraftyException( string.Format( "Step \"{0}\" in rule \"{1}\" is not a valid token or rule name.", step.Value, kvp.Key ) );
							}
						}
					}

					kvp.Value.Name = kvp.Key;
				}

				if ( rules == null || rules.Count == 0 )
				{
					throw new CraftyException( "Error setting up parser." );
				}
			}
			catch ( CraftyException ke )
			{
				//throw ke;
				throw new InternalCraftyException( ke );
			}
		}

		public void Parse ( )
		{
			if ( tokens.Count == 0 )
			{
				return;
			}

			//int result = TraverseTree( 0, rules.First( ).Value );
			IsCompleted = TraverseTree( );
			/*if ( result )
			{

			}
			else { 
				
			}*/
			/*
			if ( result == 0 )
			{
				throw new CraftyException( "Unexpected end of file." );
			}
			else if ( result != tokens.Count )
			{
				throw new CraftyException( string.Format( "Malformed {0} at token number {1} on line {2}.", tokens[result].Name, result, tokens[result].Line ) );
			}*/
			//Console.Write( "(Ended at {0}. Expected {1}.) ", result, tokens.Count );
		}

		public Token GetTokenAt ( int n )
		{
			return tokens[n];
		}

		public TreeBranch ProgramRoot { private set; get; }

		public void ReplaceToken ( Token original, Token replacement )
		{
			if ( tokens.Contains( original ) )
			{
				tokens.Insert( tokens.IndexOf( original ), replacement );
				tokens.Remove( original );
			}
		}

		private bool TraverseTree ( )
		{
			ProgramRoot = new TreeBranch( this, null, rules.First( ).Value, 0, 0, 0, 0 );

			try
			{
				ProgramRoot.Traverse( );
			}
			catch ( Exception exception )
			{
				ProgramRoot = null;
				throw exception;
			}

			if ( ProgramRoot.Success && ProgramRoot.TokenAdvance == tokens.Count )
			{
				try
				{
					ProgramRoot.DoFunctions( );
					ProgramRoot.GenerateSymbolTable( );
					ProgramRoot.CheckBranchTypes( );
					ProgramRoot.Melt( );
					ProgramRoot.FixLevels( );
					ProgramRoot.DoOpCodes( );
					return true;
				}
				catch ( CraftyException exception )
				{
					ProgramRoot = null;
					throw exception;
					//return false;
				}
			}
			else
			{
				if ( ProgramRoot.TokenAdvance == 0 )
				{
					throw new CraftyException( "Unexpected end of file." );
				}
				else if ( ProgramRoot.TokenAdvance != tokens.Count )
				{
					throw new CraftyException( string.Format( "Malformed {0} at token.", tokens[ProgramRoot.TokenAdvance].Name, ProgramRoot.TokenAdvance, tokens[ProgramRoot.TokenAdvance].Line ), tokens[ProgramRoot.TokenAdvance] );
				}
				return false;
			}
		}

		private int TraverseTree ( int startToken, ParsingRule rule, int level )
		{
			throw new NotImplementedException( );
			/*
			level++;
			if ( level > 2000 )
			{
				throw new CraftyException( "Possible stack overflow detected!" );
			}
			int subResult = 0;
			int currentOptionNumber = 0;
			int tokenOffset = 0;
			int repeatstep = 0;
			string currentToken = null;
			//List<ParsingRule> currentOption = null;
			//List<string> stepParts = null;

			// Option loop
			ParsingStep[] optionSteps = null;
			while ( true )
			{
				if ( currentOptionNumber == rule.NumberOfOptions )
				{
					break;
				}

				optionSteps = rule.GetOptionSteps( currentOptionNumber );
				for ( int stepNumber = 0; stepNumber < optionSteps.Length; )
				{
					if ( rules.ContainsKey( optionSteps[stepNumber].Value ) )
					{
						subResult = TraverseTree( startToken + tokenOffset, rules[optionSteps[stepNumber].Value], level );
						// If this statement did not evaluate and is not optional, we go to the next option.
						if ( subResult == 0 && !optionSteps[stepNumber].Optional && !optionSteps[stepNumber].Repeat )
						{
							// next option
							tokenOffset = 0;
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
						if ( startToken + tokenOffset >= tokens.Count )
						{
							tokenOffset = 0;
							goto NextOption;
						}
						else
						{
							currentToken = tokens[startToken + tokenOffset].Name;
						}





						//currentToken == optionSteps[stepNumber].Value
						if ( currentToken == optionSteps[stepNumber].Value )
						{
							//tokenOffset = 0;
							//goto NextOption;
							tokenOffset++;
							stepNumber++;
							repeatstep = 0;
						}
						else
						{
							// next option
							tokenOffset = 0;
							goto NextOption;
						}

					}
				}
				break;
			NextOption:
				currentOptionNumber++;
				repeatstep = 0;
			}

			return tokenOffset;*/
			//return 0;
		}

		public bool IsCompleted { get; private set; }
		//ParserTask Task = ParserTask.PlainParse;
	}


	enum ParserTask { PlainParse, SyntaxTree }


}