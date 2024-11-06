using UnityEngine;

namespace NByte.Transplanting
{
    [RequireComponent(typeof(LineRenderer))]
    public class ScrRoute : MonoBehaviour
    {
        [SerializeField] private LineRenderer Line;

        private void Start()
        {
            Line.positionCount = 0;
        }

        private void Reset()
        {
            Line = GetComponent<LineRenderer>();
        }
    }
}