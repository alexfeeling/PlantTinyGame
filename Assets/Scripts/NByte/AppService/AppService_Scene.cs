using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace NByte
{
    public partial class AppService
    {
        public IEnumerator LoadStartScene(AniYa.GameManager.GamePhaseEnum gamePhase)
        {
            yield return ShowCurtain();
            yield return SceneManager.LoadSceneAsync("StartScene");
            GamePhaseHandler(gamePhase);
            yield return HideCurtain();
        }
        private void GamePhaseHandler(AniYa.GameManager.GamePhaseEnum gamePhase)
        {
            //todo complete the handler
            //GameManager gameManager = FindObjectOfType<GameManager>();
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