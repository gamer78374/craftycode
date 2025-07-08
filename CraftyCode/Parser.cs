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

		public Parser ( Token[] input, IList<string> validTokens )
		{
			IsCompleted = false;
			tokens = new List<Token>( );
			tokens.AddRange( input );
			SetupRules( validTokens );
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

		private void SetupRules ( IList<string> validTokens )
		{
			try
			{
				rules = new Dictionary<string, ParsingRule>( );
				rules.Add( "program", new ParsingRule( "statementlist" ) );
				rules.Add( "statementlist", new ParsingRule( "statement+" ) );
				rules.Add( "statement", new ParsingRule( "ENDSTATEMENT | solidblock | ifrootstatement | triggerstatement | foreachstatement | forstatement | whilestatement | functiondeclare | assignstatement ENDSTATEMENT | variable_declaration_statement ENDSTATEMENT | expression ENDSTATEMENT" ) );

				rules.Add( "variable_declaration_statement", new ParsingRule( "TYPEIDENTIFIER IDENTIFIER assign?" ) );
				rules.Add( "assign", new ParsingRule( "assignoperators expression" ) );

				rules.Add( "functiondeclare", new ParsingRule( "TYPEIDENTIFIER IDENTIFIER OPENBRACKET function_argument_declaration? CLOSEBRACKET CURLYOPEN functionbody? CURLYCLOSE" ) );
				rules.Add( "functionbody", new ParsingRule( "returnstatement+ | statement+" ) );
				rules.Add( "function_argument_declaration", new ParsingRule( "TYPEIDENTIFIER IDENTIFIER function_argument_declaration_more*" ) );
				rules.Add( "function_argument_declaration_more", new ParsingRule( "COMMA TYPEIDENTIFIER IDENTIFIER" ) );

				rules.Add( "assignstatement", new ParsingRule( "IDENTIFIER assign" ) );

				rules.Add( "assignoperators", new ParsingRule( "ASSIGN | PLUSEQUAL | MINUSEQUAL | TIMESEQUAL | DIVIDEEQUAL | MODEQUAL" ) );

				rules.Add( "triggerstatement", new ParsingRule( "WHEN OPENBRACKET IDENTIFIER POINTRIGHT IDENTIFIER.event CLOSEBRACKET conditionstatement? CURLYOPEN statementlist? CURLYCLOSE" ) );
				rules.Add( "conditionstatement", new ParsingRule( "AND OPENBRACKET booleanexpression CLOSEBRACKET" ) );
				rules.Add( "forstatement_third", new ParsingRule( "incrementexpression | decrementexpression | assignstatement" ) );
				rules.Add( "forstatement", ParsingRule.CreateBlockRule( "FOR OPENBRACKET variable_declaration_statement? ENDSTATEMENT booleanexpression? ENDSTATEMENT forstatement_third? CLOSEBRACKET CURLYOPEN statementlist? CURLYCLOSE" ) );
				rules.Add( "foreachstatement", ParsingRule.CreateBlockRule( "FOREACH OPENBRACKET TYPEIDENTIFIER IDENTIFIER IN IDENTIFIER CLOSEBRACKET CURLYOPEN statementlist? CURLYCLOSE" ) );
				rules.Add( "whilestatement", ParsingRule.CreateBlockRule( "WHILE OPENBRACKET booleanexpression CLOSEBRACKET CURLYOPEN statementlist? CURLYCLOSE" ) );
				rules.Add( "solidblock", ParsingRule.CreateBlockRule( "SOLID CURLYOPEN statementlist? CURLYCLOSE" ) );
				rules.Add( "ifrootstatement", new ParsingRule( "ifstatement elseifstatement* elsestatement?" ) );
				rules.Add( "ifstatement", ParsingRule.CreateBlockRule( "IF OPENBRACKET booleanexpression CLOSEBRACKET CURLYOPEN statementlist? CURLYCLOSE" ) ); //elseifstatement* elsestatement?
				rules.Add( "elseifstatement", ParsingRule.CreateBlockRule( "ELSE IF OPENBRACKET booleanexpression CLOSEBRACKET CURLYOPEN statementlist? CURLYCLOSE" ) );
				rules.Add( "elsestatement", ParsingRule.CreateBlockRule( "ELSE CURLYOPEN statementlist? CURLYCLOSE" ) );
				rules.Add( "returnstatement", new ParsingRule( "RETURN expression? ENDSTATEMENT" ) );
				rules.Add( "expression", ParsingRule.CreateWildCardBlock( "functioncall | incrementexpression | arithmatic | booleanexpression | decrementexpression | STRING" ) ); //arithmatic 
				rules.Add( "non_boolean_expression", new ParsingRule( "arithmatic | incrementexpression | decrementexpression | STRING" ) ); //arithmatic |
				//compoundexpression | 
				rules.Add( "compoundexpression", new ParsingRule( "OPENBRACKET expression CLOSEBRACKET" ) );

				//rules.Add( "numericexpression", new ParsingRule( "arithmatic" ) );

				//rules.Add( "booleanext", new ParsingRule( "AND booleanexpression | OR booleanexpression" ) );

				rules.Add( "functioncall", new ParsingRule( "IDENTIFIER OPENBRACKET function_call_args? CLOSEBRACKET" ) );
				rules.Add( "function_call_args", new ParsingRule( "expression function_call_args_more*" ) );
				rules.Add( "function_call_args_more", new ParsingRule( "COMMA expression" ) );

				rules.Add( "compoundbooleanexpression", new ParsingRule( "OPENBRACKET booleanexpression CLOSEBRACKET" ) );
				rules.Add( "booleanexpression", new ParsingRule( "not? boolean_atomic booleanexpression_more*", "bool" ) );// booleanext*
				rules.Add( "booleanexpression_more", new ParsingRule( "boolean_joiners not? boolean_atomic" ) );// booleanext*
				rules.Add( "boolean_atomic", new ParsingRule( "boolean_string_comparison | boolean_float_comparison | compoundbooleanexpression | IDENTIFIER.bool | TRUE | FALSE" ) );
				rules.Add( "boolean_joiners", ParsingRule.CreateWildCardBlock( "AND | OR" ) );

				rules.Add( "boolean_string_comparison", new ParsingRule( "string_or_id string_comparison_operators string_or_id" ) );
				rules.Add( "boolean_float_comparison", new ParsingRule( "float_or_id float_comparison_operators float_or_id" ) );

				rules.Add( "float_or_id", new ParsingRule( "FLOAT | functioncall.float | IDENTIFIER.float", "float" ) );//float_arithmatic_expression
				rules.Add( "string_or_id", new ParsingRule( "STRING | functioncall.string | IDENTIFIER.string", "string" ) );

				rules.Add( "float_comparison_operators", ParsingRule.CreateWildCardBlock( "NOTEQUAL | EQUITY | LESSTHAN | GREATERTHAN | GREATERTHANOREQUAL | LESSTHANOREQUAL" ) );
				rules.Add( "string_comparison_operators", new ParsingRule( "NOTEQUAL | EQUITY" ) );

				rules.Add( "compound_float_arithmatic_expression", new ParsingRule( "OPENBRACKET float_arithmatic_expression CLOSEBRACKET" ) );
				rules.Add( "float_arithmatic_expression", new ParsingRule( "float_arithmatic_atomic float_arithmatic_expression_more*", "float" ) );
				rules.Add( "float_arithmatic_expression_more", new ParsingRule( "float_arithmatic_joiners float_arithmatic_atomic" ) );
				rules.Add( "float_arithmatic_atomic", new ParsingRule( "compound_float_arithmatic_expression | decrementexpression | incrementexpression | IDENTIFIER.float | FLOAT" ) );
				rules.Add( "float_arithmatic_joiners", new ParsingRule( "PLUS | MINUS | DIVIDE | TIMES | MOD" ) );

				rules.Add( "incrementexpression", new ParsingRule( "IDENTIFIER.float INCREMENT | INCREMENT IDENTIFIER.float" ) );
				rules.Add( "decrementexpression", new ParsingRule( "IDENTIFIER.float DECREMENT | DECREMENT IDENTIFIER.float" ) );

				// Added because tokens cannot be made optional or repeatable.
				rules.Add( "not", new ParsingRule( "NOT", "bool" ) );

				rules.Add( "arithmatic", new ParsingRule( "float_arithmatic_expression" ) );// | integerarithmatic

				foreach ( KeyValuePair<string, ParsingRule> kvp in rules )
				{
					for ( int i = 0; i < kvp.Value.NumberOfOptions; i++ )
					{
						foreach ( ParsingStep step in kvp.Value.GetOptionSteps( i ) )
						{
							if ( !rules.ContainsKey( step.Value ) && !validTokens.Contains( step.Value ) )
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