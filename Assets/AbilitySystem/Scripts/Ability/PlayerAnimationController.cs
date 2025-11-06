using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class PlayerAnimationController : MonoBehaviour
{
    private static readonly int _idle = Animator.StringToHash("Idle");
    private static readonly int _moving = Animator.StringToHash("Moving");
    private static readonly int _moveX = Animator.StringToHash("MoveX");
    private static readonly int _moveY = Animator.StringToHash("MoveY");

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

        var graph = PlayableGraph.Create();
        var output = AnimationPlayableOutput.Create(graph, "OneShot", _animator);
        var clipPlayable = AnimationClipPlayable.Create(graph, clip);
        output.SetSourcePlayable(clipPlayable);
        graph.Play();

        StartCoroutine(StopGraphAfter(graph, clip.length));

        IEnumerator StopGraphAfter(PlayableGraph g, float t)
        {
            yield return new WaitForSeconds(t);
            g.Destroy();
        }
    }

    public void SetMovementState(MovementState state)
    {
        if (!_animator) return;
        _animator.SetBool(_idle, state == MovementState.Idle);
        _animator.SetBool(_moving, state == MovementState.Moving);
    }

    public void SetMoveBlend(Vector2 move, float dampTime = 0.1f)
    {
        if (!_animator) return;
        _animator.SetFloat(_moveX, move.x, dampTime, Time.deltaTime);
        _animator.SetFloat(_moveY, move.y, dampTime, Time.deltaTime);
    }
}
