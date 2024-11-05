using UnityEngine;
using UnityEngine.AddressableAssets;

namespace NByte.SeedBreeding
{
    [CreateAssetMenu(menuName = "GameData/SeedBreeding/ChessmanModel")]
    public class ChessmanModel : ModelBase
    {
        public ChessmanTypes ChessmanType;
        public AssetReferenceGameObject ChessmanAsset;

        public enum ChessmanTypes
        {
            Red,
            Green,
            Blue,
            Yellow,
        }
    }
}