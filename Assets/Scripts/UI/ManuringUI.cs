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
        private PlantUnit[] _selectUnits;

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
            _operationCnt = 0;
            RefreshInfo();

            GameManager.Instance.ShowUnitPlants(false);

            var punits = GameManager.Instance.plantUnits;
            _selectUnits = new PlantUnit[punits.Length];
            for (int i = 0; i < punits.Length; i++)
            {
                var pu = punits[i];
                _selectUnits[i] = pu;
                pu.SetScale(1f);
                pu.PlayShakeRotate();
            }

            StopCoroutine("CorShowTalk");
            StartCoroutine("CorShowTalk");
        }


        private IEnumerator CorShowTalk()
        {
            ScreenUI.PlayRoleAnimation();
            var talkText = "天民田园在植物生长过程中通过水肥一体化系统对植物施肥，选择合适的肥料种类很重要。根据作物对不同养分的需求，应当选择适合作物需要的肥料";
            GameManager.Instance.ShowTalk(talkText);

            yield return new WaitForSeconds(20f);

            talkText = "你知道吗？氮肥有助于提高植物的蛋白质合成能力哦";
            GameManager.Instance.ShowTalk(talkText);

            yield return new WaitForSeconds(20f);

            talkText = "你知道吗？磷肥有助于促进植物根系生长哦";
            GameManager.Instance.ShowTalk(talkText);

            yield return new WaitForSeconds(20f);

            talkText = "你知道吗？钾肥可以提高植株的抗逆性，增强植物的光合作用";
            GameManager.Instance.ShowTalk(talkText);

            yield return new WaitForSeconds(20f);

            talkText = "你知道吗？复合肥的营养元素种类较多，能充分发挥营养元素间相互促进的作用，保证作物养分平衡";
            GameManager.Instance.ShowTalk(talkText);

        }


        public void OnClickByType(int typeInt)
        {
            if (isPlayingAnimation) return;
            GameManager.Instance.PlayButtonSound();
            _operationCnt++;
            StartCoroutine(CorPlayManuring((ManuringTypeEnum)typeInt));
        }

        private IEnumerator CorPlayManuring(ManuringTypeEnum manuringType)
        {

            isPlayingAnimation = true;

            yield return null;

            var pUnits = _selectUnits.RandomSelect(3);

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
                _operationCnt++;
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




        private string _infoText = @"剩余时间

作物生长状态   {0:0.}%
肥料满足度   {1:0.}%
施肥次数   {2}
当前评价    {3}";

        private float _refreshInfoTime;
        private int _operationCnt;

        private void RefreshInfo()
        {
            _refreshInfoTime += Time.deltaTime;
            if (_refreshInfoTime < 0.1f)
            {
                return;
            }

            var totalRate = 0f;
            for (int i = 0; i < manuringDatas.Length; i++)
            {
                totalRate += manuringDatas[i].rate;
            }
            totalRate /= manuringDatas.Length;

            var evaluation = string.Empty;
            var evaRate = totalRate;

            if (evaRate >= 0.666f)
                evaluation = "优";
            else if (evaRate >= 0.333f)
                evaluation = "良";
            else
                evaluation = "差";

            ScreenUI.InfoPanelText.text = string.Format(_infoText,
                100f * growRate / totalTime, totalRate, _operationCnt, evaluation);
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
                    pu.PlayChangeEffect();
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

                ScreenUI.TimeSlider.value = playTime / totalTime;

                RefreshInfo();

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
            StopCoroutine("CorShowTalk");
            GameManager.Instance.CloseTalk();

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