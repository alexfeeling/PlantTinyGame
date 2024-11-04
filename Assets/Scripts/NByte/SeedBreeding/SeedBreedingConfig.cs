using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace NByte.SeedBreeding
{
    [CreateAssetMenu(menuName = "GameData/SeedBreeding/Config")]
    public class SeedBreedingConfig : ScriptableObject
    {
        public AudioClip PlayMusic;
        public float MusicVolume = 1;
        public int Columns = 5;
        public int Rows = 5;
        public int ConnectionNumbers = 3;
        public float ChessmanSize = 0.5f;
        public Vector2 ChessboardSize;
        public float TrayThickness = 1f;
        public float CreateChessmanInterval = 0.2f;
        public float ShutGravityDelay = 1;
        public List<EliminateConfig> Eliminates;
        public float EliminateInterval = 0.5f;
        public int DNAMax = 20;
        public int GameLifetime = 180;
        public List<TalkConfig> Talks;

        private void OnValidate()
        {
            ChessboardSize = new(Columns * ChessmanSize, Rows * ChessmanSize);
        }

        [Serializable]
        public struct EliminateConfig
        {
            public int Numbers;
            public AssetReferenceGameObject ExplosionAsset;
            public AudioClip ExplosionSound;
            public AssetReferenceGameObject InfoAsset;
            public AudioClip InfoSound;
        }
        [Serializable]
        public struct TalkConfig
        {
            public float Delay;
            [TextArea] public string Talk;
        }
    }
}