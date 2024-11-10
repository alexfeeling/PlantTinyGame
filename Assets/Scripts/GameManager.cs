using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AniYa.UI;
using Newtonsoft.Json;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;
using TMPro;
using Febucci.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace AniYa
{
    public class GameManager : MonoBehaviour
    {
        public enum GamePhaseEnum
        {
            Breeding, //育种
            SelectSeedlings, //选苗
            Transplant, //移栽
            Manuring, //施肥
            Watering, //浇水
            Disinsection, //除草除虫
            Harvest, //收获
        }

        public Camera OperationCamera;
        public Camera ScreenCamera;

        public Animator ScreenAC;

        public GamePhaseEnum GamePhase;

        public GameData UsingGameData;

        public BaseOperationUI breedingUI;
        public BaseOperationUI selectSeedlingsUI;
        public BaseOperationUI transplantUI;
        public BaseOperationUI manuringUI;
        public BaseOperationUI wateringUI;
        public BaseOperationUI disinsectionUI;
        public BaseOperationUI harvestUI;
        public LoginUI loginUI;
        public LoginScreenUI loginScreenUI;

        public GameObject talkUI;
        public TextMeshProUGUI talkText;
        public TextAnimator_TMP talkTA;

        public GameObject panelStart;

        public GameObject BoyAnimation;
        public GameObject GirlAnimation;

        public PlantUnit[] plantUnits;

        public GameObject RainEffect;

        public BaseOperationUI GetOperationUI(GamePhaseEnum phase)
        {
            switch (phase)
            {
                case GamePhaseEnum.Breeding:
                    return breedingUI;
                case GamePhaseEnum.SelectSeedlings:
                    return selectSeedlingsUI;
                case GamePhaseEnum.Transplant:
                    return transplantUI;
                case GamePhaseEnum.Manuring:
                    return manuringUI;
                case GamePhaseEnum.Watering:
                    return wateringUI;
                case GamePhaseEnum.Disinsection:
                    return disinsectionUI;
                case GamePhaseEnum.Harvest:
                    return harvestUI;
                default:
                    break;
            }

            return null;
        }

        public static GameManager Instance;

        public static string CacheStartParam;

        public void PlayBoyAnimation()
        {
            BoyAnimation.SetActive(true);
            GirlAnimation.SetActive(false);
        }

        public void PlayGirlAnimation()
        {
            BoyAnimation.SetActive(false);
            GirlAnimation.SetActive(true);
        }

        public void StopRoleAnimation()
        {
            BoyAnimation.SetActive(false);
            GirlAnimation.SetActive(false);
        }

        public void PlayScreenCameraMoveForward()
        {
            ScreenAC.SetInteger("MoveType", 1);
        }

        public void PlayScreenCameraMoveToPlant()
        {
            ScreenAC.SetInteger("MoveType", 2);
        }

        public void RefreshUnitPlant(int plantIdx = -1, bool isGrowth = true)
        {
            if (plantIdx < 0)
                plantIdx = UsingGameData.plantIndex;
            for (int i = 0; i < plantUnits.Length; i++)
            {
                var pu = plantUnits[i];
                pu.ShowPlant(plantIdx, isGrowth);
                pu.StopManuringAnimation();
                pu.StopWateringAnimation();
                pu.StopShakeRotate();
            }
        }

        public void OnClickQuit()
        {
            Application.Quit();
        }

        private void Start()
        {
            Instance = this;

            UsingGameData = new GameData();

            UsingGameData.playerId = "test";

            ShowEnter();

            for (int i = 0; i < Display.displays.Length && i < 2; i++)
            {
                Display.displays[i].Activate();
            }

            StopRoleAnimation();

            Screen.fullScreen = false;

            if (CacheStartParam != null)
            {
                switch (CacheStartParam)
                {
                    case "ScnSeedBreeding":
                    case "ScnReturnToEarth":
                    case "Breeding":
                        StartBreeding();
                        break;
                }

                CacheStartParam = null;
            }
        }

        public void ShowEnter()
        {
            breedingUI.gameObject.SetActive(false);
            selectSeedlingsUI.gameObject.SetActive(false);
            transplantUI.gameObject.SetActive(false);
            manuringUI.gameObject.SetActive(false);
            wateringUI.gameObject.SetActive(false);
            disinsectionUI.gameObject.SetActive(false);
            harvestUI.gameObject.SetActive(false);
            panelStart.gameObject.SetActive(true);
            loginUI.gameObject.SetActive(false);
            loginScreenUI.gameObject.SetActive(false);
        }

        public void ShowLogin()
        {
            breedingUI.gameObject.SetActive(false);
            selectSeedlingsUI.gameObject.SetActive(false);
            transplantUI.gameObject.SetActive(false);
            manuringUI.gameObject.SetActive(false);
            wateringUI.gameObject.SetActive(false);
            disinsectionUI.gameObject.SetActive(false);
            harvestUI.gameObject.SetActive(false);
            panelStart.gameObject.SetActive(false);

            loginUI.ShowStart();
            loginScreenUI.Show();
        }

        public void ShowGamePlay()
        {
            loginUI.gameObject.SetActive(false);
            loginScreenUI.gameObject.SetActive(false);

            GetOperationUI(GamePhase).ShowStart();
        }

        public void PlayRain()
        {
            RainEffect.SetActive(true);
        }

        public void StopRain()
        {
            RainEffect.SetActive(false);
        }

        public void StartBreeding()
        {
            panelStart.SetActive(false);

            GamePhase = GamePhaseEnum.Breeding;

            ShowLogin();
        }

        public void StartSelectSeedlings()
        {
            panelStart.SetActive(false);
            GamePhase = GamePhaseEnum.SelectSeedlings;
            ShowLogin();
        }

        public void StartTransplant()
        {
            panelStart.SetActive(false);
            GamePhase = GamePhaseEnum.Transplant;
            ShowLogin();
        }

        public void StartManuring()
        {
            panelStart.SetActive(false);
            GamePhase = GamePhaseEnum.Manuring;
            ShowLogin();
        }

        public void StartWatering()
        {
            panelStart.SetActive(false);
            GamePhase = GamePhaseEnum.Watering;
            ShowLogin();
        }

        public void StartDisinsect()
        {
            panelStart.SetActive(false);
            GamePhase = GamePhaseEnum.Disinsection;
            ShowLogin();
        }

        public void StartHarvest()
        {
            panelStart.SetActive(false);
            GamePhase = GamePhaseEnum.Harvest;
            ShowLogin();
        }

        public void ShowUnitEmpty()
        {
            for (int i = 0; i < plantUnits.Length; i++)
            {
                var pu = plantUnits[i];
                pu.ShowEmpty();
            }
        }

        public void ShowUnitRock(int index)
        {
            var pu = plantUnits[index];
            pu.ShowRock();
        }

        public void ShowUnitHoed(int index)
        {
            var pu = plantUnits[index];
            pu.ShowHoed();
        }

        public void ShowUnitHoedUncover(int index)
        {
            var pu = plantUnits[index];
            pu.ShowHoedUncover();
        }

        public void ShowUnitFruit(int index, int fruitId)
        {
            var pu = plantUnits[index];
            pu.ShowFruit(fruitId);
        }

        public void ShowUnitPlants(bool isGrowth = true)
        {
            var plantId = UsingGameData.plantIndex;
            for (int i = 0; i < plantUnits.Length; i++)
            {
                var pu = plantUnits[i];
                pu.ShowPlant(plantId, isGrowth);
            }
        }

        public void ShowUnitPlant(int index, bool isGrowth = true)
        {
            var plantId = UsingGameData.plantIndex;
            var pu = plantUnits[index];
            pu.ShowPlant(plantId, isGrowth);
        }

        public void ShowUnitPlantUncover(int index)
        {
            var plantId = UsingGameData.plantIndex;
            var pu = plantUnits[index];
            pu.ShowPlantUncover(plantId);
        }

        public void ShowUnitInsect(int index)
        {
            var pu = plantUnits[index];
            pu.ShowUnitInsect();
        }

        public void ShowUnitWeed(int index)
        {
            var pu = plantUnits[index];
            pu.ShowUnitWeed();
        }

        public void ClearUnitInsect(int index)
        {
            var pu = plantUnits[index];
            pu.ClearUnitInsect();
        }

        public void ClearUnitWeed(int index)
        {
            var pu = plantUnits[index];
            pu.ClearUnitWeed();
        }

        public void ShowTalk(string text)
        {
            talkUI.SetActive(true);
            talkText.text = text;
        }

        public void CloseTalk()
        {
            talkUI.SetActive(false);
            StopRoleAnimation();
        }

        public void SaveGame()
        {
            if (UsingGameData.loginType == 1)
                StartCoroutine(CorSave_IDCard());
            else if (UsingGameData.loginType == 2)
                StartCoroutine(CorSave_Wechat());
        }

        private IEnumerator CorSave_IDCard()
        {
            var gameData = UsingGameData;
            gameData.gameId = (int)GamePhase + 1;
            var gameId = 1; // (int)GamePhase + 1;
            var score = gameData.GetScore(GamePhase);
            var url = $"http://124.222.207.138:8888/Game/IDCard/Finish?UserId={gameData.playerId}&GameId={gameId}";
            var gameDataJson = JsonConvert.SerializeObject(gameData, Formatting.None);
            Debug.Log(gameDataJson);

            var sendJObj = new JObject();
            sendJObj.Add("GameSave", gameDataJson);
            sendJObj.Add("RankPoints", score);
            //var formData = new WWWForm();
            //formData.AddField("GameSave", gameDataJson);
            //formData.AddField("RankPoints", score);
            //var www = UnityWebRequest.Post(url, formData);
            var www = UnityWebRequest.Post(url, JsonConvert.SerializeObject(sendJObj), "application/json");
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                var result = www.downloadHandler.text;
                Debug.LogError("WWW Error:" + result);

                var jobj = JsonConvert.DeserializeObject(result);
                Debug.Log(jobj);
            }
            else if (www.isDone)
            {
                //var result = www.downloadHandler.text;
                //Debug.Log("WWW Result:" + result);
                //var jobj = JsonConvert.DeserializeObject(result);
                //Debug.Log(jobj);
                Debug.Log("保存成功");
            }
        }

        private IEnumerator CorSave_Wechat()
        {
            var gameData = UsingGameData;
            var gameId = (int)GamePhase + 1;
            var score = gameData.GetScore(GamePhase);
            var url = $"https://youxi.hljcsdz.cn/call/game/reportIntegral";

            int timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            var dic = new Dictionary<string, string>
            {
                { "timestamp", timestamp.ToString() },
                { "user_id", gameData.playerId },
                { "game_id", gameId.ToString() },
                { "integral", score.ToString() }
            };

            var sign = SdkSigner.CreateSdkSign(dic);
            dic.Add("sign", sign);

            var www = UnityWebRequest.Post(url, dic);
            www.SetRequestHeader("server", "1");
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                var result = www.downloadHandler.text;
                Debug.LogError("WWW Error:" + result);

                var jobj = JsonConvert.DeserializeObject(result);
                Debug.Log(jobj);
            }
            else if (www.isDone)
            {
                var result = www.downloadHandler.text;
                Debug.Log("WWW Result:" + result);
                //var jobj = JsonConvert.DeserializeObject(result);
                //Debug.Log(jobj);
                Debug.Log("保存成功");
            }
        }

        // 打开特别游戏
        public void OpenSpecialGame()
        {
            StartCoroutine(CorOpenSpecialGame());
        }

        private IEnumerator CorOpenSpecialGame()
        {
            var sceneName = GamePhase switch
            {
                GamePhaseEnum.Breeding => "ReturnToEarth",//"SeedBreeding",
                GamePhaseEnum.SelectSeedlings => "",
                GamePhaseEnum.Transplant => "",
                GamePhaseEnum.Manuring => "",
                GamePhaseEnum.Watering => "",
                GamePhaseEnum.Disinsection => "",
                GamePhaseEnum.Harvest => "",
                _ => string.Empty
            };
            AsyncOperationHandle<SceneInstance>? asyncOperation = Addressables.LoadSceneAsync(sceneName);
            yield return asyncOperation;
        }

        public void OpenGameForBreedingNormal()
        {
            StartCoroutine(CorOpenGameForBreedingNormal());
        }

        private IEnumerator CorOpenGameForBreedingNormal()
        {
            AsyncOperationHandle<SceneInstance>? asyncOperation = Addressables.LoadSceneAsync("SeedBreeding");
            yield return asyncOperation;
        }

        #region 播放音效

        [SerializeField] private AudioSource _audioSrc;
        [SerializeField] private AudioClip _buttonSound;
        [SerializeField] private AudioClip _touchSound;
        [SerializeField] private AudioClip _errorSound;
        [SerializeField] private AudioClip _correctSound;

        public void PlayButtonSound()
        {
            _audioSrc.clip = _buttonSound;
            _audioSrc.Play();
        }

        public void PlayTouchSound()
        {
            _audioSrc.clip = _touchSound;
            _audioSrc.Play();
        }

        public void PlayErrorSound()
        {
            _audioSrc.clip = _errorSound;
            _audioSrc.Play();
        }

        public void PlayCorrectSound()
        {
            _audioSrc.clip = _correctSound;
            _audioSrc.Play();
        }

        #endregion

        public static string GetPhaseName(int phase)
        {
            return GetPhaseName((GamePhaseEnum)(phase - 1));
        }

        public static string GetPhaseName(GamePhaseEnum phase)
        {
            switch (phase)
            {
                case GamePhaseEnum.Breeding:
                    return "育种";
                case GamePhaseEnum.SelectSeedlings:
                    return "选苗";
                case GamePhaseEnum.Transplant:
                    return "移栽";
                case GamePhaseEnum.Manuring:
                    return "施肥";
                case GamePhaseEnum.Watering:
                    return "浇水";
                case GamePhaseEnum.Disinsection:
                    return "除虫除草";
                case GamePhaseEnum.Harvest:
                    return "收获";
                default:
                    return string.Empty;
            }
        }
    }
}