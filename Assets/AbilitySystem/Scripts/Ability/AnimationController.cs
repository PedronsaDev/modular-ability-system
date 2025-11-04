using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator _animator;
    private CountdownTimer _animationTimer;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();

        _animationTimer = new CountdownTimer(0f);
    }

    public void PlayOneShot(AnimationClip clip)
    {
        if (!clip || !_animator)
            return;

        Debug.Log($"Playing one-shot animation: {clip.name}");

        _animationTimer.Reset(clip.length);
        _animationTimer.Start();
        _animator.CrossFade(clip.name, 0.1f);
    }
}
