using UnityEngine;

namespace NByte
{
    [DefaultExecutionOrder(-10)]
    public abstract class ScrSceneBase : MonoBehaviour
    {
        protected static AppService AppService => AppService.Instance;

        protected virtual void Awake()
        {
            AppService.AwakeState = true;
        }
        protected virtual void Start()
        {
            AppService.StartState = true;
        }
    }
}

