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
        }

        private IEnumerator CorPlayAnimation_Filling(int index)
        {
            isPlayingAnimation = true;
            yield return null;
            var bd = blockDatas[index];
            bd.phase = BlockPhase.Coverd;

            isPlayingAnimation = false;

            GameManager.Instance.ShowUnitPlant(index, false);

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

            CurrOperationType = OperationTypeEnum.None;

            ErrorCG.gameObject.SetActive(false);

            GameManager.Instance.ShowUnitEmpty();
        }

        protected override void Update()
        {
            base.Update();
            if (isPlaying)
            {
                playTime -= Time.deltaTime;
                timeText.text = $"{playTime:0}秒";

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