/// <summary>
/// Factory interface for creating IEffect instances
/// </summary>
public interface IEffectFactory<in TTarget>
{
    /// <summary>Create a fresh runtime effect instance ready for application.</summary>
    IEffect<TTarget> CreateEffect();
}
