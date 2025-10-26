using System.Collections.Generic;

namespace CraftyCode
{
	public class InstructionState
	{
		public Operation CurrentOp = null;
		public Operation NextOp = null;
		public bool ExecuteDebug = false;
		public string DebugString = "";
		
		public CraftyStack VariableStack = null;
		public Stack<Operation> CallStack = null;

		public SymbolTableCollection SymbolTableCollection = null;

		public IList<float> FloatRegisters = null;
		public int InterruptsAllowed = 0;
	}
}
