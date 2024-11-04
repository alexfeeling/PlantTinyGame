using UnityEngine;

namespace NByte.ReturnToEarth
{
    public class ScrObstacle : MonoBehaviour
    {
        [SerializeField] private Transform Top;
        [SerializeField] private Transform Bottom;

        public void Init(Vector3 position, float gap)
        {
            transform.localPosition = position;
            float offset = gap / 2;
            Top.transform.localPosition = new Vector3(0, offset, 0);
            Bottom.transform.localPosition = new Vector3(0, -offset, 0);
        }
    }
}