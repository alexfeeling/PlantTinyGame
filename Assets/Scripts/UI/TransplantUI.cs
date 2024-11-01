using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AniYa.UI
{

    /// <summary>
    /// 移载UI
    /// </summary>
    public class TransplantUI : BaseOperationUI
    {


        public Text scoreText;
        public Text timeText;

        public CanvasGroup ErrorCG;

        public GameObject SelectedHoe;
        public GameObject SelectedPlant;
        public GameObject SelectedCover;
        public GameObject SelectedDigRock;

        private enum BlockPhase
        {
            Empty,
            Hoed,
            RockBlock,
            Planted,
            Coverd
        }

        private class BlockData
        {
            public int index;
            public BlockPhase phase;
        }

        public enum OperationTypeEnum
        {
            None,
            Hoe,
            Plant,
            Filling,
            DigRock,
        }

        private OperationTypeEnum CurrOperationType;

        public void OnClickOperationMode(int typeInt)
        {
            var operationType = (OperationTypeEnum)typeInt;
            if (CurrOperationType == operationType)
                CurrOperationType = OperationTypeEnum.None;
            else
                CurrOperationType = operationType;
        }

        protected override void OnClickPlantUnit(int index)
        {
            if (CurrOperationType == OperationTypeEnum.None)
            {
                ShowError();
                return;
            }

            var bd = blockDatas[index];
            switch (CurrOperationType)
            {
                case OperationTypeEnum.Hoe:
                    if (bd.phase == BlockPhase.Empty)
                    {
                        PlayAnimation(index, CurrOperationType);
                        return;
                    }
                    break;
                case OperationTypeEnum.Plant:
                    if (bd.phase == BlockPhase.Hoed)
                    {
                        PlayAnimation(index, CurrOperationType);
                        return;
                    }
                    break;
                case OperationTypeEnum.Filling:
                    if (bd.phase == BlockPhase.Planted)
                    {
                        PlayAnimation(index, CurrOperationType);
                        return;
                    }
                    break;
                case OperationTypeEnum.DigRock:
                    if (bd.phase == BlockPhase.RockBlock)
                    {
                        PlayAnimation(index, CurrOperationType);
                        return;
                    }
                    break;
                default:
                    break;
            }

            _operationTotalCnt++;
            ShowError();
        }

        private void ShowError()
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
            yield return ErrorCG.DOFade(0f, 1).SetEase(Ease.InCubic).WaitForCompletion();

            ErrorCG.gameObject.SetActive(false);
        }

        private void PlayAnimation(int index, OperationTypeEnum operationType)
        {
            switch (operationType)
            {
                case OperationTypeEnum.Hoe:
                    StartCoroutine(CorPlayAnimation_Hoe(index));
                    break;
                case OperationTypeEnum.Plant:
                    StartCoroutine(CorPlayAnimation_Plant(index));
                    break;
                case OperationTypeEnum.Filling:
                    StartCoroutine(CorPlayAnimation_Filling(index));
                    break;
                case OperationTypeEnum.DigRock:
                    StartCoroutine(CorPlayAnimation_DigRock(index));
                    break;
                default:
                    return;
            }
            GameManager.Instance.plantUnits[index].PlayScaleMotion();

            _operationCorrectCnt++;
        }

        private IEnumerator CorPlayAnimation_Hoe(int index)
        {
            isPlayingAnimation = true;
            yield return null;

            var bd = blockDatas[index];
            var hasRock = Random.Range(0, 1f) < 0.3f;
            if (hasRock)
            {
                bd.phase = BlockPhase.RockBlock;
                GameManager.Instance.ShowUnitRock(index);
            }
            else
            {
                bd.phase = BlockPhase.Hoed;
                GameManager.Instance.ShowUnitHoedUncover(index);
            }

            GameManager.Instance.plantUnits[index].PlayChangeEffect();

            isPlayingAnimation = false;

        }


        private IEnumerator CorPlayAnimation_Plant(int index)
        {
            isPlayingAnimation = true;
            yield return null;
            var bd = blockDatas[index];
            bd.phase = BlockPhase.Planted;

            isPlayingAnimation = false;

            GameManager.Instance.ShowUnitPlantUncover(index);

            GameManager.Instance.plantUnits[index].PlayChangeEffect();
        }

        private IEnumerator CorPlayAnimation_Filling(int index)
        {
            isPlayingAnimation = true;
            yield return null;
            var bd = blockDatas[index];
            bd.phase = BlockPhase.Coverd;

            isPlayingAnimation = false;

            GameManager.Instance.ShowUnitPlant(index, false);

            GameManager.Instance.plantUnits[index].PlayChangeEffect();

            for (int i = 0; i < blockDatas.Length; i++)
            {
                if (blockDatas[i].phase != BlockPhase.Coverd)
                {
                    yield break;
                }
            }

            yield return new WaitForSeconds(2f);
            //全部已完成，结束
            Finish();
        }

        private IEnumerator CorPlayAnimation_DigRock(int index)
        {
            isPlayingAnimation = true;
            yield return null;
            var bd = blockDatas[index];
            bd.phase = BlockPhase.Hoed;

            isPlayingAnimation = false;

            GameManager.Instance.plantUnits[index].PlayStarEffect();

            GameManager.Instance.ShowUnitHoedUncover(index);
        }


        private BlockData[] blockDatas = new BlockData[9];
        private float playTime = 0f;
        private float gameTime = 90f;

        public override void StartPlay()
        {
            base.StartPlay();

            for (int i = 0; i < blockDatas.Length; i++)
            {
                var bd = new BlockData();
                bd.phase = BlockPhase.Empty;
                blockDatas[i] = bd;
            }

            playTime = gameTime;

            _operationTotalCnt = 0;
            _operationCorrectCnt = 0;

            CurrOperationType = OperationTypeEnum.None;

            ErrorCG.gameObject.SetActive(false);

            GameManager.Instance.ShowUnitEmpty();

            RefreshInfo();

            StopCoroutine("CorShowTalk");
            StartCoroutine("CorShowTalk");
        }



        private IEnumerator CorShowTalk()
        {
            ScreenUI.PlayRoleAnimation();
            var talkText = "天民田园把选用的优质种苗从育苗床移栽到农田中，移栽之前，要准备好包括铁锨、水桶、剪刀、苗圃土、肥料等工具哦";
            GameManager.Instance.ShowTalk(talkText);

            yield return new WaitForSeconds(20f);

            talkText = "你知道吗？季和秋季是最佳移栽时间，气温适宜，有利于植物生根和发育";
            GameManager.Instance.ShowTalk(talkText);

            yield return new WaitForSeconds(20f);

            talkText = "移栽的方法和步骤都会影响成活率，所以一定要按步骤进行移栽哦";
            GameManager.Instance.ShowTalk(talkText);
            
            yield return new WaitForSeconds(20f);

            talkText = "移栽后的养护管理也很重要，移栽后需注意浇水、施肥、修剪等养护措施哦";
            GameManager.Instance.ShowTalk(talkText);
            
            yield return new WaitForSeconds(20f);

            talkText = "移栽完成后，就可以看着我们的小树苗健康成长啦";
            GameManager.Instance.ShowTalk(talkText);

        }




        private string _infoText = @"剩余时间

田块平整度   {0:0.}%
移栽完成度   {1:0.}%
当前评价    {2}";

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


        protected override void Update()
        {
            base.Update();
            if (isPlaying)
            {
                playTime -= Time.deltaTime;
                timeText.text = $"{playTime:0}秒";

                ScreenUI.TimeSlider.value = playTime / gameTime;

                RefreshInfo();

                if (playTime <= 0)
                {
                    Finish();
                }

                SelectedHoe.SetActive(CurrOperationType == OperationTypeEnum.Hoe);
                SelectedPlant.SetActive(CurrOperationType == OperationTypeEnum.Plant);
                SelectedCover.SetActive(CurrOperationType == OperationTypeEnum.Filling);
                SelectedDigRock.SetActive(CurrOperationType == OperationTypeEnum.DigRock);

            }
        }

        public override void Finish(bool quit = false)
        {
            base.Finish(quit);

            StopCoroutine("CorShowTalk");
            GameManager.Instance.CloseTalk();

            if (!quit)
            {
                var coveredCnt = 0;
                for (int i = 0; i < blockDatas.Length; i++)
                {
                    var bd = blockDatas[i];
                    if (bd.phase == BlockPhase.Coverd)
                    {
                        coveredCnt++;
                    }
                }

                var rate = playTime / gameTime * coveredCnt / blockDatas.Length;
                var score = Mathf.RoundToInt(Mathf.Lerp(10, 60, rate));

                GameManager.Instance.UsingGameData.scoreTransplant = score;

                scoreText.text = score.ToString();
                GameManager.Instance.SaveGame();
            }
        }

    }

}