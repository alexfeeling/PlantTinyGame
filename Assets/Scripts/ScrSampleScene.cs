using NByte;
using UnityEngine;

public class ScrSampleScene : MonoBehaviour
{
    private AppService AppService => AppService.Instance;

    public void LoadScnReturnToEarth()
    {
        StartCoroutine(AppService.LoadScnReturnToEarth());
    }
    public void LoadScnSeedBreeding()
    {
        StartCoroutine(AppService.LoadScnSeedBreeding());
    }
    public void LoadScnJumpAndGrow()
    {
        StartCoroutine(AppService.LoadScnJumpAndGrow());
    }
    public void LoadScnTransplanting()
    {
        StartCoroutine(AppService.LoadScnTransplanting());
    }

    private void Awake()
    {
        AppService.BootOnAwake();
    }
    private void Start()
    {
        AppService.BootOnStart();
        AppService.HideCurtain();
    }
}
