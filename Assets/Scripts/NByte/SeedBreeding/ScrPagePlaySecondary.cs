using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NByte.SeedBreeding
{
    public class ScrPagePlaySecondary : MonoBehaviour
    {
        [SerializeField] private TMP_Text TbxTimer;
        [SerializeField] private TMP_Text TbxPoints;
        [SerializeField] private TMP_Text TbxOperations;
        [SerializeField] private Image ImgRed;
        [SerializeField] private Image ImgGreen;
        [SerializeField] private Image ImgBlue;
        [SerializeField] private Image ImgYellow;
        [SerializeField] private TMP_Text TbxTalk;

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
        public float ProgressRed
        {
            set => ImgRed.fillAmount = value;
        }
        public float ProgressGreen
        {
            set => ImgGreen.fillAmount = value;
        }
        public float ProgressBlue
        {
            set => ImgBlue.fillAmount = value;
        }
        public float ProgressYellow
        {
            set => ImgYellow.fillAmount = value;
        }
        public string Talk
        {
            set => TbxTalk.text = value;
        }
    }
}