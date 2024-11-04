using TMPro;
using UnityEngine;

namespace NByte.SeedBreeding
{
    public class ScrPagePlayMain : MonoBehaviour
    {
        [SerializeField] private TMP_Text TbxTimer;
        [SerializeField] private TMP_Text TbxPoints;
        [SerializeField] private TMP_Text TbxOperations;

        public int Timer
        {
            set => TbxTimer.text = value.ToString();
        }
        public int Points
        {
            set => TbxPoints.text = value.ToString();
        }
        public int Operations
        {
            set => TbxOperations.text = value.ToString();
        }
    }
}