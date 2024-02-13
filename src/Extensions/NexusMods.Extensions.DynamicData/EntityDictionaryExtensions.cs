using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Kernel;
using NexusMods.Abstractions.Serialization.DataModel;
using NexusMods.Abstractions.Serialization.DataModel.Ids;

namespace NexusMods.Extensions.DynamicData;

/// <summary/>
public static class EntityDictionaryExtensions
{
    /// <summary>
    /// Creates a changeset between this dictionary and another.
    /// </summary>
    /// <param name="dict">The current dictionary.</param>
    /// <param name="old">The old/previous dictionary.</param>
    /// <returns></returns>
    public static IChangeSet<IId, TK> Diff<TK, TV>(this EntityDictionary<TK,TV> dict, EntityDictionary<TK,TV> old)
        where TK : notnull
        where TV : Entity
    {
        var changes = new ChangeSet<IId,TK>();
        var dictCollection = dict.Collection;
        var oldCollection = old.Collection;

        foreach (var (key, id) in dictCollection)
        {
            if (!oldCollection.TryGetValue(key, out var oldId))
            {
                changes.Add(new Change<IId, TK>(ChangeReason.Add, key, id));
                continue;
            }

            if (!id.Equals(oldId))
            {
                changes.Add(new Change<IId, TK>(ChangeReason.Update, key, id,  Optional.Some(oldId)));
            }
        }

        foreach (var (key, id) in oldCollection)
        {
            if (!dictCollection.ContainsKey(key))
                changes.Add(new Change<IId, TK>(ChangeReason.Remove, key, id));
        }

        return changes;
    }


    /// <summary>
    /// Compares the current collection with the previous collection and returns the changes.
    /// Uses the key selector to determine the key, and the value selector to determine the value.
    /// </summary>
    /// <param name="colls"></param>
    /// <param name="keySelector"></param>
    /// <param name="valueSelector"></param>
    /// <typeparam name="TInV"></typeparam>
    /// <typeparam name="TOutV"></typeparam>
    /// <typeparam name="TOutK"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IObservable<IChangeSet<TOutV, TOutK>>
        ToDiffedChangeSet<TInV, TOutV, TOutK>(
            this IObservable<IEnumerable<TInV>> colls,
            Func<TInV, TOutK> keySelector, Func<TInV, TOutV> valueSelector)
        where TOutK : notnull where TOutV : notnull
    {
        var changeSet =
            new SourceCache<TOutV, TOutK>(x =>
                throw new InvalidOperationException());

        return Observable.Create<IChangeSet<TOutV, TOutK>>(observer =>
        {

            var disp1 = colls.Subscribe(coll =>
            {
                var indexed = coll.ToDictionary(keySelector, valueSelector);

                changeSet.Edit(x =>
                {
                    foreach (var (key, value) in indexed)
                    {
                        x.AddOrUpdate(value, key);
                    }

                    foreach (var (key, value) in x.KeyValues)
                    {
                        if (!indexed.ContainsKey(key))
                        {
                            x.Remove(key);
                        }
                    }
                });
            });

            var disp2 = changeSet.Connect().Subscribe(observer);

            return new CompositeDisposable(disp1, disp2);
        });

    }
}
