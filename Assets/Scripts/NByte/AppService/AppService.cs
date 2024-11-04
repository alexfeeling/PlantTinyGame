using UnityEngine;
using UnityEngine.AddressableAssets;

namespace NByte
{
    public partial class AppService : MonoBehaviour
    {
        public static AppService Instance { get; private set; }

        private static bool awakeState;
        public static bool AwakeState
        {
            get => awakeState;
            set
            {
                if (awakeState != value)
                {
                    awakeState = value;
                    if (awakeState)
                    {
                        GameObject obj = Addressables.InstantiateAsync("AppService").WaitForCompletion();
                        Instance = obj.GetComponent<AppService>();
                        DontDestroyOnLoad(obj);
                        Instance.AwakeProcess();
                    }
                }
            }
        }

        private static bool startState;
        public static bool StartState
        {
            get => startState;
            set
            {
                if (startState != value)
                {
                    startState = value;
                    if (startState)
                    {
                        Instance.StartProcess();
                    }
                }
            }
        }

        public void AwakeProcess()
        {
            AwakeOfGameData();
        }
        public void StartProcess()
        {

        }
    }
}