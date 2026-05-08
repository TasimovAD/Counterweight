using System;
using System.Collections.Generic;
using UnityEngine;

namespace Counterweight.Demolition
{
    /// <summary>
    /// Top-level container for a destructible structure (e.g. a tower).
    /// Tracks how much of the original mass is still in place and fires events
    /// when stability changes or the demolition objective is met.
    ///
    /// "In place" = block has not moved more than <see cref="standingTolerance"/>
    /// from its starting position. This counts both fallen and merely shifted
    /// blocks as no-longer-standing.
    /// </summary>
    public sealed class DemolitionTarget : MonoBehaviour
    {
        [Header("Objective")]
        [Tooltip("Stability fraction at or below which the structure is considered demolished.")]
        [Range(0f, 1f)] [SerializeField] private float completionThreshold = 0.2f;

        [Header("Tracking")]
        [Tooltip("Distance from a block's starting position beyond which it is considered no longer standing.")]
        [Min(0.05f)] [SerializeField] private float standingTolerance = 0.5f;

        [Tooltip("How often (seconds) stability is recomputed.")]
        [Min(0.05f)] [SerializeField] private float pollInterval = 0.25f;

        public event Action<float> StabilityChanged;
        public event Action DemolitionComplete;

        public float Stability { get; private set; } = 1f;
        public bool IsComplete { get; private set; }

        private readonly List<Block> blocks = new();
        private float startingMass;
        private float pollTimer;

        private void Start()
        {
            blocks.Clear();
            GetComponentsInChildren(true, blocks);
            startingMass = ComputeStandingMass();
            Stability = 1f;
        }

        private void Update()
        {
            pollTimer += Time.deltaTime;
            if (pollTimer < pollInterval) return;
            pollTimer = 0f;

            float current = startingMass > 0f ? ComputeStandingMass() / startingMass : 0f;
            if (!Mathf.Approximately(current, Stability))
            {
                Stability = current;
                StabilityChanged?.Invoke(Stability);
            }

            if (!IsComplete && Stability <= completionThreshold)
            {
                IsComplete = true;
                DemolitionComplete?.Invoke();
            }
        }

        private float ComputeStandingMass()
        {
            float total = 0f;
            float toleranceSqr = standingTolerance * standingTolerance;
            foreach (Block block in blocks)
            {
                if (block == null) continue;
                Vector3 offset = block.transform.position - block.StartingPosition;
                if (offset.sqrMagnitude < toleranceSqr)
                {
                    total += block.Mass;
                }
            }
            return total;
        }
    }
}
