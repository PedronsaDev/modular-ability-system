using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHealth = 100;
    [SerializeField] private float _currentHealth = 100;

    private bool _isDead = false;

    readonly List<IEffect<IDamageable>> _activeEffects = new();

    private void Awake() => _currentHealth = _maxHealth;

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        Debug.Log($"Player took {damage} damage. Current health: {_currentHealth}/{_maxHealth}");

        if (_currentHealth <= 0)
            Die();
        else if (_currentHealth > _maxHealth)
            _currentHealth = _maxHealth;
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
        if (_isDead)
            return;

        Debug.Log("Enemy died: " + gameObject.name);

        foreach (IEffect<IDamageable> effect in _activeEffects)
        {
            effect.OnCompleted -= RemoveEffect;
            effect.Cancel();
        }
        _activeEffects.Clear();
        _isDead = true;
        Destroy(gameObject);
    }
}
