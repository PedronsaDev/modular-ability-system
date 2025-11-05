using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject _damagePopupPrefab;
    public float Health = 50;

    readonly List<IEffect<IDamageable>> _activeEffects = new List<IEffect<IDamageable>>();

    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
            Die();

        if (_damagePopupPrefab)
            ShowDamagePopup(damage);
    }
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
