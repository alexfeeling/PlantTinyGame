using UnityEngine;
using UnityEngine.AddressableAssets;

namespace NByte.Transplanting
{
    [CreateAssetMenu(menuName = "GameData/Transplanting/Config")]
    public class TransplantingConfig : ScriptableObject
    {
        public Vector2Int LandSize = new(5, 5);
        public AssetReferenceGameObject FieldAsset;
        public float FieldSpacing = 1;
    }
}