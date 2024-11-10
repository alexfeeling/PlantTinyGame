using System.Collections;
using AniYa;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace NByte
{
    public partial class AppService
    {
        public IEnumerator LoadStartScene(AniYa.GameManager.GamePhaseEnum gamePhase)
        {
            yield return ShowCurtain();
            GamePhaseHandler(gamePhase);
            yield return SceneManager.LoadSceneAsync("StartScene");
            // yield return HideCurtain();
        }
        private void GamePhaseHandler(AniYa.GameManager.GamePhaseEnum gamePhase)
        {
            // Debug.Log("LoadStartScene: " + gamePhase.ToString());
            GameManager.CacheStartParam = gamePhase.ToString();
            SceneManager.LoadScene("StartScene", LoadSceneMode.Single);
        }

        public IEnumerator LoadScnReturnToEarth()
        {
            yield return LoadScene("ReturnToEarth");
        }
        public IEnumerator LoadScnSeedBreeding()
        {
            yield return LoadScene("SeedBreeding");
        }
        public IEnumerator LoadScnJumpAndGrow()
        {
            yield return LoadScene("JumpAndGrow");
        }
        public IEnumerator LoadScnTransplanting()
        {
            yield return LoadScene("Transplanting");
        }
        private IEnumerator LoadScene(string key)
        {
            yield return ShowCurtain();
            yield return Addressables.LoadSceneAsync(key);
        }

        public IEnumerator LoadSampleScene()
        {
            yield return ShowCurtain();
            yield return SceneManager.LoadSceneAsync("SampleScene");
        }
    }
}