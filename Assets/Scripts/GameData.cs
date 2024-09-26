using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AniYa
{

    public class GameData
    {

        public string playerId;         //玩家ID
        public byte loginType;          //登录类型，0:无登录，1:ID卡，2:微信扫码
        public int gameId;              //游戏ID
        public int plantIndex;          //作物数字ID
        public int blockIndex;          //地块ID
        public int envirIndex;          //环境ID
        public int scoreBreeding;       //育种
        public int scoreSelectSeedlings;//选苗
        public int scoreTransplant;     //移栽
        public int scoreManuring;       //施肥
        public int scoreWatering;       //浇水
        public int scoreDisinsection;   //除草除虫
        public int scoreHarvest;        //收获
        public int scoreTotal;          //总分

        public int GetScore(GameManager.GamePhaseEnum gamePhase)
        {
            switch (gamePhase)
            {
                case GameManager.GamePhaseEnum.Breeding:
                    return scoreBreeding;
                case GameManager.GamePhaseEnum.SelectSeedlings:
                    return scoreSelectSeedlings;
                case GameManager.GamePhaseEnum.Transplant:
                    return scoreTransplant;
                case GameManager.GamePhaseEnum.Manuring:
                    return scoreManuring;
                case GameManager.GamePhaseEnum.Watering:
                    return scoreWatering;
                case GameManager.GamePhaseEnum.Disinsection:
                    return scoreDisinsection;
                case GameManager.GamePhaseEnum.Harvest:
                    return scoreHarvest;
                default:
                    return 0;
            }
        }

    }

}