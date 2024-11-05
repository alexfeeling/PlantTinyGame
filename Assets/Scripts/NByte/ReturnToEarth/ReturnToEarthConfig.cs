using System.Collections.Generic;
using UnityEngine;

namespace NByte.ReturnToEarth
{
    [CreateAssetMenu(menuName = "GameData/ReturnToEarth/Config")]
    public class ReturnToEarthConfig : ScriptableObject
    {
        public AudioClip StartMusic;
        public AudioClip PlayMusic;
        public float MapWidth = 40f;
        public float MapDelay = 0.5f;
        public float MapSpeed = 0.5f;
        public float MapDistance = 500f;
        public float DistancePerTime;
        public float ObstacleOffset = 3f;
        public float SpaceshipGravity = 1f;
        public float SpaceshipClimb = 1f;
        public float SpaceshipFlyIn = 3f;
        public float LandingPosition = -5f;
        public float LandingTime = 5f;
        public List<MapModel> Maps;

        private void OnValidate()
        {
            DistancePerTime = MapDistance / (MapWidth / MapSpeed);
        }
    }
}