using UnityEngine;
using UnityEngine.AddressableAssets;

namespace NByte.Transplanting
{
    [CreateAssetMenu(menuName = "GameData/Transplanting/Config")]
    public class TransplantingConfig : ScriptableObject
    {
        public AudioClip Music;
        public Vector2Int LandSize = new(10, 7);
        public AssetReferenceGameObject FieldAsset;
        public float FieldSpacing = 1;
        public int ObstaclesMin = 1;
        public int DifficultyMax = 5;
        public int DifficultySteps = 1;
        public float DriveSpeed = 0.2f;
        public AssetReferenceGameObject CompleteAsset;
        public AudioClip CompleteSound;
        public float CompleteDelay = 1;
    }
}