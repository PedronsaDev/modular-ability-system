using System;
using UnityEngine;

public interface IEffect<in TTarget>
{
    void Apply(GameObject caster,TTarget target);
    void Cancel();
    event Action<IEffect<TTarget>> OnCompleted;
}