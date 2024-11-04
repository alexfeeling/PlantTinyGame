using UnityEngine;
using UnityEngine.AddressableAssets;

namespace NByte.ReturnToEarth
{
    [CreateAssetMenu(menuName = "GameData/ReturnToEarth/MapModel")]
    public class MapModel : ModelBase
    {
        public MapTypes MapType;
        [TextArea] public string MapInfo;
        public AssetReference MapAsset;
        public AssetReference ObstacleAsset;
        public int LoopTimes;
        public float NumberOfObstacles;
        public float GapOfObsoletes;

        public enum MapTypes
        {
            Takeoff,
            Sailing,
            Landing,
        }
    }
}