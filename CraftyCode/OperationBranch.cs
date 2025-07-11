using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CraftyCode
{
	class StatementListBranch : TreeBranch
	{
		public StatementListBranch ( Parser p, TreeBranch t, Token k, int optionNum, int stepNum, int level )
			: base( p, t, k, optionNum, stepNum, level )
		{}

		public StatementListBranch ( Parser p, TreeBranch t, ParsingRule rule, int optionNum, int stepnum, int tokenStartAt, int level )
			: base( p, t, rule, optionNum, stepnum, tokenStartAt, level )
		{ }

		public void Do ( )
		{
			foreach ( TreeBranch t in GetBranches( false ) )
			{
				if ( t.IsTokenBranch )
				{
					continue;
				}
				else if ( t.IsRuleBranch && ( t as StatementBranch ) != null )
				{
					( (StatementBranch) t ).Do( );
				}
			}
		}
	}

	abstract class StatementBranch : TreeBranch
	{
		public StatementBranch ( Parser p, TreeBranch t, Token k, int optionNum, int stepNum, int level )
			: base( p, t, k, optionNum, stepNum, level )
		{ }
		public StatementBranch ( Parser p, TreeBranch t, ParsingRule rule, int optionNum, int stepnum, int tokenStartAt, int level )
			: base( p, t, rule, optionNum, stepnum, tokenStartAt, level )
		{ }
		public abstract void Do ( );
	}

	class IfRootBranch : StatementBranch
	{
		public IfRootBranch ( Parser p, TreeBranch t, Token k, int optionNum, int stepNum, int level )
			: base( p, t, k, optionNum, stepNum, level )
		{ }

		public IfRootBranch ( Parser p, TreeBranch t, ParsingRule rule, int optionNum, int stepnum, int tokenStartAt, int level )
			: base( p, t, rule, optionNum, stepnum, tokenStartAt, level )
		{ }

		public override void Do ( )
		{
			for ( int i = 0; i < Branches.Count; i++ )
			{
				( (IfBranch) Branches[i] ).Do( );
				if ( ( (IfBranch) Branches[i] ).Result )
				{
					break;
				}
			}
		}
	}

	class IfBranch : StatementBranch
	{
		public IfBranch ( Parser p, TreeBranch t, Token k, int optionNum, int stepNum, int level )
			: base( p, t, k, optionNum, stepNum, level )
		{ }

		public IfBranch ( Parser p, TreeBranch t, ParsingRule rule, int optionNum, int stepnum, int tokenStartAt, int level )
			: base( p, t, rule, optionNum, stepnum, tokenStartAt, level )
		{ }

		public bool Result { get; private set; }
		public bool IsElse { get; set; }

		public override void Do ( )
		{
			int startAt = 0;
			if ( Branches[0].IsTokenBranch )
			{
				startAt++;
			}
			Result = ( (BooleanExpressionBranch) Branches[startAt + 2] ).GetResult( ) || IsElse;
			if ( Result )
			{
				( (StatementListBranch) Branches[startAt + 5] ).Do( );
			}
		}
	}

	class WhileBranch : StatementBranch
	{
		public WhileBranch ( Parser p, TreeBranch t, Token k, int optionNum, int stepNum, int level )
			: base( p, t, k, optionNum, stepNum, level )
		{ }

		public WhileBranch ( Parser p, TreeBranch t, ParsingRule rule, int optionNum, int stepnum, int tokenStartAt, int level )
			: base( p, t, rule, optionNum, stepnum, tokenStartAt, level )
		{ }

		public override void Do ( )
		{
			while ( ( (BooleanExpressionBranch) Branches[2] ).GetResult( ) )
			{
				( (StatementListBranch) Branches[5] ).Do( );
			}
		}
	}

	class ForBranch : StatementBranch
	{
		public ForBranch ( Parser p, TreeBranch t, Token k, int optionNum, int stepNum, int level )
			: base( p, t, k, optionNum, stepNum, level )
		{ }

		public ForBranch ( Parser p, TreeBranch t, ParsingRule rule, int optionNum, int stepnum, int tokenStartAt, int level )
			: base( p, t, rule, optionNum, stepnum, tokenStartAt, level )
		{ }

		public override void Do ( )
		{
			int header = 0;
			BooleanExpressionBranch booleanRule = null;
			ExpressionBranch expressionBranch = null;
			int startAt = 0;

			for ( int i = 2; i < Branches.Count; i++ )
			{
				if ( header == 0 && Branches[i].IsRuleBranch && Branches[i].BranchRule.Name == "assign_statement" )
				{
					( (AssignStatementBranch) Branches[i] ).Do( );
				}
				else if ( header == 1 && Branches[i].IsRuleBranch && Branches[i].BranchRule.Name == "boolean_expression" )
				{
					booleanRule = Branches[i] as BooleanExpressionBranch;
				}
				else if ( header == 2 && Branches[i].IsRuleBranch && Branches[i].BranchRule.Name == "expression" )
				{
					expressionBranch = Branches[i] as ExpressionBranch;
				}
				if ( header <= 2 && Branches[i].IsTokenBranch && Branches[i].BranchToken.Name == "END_STATEMENT" )
				{
					header++;
				}
				if ( Branches[i].IsTokenBranch && Branches[i].BranchToken.Name == "CURLY_OPEN" )
				{
					startAt = i;
					break;
				}
			}
			while ( booleanRule.GetResult( ) )
			{
				( (StatementListBranch) Branches[startAt + 1] ).Do( );
				expressionBranch.Do( );
			}
		}
	}

	class AssignStatementBranch : TreeBranch { 
		public AssignStatementBranch ( Parser p, TreeBranch t, Token k, int optionNum, int stepNum, int level )
			: base( p, t, k, optionNum, stepNum, level )
		{ }

		public AssignStatementBranch ( Parser p, TreeBranch t, ParsingRule rule, int optionNum, int stepnum, int tokenStartAt, int level )
			: base( p, t, rule, optionNum, stepnum, tokenStartAt, level )
		{ }

		public void Do ( )
		{
			
		}
	}

	class ExpressionBranch : TreeBranch { 
		public ExpressionBranch ( Parser p, TreeBranch t, Token k, int optionNum, int stepNum, int level )
			: base( p, t, k, optionNum, stepNum, level )
		{}

		public ExpressionBranch ( Parser p, TreeBranch t, ParsingRule rule, int optionNum, int stepnum, int tokenStartAt, int level )
			: base( p, t, rule, optionNum, stepnum, tokenStartAt, level )
		{}

		public virtual void Do ( )
		{
		}
	}

	class BooleanExpressionBranch : ExpressionBranch
	{
		public BooleanExpressionBranch ( Parser p, TreeBranch t, Token k, int optionNum, int stepNum, int level )
			: base( p, t, k, optionNum, stepNum, level )
		{ }

		public BooleanExpressionBranch ( Parser p, TreeBranch t, ParsingRule rule, int optionNum, int stepnum, int tokenStartAt, int level )
			: base( p, t, rule, optionNum, stepnum, tokenStartAt, level )
		{ }

		public bool Result { get; private set; }

		public bool GetResult ( )
		{
			Do( );
			return Result;
		}

		public override void Do ( )
		{
			bool lastResult = false;
			foreach ( TreeBranch t in GetBranches( false ) )
			{
				if ( t.IsRuleBranch )
				{
					( (BooleanExpressionBranch) t ).Do( );
				}
				if ( t.IsTokenBranch )
				{
					if ( t.BranchToken.IsIdentifier && GetSymbolFromID( t.BranchToken ).BooleanValue )
					{
						lastResult = true;
					}
					else if ( t.BranchToken.Name == "TRUE" )
					{
						lastResult = true;
					}

				}
				else if ( t.IsRuleBranch && ( (BooleanExpressionBranch) t ).Result )
				{
					lastResult = true;
				}
				else if ( !lastResult && t.IsTokenBranch && t.BranchToken.Name == "AND" )
				{
					break;
				}
				else if ( lastResult && t.IsTokenBranch && t.BranchToken.Name == "OR" )
				{
					break;
				}
			}
			Result = lastResult;
			return;
		}
	}

	class AssignmentBranch : TreeBranch
	{
		public AssignmentBranch ( Parser p, TreeBranch t, Token k, int optionNum, int stepNum, int level )
			: base( p, t, k, optionNum, stepNum, level )
		{}

		public AssignmentBranch ( Parser p, TreeBranch t, ParsingRule rule, int optionNum, int stepnum, int tokenStartAt, int level )
			: base( p, t, rule, optionNum, stepnum, tokenStartAt, level )
		{}
		
		public void GetResult ( )
		{

		}
	}
}