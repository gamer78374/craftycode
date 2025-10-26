using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CraftyCode
{
	/// <summary>
	/// Indexer of functions used executing byte code. 
	/// </summary>
	/// <typeparam name="enum">Enum of instruction names. 0 is NoOp.</typeparam>
	public abstract class InstructionSet
	{
		public List<Func<InstructionState, bool>> Operations = new List<Func<InstructionState, bool>>( );

		public InstructionSet ( )
		{
			for ( int i = 0; i < 256; i++ )
			{
				Operations.Add( null );
			}
		}
	}
}
