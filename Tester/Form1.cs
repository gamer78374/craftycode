using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using CraftyCode;

namespace Tester
{
	public partial class Form1 : Form
	{
		public Form1( )
		{
			InitializeComponent( );
		}

		CraftyInstance craftyEngine = new CraftyInstance( );

		private void PopulateSyntaxTree( )
		{
			treeSyntaxTree.Nodes.Clear( );
			if ( craftyEngine != null && craftyEngine.Parser != null )
			{
				treeSyntaxTree.Nodes.Add( TraverseNode( craftyEngine.Parser.ProgramRoot ) );
			}
			UpdateSymbolList( craftyEngine );
		}

		private void UpdateSymbolList( CraftyInstance engine )
		{
			if ( engine.Parser != null )
			{
				UpdateSymbolList( engine.Parser.ProgramRoot );
			}
			else
			{
				UpdateSymbolList( (TreeBranch) null );
			}
		}

		Dictionary<Operation, int> opcodeToListBox = new Dictionary<Operation, int>( );

		private TreeNode TraverseNode( TreeBranch branch )
		{
			TreeNode node = null;
			string tooltip = "";
			if ( branch.IsRuleBranch )
			{
				node = new TreeNode( branch.BranchRule.Name );
				foreach ( TreeBranch t in branch.GetAllBranches( ) )
				{
					node.Nodes.Add( TraverseNode( t ) );
				}
				node.Tag = branch;
			}
			else if ( branch.IsTokenBranch )
			{
				if ( branch.BranchToken.Name == "FLOAT" )
				{
					tooltip = branch.BranchToken.FloatValue.ToString( );
				}
				else if ( branch.BranchToken.Name == "TYPEIDENTIFIER" )
				{
					tooltip = branch.BranchToken.StringValue;
				}
				else if ( branch.BranchToken.Name == "IDENTIFIER" )
				{
					if ( branch.BranchToken.IsSymbol )
					{
						tooltip = "Symbol\r\n";
						if ( branch.BranchToken.IdentifierSymbol == null )
						{
							tooltip += "Name: " + branch.BranchToken.StringValue + " [symbol lost]";
						}
						else
						{
							tooltip += "Name: " + branch.BranchToken.IdentifierSymbol.Name + "\r\nType: ";
							if ( branch.BranchToken.IdentifierSymbol.IsBool )
							{
								tooltip += "bool";
							}
							else if ( branch.BranchToken.IdentifierSymbol.IsNumeric )
							{
								tooltip += "float";
							}
							else if ( branch.BranchToken.IdentifierSymbol.IsString )
							{
								tooltip += "string";
							}
							else if ( branch.BranchToken.IdentifierSymbol.IsVoid )
							{
								tooltip += "void";
							}
							else
							{
								tooltip += "unknown";
							}
						}
					}
				}
				node = new TreeNode( branch.BranchToken.Name );
				node.ToolTipText = tooltip;
				node.Tag = branch;
			}

			return node;
		}

		private void compileButton_Click( object sender, EventArgs e )
		{
			openFileDialog1.FileName = "";
			openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory( );

			string filePath = null;
			if ( openFileDialog1.ShowDialog( this ) == System.Windows.Forms.DialogResult.OK )
			{
				filePath = openFileDialog1.FileName;
			}
			else
			{
				return;
			}

			//string filePath = Path.Combine( Directory.GetCurrentDirectory( ), fileToLoad );
			string input = null;
			string fileContents = "";

			Stopwatch stopwatch = new Stopwatch( );

			if ( !string.IsNullOrWhiteSpace( filePath ) && File.Exists( Path.GetFullPath( filePath ) ) )
			{
				using ( StreamReader re = File.OpenText( Path.GetFullPath( filePath ) ) )
				{
					while ( ( input = re.ReadLine( ) ) != null )
					{
						fileContents += input;
						fileContents += "\r\n";
					}
				}

				try
				{

					craftyEngine.Compile( fileContents, Path.ChangeExtension( filePath, ".ccx" ) );
				}
				catch ( CraftyException ke )
				{
					MessageBox.Show( ke.Message, "Compilation Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				//input = ne
			}
			else
			{
				Console.WriteLine( "Invalid file {0}.", Path.GetFullPath( filePath ) );
			}

			PopulateSyntaxTree( );
			UpdateOpCodeList( );
			UpdateStackList( );
		}

		private void treeSyntaxTree_AfterExpand( object sender, TreeViewEventArgs e )
		{
			TreeNode node = null;
			for ( int i = 0; i < e.Node.Nodes.Count; i++ )
			{
				if ( e.Node.Nodes[i].Nodes.Count > 0 )
				{
					if ( node == null )
					{
						node = e.Node.Nodes[i];
					}
					else
					{
						return;
					}
				}
			}

			if ( node != null )
			{
				node.Expand( );
			}
		}

		DebugCraftyMachine machine = null;

		private void runButton_Click( object sender, EventArgs e )
		{
			if ( craftyEngine.Parser == null )
			{
				MessageBox.Show( "Code has not been compiled.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				return;
			}
			int codesRun = 0; Stopwatch watch;

			if ( machine == null )
			{
				machine = new DebugCraftyMachine( craftyEngine ); // .Parser.ProgramRoot.Operations.ToArray( )
			}

			watch = new Stopwatch( );
			watch.Start( );
			codesRun += machine.Run( );
			watch.Stop( );
			UpdateSymbolList( );
			UpdateOpCodeList( true );
			UpdateStackList( );
			MessageBox.Show( string.Format( "Ran {0} opcodes in {1} seconds. {2} opcodes/ms.", codesRun, watch.Elapsed.TotalSeconds, Math.Round( (double) codesRun / watch.Elapsed.TotalMilliseconds ) ) );
		}

		private void buttonStep_Click( object sender, EventArgs e )
		{
			if ( craftyEngine.Parser == null )
			{
				MessageBox.Show( "Code has not been compiled.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				return;
			}
			if ( machine == null )
			{
				machine = new DebugCraftyMachine( craftyEngine );
			}

			machine.Run( 1 );
			UpdateSymbolList( machine.SymbolList );
			UpdateOpCodeListColour( );
			UpdateStackList( );
		}

		private void splitContainer1_Panel1_Paint( object sender, PaintEventArgs e )
		{

		}

		private void groupBox2_Enter( object sender, EventArgs e )
		{

		}

		private void UpdateCallStackList( )
		{

		}

		private void UpdateStackList( )
		{
			int count = machine == null ? 0 : machine.Stack.Count;
			listViewStack.SuspendLayout( );
			ListViewItem item = null;
			while ( count > listViewStack.Items.Count )
			{
				item = new ListViewItem( );
				item.SubItems.Add( "[bool]" );
				item.SubItems.Add( "[float]" );
				item.SubItems.Add( "[string]" );
				listViewStack.Items.Add( item );
			}

			while ( count < listViewStack.Items.Count )
			{
				listViewStack.Items.RemoveAt( listViewStack.Items.Count - 1 );
			}

			StackValue s = null;

			for ( int i = 0; i < listViewStack.Items.Count; i++ )
			{
				s = machine.Stack[i];
				item = listViewStack.Items[i];
				item.SubItems[0].Text = s.BooleanValue.ToString( );
				item.SubItems[1].Text = s.FloatValue.ToString( );
				item.SubItems[2].Text = s.StringValue;
			}
			listViewStack.ResumeLayout( );
			//listViewStack.AutoResizeColumns( ColumnHeaderAutoResizeStyle.None );
		}

		private void UpdateOpCodeList( )
		{
			UpdateOpCodeList( false );
		}

		/// <summary>
		/// Just updates which item is highlighted. 
		/// </summary>
		private void UpdateOpCodeListColour( )
		{
			listViewOpcodes.SuspendLayout( );
			Operation o = null;
			UpdateOpCodeList( );

			ListViewItem selectedItem = null;
			Operation selectedOperation = null;

			if ( listViewOpcodes.SelectedItems.Count == 1 )
			{
				selectedItem = listViewOpcodes.SelectedItems[0];
				selectedOperation = selectedItem.Tag as Operation;
			}

			for ( int i = 0; i < listViewOpcodes.Items.Count; i++ )
			{
				o = listViewOpcodes.Items[i].Tag as Operation;
				if ( machine != null && machine.IsMidExecution )
				{
					if ( o == machine.CurrentOperation )
					{
						listViewOpcodes.Items[i].ForeColor = Color.Black;
						listViewOpcodes.EnsureVisible( i );
					}
					else if ( o == machine.NextOperation )
					{
						listViewOpcodes.Items[i].ForeColor = Color.Blue;
						listViewOpcodes.EnsureVisible( i );
					}
					else
					{
						listViewOpcodes.Items[i].ForeColor = Color.Gray;
					}
				}
				else
				{
					listViewOpcodes.Items[i].ForeColor = Color.Black;
					listViewOpcodes.Items[i].BackColor = Color.White;
					if ( selectedOperation != null )
					{
						if ( selectedOperation.JumpToOperation != null && selectedOperation.JumpToOperation == o )
						{
							listViewOpcodes.Items[i].BackColor = Color.LightGray;
						}
					}
				}
			}
			listViewOpcodes.ResumeLayout( );
		}

		/// <summary>
		/// Updates the listview of opcodes.
		/// </summary>
		/// <param name="force">Forces an update of the listview.</param>
		private void UpdateOpCodeList( bool force )
		{
			int count = 0;
			IList<Operation> operations = null;
			try
			{
				count = machine != null ? machine.Operations.Count : craftyEngine.Parser.ProgramRoot.Operations.Count;

				operations = machine == null ? craftyEngine.Parser.ProgramRoot.Operations : machine.Operations;
			}
			catch ( NullReferenceException exception )
			{
				Debug.Print( "NullReferenceException while updating opcode list: " + exception.Message );
				operations = new List<Operation>( );
			}

			ListViewItem item = null;

			if ( count != listViewOpcodes.Items.Count || force )
			{
				listViewOpcodes.SuspendLayout( );
				while ( listViewOpcodes.Items.Count > count )
				{
					listViewOpcodes.Items.RemoveAt( listViewOpcodes.Items.Count - 1 );
				}
				while ( listViewOpcodes.Items.Count < count )
				{
					item = new ListViewItem( );
					item.SubItems.Add( "[#]" );
					item.SubItems.Add( "[code]" );
					item.SubItems.Add( "[info]" );
					listViewOpcodes.Items.Add( item );
				}
				string moreinfo = null;
				Operation o = null;

				for ( int i = 0; i < operations.Count; i++ )
				{
					o = operations[i];
					moreinfo = "";
					//item = new ListViewItem( o.Code.ToString( ) );
					if ( o.Code == OpCode.Store || o.Code == OpCode.PushSymbol )
					{
						if ( o.Symbol == null )
						{
							moreinfo = "[lost symbol]";
							moreinfo = o.SymbolName;
						}
						else
						{
							moreinfo = o.Symbol.Name + " (" + o.Symbol.Type + ")";
						}
					}
					else if ( o.Code == OpCode.PushBoolean )
					{
						moreinfo = o.BoolValue.ToString( );
					}
					else if ( o.Code == OpCode.PushFloat )
					{
						moreinfo = o.FloatValue.ToString( );
					}
					else if ( o.StringValue != null && ( o.Code == OpCode.PushString || o.Code == OpCode.Dummy || o.Code == OpCode.NoOperation ) )
					{
						moreinfo = o.StringValue;
					}
					else if ( o.Code == OpCode.Jump || o.Code == OpCode.JumpIfFalse || o.Code == OpCode.JumpIfTrue )
					{
						if ( o.JumpToOperation == null )
						{
							moreinfo = "(Unknown Instruction)";
						}
						else
						{
							moreinfo = o.JumpToOperation.Number.ToString( );
						}
					}
					else if ( o.Code == OpCode.InitSymbol )
					{
						moreinfo = o.StringValue;
					}

					listViewOpcodes.Items[i].SubItems[0].Text = o.Number.ToString( );
					listViewOpcodes.Items[i].SubItems[1].Text = o.Code.ToString( );
					listViewOpcodes.Items[i].SubItems[2].Text = moreinfo;
					listViewOpcodes.Items[i].Tag = o;

					//listBoxOpcodes.Items.Add( o.Number.ToString( ).PadRight( 6 ) + o.Code.ToString( ).PadRight( 16 ) + moreinfo );

					//opcodeToListBox[o] = listBoxOpcodes.Items.Count - 1;

					//item.SubItems.Add( o.Number.ToString( ) );
					//item.SubItems.Add( o.Code.ToString() );
					//item.SubItems.Add( moreinfo );
					//listViewOpcodes.Items.Add( item );
				}
				listViewOpcodes.ResumeLayout( );
				listViewOpcodes.AutoResizeColumns( ColumnHeaderAutoResizeStyle.ColumnContent );
				UpdateOpCodeListColour( );
			}
		}

		private void UpdateSymbolList( )
		{
			if ( treeSyntaxTree.SelectedNode == null )
			{
				UpdateSymbolList( craftyEngine.Parser.ProgramRoot );
			}
			else
			{
				UpdateSymbolList( ( treeSyntaxTree.SelectedNode.Tag as TreeBranch ) ?? craftyEngine.Parser.ProgramRoot );
			}
		}

		private void UpdateSymbolList( TreeBranch branch )
		{
			List<Symbol> symbols = new List<Symbol>( );

			if ( branch != null )
			{
				foreach ( Symbol s in branch.GetSymbols( ) )
				{
					symbols.Add( s );
				}
				//throw new NullReferenceException( );
			}

			UpdateSymbolList( symbols, branch );
		}

		private void UpdateSymbolList( IList<Symbol> listOfSymbols )
		{
			UpdateSymbolList( listOfSymbols, null );
		}

		private void UpdateSymbolList( IList<Symbol> listOfSymbols, TreeBranch branch )
		{
			listViewSymbols.SuspendLayout( );
			ListViewItem item = null;

			while ( listViewSymbols.Items.Count > listOfSymbols.Count )
			{
				listViewSymbols.Items.RemoveAt( listViewSymbols.Items.Count - 1 );
			}

			while ( listViewSymbols.Items.Count < listOfSymbols.Count )
			{
				item = new ListViewItem( "[name]" );
				item.SubItems.Add( "[type]" );
				item.SubItems.Add( "[value]" );
				listViewSymbols.Items.Add( item );
			}

			Symbol symbol = null;

			for ( int i = 0; i < listOfSymbols.Count; i++ )
			{
				symbol = listOfSymbols[i];
				item = listViewSymbols.Items[i];
				item.Text = symbol.Name;
				item.SubItems[0].Text = symbol.Name;
				item.SubItems[1].Text = symbol.Type;

				if ( symbol.IsBool )
				{
					item.SubItems[2].Text = symbol.BooleanValue.ToString( );
				}
				else if ( symbol.IsNumeric )
				{
					item.SubItems[2].Text = symbol.FloatValue.ToString( );
				}
				else if ( symbol.IsString )
				{
					item.SubItems[2].Text = symbol.StringValue;
				}
				else if ( symbol.IsFunction )
				{
					item.SubItems[2].Text = symbol.FunctionReturnType;
				}
				else
				{
					item.SubItems[2].Text = "[unknown]";
				}

				item.ForeColor = Color.Black;

				if ( branch != null )
				{
					item.ForeColor = branch.IsLocalSymbol( symbol ) ? Color.Black : Color.Gray;
				}
				else
				{
					if ( machine != null && machine.IsMidExecution )
					{
						item.ForeColor = symbol.IsActive ? Color.Black : Color.Gray;
					}
				}
			}

			listViewSymbols.ResumeLayout( );
		}

		private void treeSyntaxTree_AfterSelect( object sender, TreeViewEventArgs e )
		{
			UpdateSymbolList( ( e.Node.Tag as TreeBranch ) );
		}

		private void listViewOpcodes_SelectedIndexChanged( object sender, EventArgs e )
		{
			UpdateOpCodeListColour( );
		}

		private void treeSyntaxTree_NodeMouseDoubleClick( object sender, TreeNodeMouseClickEventArgs e )
		{
			if ( e.Button == System.Windows.Forms.MouseButtons.Right )
			{
				if ( !e.Node.IsExpanded )
				{
					e.Node.ExpandAll( );
				}
				else
				{
					e.Node.Collapse( false );
				}
			}
		}

		private void treeSyntaxTree_NodeMouseClick( object sender, TreeNodeMouseClickEventArgs e )
		{

		}

		private void buttonLoad_Click( object sender, EventArgs e )
		{
			if ( openFileDialog1.ShowDialog( this ) == System.Windows.Forms.DialogResult.OK )
			{

			}
		}
	}
}