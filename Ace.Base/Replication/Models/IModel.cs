using System.Collections;
using System.Collections.Generic;
using Ace.Serialization;

namespace Ace.Replication.Models
{
	public interface IModel : ICollection
	{
	}

	public interface IModel<T> : ICollection<T>, IModel
	{
	}
}