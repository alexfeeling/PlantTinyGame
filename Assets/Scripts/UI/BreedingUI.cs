using LibVLCSharp;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AniYa.UI
{

    /// <summary>
    /// 育种UI
    /// 选择作物 - 选择育种环境 - 育种动画
    /// </summary>
    public class BreedingUI : BaseOperationUI
    {

        //选择地块
        public GameObject SelectBlockUI;
        //选择作物
        public GameObject SelectPlantUI;
        //选择育种环境
        public GameObject SelectEnviromentUI;
        //育种动画操作
        public GameObject BreedingOperation;

        public static LibVLC libVLC;
        public MediaPlayer mediaPlayer;
        public string mediaPath;
        public RenderTexture mediaRT;
        public Texture2D _vlcTexture;

        public Text scoreText;

        //育种视频
        public GameObject BreedingVideoObj;

        public Image CurBlockImg;

        private void Awake()
        {

            if (libVLC == null)
                CreateLibVLC();
        }

        void CreateLibVLC()
        {
            //Log("VLCPlayerExample CreateLibVLC");
            //Dispose of the old libVLC if necessary
            if (libVLC != null)
            {
                libVLC.Dispose();
                libVLC = null;
            }

            Core.Initialize(Application.dataPath); //Load VLC dlls
            libVLC = new LibVLC(enableDebugLogs: true); //You can customize LibVLC with advanced CLI options here https://wiki.videolan.org/VLC_command-line_help/

            //Setup Error Logging
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            //libVLC.Log += (s, e) =>
            //{
            //    //Always use try/catch in LibVLC events.
            //    //LibVLC can freeze Unity if an exception goes unhandled inside an event handler.
            //    try
            //    {
            //        if (logToConsole)
            //        {
            //            Log(e.FormattedLog);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Log("Exception caught in libVLC.Log: \n" + ex.ToString());
            //    }

            //};
        }

        public override void StartPlay()
        {
            base.StartPlay();

            if (mediaPlayer == null)
            {
                mediaPlayer = new MediaPlayer(libVLC);
                var path = System.IO.Path.Combine(Application.streamingAssetsPath, mediaPath);
                var trimmedPath = path.Trim(new char[] { '"' });//Windows likes to copy paths with quotes but Uri does not like to open them
                mediaPlayer.Media = new Media(new System.Uri(trimmedPath));
                //mediaPlayer.Media = new Media(mediaPath);
            }

            //StartSelectBlock();
            StartSelectPlant();

            BreedingVideoObj.SetActive(false);

            var punits = GameManager.Instance.plantUnits;
            for (int i = 0; i < punits.Length; i++)
            {
                var pu = punits[i];
                pu.StopShakeRotate();
            }
        }

        #region VLC

        private void UpdateVLC()
        {

            uint height = 0;
            uint width = 0;
            mediaPlayer.Size(0, ref width, ref height);

            //Update the vlc texture (tex)
            var texptr = mediaPlayer.GetTexture(width, height, out bool updated);
            if (texptr == System.IntPtr.Zero) return;

            try
            {
                if (_vlcTexture == null)
                {
                    _vlcTexture = Texture2D.CreateExternalTexture((int)width, (int)height, TextureFormat.RGBA32, false, true, texptr);
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("texptr:" + texptr);
                Debug.LogException(e);
            }

            if (_vlcTexture != null && updated)
            {
                _vlcTexture.UpdateExternalTexture(texptr);
                Graphics.Blit(_vlcTexture, mediaRT, Vector2.one * -1, Vector2.zero);
            }

        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (mediaPlayer != null)
                mediaPlayer.Stop();
        }

        #endregion


        #region 选择地块
        public void StartSelectBlock()
        {
            SelectBlockUI.SetActive(true);
            SelectPlantUI.SetActive(false);
            SelectEnviromentUI.SetActive(false);
            BreedingOperation.SetActive(false);

            blockIndex = 0;
            RefreshBlockShow();

            GameManager.Instance.PlayButtonSound();
        }

        private void RefreshBlockShow()
        {

        }

        private int blockIndex;
        private static int MaxBlock = 5;
        public void OnClickSwitchBlock(bool isNext)
        {
            if (isNext)
            {
                blockIndex++;
                if (blockIndex >= MaxBlock)
                    blockIndex = 0;
            }
            else
            {
                blockIndex--;
                if (blockIndex < 0)
                    blockIndex = MaxBlock - 1;
            }

            GameManager.Instance.PlayButtonSound();
            RefreshBlockShow();
        }

        public void OnClickConfirmBlock()
        {
            GameManager.Instance.UsingGameData.blockIndex = blockIndex;
            StartSelectPlant();

            GameManager.Instance.PlayButtonSound();
        }

        #endregion

        #region 选择作物

        private static string[] plantIdArr = new[] {
            "sugarcane",
            "tomato",
            "strawberry",
            "cucumber",
            "blueberry"
        };
        private int plantIdx;
        public void StartSelectPlant()
        {
            SelectBlockUI.SetActive(false);
            SelectPlantUI.SetActive(true);
            SelectEnviromentUI.SetActive(false);
            BreedingOperation.SetActive(false);

            plantIdx = 1;
            GameManager.Instance.UsingGameData.plantIndex = plantIdx;
            GameManager.Instance.RefreshUnitPlant(plantIdx);

            GameManager.Instance.PlayButtonSound();
        }

        public void SetPlantIndex(int index)
        {
            plantIdx = index;
            GameManager.Instance.PlayTouchSound();
            GameManager.Instance.RefreshUnitPlant(plantIdx);
        }

        public void OnClickPlantConfirm()
        {
            GameManager.Instance.PlayButtonSound();

            GameManager.Instance.UsingGameData.plantIndex = plantIdx;
            StartSelectEnviroment();
        }

        #endregion

        #region 选择育种环境

        private int envirIndex;

        public void StartSelectEnviroment()
        {
            SelectBlockUI.SetActive(false);
            SelectPlantUI.SetActive(false);
            SelectEnviromentUI.SetActive(true);
            BreedingOperation.SetActive(false);

            envirIndex = 0;
        }

        public void SetEnviromentIndex(int index)
        {
            envirIndex = index;

            GameManager.Instance.PlayTouchSound();
        }

        public void OnClickEnviromentConfirm()
        {
            GameManager.Instance.UsingGameData.envirIndex = envirIndex;
            GameManager.Instance.PlayButtonSound();

            StartBreedingOperation();
        }

        #endregion

        #region 育种动画与操作

        public void StartBreedingOperation()
        {
            SelectBlockUI.SetActive(false);
            SelectPlantUI.SetActive(false);
            SelectEnviromentUI.SetActive(false);
            BreedingOperation.SetActive(true);

            StartCoroutine(CorPlayBreedingOperation());

        }

        private IEnumerator CorPlayBreedingOperation()
        {
            isPlayingAnimation = true;

            BreedingVideoObj.SetActive(true);

            mediaPlayer.Play();

            yield return new WaitForSecondsRealtime(5);

            //var vp = BreedingVideoObj.GetComponent<VideoPlayer>();
            //vp.Prepare();
            //while (vp.isPlaying)
            //    yield return null;

            while (mediaPlayer.IsPlaying)
                yield return null;

            BreedingVideoObj.SetActive(false);

            isPlayingAnimation = false;

            Finish();
        }

        protected override void Update()
        {
            base.Update();
            if (mediaPlayer != null && mediaPlayer.IsPlaying)
                UpdateVLC();
        }

        public override void Finish(bool quit = false)
        {
            base.Finish(quit);
            if (!quit)
            {
                var score = 60;

                GameManager.Instance.UsingGameData.scoreBreeding = score;

                scoreText.text = score.ToString();

                GameManager.Instance.SaveGame();
            }
        }

        public void OnClickBreeding()
        {
            GameManager.Instance.PlayButtonSound();

        }

        #endregion
    }

}