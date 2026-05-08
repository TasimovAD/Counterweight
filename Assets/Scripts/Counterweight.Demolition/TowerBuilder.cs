using System.Collections.Generic;
using UnityEngine;

namespace Counterweight.Demolition
{
    /// <summary>
    /// Edit-time grid builder. Spawns a width × depth × layers stack of block
    /// instances under this GameObject and connects neighbors with FixedJoints.
    /// Use the context menu (Build Tower / Clear) on the component header.
    ///
    /// Built blocks are children of this GameObject; place a <see cref="DemolitionTarget"/>
    /// on the same GameObject to track stability of the structure.
    /// </summary>
    public sealed class TowerBuilder : MonoBehaviour
    {
        [Header("Block")]
        [SerializeField] private GameObject blockPrefab;
        [SerializeField] private BlockConfig blockConfig;
        [SerializeField, Min(0.1f)] private float blockSize = 1f;

        [Header("Grid")]
        [SerializeField, Min(1)] private int width = 2;
        [SerializeField, Min(1)] private int depth = 2;
        [SerializeField, Min(1)] private int layers = 15;

        [Header("Placement")]
        [Tooltip("Y offset of the bottom face of the lowest layer above this GameObject's pivot.")]
        [SerializeField] private float baseHeightOffset = 0f;

        [ContextMenu("Build Tower")]
        public void BuildTower()
        {
            if (blockPrefab == null)
            {
                Debug.LogError("[TowerBuilder] Block prefab is not assigned.", this);
                return;
            }
            if (blockConfig == null)
            {
                Debug.LogError("[TowerBuilder] Block config is not assigned.", this);
                return;
            }

            Clear();

            Block[,,] grid = new Block[width, depth, layers];

            for (int y = 0; y < layers; y++)
            for (int x = 0; x < width; x++)
            for (int z = 0; z < depth; z++)
            {
                Vector3 localPos = new Vector3(
                    (x - (width - 1) * 0.5f) * blockSize,
                    baseHeightOffset + y * blockSize + blockSize * 0.5f,
                    (z - (depth - 1) * 0.5f) * blockSize);

                GameObject instance = Instantiate(blockPrefab, transform);
                instance.transform.localPosition = localPos;
                instance.transform.localRotation = Quaternion.identity;
                instance.transform.localScale = Vector3.one * blockSize;
                instance.name = $"Block_{x}_{z}_L{y}";

                Block block = instance.GetComponent<Block>();
                if (block == null)
                {
                    Debug.LogError("[TowerBuilder] Block prefab is missing the Block component.", this);
                    return;
                }
                grid[x, z, y] = block;
            }

            for (int y = 0; y < layers; y++)
            for (int x = 0; x < width; x++)
            for (int z = 0; z < depth; z++)
            {
                Block block = grid[x, z, y];
                if (block == null) continue;

                if (x + 1 < width)  ConnectBlocks(block, grid[x + 1, z, y]);
                if (z + 1 < depth)  ConnectBlocks(block, grid[x, z + 1, y]);
                if (y - 1 >= 0)     ConnectBlocks(block, grid[x, z, y - 1]);
            }

            foreach (Block block in grid)
            {
                if (block != null) block.ApplyJointSettingsToOwnedJoints();
            }

            Debug.Log($"[TowerBuilder] Built {width * depth * layers} blocks.", this);
        }

        [ContextMenu("Clear")]
        public void Clear()
        {
            List<Transform> children = new();
            foreach (Transform child in transform) children.Add(child);
            foreach (Transform child in children)
            {
                if (Application.isPlaying) Destroy(child.gameObject);
                else DestroyImmediate(child.gameObject);
            }
        }

        private static void ConnectBlocks(Block a, Block b)
        {
            if (a == null || b == null) return;
            Rigidbody rbB = b.GetComponent<Rigidbody>();
            if (rbB == null) return;

            FixedJoint joint = a.gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = rbB;
            joint.enableCollision = false;
            // Preprocessing helps the solver stabilize stacked joints — keep on (Unity default).
        }
    }
}
