using UnityEngine;

namespace NByte
{
    public partial class AppService
    {
        [SerializeField] private Animator AmtCurtain;

        private CustomAwaiter AwaiterCurtain { get; set; } = new();

        public CustomAwaiter ShowCurtain()
        {
            AwaiterCurtain.IsCompleted = false;
            AmtCurtain.SetTrigger("Show");
            return AwaiterCurtain;
        }
        public CustomAwaiter HideCurtain()
        {
            AwaiterCurtain.IsCompleted = false;
            AmtCurtain.SetTrigger("Hide");
            return AwaiterCurtain;
        }
        public void FinishCurtain()
        {
            AwaiterCurtain.IsCompleted = true;
        }
    }
}