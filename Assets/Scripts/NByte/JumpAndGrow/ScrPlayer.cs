using System.Collections;
using UnityEngine;

namespace NByte.JumpAndGrow
{
    public class ScrPlayer : MonoBehaviour
    {
        private static JumpAndGrowConfig Config => AppService.Instance.JumpAndGrow.Config;

        [SerializeField] private Rigidbody2D Rigidbody;
        [SerializeField] private SpriteRenderer ChargeBar;

        public bool InputState { get; set; }
        public bool IsCharging { get; set; }

        private float jumpPower;
        private float JumpPower
        {
            get => jumpPower;
            set
            {
                jumpPower = value;
                ChargeBar.material.SetFloat("_Progress", jumpPower);
            }
        }

        private void Awake()
        {
            JumpPower = 0;
        }

        private void Update()
        {
            if (InputState)
            {
                PointDown();
                PointUp();
            }
        }
        private void PointDown()
        {
            if (Input.GetMouseButtonDown(0) || (Input.touchSupported && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                if (!IsCharging)
                {
                    StartCoroutine(Steps());
                }
            }

            IEnumerator Steps()
            {
                IsCharging = true;
                float timer = 0;
                while (IsCharging)
                {
                    yield return null;
                    timer += Time.deltaTime;
                    JumpPower = Mathf.PingPong(timer, Config.ChargeDuration) / Config.ChargeDuration;
                }
                Rigidbody.AddForce(Config.JumpVelocity * (0.5f + JumpPower), ForceMode2D.Impulse);
                JumpPower = 0;
            }
        }
        private void PointUp()
        {
            if (Input.GetMouseButtonUp(0) || (Input.touchSupported && Input.GetTouch(0).phase == TouchPhase.Ended))
            {
                IsCharging = false;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Rigidbody.velocity = Vector2.zero;
            InputState = true;
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            InputState = false;
        }
    }
}