using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CraftyCode
{
	public class CraftyContext
	{
		public CraftyContext ( )
		{

		}

		public Operation currentOp = null;
		public Operation nextOp = null;
	}

	public class ReadonlyCraftyContext
	{
		public ReadonlyCraftyContext ( CraftyContext context )
		{
			parent = context;
		}

		CraftyContext parent = null;
		public Operation CurrentOperation
		{
			get { return parent.currentOp; }
		}

		public Operation NextOperation
		{
			get { return parent.nextOp; }
		}
	}
}
