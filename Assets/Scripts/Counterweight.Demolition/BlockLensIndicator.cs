using UnityEngine;

namespace Counterweight.Demolition
{
    /// <summary>
    /// While the engineer's lens is active, tints this block's MeshRenderer
    /// based on the block's current HP fraction (green = full, red = near zero).
    /// Detached blocks are left untinted so the player sees what's still
    /// part of the structure.
    ///
    /// Uses MaterialPropertyBlock to avoid creating per-block material instances.
    /// </summary>
    [RequireComponent(typeof(Block))]
    [RequireComponent(typeof(MeshRenderer))]
    public sealed class BlockLensIndicator : MonoBehaviour
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        [Tooltip("Tint at full HP.")]
        [SerializeField] private Color healthyColor = new(0.55f, 0.95f, 0.65f, 1f);

        [Tooltip("Tint at zero HP.")]
        [SerializeField] private Color damagedColor = new(1f, 0.45f, 0.3f, 1f);

        private Block block;
        private MeshRenderer meshRenderer;
        private MaterialPropertyBlock propertyBlock;

        private void Awake()
        {
            block = GetComponent<Block>();
            meshRenderer = GetComponent<MeshRenderer>();
            propertyBlock = new MaterialPropertyBlock();
        }

        private void OnEnable()
        {
            block.Damaged += HandleDamaged;
            block.Detached += HandleDetached;
            EngineerLens.ActiveChanged += HandleLensChanged;
            Refresh();
        }

        private void OnDisable()
        {
            block.Damaged -= HandleDamaged;
            block.Detached -= HandleDetached;
            EngineerLens.ActiveChanged -= HandleLensChanged;
            ClearTint();
        }

        private void HandleDamaged(float damage, Vector3 point) => Refresh();
        private void HandleDetached(Vector3 point) => Refresh();
        private void HandleLensChanged(bool _) => Refresh();

        private void Refresh()
        {
            if (!EngineerLens.IsActive || block.IsDetached)
            {
                ClearTint();
                return;
            }
            Color color = Color.Lerp(damagedColor, healthyColor, block.HpFraction);
            propertyBlock.SetColor(BaseColorId, color);
            meshRenderer.SetPropertyBlock(propertyBlock);
        }

        private void ClearTint()
        {
            propertyBlock.Clear();
            meshRenderer.SetPropertyBlock(null);
        }
    }
}
