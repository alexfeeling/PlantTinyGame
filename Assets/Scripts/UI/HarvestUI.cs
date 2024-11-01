using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AniYa.UI
{

    /// <summary>
    /// 收获UI
    /// </summary>
    public class HarvestUI : BaseOperationUI
    {

        public Text scoreText;
        public Text timeText;
        public Text skillCntText;

        public Transform[] Hands;

        public Image ScoreBarIcon;
        public Image ScoreBarIcon2;
        public Text ScoreBarText;
        public Text ScoreBarText2;

        public override void ShowStart()
        {
            base.ShowStart();

        }

        public void OnClickStart()
        {
            GameManager.Instance.PlayButtonSound();

            StartPlay();

            round = 0;
            generateCD = 0;
            skillCnt = 3;
            totalTime = 90f;
            score = 0;
            maxScore = 0;
            for (int i = 0; i < fruitArr.Length; i++)
                fruitArr[i] = 0;

            playTime = totalTime;
            _missCnt = 0;
            skillCntText.text = skillCnt.ToString();

            GameManager.Instance.ShowUnitPlants();

            var plantId = GameManager.Instance.UsingGameData.plantIndex;
            var fpu = GameManager.Instance.plantUnits[0];
            ScoreBarIcon.sprite = fpu.FruitSps[plantId];
            ScoreBarIcon2.sprite = fpu.FruitSps[plantId];
            ScoreBarText.text = "x 0";
            ScoreBarText2.text = "x 0";

            var punits = GameManager.Instance.plantUnits;
            for (int i = 0; i < punits.Length; i++)
            {
                var pu = punits[i];
                pu.PlayShakeRotate();
            }

            for (int i = 0; i < Hands.Length; i++)
            {
                var hand = Hands[i];
                hand.gameObject.SetActive(false);
            }

            GenerateFruit();

            RefreshInfo();

            StopCoroutine("CorShowTalk");
            StartCoroutine("CorShowTalk");
        }


        private IEnumerator CorShowTalk()
        {
            ScreenUI.PlayRoleAnimation();
            var talkText = "天民田园每个季节都有至少四种农作物可以采收，丰收时节瓜果飘香，口齿生津。你知道吗？不同的作物成熟时间也不同，在一年四季中，不同的季节，收获方式也不一样哦";
            GameManager.Instance.ShowTalk(talkText);

            yield return new WaitForSeconds(20f);

            talkText = "稻谷成熟笑脸开，麦穗儿重心欢扬。瓜果红艳田间秀，农民喜笑高歌响";
            GameManager.Instance.ShowTalk(talkText);

            yield return new WaitForSeconds(20f);

            talkText = "你知道吗？大部分农作物在成熟时，果实的外观都会发生显著的变化。最常见的就是颜色变黄或者变红";
            GameManager.Instance.ShowTalk(talkText);

            yield return new WaitForSeconds(20f);

            talkText = "你知道吗？大部分粮食在收获后进行处理，在低温和干燥的环境下进行存储，再运输到农贸市场哦";
            GameManager.Instance.ShowTalk(talkText);

            yield return new WaitForSeconds(20f);

            talkText = "加油加油，马上就可以品尝丰盛的果实了哦！";
            GameManager.Instance.ShowTalk(talkText);

        }

        public void OnClickSkill()
        {
            if (isPlayingAnimation) return;

            GameManager.Instance.PlayButtonSound();

            if (skillCnt > 0)
            {
                skillCnt--;
                skillCntText.text = skillCnt.ToString();

                StartCoroutine(CorPlaySkill());
            }
        }

        private IEnumerator CorPlaySkill()
        {
            isPlayingAnimation = true;

            for (int i = 0; i < Hands.Length; i++)
            {
                var fv = fruitArr[i];
                if (fv <= 0) continue;
                var hand = Hands[i];
                hand.gameObject.SetActive(true);
                var handChild = hand.GetChild(0);
                handChild.localPosition = new Vector3(80, 0, 0);
                handChild.DOLocalMove(Vector3.zero, 0.4f).SetEase(Ease.OutCubic).WaitForCompletion();
            }

            yield return new WaitForSeconds(0.4f);

            for (int i = 0; i < Hands.Length; i++)
            {
                var hand = Hands[i];
                var fv = fruitArr[i];
                if (fv <= 0) continue;

                var handChild = hand.GetChild(0);
                handChild.DOLocalMoveY(-20, 0.15f).SetEase(Ease.InCubic).WaitForCompletion();
            }
            yield return new WaitForSeconds(0.15f);
            for (int i = 0; i < Hands.Length; i++)
            {
                var hand = Hands[i];
                var fv = fruitArr[i];
                if (fv <= 0) continue;
                var handChild = hand.GetChild(0);

                handChild.DOLocalMoveY(0, 0.1f).SetEase(Ease.OutCubic).WaitForCompletion();
            }

            yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < Hands.Length; i++)
            {
                var hand = Hands[i];
                var fv = fruitArr[i];
                if (fv <= 0) continue;
                hand.gameObject.SetActive(false);
            }

            isPlayingAnimation = false;

            for (int i = 0; i < fruitArr.Length; i++)
            {
                var fv = fruitArr[i];
                fruitArr[i] = 0;
                if (fv == 1)
                {
                    score++;
                }
                else if (fv == 2)
                {
                    score += 5;
                }


                GameManager.Instance.ShowUnitFruit(i, 0);
            }
            ScoreBarText.text = "x " + score;
            ScoreBarText2.text = "x " + score;

            generateCD = 0f;

        }

        private float generateCD = 0f;
        private float generateTime = 10f;

        public int skillCnt = 0;

        private float totalTime = 90f;
        private float playTime = 0f;

        private int score = 0;
        private int maxScore = 0;

        private int round = 0;

        private static int[] arrayIndexes = new[]
        {
            0,1,2,3,4,5,6,7,8
        };

        //记录果实的数据，0：无果，1：有普通果实，2：有果王
        private int[] fruitArr = new int[9];

        private void GenerateFruit()
        {
            var count = Random.Range(3, 6);
            var indexes = arrayIndexes.RandomSelect(count);

            for (int i = 0; i < fruitArr.Length; i++)
            {
                fruitArr[i] = 0;
                GameManager.Instance.ShowUnitPlant(i);
                GameManager.Instance.ShowUnitFruit(i, 0);
            }

            for (int i = 0; i < indexes.Length; i++)
            {
                var index = indexes[i];
                var isFruitKing = Random.Range(0, 1f) < 0.2f;
                if (isFruitKing)
                {
                    fruitArr[index] = 2;
                    maxScore += 5;
                }
                else
                {
                    fruitArr[index] = 1;
                    maxScore++;
                }

                GameManager.Instance.ShowUnitFruit(index, fruitArr[index]);
            }

            generateCD = generateTime;
        }

        protected override void OnClickPlantUnit(int index)
        {
            GameManager.Instance.PlayButtonSound();

            StartCoroutine(PickFruit(index));
        }


        private string _infoText = @"剩余时间

采收数量   {0}
遗漏数量   {1}
当前评价    {2}";

        private int _missCnt;
        private float _refreshInfoTime;

        private void RefreshInfo()
        {
            _refreshInfoTime += Time.deltaTime;
            if (_refreshInfoTime < 0.1f)
            {
                return;
            }

            var evaluation = string.Empty;
            var evaRate = 0f;
            if (maxScore <= 0)
                evaRate = 0.5f;
            else
                evaRate = 1f * score / maxScore;
            if (evaRate >= 0.666f)
                evaluation = "优";
            else if (evaRate >= 0.333f)
                evaluation = "良";
            else
                evaluation = "差";

            ScreenUI.InfoPanelText.text = string.Format(_infoText,
                score, _missCnt, evaluation);
        }

        protected override void Update()
        {
            base.Update();

            if (isPlaying)
            {

                playTime -= Time.deltaTime;
                timeText.text = $"{playTime:0}秒";
                
                RefreshInfo();

                ScreenUI.TimeSlider.value = playTime / totalTime;

                if (playTime <= 0f)
                {
                    Finish();
                }

                //if (generateCD > 0)
                {
                    generateCD -= Time.deltaTime;
                    if (generateCD <= 0 && !isPlayingAnimation)
                    {
                        NextRound();
                    }
                }
            }
        }

        private IEnumerator PickFruit(int index)
        {
            var pu = GameManager.Instance.plantUnits[index];
            pu.PlayScaleMotion();
            isPlayingAnimation = true;
            var fruitVal = fruitArr[index];
            if (fruitVal == 1)
            {
                //普通果实
                score++;

                fruitArr[index] = 0;
            }
            else if (fruitVal == 2)
            {
                score += 5;

                fruitArr[index] = 0;
            }
            else
            {
                yield break;
            }

            var hand = Hands[index];
            hand.gameObject.SetActive(true);
            var handChild = hand.GetChild(0);
            handChild.localPosition = new Vector3(80, 0, 0);
            yield return handChild.DOLocalMove(Vector3.zero, 0.4f).SetEase(Ease.OutCubic).WaitForCompletion();
            yield return handChild.DOLocalMoveY(-20, 0.15f).SetEase(Ease.InCubic).WaitForCompletion();
            yield return handChild.DOLocalMoveY(0, 0.1f).SetEase(Ease.OutCubic).WaitForCompletion();
            hand.gameObject.SetActive(false);

            GameManager.Instance.ShowUnitFruit(index, 0);

            isPlayingAnimation = false;

            ScoreBarText.text = "x " + score;
            ScoreBarText2.text = "x " + score;

            yield return new WaitForSeconds(2f);

            while (isPlayingAnimation)
                yield return null;

            var clearFlag = true;
            for (int i = 0; i < fruitArr.Length; i++)
            {
                if (fruitArr[i] > 0)
                {
                    clearFlag = false;
                    break;
                }
            }
            if (clearFlag)
            {
                generateCD = 0;
                NextRound();
            }
        }

        private void NextRound()
        {
            round++;

            if (round > 18)
            {
                Finish();
            }
            else
            {
                for (int i = 0; i < fruitArr.Length; i++)
                {
                    if (fruitArr[i] > 0)
                        _missCnt++;
                }

                GenerateFruit();
            }
        }

        public override void Finish(bool quit = false)
        {
            base.Finish(quit);
            StopCoroutine("CorShowTalk");
            GameManager.Instance.CloseTalk();
            if (!quit)
            {
                generateCD = 0;
                round = 0;

                var rate = 1f * score / maxScore;
                score = Mathf.RoundToInt(Mathf.Lerp(10, 100, rate));

                GameManager.Instance.UsingGameData.scoreHarvest = score;

                scoreText.text = score.ToString();
                GameManager.Instance.SaveGame();
            }
        }

    }

}