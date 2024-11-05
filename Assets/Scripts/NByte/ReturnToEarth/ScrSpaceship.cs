using UnityEngine;

namespace NByte.ReturnToEarth
{
    public class ScrSpaceship : MonoBehaviour
    {
        private static ReturnToEarthConfig Config => AppService.Instance.ReturnToEarth.Config;

        [SerializeField] private Rigidbody2D Rigidbody;
        [SerializeField] private Animator AmtFlame;

        public ScrScnReturnToEarth ScnReturnToEarth { get; private set; }

        private bool state;
        public bool State
        {
            get => state;
            set
            {
                state = value;
                Rigidbody.gravityScale = state ? Config.SpaceshipGravity : 0;
                if (state)
                {
                    Rigidbody.gravityScale = Config.SpaceshipGravity;
                }
                else
                {
                    Rigidbody.gravityScale = 0;
                    Rigidbody.velocity = Vector2.zero;
                }
            }
        }

        private void Update()
        {
            Climb();
        }
        private void Climb()
        {
            if (State)
            {
                if (Input.GetMouseButtonDown(0) || (Input.touchSupported && Input.GetTouch(0).phase == TouchPhase.Began))
                {
                    //Rigidbody.velocity = Vector2.up * Config.SpaceshipClimb;
                    Rigidbody.AddForce(Vector2.up * Config.SpaceshipClimb, ForceMode2D.Impulse);
                }
            }
        }

        public void Ini(ScrScnReturnToEarth scnReturnToEarth)
        {
            ScnReturnToEarth = scnReturnToEarth;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            ScnReturnToEarth.StopGame();
        }
    }
}