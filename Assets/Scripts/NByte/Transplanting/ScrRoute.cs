using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NByte.Transplanting
{
    [RequireComponent(typeof(LineRenderer))]
    public class ScrRoute : MonoBehaviour
    {
        [SerializeField] private LineRenderer Line;

        public List<ScrField> RoutePoints
        {
            set
            {
                if (value == null)
                {
                    Line.positionCount = 0;
                }
                else
                {
                    Line.positionCount = value.Count;
                    Line.SetPositions(value.Select(t => t.transform.position).ToArray());
                }
            }
        }

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