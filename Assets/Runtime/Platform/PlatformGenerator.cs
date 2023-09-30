using UnityEngine;
using UnityEngine.Pool;

namespace CMIYC.Platform
{
    public class PlatformGenerator : MonoBehaviour
    {
        [SerializeField]
        private Vector2Int _motherboardSize = new(5, 5);

        [SerializeField]
        private float _daughterboardUnitSize = 8f;

        private Vector2 MotherboardSize => _motherboardSize;

        private void Start()
        {
            BuildMotherboard();
        }

        private void BuildMotherboard()
        {
            var initialOffset = new Vector2(_daughterboardUnitSize, _daughterboardUnitSize) * 0.5f;
            _ = initialOffset;
        }

        private void OnDrawGizmos()
        {
            var target = transform;
            Gizmos.matrix = target.localToWorldMatrix;
            var pos = target.position;

            var motherboards = ListPool<Vector2>.Get();

            const int height = 10;

            var offset = MotherboardSize * _daughterboardUnitSize;
            var physicalMotherboardSize = MotherboardSize * _daughterboardUnitSize;

            motherboards.Add(offset);
            motherboards.Add(offset.WithY(0));
            motherboards.Add(offset.WithX(0));
            motherboards.Add(new Vector2(0, 0));

            var offsetHalf = offset / 2f;

            foreach (var board in motherboards)
            {
                var center = board + offsetHalf;
                var (x, z) = center;

                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(new Vector3(x, height / 2f, z) + pos, new Vector3(physicalMotherboardSize.x, height, physicalMotherboardSize.y));

                var cellOffset = _daughterboardUnitSize / 2f;
                for (int mx = 0; mx < _motherboardSize.x; mx++)
                {
                    for (int my = 0; my < _motherboardSize.y; my++)
                    {
                        var daughterCenter = new Vector3(mx * _daughterboardUnitSize + cellOffset, height / 2f, my * _daughterboardUnitSize + cellOffset) + new Vector3(board.x, 0, board.y);

                        Gizmos.color = Color.red.WithA(0.25f);
                        Gizmos.DrawWireCube(daughterCenter + pos, new Vector3(_daughterboardUnitSize, height, _daughterboardUnitSize));

                        Gizmos.color = Color.blue.WithA(0.1f);
                        Gizmos.DrawLine(daughterCenter.WithY(-height) + pos, daughterCenter.WithY(height * 2));

                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(daughterCenter.WithY(0) + pos, 0.1f);
                    }
                }

            }

            ListPool<Vector2>.Release(motherboards);
        }
    }
}
