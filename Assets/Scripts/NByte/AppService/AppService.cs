using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace NByte
{
    public partial class AppService : MonoBehaviour
    {
        public static AppService Instance { get; private set; }

        private static bool BootStateOnAwake { get; set; }
        public static void BootOnAwake()
        {
            if (!BootStateOnAwake)
            {
                BootStateOnAwake = true;
                GameObject obj = Addressables.InstantiateAsync("AppService").WaitForCompletion();
                Instance = obj.GetComponent<AppService>();
                // DontDestroyOnLoad(obj);
                Instance.AwakeProcess();
            }
        }

        private void OnDestroy()
        {
            Instance = null;
            BootStateOnAwake = false;
        }

        private static bool BootStateOnStart { get; set; }
        public static void BootOnStart()
        {
            if (!BootStateOnStart)
            {
                BootStateOnStart = true;
                Instance.StartProcess();
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