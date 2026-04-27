using System;
using System.Collections.Generic;
using System.Reflection;
using Ace.Replication.Models;
using Ace.Replication.Replicators;

namespace Ace.Replication;

public class Replicator
{
	public virtual bool CanTranslate(object value, TranslationArgs args, Type baseType) => true;

	public virtual bool CanReplicate(object value, ReplicationArgs args, Type baseType) => true;

	public virtual object Translate(object value, TranslationArgs args, Type baseType) => value?.ToString();

	public virtual object Replicate(object value, ReplicationArgs args, Type baseType) => value?.ToString();

	public virtual List<MemberInfo> GetDataMembers(Type type, Func<MemberInfo, bool> filter)
		=> throw new NotSupportedException();

	public virtual string GetDataKey(MemberInfo member)
		=> throw new NotSupportedException();
}

public class Replicator<TValue> : Replicator
{
	public readonly Type ActiveType = TypeOf<TValue>.Raw;

	public override bool CanTranslate(object value, TranslationArgs args, Type baseType = null)
		=> value is TValue;

	public override bool CanReplicate(object value, ReplicationArgs args, Type baseType = null)
		=> TypeOf.Object.Raw.Is(ActiveType) || baseType.Is(ActiveType) || value is Map map &&
		map.TryGetValue(args.Profile.TypeKey, out var typeValue) && RestoreType(typeValue).Is(ActiveType);

	private Type RestoreType(object typeValue) => typeValue switch
	{
		string typeName => Type.GetType(typeName),
		Type type => type,
		_ => typeValue?.GetType()
	};

}
