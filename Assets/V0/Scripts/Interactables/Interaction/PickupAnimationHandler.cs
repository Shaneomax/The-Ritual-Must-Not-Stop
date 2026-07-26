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
    [Tooltip("How long the item takes to fly to the player's screen.")]
    [SerializeField] private float _flyDuration = 0.3f;

    [Tooltip("How long the item pauses in front of the screen so the player can see it.")]
    [SerializeField] private float _pauseDuration = 0.2f;

    [Tooltip("How long it takes to shrink away into the inventory.")]
    [SerializeField] private float _shrinkDuration = 0.15f;
    
    [Tooltip("The DOTween ease type for the flying motion.")]
    [SerializeField] private Ease _flyEase = Ease.OutBack;

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

        // Target: Exactly in front of the camera so it flies straight at the player's face
        Vector3 collectPosition = cam.transform.position + (cam.transform.forward * 0.5f);

        // ── Build Sequence ────────────────────────────────────────────────────
        Sequence seq = DOTween.Sequence();

        // Phase 1 — Fly straight to the camera (without shrinking)
        seq.Append(transform.DOMove(collectPosition, _flyDuration).SetEase(_flyEase));

        // Phase 2 — Pause briefly so the player sees what they picked up
        if (_pauseDuration > 0f)
        {
            seq.AppendInterval(_pauseDuration);
        }

        // Phase 3 — Shrink away into the inventory
        seq.Append(transform.DOScale(Vector3.zero, _shrinkDuration).SetEase(Ease.InBack));

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
