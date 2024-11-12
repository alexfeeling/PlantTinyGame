using TMPro;
using UnityEngine;

namespace NByte.Transplanting
{
    public class ScrPagePlaySecondary : MonoBehaviour
    {
        [SerializeField] private TMP_Text TbxTimer;
        [SerializeField] private TMP_Text TbxPoints;
        [SerializeField] private TMP_Text TbxLevel;

        public int Timer
        {
            set => TbxTimer.text = value.ToString();
        }
        public int Points
        {
            set => TbxPoints.text = value.ToString();
        }

        private int difficulty;
        public int Difficulty
        {
            get => difficulty;
            set
            {
                difficulty = value;
                RefreshLevel();
            }
        }

        private int progress;
        public int Progress
        {
            get => progress;
            set
            {
                progress = value;
                RefreshLevel();
            }
        }

        private void RefreshLevel()
        {
            TbxLevel.text = $"{Difficulty}-{Progress}";
        }
    }
}