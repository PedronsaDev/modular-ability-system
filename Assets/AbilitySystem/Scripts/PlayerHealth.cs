using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _currentHealth = 100;

    readonly List<IEffect<IDamageable>> _activeEffects = new List<IEffect<IDamageable>>();

    private void Awake() => _currentHealth = _maxHealth;

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        Debug.Log($"Player took {damage} damage. Current health: {_currentHealth}/{_maxHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    public void ApplyEffect(GameObject caster, IEffect<IDamageable> effect)
    {
        effect.OnCompleted += RemoveEffect;
        _activeEffects.Add(effect);
        effect.Apply(caster, this);
    }

    private void RemoveEffect(IEffect<IDamageable> effect)
    {
        effect.OnCompleted -= RemoveEffect;
        _activeEffects.Remove(effect);
    }

    private void Die()
    {
        Debug.Log("Enemy died: " + gameObject.name);

        foreach (IEffect<IDamageable> effect in _activeEffects)
        {
            effect.OnCompleted -= RemoveEffect;
            effect.Cancel();
        }
        _activeEffects.Clear();

        Destroy(gameObject);
    }
}
