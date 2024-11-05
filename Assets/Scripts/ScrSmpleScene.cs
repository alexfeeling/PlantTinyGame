using NByte;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class ScrSmpleScene : ScrSceneBase
{
    [SerializeField] private AssetReference ScnReturnToEarth;
    [SerializeField] private AssetReference ScnSeedBreeding;
    [SerializeField] private AssetReference ScnJumpAndGrow;
    [SerializeField] private AssetReference ScnTransplanting;

    public void LoadScnReturnToEarth() => LoadScene(Scenes.ReturnToEarth);
    public void LoadScnSeedBreeding() => LoadScene(Scenes.SeedBreeding);
    public void LoadScnJumpAndGrow() => LoadScene(Scenes.JumpAndGrow);
    public void LoadScnTransplanting() => LoadScene(Scenes.Transplanting);
    private void LoadScene(Scenes scene)
    {
        StartCoroutine(Steps());

        IEnumerator Steps()
        {
            yield return AppService.ShowCurtain();
            AsyncOperationHandle<SceneInstance>? asyncOperation = scene switch
            {
                Scenes.ReturnToEarth => ScnReturnToEarth.LoadSceneAsync(),
                Scenes.SeedBreeding => Addressables.LoadSceneAsync("SeedBreeding"),
                Scenes.JumpAndGrow => ScnJumpAndGrow.LoadSceneAsync(),
                Scenes.Transplanting => ScnTransplanting.LoadSceneAsync(),
                _ => null,
            };
            yield return asyncOperation;
        }
    }

    protected override void Start()
    {
        base.Start();
        AppService.HideCurtain();
    }

    public enum Scenes
    {
        ReturnToEarth,
        SeedBreeding,
        JumpAndGrow,
        Transplanting,
    }
}
