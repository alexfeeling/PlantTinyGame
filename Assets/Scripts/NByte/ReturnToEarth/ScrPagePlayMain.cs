using TMPro;
using UnityEngine;

namespace NByte.ReturnToEarth
{
    public class ScrPagePlayMain : MonoBehaviour
    {
        [SerializeField] private TMP_Text TbxDistanceToEarth;
        [SerializeField] private Animator AmtCountdown;
        [SerializeField] private Material MatRay;
        [SerializeField] private Vector2 MatRayOffset;
        [SerializeField] private Material MatBand;
        [SerializeField] private Vector2 MatBandOffset;

        private CustomAwaiter AwaiterCountdown { get; set; } = new();

        public float DistanceToEarth
        {
            set { TbxDistanceToEarth.text = value.ToString("N0"); }
        }

        public CustomAwaiter Countdown()
        {
            AwaiterCountdown.IsCompleted = false;
            AmtCountdown.SetTrigger("Start");
            return AwaiterCountdown;
        }
        public void FinishCountdown()
        {
            AwaiterCountdown.IsCompleted = true;
        }

        private void Update()
        {
            MoveMaterial();
        }
        private void MoveMaterial()
        {
            MatRay.mainTextureOffset += MatRayOffset * Time.deltaTime;
            MatBand.mainTextureOffset += MatBandOffset * Time.deltaTime;
        }
    }
}