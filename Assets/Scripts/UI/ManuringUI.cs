using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AniYa.UI
{

    /// <summary>
    /// 施肥UI
    /// </summary>
    public class ManuringUI : BaseOperationUI
    {

        public enum ManuringTypeEnum
        {
            氮肥,
            磷肥,
            钾肥,
            有机肥
        }

        public class ManuringData
        {
            public ManuringTypeEnum manuringType;
            public float value;
            public int maxValue = 100;
            public float rate => value / maxValue;

            public void AddValue(float addValue)
            {
                value = Mathf.Clamp(value + addValue, 0, maxValue);
            }
        }

        public Text scoreText;
        public Text timeText;
        public GameObject skillCDGO;
        public Text skillCDText;

        public Slider[] ManureBarProgress;

        public ManuringData[] manuringDatas;

        /// <summary>
        /// 成长进度
        /// </summary>
        public float growRate;

        public float totalTime = 120;
        public float playTime = 0f;

        public float skillCD = 0f;
        public float skillTime = 30f;

        public float discreaseFactor = 1f;

        public float[] discreaseSpeeds = new[]
        {
            1f,
            0.8f,
            0.5f,
            0.2f,
        };

        private List<int> _growUpIndexes;
        private float _growUpCountTime = 0;

        public override void StartPlay()
        {
            base.StartPlay();

            GameManager.Instance.PlayButtonSound();

            _growUpIndexes = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            _growUpCountTime = 0f;

            manuringDatas = new ManuringData[4];
            for (int i = 0; i < manuringDatas.Length; i++)
            {
                var md = new ManuringData();
                md.manuringType = (ManuringTypeEnum)i;
                md.value = 60;
                md.maxValue = 100;
                manuringDatas[i] = md;
            }

            playTime = totalTime;

            GameManager.Instance.ShowUnitPlants(false);

            var punits = GameManager.Instance.plantUnits;
            for (int i = 0; i < punits.Length; i++)
            {
                var pu = punits[i];
                pu.SetScale(1f);
                pu.PlayShakeRotate();
            }
        }


        public void OnClickByType(int typeInt)
        {
            if (isPlayingAnimation) return;
            GameManager.Instance.PlayButtonSound();

            StartCoroutine(CorPlayManuring((ManuringTypeEnum)typeInt));
        }

        private IEnumerator CorPlayManuring(ManuringTypeEnum manuringType)
        {

            isPlayingAnimation = true;

            yield return null;

            var pUnits = GameManager.Instance.plantUnits.RandomSelect(3);

            for (int i = 0; i < pUnits.Length; i++)
            {
                pUnits[i].PlayManuringAnimation((int)manuringType);
            }

            yield return new WaitForSeconds(1.2f);

            for (int i = 0; i < pUnits.Length; i++)
            {
                pUnits[i].StopManuringAnimation();
            }

            var md = manuringDatas[(int)manuringType];
            md.AddValue(10);

            isPlayingAnimation = false;
        }

        /// <summary>
        /// 水肥一体化
        /// </summary>
        public void OnClickSkill()
        {
            if (isPlayingAnimation) return;
            GameManager.Instance.PlayButtonSound();
            if (skillCD > 0)
            {
                return;
            }
            if (isPlaying && !isPlayingAnimation)
            {
                StartCoroutine(CorPlaySkill());
            }
        }

        private IEnumerator CorPlaySkill()
        {
            isPlayingAnimation = true;
            skillCD = skillTime;
            //播放技能动画
            yield return null;


            var pUnits = GameManager.Instance.plantUnits;
            for (int i = 0; i < pUnits.Length; i++)
            {
                var pu = pUnits[i];
                var r = Random.Range(0, 4);
                pu.PlayManuringAnimation(r);
            }

            yield return new WaitForSecondsRealtime(2f);

            //加满所有肥力
            for (int i = 0; i < manuringDatas.Length; i++)
            {
                manuringDatas[i].value = manuringDatas[i].maxValue;
            }

            for (int i = 0; i < pUnits.Length; i++)
            {
                var pu = pUnits[i];
                pu.StopManuringAnimation();
            }

            isPlayingAnimation = false;
        }

        protected override void Update()
        {
            base.Update();

            if (isPlaying /*&& !isPlayingAnimation*/)
            {
                _growUpCountTime += Time.deltaTime;
                if (_growUpCountTime >= 8f)
                {
                    _growUpCountTime -= 8;
                    var growUpIdx = _growUpIndexes.RandomPick(1)[0];
                    var pu = GameManager.Instance.plantUnits[growUpIdx];
                    pu.SetScale(1.4f, true);
                }

                var isLowValue = false;
                for (int i = 0; i < manuringDatas.Length; i++)
                {
                    manuringDatas[i].AddValue(-discreaseSpeeds[i] * Time.deltaTime * discreaseFactor);
                    var rate = manuringDatas[i].rate;
                    if (rate < 0.8f)
                        isLowValue = true;
                    ManureBarProgress[i].value = rate;
                }
                if (isLowValue)
                {
                    growRate += Time.deltaTime * 0.5f;
                }
                else
                {
                    growRate += Time.deltaTime;
                }

                if (skillCD > 0)
                    skillCD -= Time.deltaTime;

                playTime -= Time.deltaTime;
                if (playTime < 0)
                    playTime = 0;
                timeText.text = $"{playTime:0}秒";
                skillCDText.text = $"{skillCD:0}秒";
                if (skillCD > 0)
                    skillCDGO.SetActive(true);
                else
                    skillCDGO.SetActive(false);

                if (!isPlayingAnimation && playTime <= 0)
                {
                    Finish();
                }
            }
        }

        /// <summary>
        /// 结束游戏，计算积分
        /// </summary>
        public override void Finish(bool quit = false)
        {
            base.Finish(quit);
            if (!quit)
            {
                var score = Mathf.RoundToInt(Mathf.Lerp(10, 60, growRate / totalTime));

                GameManager.Instance.UsingGameData.scoreManuring = score;

                //结算积分，发送到服务端

                scoreText.text = score.ToString();
                GameManager.Instance.SaveGame();
            }
        }

    }

}