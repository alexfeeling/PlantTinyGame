using TMPro;
using UnityEngine;

namespace NByte.Transplanting
{
    public class ScrPageFinishMain : MonoBehaviour
    {
        [SerializeField] private TMP_Text TbxTimer;
        [SerializeField] private TMP_Text TbxPoints;
        [SerializeField] private TMP_Text TbxLevel;

        private ScrScnTransplanting ScnTransplanting { get; set; }

        public void Init(ScrScnTransplanting scnTransplanting)
        {
            ScnTransplanting = scnTransplanting;
        }

        private void OnEnable()
        {
            TbxTimer.text = ScnTransplanting.Timer.ToString();
            TbxPoints.text = ScnTransplanting.Points.ToString();
            TbxLevel.text = $"{ScnTransplanting.Difficulty}-{ScnTransplanting.Progress}";
        }
    }
}