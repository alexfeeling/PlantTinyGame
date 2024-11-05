using TMPro;
using UnityEngine;

namespace NByte.SeedBreeding
{
    public class ScrPageFinishMain : MonoBehaviour
    {
        [SerializeField] private TMP_Text TbxPoints;

        public int Points
        {
            set => TbxPoints.text = value.ToString();
        }
    }
}