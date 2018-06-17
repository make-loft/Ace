using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Ace.Serialization;
using Ace.Serialization.Escapers;
using Ace.Serialization.Serializators;

namespace Ace.Replication.Models
{
	public class Simplex : List<string>
	{
		public override string ToString() => this.Aggregate("", (a, b) => a + b);
	}
}