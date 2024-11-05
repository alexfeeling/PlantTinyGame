using TMPro;
using UnityEngine;

namespace NByte.Transplanting
{
    public class ScrPageTitleMain : MonoBehaviour
    {
        [SerializeField] private TMP_Text TbxBlocks;

        public int Blocks
        {
            set => TbxBlocks.text = value.ToString();
        }
    }
}