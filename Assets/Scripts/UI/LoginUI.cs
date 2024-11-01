using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System;
using DG.Tweening;

namespace AniYa.UI
{


    public class LoginUI : MonoBehaviour
    {

        public GameObject PanelStart;
        public GameObject PanelIDCard;
        public GameObject PanelWechat;

        public RawImage QRCode;

        public InputField Text_IDCard;
        public UnityEngine.EventSystems.EventSystem eventSystem;

        public CanvasGroup ErrorCG;
        public Text ErrorText;


        public void ShowStart()
        {
            gameObject.SetActive(true);
            PanelStart.SetActive(true);
            PanelIDCard.SetActive(false);
            PanelWechat.SetActive(false);

            ErrorCG.gameObject.SetActive(false);
        }


        public void ShowIdCardLogin()
        {
            PanelStart.SetActive(false);
            PanelIDCard.SetActive(true);
            PanelWechat.SetActive(false);
            Text_IDCard.Select();

            GameManager.Instance.PlayButtonSound();
        }

        public void ShowWechatLogin()
        {
            PanelStart.SetActive(false);
            PanelIDCard.SetActive(false);
            PanelWechat.SetActive(true);
            StartCoroutine(CorShowWechatQRCode());

            GameManager.Instance.PlayButtonSound();
        }

        public void OnClickLogin_IDCard()
        {
            GameManager.Instance.PlayButtonSound();

            //开始登录流程
            StartCoroutine(CorLogin());
            //GameManager.Instance.ShowGamePlay();
        }

        private IEnumerator CorLogin()
        {
            var userId = Text_IDCard.text;
            Text_IDCard.text = string.Empty;
            if (string.IsNullOrEmpty(userId))
            {

                yield break;
            }
            var gameId = 1;// (int)GameManager.Instance.GamePhase + 1;
            var url = $"http://124.222.207.138:8888/Game/IDCard/SignIn?UserId={userId}&GameId={gameId}";
            Debug.Log(url);
            var www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                var result = www.downloadHandler.text;
                Debug.LogError("WWW Error:" + result);

                var jobj = JsonConvert.DeserializeObject(result);
                Debug.Log(jobj);
                //var dic = JsonUtility.FromJson<Dictionary<string, string>>(result);
                //foreach (var pair in dic)
                //{
                //    Debug.Log($"{pair.Key}:{pair.Value}");
                //}
                ShowStart();
                ErrorText.text = "登录失败，非法帐户";
                PlayError();
                yield break;
            }
            else if (www.isDone)
            {
                var result = www.downloadHandler.text;
                Debug.Log("WWW Result:" + result);
                var jobj = JsonConvert.DeserializeObject<JObject>(result);
                if (jobj.TryGetValue("GameSave", out var gameSaveJT))
                {
                    var gameSaveJson = gameSaveJT.ToString();
                    var gdObj = JsonConvert.DeserializeObject<JObject>(gameSaveJson);
                    var serverGameData = JsonConvert.DeserializeObject<GameData>(gameSaveJson);
                    if (serverGameData.playerId == userId)
                    {
                        GameManager.Instance.UsingGameData = serverGameData;
                        /*var serverGameId = serverGameData.gameId;
                        var nextGameId = serverGameId + 1;
                        if (nextGameId > 7) nextGameId = 1;
                        var thisGameId = (int)GameManager.Instance.GamePhase + 1;
                        if (thisGameId == nextGameId)
                            GameManager.Instance.UsingGameData = serverGameData;
                        else
                        {
                            ShowStart();
                            var str = $"请按顺序游玩游戏，当前玩到【{nextGameId}.{GameManager.GetPhaseName(nextGameId)}】";
                            ErrorText.text = str;
                            PlayError();
                            yield break;
                        }*/
                    }
                    else
                    {
                        ShowStart();
                        ErrorText.text = "登录失败，用户不一致";
                        PlayError();
                        yield break;
                    }
                }
                else if (GameManager.Instance.GamePhase != GameManager.GamePhaseEnum.Breeding)
                {
                    ShowStart();
                    ErrorText.text = "登录失败，没有存档数据";
                    PlayError();
                    yield break;
                }

                Debug.Log(jobj);

                GameManager.Instance.UsingGameData.playerId = userId;
                GameManager.Instance.UsingGameData.loginType = 1;
                GameManager.Instance.ShowGamePlay();
            }

        }

        private void PlayError()
        {
            StopAllCoroutines();

            StartCoroutine(CorShowError());
        }

        private IEnumerator CorShowError()
        {
            GameManager.Instance.PlayErrorSound();

            ErrorCG.gameObject.SetActive(true);
            ErrorCG.alpha = 0f;
            yield return ErrorCG.DOFade(1f, 0.2f).WaitForCompletion();

            yield return ErrorCG.DOFade(0f, 1).SetEase(Ease.InCubic).SetDelay(2).WaitForCompletion();

            ErrorCG.gameObject.SetActive(false);
        }

        public void OnClickNotLogin()
        {
            GameManager.Instance.PlayButtonSound();
            //不登录，直接玩
            GameManager.Instance.ShowGamePlay();
        }

        public void OnClickLogin_Wechat()
        {
            GameManager.Instance.PlayButtonSound();

        }

        public void OnClickClose()
        {
            GameManager.Instance.PlayButtonSound();
            GameManager.Instance.ShowEnter();
            //ShowStart();
        }

        public void OnClickStopIDCard()
        {
            StopAllCoroutines();
            ShowStart();
        }

        public void OnClickStopWechat()
        {
            StopAllCoroutines();
            QRCode.texture = null;
            ShowStart();
        }

        private IEnumerator CorShowWechatQRCode()
        {
            var formData = new WWWForm();
            formData.headers.Add("server", "1");
            int timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            Debug.Log("timestamp:" + timestamp);
            //Debug.Log("timestamp2:" + (DateTime.UtcNow.Ticks / timestamp));
            //var timestamp = System.DateTime.Now.Second;
            formData.AddField("timestamp", timestamp);
            var dic = new Dictionary<string, string>
            {
                { "timestamp", timestamp.ToString() }
            };
            var sign = SdkSigner.CreateSdkSign(dic);
            formData.AddField("sign", sign);
            Debug.Log("sign:" + sign);

            var getTexAddressReq = UnityWebRequest.Post("https://youxi.hljcsdz.cn//call/game/getLoginQrcode", formData);
            getTexAddressReq.SetRequestHeader("server", "1");
            yield return getTexAddressReq.SendWebRequest();
            if (getTexAddressReq.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("getTexAddressReq.result:" + getTexAddressReq.result);
                yield break;
            }
            else
            {
                var texAddressStr = getTexAddressReq.downloadHandler.text;
                Debug.Log("texAddressStr:" + texAddressStr);
                var jsonObj = JsonConvert.DeserializeObject<JObject>(texAddressStr);
                var qrcodeAddress = jsonObj["data"]["qrcode"].ToString();
                var uniqid = jsonObj["data"]["uniqid"].ToString();
                dic.Add("uniqid", uniqid);
                qrcodeAddress = qrcodeAddress.Replace("47.101.175.30", "youxi.hljcsdz.cn");
                Debug.Log("qrcodeAddress:" + qrcodeAddress);
                var getTexReq = UnityWebRequestTexture.GetTexture(qrcodeAddress);
                yield return getTexReq.SendWebRequest();
                if (getTexReq.result != UnityWebRequest.Result.Success)
                {
                    yield break;
                }
                else
                {
                    QRCode.texture = ((DownloadHandlerTexture)getTexReq.downloadHandler).texture;


                    //开始轮询
                    for (int i = 0; i < 30; i++)
                    {
                        yield return new WaitForSecondsRealtime(1f);


                        timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

                        var formData2 = new WWWForm();
                        formData2.headers.Add("server", "1");
                        formData2.AddField("timestamp", timestamp);
                        formData2.AddField("uniqid", uniqid);

                        dic["timestamp"] = timestamp.ToString();

                        sign = SdkSigner.CreateSdkSign(dic);
                        formData2.AddField("sign", sign);

                        var getLoginStatusReq = UnityWebRequest.Post("https://youxi.hljcsdz.cn//call/game/userIsLogin", formData2);
                        getLoginStatusReq.SetRequestHeader("server", "1");
                        yield return getLoginStatusReq.SendWebRequest();

                        if (getLoginStatusReq.result == UnityWebRequest.Result.Success)
                        {
                            var text = getLoginStatusReq.downloadHandler.text;
                            Debug.Log(text);
                            var loginJOBJ = JsonConvert.DeserializeObject<JObject>(text);
                            if (loginJOBJ.Value<int>("code") == 0)
                            {
                                var user_token = loginJOBJ["data"]["user_token"].ToString();
                                //登录成功
                                GameManager.Instance.UsingGameData.playerId = user_token;
                                GameManager.Instance.UsingGameData.loginType = 2;
                                GameManager.Instance.ShowGamePlay();

                                yield break;
                            }
                        }
                    }
                }

            }


            ShowStart();


        }


    }

    public class SdkSigner
    {
        public static string CreateSdkSign(Dictionary<string, string> parameters)
        {
            parameters.Remove("sign");
            string secret = "game@HbnbwU";
            var sortedParams = parameters.OrderBy(p => p.Key);
            string str = string.Join("&", sortedParams.Select(p => $"{p.Key}={p.Value}")) + secret;
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(str);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}