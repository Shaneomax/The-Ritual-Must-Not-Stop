using System;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Smooth two-phase DOTween pickup animation:
///
///   Phase 1 — FOCUS: The item glides smoothly to the center of the camera view.
///                    A slight upward arc prevents it from clipping through tables.
///
///   Phase 2 — COLLECT: The item rushes into the player and shrinks to nothing.
///
/// Inspector setup:
///   - Focus Distance: how far in front of the camera the item should float (e.g., 1.5m)
///   - Arc Height: adds a slight curve to the movement to clear surfaces (0 = straight line)
/// </summary>
public class PickupAnimationHandler : MonoBehaviour
{
    // ── Phase 1 — Float Into Camera View ──────────────────────────────────────

    [Header("Phase 1 — Float Into Camera View")]
    [Tooltip("How long the item takes to glide to the centre of the camera view.")]
    [SerializeField] private float _focusDuration = 0.6f;
    [Tooltip("OutQuad is smooth and has no 'bounce' at the end.")]
    [SerializeField] private Ease _focusEase = Ease.OutQuad;

    [Tooltip("How far in front of the camera (metres) the item floats to.")]
    [SerializeField] private float _focusDistance = 1.5f;

    [Tooltip("Adds a slight upward curve to the path so it clears tables/floors smoothly.")]
    [SerializeField] private float _arcHeight = 0.2f;

    // ── Phase 2 — Come Closer to Player ───────────────────────────────────────

    [Header("Phase 2 — Come Closer to Player")]
    [Tooltip("How long the item takes to rush toward the player.")]
    [SerializeField] private float _collectDuration = 0.4f;
    [Tooltip("InQuad starts slow then accelerates into the player.")]
    [SerializeField] private Ease _collectEase = Ease.InQuad;

    [Tooltip("Extra distance the item travels PAST the camera into the player body.")]
    [SerializeField] private float _collectPassDistance = 0.5f;

    [Tooltip("Shrink the item to zero as it flies in.")]
    [SerializeField] private bool _scaleDownOnCollect = true;

    // ── State ─────────────────────────────────────────────────────────────────

    public bool IsPlaying { get; private set; }

    // ── Public API ────────────────────────────────────────────────────────────

    public void Play(PlayerContext playerContext, Action onComplete)
    {
        if (IsPlaying) return;
        IsPlaying = true;

        // Disable collider to prevent clipping and double-triggering
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        playerContext.FreezePlayer();

        Camera cam = playerContext.MainCamera;
        if (cam == null) cam = Camera.main;

        // ── Compute targets ────────────────────────────────────────────────────

        // Target 1: Exactly in front of the camera
        Vector3 focusPosition = cam.transform.position + cam.transform.forward * _focusDistance;

        // Target 2: Past the camera, into the player
        Vector3 collectPosition = cam.transform.position - cam.transform.forward * _collectPassDistance;

        // ── Build Sequence ────────────────────────────────────────────────────
        Sequence seq = DOTween.Sequence();

        // Phase 1 — Smooth glide to center of view with a slight arc to avoid table clipping
        if (_arcHeight > 0f)
        {
            // DOJump creates a smooth parabola. numJumps=1 means it just arcs once to the target.
            seq.Append(transform.DOJump(focusPosition, _arcHeight, 1, _focusDuration).SetEase(_focusEase));
        }
        else
        {
            seq.Append(transform.DOMove(focusPosition, _focusDuration).SetEase(_focusEase));
        }

        // Phase 2 — Rush toward the player
        seq.Append(transform.DOMove(collectPosition, _collectDuration).SetEase(_collectEase));

        if (_scaleDownOnCollect)
        {
            // Start shrinking exactly when Phase 2 starts
            seq.Join(transform.DOScale(Vector3.zero, _collectDuration).SetEase(_collectEase));
        }

        seq.OnComplete(() =>
        {
            IsPlaying = false;
            playerContext.UnfreezePlayer();
            onComplete?.Invoke();
        });
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}
