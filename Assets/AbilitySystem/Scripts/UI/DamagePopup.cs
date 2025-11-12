using PrimeTween;
using TMPro;
using UnityEngine;

/// <summary>
/// Simple animated popup to display damage/heal numbers using PrimeTween for scale animation.
/// </summary>
public class DamagePopup : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>Configure popup text & play spawn scale animation; schedules destruction.</summary>
    public void SetDamage(float damage)
    {
        this.gameObject.GetComponent<TMP_Text>().SetText(damage.ToString("N0"));
        transform.localScale = Vector3.zero;
        Tween.Scale(transform, 0.1f, 0.2f, Ease.OutExpo);
        Destroy(this.gameObject, 0.5f);
    }
}
