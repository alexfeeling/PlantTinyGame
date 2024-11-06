using UnityEngine;

namespace NByte.Transplanting
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public class ScrMachine : MonoBehaviour
    {
        [SerializeField] private Animator Animator;

        public MachineStates MachineState
        {
            set => Animator.SetInteger("State", (int)value);
        }

        private void Reset()
        {
            Animator = GetComponent<Animator>();
        }

        public enum MachineStates
        {
            Idle,
            Up,
            Down,
            Left,
            Right,
        }
    }
}