using TMPro;
using UnityEngine;

namespace NByte.ReturnToEarth
{
    public class ScrPagePlaySecondary : MonoBehaviour
    {
        [SerializeField] private TMP_Text TbxDistanceToEarth;
        [SerializeField] private TMP_Text TbxTalk;

        public float DistanceToEarth
        {
            set { TbxDistanceToEarth.text = value.ToString("N0"); }
        }
        public string Talk
        {
            set { TbxTalk.text = value; }
        }
    }
}