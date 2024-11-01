using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AniYa.UI
{

    /// <summary>
    /// 除草除虫UI
    /// </summary>
    public class DisinsectionUI : BaseOperationUI
    {

        public Text scoreText;
        public Text timeText;
        public Text skillCntText;

        public GameObject SelectedDisinsect;
        public GameObject SelectedWeed;

        private float totalTime = 90f;
        private float playTime = 0f;

        private float zaihaiTime = 0f;

        private int skillCnt = 0;

        private enum OperationTypeEnum
        {
            None,
            Disinsect,
            Weed,
        }

        private OperationTypeEnum CurrOperationType;

        private class PlantData
        {
            public int index;
            public int insectCnt = 0;
            public int weedCnt = 0;
            public float growTime = 0f;

            public void Update(float delta)
            {
                if (insectCnt <= 0 && weedCnt <= 0)
                {
                    growTime += delta;
                }
                else
                {
                    growTime += delta * 0.25f;
                }

            }

            public void Clear()
            {
                insectCnt = 0;
                weedCnt = 0;
            }
        }

        private PlantData[] plantDatas = new PlantData[9];

        public void OnClickPlay()
        {
            GameManager.Instance.PlayButtonSound();

            StartPlay();

            CurrOperationType = OperationTypeEnum.None;

            playTime = totalTime;
            skillCnt = 3;
            for (int i = 0; i < plantDatas.Length; i++)
            {
                var pd = new PlantData();
                plantDatas[i] = pd;
                pd.index = i;
            }

            skillCntText.text = skillCnt.ToString();
            GameManager.Instance.ShowUnitPlants();

            var punits = GameManager.Instance.plantUnits;
            for (int i = 0; i < punits.Length; i++)
            {
                var pu = punits[i];
                pu.PlayShakeRotate();
            }
            RefreshInfo();

            StopCoroutine("CorShowTalk");
            StartCoroutine("CorShowTalk");
        }


        private IEnumerator CorShowTalk()
        {
            ScreenUI.PlayRoleAnimation();
            var talkText = "天民田园通过生物防治或物理防治方式除草除虫，除草除虫是农业管理中非常重要的一环，想要提高产量，就一定要及时除草除虫";
            GameManager.Instance.ShowTalk(talkText);

            yield return new WaitForSeconds(20f);

            talkText = "你知道吗？现代农业常用的除草手段有物理除草，化学除草和生物除草";
            GameManager.Instance.ShowTalk(talkText);

            yield return new WaitForSeconds(20f);

            talkText = "你知道吗？杂草往往拥有比农作物更强的生命力，会和农作物争夺土壤养分及太阳光照";
            GameManager.Instance.ShowTalk(talkText);

            yield return new WaitForSeconds(20f);

            talkText = "你知道吗？除虫需要依据害虫种类、数量、生活习性等因素选择合适的除虫方法，做到科学、有效地控制害虫数量";
            GameManager.Instance.ShowTalk(talkText);

            yield return new WaitForSeconds(20f);

            talkText = "你知道吗？很多害虫都有趋光性，诱虫灯就是利用这个特点来做到诱杀害虫并且生态友好的哦";
            GameManager.Instance.ShowTalk(talkText);

        }

        public void OnClickDisinsect()
        {
            GameManager.Instance.PlayButtonSound();

            if (CurrOperationType != OperationTypeEnum.Disinsect)
                CurrOperationType = OperationTypeEnum.Disinsect;
            else
                CurrOperationType = OperationTypeEnum.None;
        }

        public void OnClickWeed()
        {
            GameManager.Instance.PlayButtonSound();

            if (CurrOperationType != OperationTypeEnum.Weed)
                CurrOperationType = OperationTypeEnum.Weed;
            else
                CurrOperationType = OperationTypeEnum.None;
        }

        public void OnClickSkill()
        {
            GameManager.Instance.PlayButtonSound();

            if (!isPlaying || isPlayingAnimation) return;
            if (skillCnt <= 0)
            {
                return;
            }
            skillCnt--;
            skillCntText.text = skillCnt.ToString();
            StartCoroutine(CorPlaySkill());
        }

        private IEnumerator CorPlaySkill()
        {
            isPlayingAnimation = true;
            yield return null;


            for (int i = 0; i < plantDatas.Length; i++)
            {
                plantDatas[i].Clear();

                GameManager.Instance.ClearUnitInsect(i);

                GameManager.Instance.ClearUnitWeed(i);
            }


            isPlayingAnimation = false;
        }

        private void OperateOn(int index)
        {
            GameManager.Instance.PlayTouchSound();

            if (CurrOperationType == OperationTypeEnum.None) return;
            var pd = plantDatas[index];
            if (CurrOperationType == OperationTypeEnum.Disinsect)
            {
                if (pd.insectCnt > 0)
                {
                    GameManager.Instance.plantUnits[index].PlayScaleMotion();
                    StartCoroutine(CorPlayDisinsect(index));
                }
            }
            else if (CurrOperationType == OperationTypeEnum.Weed)
            {
                if (pd.weedCnt > 0)
                {
                    GameManager.Instance.plantUnits[index].PlayScaleMotion();
                    StartCoroutine(CorPlayWeed(index));
                }
            }
        }

        private IEnumerator CorPlayDisinsect(int index)
        {
            isPlayingAnimation = true;
            yield return null;

            var pd = plantDatas[index];
            pd.insectCnt--;

            if (pd.insectCnt <= 0)
            {
                GameManager.Instance.ClearUnitInsect(index);
                GameManager.Instance.plantUnits[index].PlayStarEffect();
            }

            isPlayingAnimation = false;
        }

        private IEnumerator CorPlayWeed(int index)
        {
            isPlayingAnimation = true;

            yield return null;
            var pd = plantDatas[index];
            pd.weedCnt--;

            if (pd.weedCnt <= 0)
            {
                GameManager.Instance.ClearUnitWeed(index);
                GameManager.Instance.plantUnits[index].PlayStarEffect();
            }

            isPlayingAnimation = false;
        }

        private string _infoText = @"剩余时间

作物健康度   {0:0.}%
虫害感染率   {1:0.}%
杂草侵占度   {2:0.}%
当前评价    {3}";

        private float _refreshInfoTime;

        private void RefreshInfo()
        {
            _refreshInfoTime += Time.deltaTime;
            if (_refreshInfoTime < 0.1f)
            {
                return;
            }
            var healthCnt = 0;
            var insectCnt = 0;
            var weedCnt = 0;
            for (int i = 0; i < plantDatas.Length; i++)
            {
                var pd = plantDatas[i];
                if (pd.insectCnt <= 0 && pd.weedCnt <= 0)
                {
                    healthCnt++;
                }
                if (pd.insectCnt > 0)
                    insectCnt++;
                if (pd.weedCnt > 0)
                    weedCnt++;
            }

            var healthRate = 100f * healthCnt / plantDatas.Length;
            var insectRate = 100f * insectCnt / plantDatas.Length;
            var weedRate = 100f * weedCnt / plantDatas.Length;

            var evaluation = string.Empty;
            var evaRate = healthRate;

            if (evaRate >= 0.666f)
                evaluation = "优";
            else if (evaRate >= 0.333f)
                evaluation = "良";
            else
                evaluation = "差";

            ScreenUI.InfoPanelText.text = string.Format(_infoText,
                healthRate, insectRate, weedRate, evaluation);
        }

        protected override void Update()
        {
            base.Update();

            if (isPlaying)
            {
                for (int i = 0; i < plantDatas.Length; i++)
                {
                    plantDatas[i].Update(Time.deltaTime);
                }

                zaihaiTime -= Time.deltaTime;
                if (zaihaiTime <= 0)
                {
                    zaihaiTime = 5f;
                    GenerateZaihai();
                }

                SelectedDisinsect.SetActive(CurrOperationType == OperationTypeEnum.Disinsect);
                SelectedWeed.SetActive(CurrOperationType == OperationTypeEnum.Weed);

                playTime -= Time.deltaTime;

                ScreenUI.TimeSlider.value = playTime / totalTime;
                RefreshInfo();

                timeText.text = $"{playTime:0}秒";

                if (playTime <= 0)
                {
                    Finish();
                    return;
                }
            }
        }

        private void GenerateZaihai()
        {
            var idx = Random.Range(0, plantDatas.Length);
            var pd = plantDatas[idx];
            var isInsect = Random.Range(0, 1f) < 0.5f;
            if (isInsect)
            {
                pd.insectCnt++;
                GameManager.Instance.ShowUnitInsect(idx);
            }
            else
            {
                pd.weedCnt++;
                GameManager.Instance.ShowUnitWeed(idx);
            }
        }

        public override void Finish(bool quit = false)
        {
            base.Finish(quit);
            StopCoroutine("CorShowTalk");
            GameManager.Instance.CloseTalk();

            if (!quit)
            {
                var sum = 0f;
                for (int i = 0; i < plantDatas.Length; i++)
                {
                    var pd = plantDatas[i];
                    var rate = pd.growTime / totalTime;
                    sum += rate;
                }
                sum /= plantDatas.Length;

                var score = Mathf.RoundToInt(Mathf.Lerp(10, 60, sum));

                GameManager.Instance.UsingGameData.scoreDisinsection = score;

                scoreText.text = score.ToString();

                GameManager.Instance.SaveGame();
            }
        }

        protected override void OnClickPlantUnit(int index)
        {
            OperateOn(index);
        }

    }

}