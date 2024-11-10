using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace NByte.Transplanting
{
    [RequireComponent(typeof(Animator))]
    public class ScrMachine : MonoBehaviour
    {
        private static AppService AppService => AppService.Instance;
        private static TransplantingConfig Config => AppService.Transplanting.Config;

        [SerializeField] private Animator Animator;

        private MachineStates machineState;
        public MachineStates MachineState
        {
            get => machineState;
            set
            {
                if (machineState != value)
                {
                    machineState = value;
                    Animator.SetInteger("State", (int)value);
                }
            }
        }

        public IEnumerator Move(ScrField field)
        {
            Vector3 current = transform.position;
            Vector3 target = field.transform.position;
            if (target.y > current.y)
            {
                MachineState = MachineStates.Up;
            }
            else if (target.y < current.y)
            {
                MachineState = MachineStates.Down;
            }
            else if (target.x < current.x)
            {
                MachineState = MachineStates.Left;
            }
            else
            {
                MachineState = MachineStates.Right;
            }
            yield return transform.DOMove(target, Config.DriveSpeed).SetEase(Ease.Linear).WaitForCompletion();
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