using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject _damagePopupPrefab;
    [SerializeField] private float _maxHealth = 50;
    [SerializeField, ReadOnly] private float _health = 50;

    readonly List<IEffect<IDamageable>> _activeEffects = new List<IEffect<IDamageable>>();

    private void Start()
    {
        _health = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        ModifyHealth(-damage);
    }

    public void Heal(float amount)
    {
        ModifyHealth(amount);
    }

    private void ModifyHealth(float amount)
    {
        _health += amount;
        if (_health <= 0)
            Die();
        else if (_health > _maxHealth)
            _health = _maxHealth;

        if (_damagePopupPrefab)
            ShowDamagePopup(amount);
    }

    /// <summary>Spawn a damage popup showing amount delta.</summary>
    private void ShowDamagePopup(float damage)
    {
        GameObject popup = Instantiate(_damagePopupPrefab, transform.position + Vector3.up * 2, Quaternion.identity);

        DamagePopup damagePopup = popup.GetComponent<DamagePopup>();
        if (damagePopup)
            damagePopup.SetDamage(damage);
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

    /// <summary>Handle enemy death: cancels active effects and destroys GameObject.</summary>
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
