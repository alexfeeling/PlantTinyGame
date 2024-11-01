using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AniYa.UI
{

    

    /// <summary>
    /// 选苗UI
    /// </summary>
    public class SelectSeedlingsUI : BaseOperationUI
    {

        public Image CurSeedling;
        public Text scoreText;
        public Text timeText;
        public Text noticeText;

        public Sprite[] NormalSp;
        public Sprite[] WeakSp;
        public Sprite[] WitheredSp;

        public GameObject SelectPlantUI;

        public GameObject PlantParentObj;

        public Seedling[] SeedlingObjs;

        //public Image[] SeedlingImgs;

        public CanvasGroup ErrorCG;

        public Image ErrorBG;
        public Image CorrectBG;

        private enum SeedlingTypeEnum
        {
            Normal,
            Weak,
            Withered
        }

        private SeedlingTypeEnum CurSeedlikngType;

        private int score;

        private int maxScore = 60 / 5;

        private float playTime;
        private float totalTime = 60;

        private void GenerateSeedling()
        {
            var plantIndex = GameManager.Instance.UsingGameData.plantIndex;

            CurSeedlikngType = (SeedlingTypeEnum)Random.Range(0, 3);
            switch (CurSeedlikngType)
            {
                case SeedlingTypeEnum.Normal:
                    CurSeedling.sprite = NormalSp[plantIndex];
                    break;
                case SeedlingTypeEnum.Weak:
                    CurSeedling.sprite = WeakSp[plantIndex];
                    break;
                case SeedlingTypeEnum.Withered:
                    CurSeedling.sprite = WitheredSp[plantIndex];
                    break;
                default:
                    break;
            }

            CurSeedling.SetNativeSize();

            CurSeedling.transform.localScale = Vector3.one;
            CurSeedling.transform.DOKill();
            CurSeedling.transform.DOPunchScale(new Vector3(0.25f, 0.25f, 0.25f), 0.6f, elasticity: 0.2f);
        }

        private void ShowNotice()
        {
            StopCoroutine("CorShowError");

            StartCoroutine("CorShowError");
        }

        private IEnumerator CorShowError()
        {
            ErrorCG.gameObject.SetActive(true);
            ErrorCG.DOKill();
            ErrorCG.alpha = 0f;

            ErrorCG.transform.DOKill();
            ErrorCG.transform.localScale = Vector3.one * 0.4f;

            yield return ErrorCG.transform.DOScale(1, 0.4f).SetEase(Ease.OutElastic, 1.2f);
            ErrorCG.DOFade(1f, 0.2f).WaitForCompletion();

            yield return ErrorCG.DOFade(0f, 1).SetEase(Ease.InCubic).SetDelay(0.5f).WaitForCompletion();

            ErrorCG.gameObject.SetActive(false);
        }

        #region 选择植物
        public bool UsingSelectPlant = true;
        private int plantIdx;
        public void StartSelectPlant()
        {

            PanelStart.SetActive(false);
            PanelPlay.SetActive(false);
            PanelFinish.SetActive(false);
            ScreenUI.PanelStart.SetActive(false);
            ScreenUI.PanelPlay.SetActive(true);
            ScreenUI.PanelFinish.SetActive(false);

            GameManager.Instance.StopRoleAnimation();

            SelectPlantUI.SetActive(true);

            plantIdx = 2;
            GameManager.Instance.UsingGameData.plantIndex = plantIdx;
            GameManager.Instance.RefreshUnitPlant(plantIdx);
        }

        public void SetPlantIndex(int index)
        {
            plantIdx = index;
            GameManager.Instance.RefreshUnitPlant(plantIdx);
        }

        public void OnClickPlantConfirm()
        {
            GameManager.Instance.UsingGameData.plantIndex = plantIdx;

            StartPlay();
        }

        #endregion


        public override void StartPlay()
        {
            base.StartPlay();

            isPlayingAnimation = false;

            playTime = totalTime;
            score = 0;
            _normalIndex = 0;
            GenerateSeedling();
            //noticeText.gameObject.SetActive(false);

            PlantParentObj.SetActive(true);
            for (int i = 0; i < SeedlingObjs.Length; i++)
            {
                SeedlingObjs[i].gameObject.SetActive(false);
                SeedlingObjs[i].Refresh();
            }

            var punits = GameManager.Instance.plantUnits;
            for (int i = 0; i < punits.Length; i++)
            {
                var pu = punits[i];
                pu.StopShakeRotate();
            }

            /*for (int i = 0; i < SeedlingImgs.Length; i++)
            {
                SeedlingImgs[i].gameObject.SetActive(false);
            }*/

            GameManager.Instance.ShowUnitEmpty();

            StopCoroutine("CorShowTalk");
            StartCoroutine("CorShowTalk");
        }


        private IEnumerator CorShowTalk()
        {
            ScreenUI.PlayRoleAnimation();
            var talkText = "天民田园在自动育苗棚室育苗之后优先选取生长茁壮的种苗，进行选苗是种植作物时非常重要的一步，选用优质的种苗直接影响着作物后期的生长发育和产量。";
            GameManager.Instance.ShowTalk(talkText);

            yield return new WaitForSeconds(20f);

            talkText = "加油，记得选择外形完整，无明显畸形或伤口的种苗哦";
            GameManager.Instance.ShowTalk(talkText);

            yield return new WaitForSeconds(20f);

            talkText = "健康的种苗，会看起来鲜绿丰满，富有生命的活力";
            GameManager.Instance.ShowTalk(talkText);

        }

        /*
                private string _infoText = @"剩余时间

        筛查总苗数   {0}
        优选种苗数   {1}
        筛查速度    {2}
        筛查精度    {3}
        当前评价    {4}";

                private int _operationCorrectCnt;
                private int _operationTotalCnt;
                private float _refreshInfoTime;

                private void RefreshInfo()
                {
                    _refreshInfoTime += Time.deltaTime;
                    if (_refreshInfoTime < 0.1f)
                    {
                        return;
                    }

                    var hoedCnt = 0;
                    var coveredCnt = 0;
                    for (int i = 0; i < blockDatas.Length; i++)
                    {
                        var bd = blockDatas[i];
                        if (bd.phase >= BlockPhase.Hoed)
                        {
                            hoedCnt++;
                        }
                        if (bd.phase >= BlockPhase.Coverd)
                        {
                            coveredCnt++;
                        }
                    }

                    var evaluation = string.Empty;
                    var evaRate = 0f;
                    if (_operationTotalCnt <= 0)
                        evaRate = 0.5f;
                    else
                        evaRate = 1f * _operationCorrectCnt / _operationTotalCnt;
                    if (evaRate >= 0.666f)
                        evaluation = "优";
                    else if (evaRate >= 0.333f)
                        evaluation = "良";
                    else
                        evaluation = "差";

                    ScreenUI.InfoPanelText.text = string.Format(_infoText,
                        100f * hoedCnt / blockDatas.Length,
                        100f * coveredCnt / blockDatas.Length, evaluation);
                }
        */
        protected override void Update()
        {
            base.Update();
            if (isPlaying)
            {
                playTime -= Time.deltaTime;
                timeText.text = $"{playTime:0}秒";


                ScreenUI.TimeSlider.value = playTime / totalTime;

                //RefreshInfo();

                if (playTime <= 0)
                {
                    Finish();
                }
            }
        }

        //点击开始游戏
        public void OnClickStart()
        {
            GameManager.Instance.PlayButtonSound();

            if (UsingSelectPlant)
                StartSelectPlant();
            else
                StartPlay();
        }


        public void OnClickPick()
        {
            if (isPlayingAnimation) return;
            GameManager.Instance.PlayButtonSound();

            //播放选择动画，然后生成下一个
            StartCoroutine(CorPlayPick());
        }

        private int _normalIndex;
        private IEnumerator CorPlayPick()
        {
            isPlayingAnimation = true;
            yield return null;

            //var sp = NormalSp[GameManager.Instance.UsingGameData.plantIndex];
            if (CurSeedlikngType == SeedlingTypeEnum.Normal)
            {
                if (_normalIndex < SeedlingObjs.Length)
                {
                    var seedImg = SeedlingObjs[_normalIndex];
                    seedImg.gameObject.SetActive(true);
                    //seedImg.sprite = sp;
                    seedImg.transform.localScale = Vector3.one * 0.25f;
                    seedImg.transform.DOScale(1, 1.4f).SetEase(Ease.OutElastic, 1.2f);
                }

                _normalIndex++;
                score++;
            }
            else
            {
            }

            ShowNoticeText(true);

            CurSeedling.transform.DOKill();
            yield return CurSeedling.transform.DOScale(0, 1f).SetEase(Ease.InBack).SetDelay(0.5f).WaitForCompletion();

            isPlayingAnimation = false;
            GenerateSeedling();
        }

        public void OnClickAbandon()
        {
            if (isPlayingAnimation) return;
            GameManager.Instance.PlayButtonSound();

            //播放放弃动画，然后生成下一个
            StartCoroutine(CorPlayAbandon());
        }

        private void ShowNoticeText(bool isPick)
        {

            ShowNotice();
            //noticeText.gameObject.SetActive(true);
            if (CurSeedlikngType == SeedlingTypeEnum.Normal)
            {
                if (isPick)
                {
                    noticeText.text = "健康的幼苗 +1";
                    ErrorBG.gameObject.SetActive(false);
                    CorrectBG.gameObject.SetActive(true);

                    GameManager.Instance.PlayCorrectSound();
                }
                else
                {
                    noticeText.text = "健康的幼苗，不要错过了";
                    ErrorBG.gameObject.SetActive(true);
                    CorrectBG.gameObject.SetActive(false);

                    GameManager.Instance.PlayErrorSound();
                }
            }
            else if (CurSeedlikngType == SeedlingTypeEnum.Weak)
            {
                if (isPick)
                {
                    noticeText.text = "营养不良的幼苗，不可选用";
                    ErrorBG.gameObject.SetActive(true);
                    CorrectBG.gameObject.SetActive(false);

                    GameManager.Instance.PlayErrorSound();
                }
                else
                {
                    noticeText.text = "营养不良的幼苗，不可选用";
                    ErrorBG.gameObject.SetActive(false);
                    CorrectBG.gameObject.SetActive(true);
                    GameManager.Instance.PlayCorrectSound();
                }
            }
            else if (CurSeedlikngType == SeedlingTypeEnum.Withered)
            {
                if (isPick)
                {
                    noticeText.text = "枯萎的幼苗，不可选用";
                    ErrorBG.gameObject.SetActive(true);
                    CorrectBG.gameObject.SetActive(false);

                    GameManager.Instance.PlayErrorSound();
                }
                else
                {
                    noticeText.text = "枯萎的幼苗，不可选用";
                    ErrorBG.gameObject.SetActive(false);
                    CorrectBG.gameObject.SetActive(true);
                    GameManager.Instance.PlayCorrectSound();
                }
            }
            else
                noticeText.text = string.Empty;

            //noticeText.transform.localPosition = Vector3.zero;
            //noticeText.transform.DOKill();
            //noticeText.transform.DOLocalMoveY(100, 2f).SetEase(Ease.OutCubic);
            //var color = noticeText.color;
            //color.a = 1;
            //noticeText.DOKill();
            //noticeText.color = color;
            //noticeText.DOFade(0, 2).SetEase(Ease.OutCubic);
        }

        private IEnumerator CorPlayAbandon()
        {
            isPlayingAnimation = true;
            yield return null;

            if (CurSeedlikngType != SeedlingTypeEnum.Normal)
            {
                score++;
            }
            else
            {

            }

            ShowNoticeText(false);

            CurSeedling.transform.DOKill();
            yield return CurSeedling.transform.DOScale(0, 1f).SetEase(Ease.InBack).SetDelay(1).WaitForCompletion();

            isPlayingAnimation = false;
            GenerateSeedling();
        }

        public override void Finish(bool quit = false)
        {
            base.Finish(quit);

            PlantParentObj.SetActive(false);

            StopCoroutine("CorShowTalk");
            GameManager.Instance.CloseTalk();

            score = Mathf.RoundToInt(Mathf.Lerp(10, 60, 1f * score / maxScore));
            GameManager.Instance.UsingGameData.scoreSelectSeedlings = score;

            scoreText.text = score.ToString();
            GameManager.Instance.SaveGame();
        }


    }

}