namespace Tester
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose ( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose( );
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ( )
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.treeSyntaxTree = new System.Windows.Forms.TreeView();
			this.button1 = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.listViewOpcodes = new System.Windows.Forms.ListView();
			this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.listViewStack = new System.Windows.Forms.ListView();
			this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonStep = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.listViewSymbols = new System.Windows.Forms.ListView();
			this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeaderType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeaderValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.listViewCallStack = new System.Windows.Forms.ListView();
			this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.AutoSize = true;
			this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupBox1.Controls.Add(this.treeSyntaxTree);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(4, 3);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.groupBox1.Size = new System.Drawing.Size(364, 1041);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Syntax Tree";
			// 
			// treeSyntaxTree
			// 
			this.treeSyntaxTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeSyntaxTree.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.treeSyntaxTree.Location = new System.Drawing.Point(4, 27);
			this.treeSyntaxTree.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.treeSyntaxTree.Name = "treeSyntaxTree";
			this.treeSyntaxTree.ShowNodeToolTips = true;
			this.treeSyntaxTree.Size = new System.Drawing.Size(356, 1011);
			this.treeSyntaxTree.TabIndex = 0;
			this.treeSyntaxTree.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeSyntaxTree_AfterExpand);
			this.treeSyntaxTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeSyntaxTree_AfterSelect);
			this.treeSyntaxTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeSyntaxTree_NodeMouseClick);
			this.treeSyntaxTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeSyntaxTree_NodeMouseDoubleClick);
			// 
			// button1
			// 
			this.button1.AutoSize = true;
			this.button1.Location = new System.Drawing.Point(76, 3);
			this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(294, 55);
			this.button1.TabIndex = 1;
			this.button1.Text = "Load and Compile";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.compileButton_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.listViewOpcodes);
			this.groupBox2.Location = new System.Drawing.Point(748, 3);
			this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.groupBox2.Size = new System.Drawing.Size(364, 1041);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "OpCodes";
			this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
			// 
			// listViewOpcodes
			// 
			this.listViewOpcodes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
			this.listViewOpcodes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewOpcodes.FullRowSelect = true;
			this.listViewOpcodes.HideSelection = false;
			this.listViewOpcodes.Location = new System.Drawing.Point(4, 27);
			this.listViewOpcodes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.listViewOpcodes.MultiSelect = false;
			this.listViewOpcodes.Name = "listViewOpcodes";
			this.listViewOpcodes.ShowItemToolTips = true;
			this.listViewOpcodes.Size = new System.Drawing.Size(356, 1011);
			this.listViewOpcodes.TabIndex = 1;
			this.listViewOpcodes.UseCompatibleStateImageBehavior = false;
			this.listViewOpcodes.View = System.Windows.Forms.View.Details;
			this.listViewOpcodes.SelectedIndexChanged += new System.EventHandler(this.listViewOpcodes_SelectedIndexChanged);
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "#";
			this.columnHeader4.Width = 48;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Code";
			this.columnHeader5.Width = 106;
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "Info";
			this.columnHeader6.Width = 124;
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.listViewStack);
			this.groupBox3.Location = new System.Drawing.Point(1120, 3);
			this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.groupBox3.Size = new System.Drawing.Size(364, 1041);
			this.groupBox3.TabIndex = 4;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Stack";
			// 
			// listViewStack
			// 
			this.listViewStack.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
			this.listViewStack.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewStack.FullRowSelect = true;
			this.listViewStack.HideSelection = false;
			this.listViewStack.Location = new System.Drawing.Point(4, 27);
			this.listViewStack.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.listViewStack.Name = "listViewStack";
			this.listViewStack.Size = new System.Drawing.Size(356, 1011);
			this.listViewStack.TabIndex = 0;
			this.listViewStack.UseCompatibleStateImageBehavior = false;
			this.listViewStack.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader7
			// 
			this.columnHeader7.Text = "Bool";
			this.columnHeader7.Width = 78;
			// 
			// columnHeader8
			// 
			this.columnHeader8.Text = "Numeric";
			this.columnHeader8.Width = 204;
			// 
			// columnHeader9
			// 
			this.columnHeader9.Text = "String";
			// 
			// buttonStep
			// 
			this.buttonStep.AutoSize = true;
			this.buttonStep.Location = new System.Drawing.Point(465, 3);
			this.buttonStep.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.buttonStep.Name = "buttonStep";
			this.buttonStep.Size = new System.Drawing.Size(112, 55);
			this.buttonStep.TabIndex = 5;
			this.buttonStep.Text = "Step";
			this.buttonStep.UseVisualStyleBackColor = true;
			this.buttonStep.Click += new System.EventHandler(this.buttonStep_Click);
			// 
			// button2
			// 
			this.button2.AutoSize = true;
			this.button2.Location = new System.Drawing.Point(585, 3);
			this.button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(112, 55);
			this.button2.TabIndex = 6;
			this.button2.Text = "Run";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.runButton_Click);
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel1.Controls.Add(this.label1);
			this.flowLayoutPanel1.Controls.Add(this.button1);
			this.flowLayoutPanel1.Controls.Add(this.label2);
			this.flowLayoutPanel1.Controls.Add(this.buttonStep);
			this.flowLayoutPanel1.Controls.Add(this.button2);
			this.flowLayoutPanel1.Location = new System.Drawing.Point(24, 23);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(6);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(1860, 102);
			this.flowLayoutPanel1.TabIndex = 8;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 0);
			this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 25);
			this.label1.TabIndex = 7;
			this.label1.Text = "Build";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(380, 0);
			this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(75, 25);
			this.label2.TabIndex = 8;
			this.label2.Text = "Debug";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 5;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.groupBox3, 3, 0);
			this.tableLayoutPanel1.Controls.Add(this.groupBox2, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.groupBox4, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.groupBox5, 4, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(24, 134);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1864, 1047);
			this.tableLayoutPanel1.TabIndex = 9;
			// 
			// groupBox4
			// 
			this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox4.Controls.Add(this.listViewSymbols);
			this.groupBox4.Location = new System.Drawing.Point(376, 3);
			this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.groupBox4.Size = new System.Drawing.Size(364, 1041);
			this.groupBox4.TabIndex = 5;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Symbols";
			// 
			// listViewSymbols
			// 
			this.listViewSymbols.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderType,
            this.columnHeaderValue});
			this.listViewSymbols.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewSymbols.FullRowSelect = true;
			this.listViewSymbols.HideSelection = false;
			this.listViewSymbols.Location = new System.Drawing.Point(4, 27);
			this.listViewSymbols.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.listViewSymbols.Name = "listViewSymbols";
			this.listViewSymbols.Size = new System.Drawing.Size(356, 1011);
			this.listViewSymbols.TabIndex = 0;
			this.listViewSymbols.UseCompatibleStateImageBehavior = false;
			this.listViewSymbols.View = System.Windows.Forms.View.Details;
			// 
			// columnHeaderName
			// 
			this.columnHeaderName.Text = "Name";
			this.columnHeaderName.Width = 130;
			// 
			// columnHeaderType
			// 
			this.columnHeaderType.Text = "Type";
			// 
			// columnHeaderValue
			// 
			this.columnHeaderValue.Text = "Value";
			this.columnHeaderValue.Width = 90;
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.listViewCallStack);
			this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox5.Location = new System.Drawing.Point(1494, 6);
			this.groupBox5.Margin = new System.Windows.Forms.Padding(6);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Padding = new System.Windows.Forms.Padding(6);
			this.groupBox5.Size = new System.Drawing.Size(364, 1035);
			this.groupBox5.TabIndex = 6;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Call Stack";
			// 
			// listViewCallStack
			// 
			this.listViewCallStack.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10});
			this.listViewCallStack.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewCallStack.FullRowSelect = true;
			this.listViewCallStack.HideSelection = false;
			this.listViewCallStack.Location = new System.Drawing.Point(6, 30);
			this.listViewCallStack.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.listViewCallStack.Name = "listViewCallStack";
			this.listViewCallStack.Size = new System.Drawing.Size(352, 999);
			this.listViewCallStack.TabIndex = 1;
			this.listViewCallStack.UseCompatibleStateImageBehavior = false;
			this.listViewCallStack.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader10
			// 
			this.columnHeader10.Text = "Operation";
			this.columnHeader10.Width = 160;
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1908, 1200);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.flowLayoutPanel1);
			this.Margin = new System.Windows.Forms.Padding(6);
			this.MinimumSize = new System.Drawing.Size(1258, 866);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Crafty Compiler";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TreeView treeSyntaxTree;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button buttonStep;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.ListView listViewSymbols;
		private System.Windows.Forms.ColumnHeader columnHeaderName;
		private System.Windows.Forms.ColumnHeader columnHeaderType;
		private System.Windows.Forms.ColumnHeader columnHeaderValue;
		private System.Windows.Forms.ListView listViewOpcodes;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.ListView listViewStack;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.ColumnHeader columnHeader8;
		private System.Windows.Forms.ColumnHeader columnHeader9;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.ListView listViewCallStack;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
	}
}

