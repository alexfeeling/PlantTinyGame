using UnityEngine;

namespace NByte.JumpAndGrow
{
    [CreateAssetMenu(menuName = "GameData/JumpAndGrow/Config")]
    public class JumpAndGrowConfig : ScriptableObject
    {
        public Vector2 JumpVelocity = new(2, 2);
        public float ChargeDuration = 1f;
    }
}