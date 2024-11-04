using UnityEngine;

namespace NByte
{
    public class CustomAwaiter : CustomYieldInstruction
    {
        public bool IsCompleted { get; set; }

        public override bool keepWaiting => !IsCompleted;

        public CustomAwaiter()
        {
            IsCompleted = false;
        }
    }
}