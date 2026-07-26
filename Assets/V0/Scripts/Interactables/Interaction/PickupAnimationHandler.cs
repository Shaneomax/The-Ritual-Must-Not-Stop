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
    [Header("Pickup Animation Settings")]
    [Tooltip("How long the item takes to zip into the player.")]
    [SerializeField] private float _collectDuration = 0.2f;
    
    [Tooltip("The DOTween ease type. InQuad starts slow and snaps fast into the player.")]
    [SerializeField] private Ease _collectEase = Ease.InQuad;

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

        // Target: Just slightly below/inside the camera so it looks like it goes into the player's body
        Vector3 collectPosition = cam.transform.position - cam.transform.up * 0.5f;

        // ── Build Sequence ────────────────────────────────────────────────────
        Sequence seq = DOTween.Sequence();

        // Single Phase — Rush straight into the player fast
        seq.Append(transform.DOMove(collectPosition, _collectDuration).SetEase(_collectEase));

        if (_scaleDownOnCollect)
        {
            // Shrink as it flies
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
