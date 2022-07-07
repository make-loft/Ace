using System.Collections.Generic;
using System.Linq;

namespace Ace.Replication.Models
{
	public class Simplex : List<string>
	{
		public override string ToString() => this.Aggregate("", (a, b) => a + b);
	}
}