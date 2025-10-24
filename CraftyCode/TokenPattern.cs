using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CraftyCode
{
	public class TokenPattern
	{
		public string RegexPattern { get; set; }
		public string TokenText { get; set; }

		public TokenPattern() { }
		
		public TokenPattern ( string token ) : this( "", token ) { }
		
		public TokenPattern ( string regex, string text ) 
		{
			RegexPattern = regex;
			TokenText = text;
		}
	}
}
