using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;

namespace NByte
{
    public partial class AppService
    {
        public ReturnToEarthGameData ReturnToEarth { get; private set; } = new();
        public SeedBreedingGameData SeedBreeding { get; private set; } = new();
        public JumpAndGrowGameData JumpAndGrow { get; private set; } = new();
        public TransplantingGameData Transplanting { get; private set; } = new();

        private void AwakeOfGameData()
        {
            ReturnToEarth.Load();
            SeedBreeding.Load();
            JumpAndGrow.Load();
            Transplanting.Load();
        }
    }

    public abstract class GameDataBase
    {
        private const string GameData = "GameData";

        public abstract void Load();

        protected T LoadAsset<T>()
        {
            return Addressables.LoadAssetAsync<T>(GameData).WaitForCompletion();
        }
        protected List<T> LoadAssets<T>()
        {
            return Addressables.LoadAssetsAsync<T>(GameData, null).WaitForCompletion().ToList();
        }
    }
    public class ReturnToEarthGameData : GameDataBase
    {
        public ReturnToEarth.ReturnToEarthConfig Config { get; private set; }
        public List<ReturnToEarth.MapModel> MapModels { get; private set; }

        public override void Load()
        {
            Config = LoadAsset<ReturnToEarth.ReturnToEarthConfig>();
            MapModels = LoadAssets<ReturnToEarth.MapModel>();
        }
    }
    public class SeedBreedingGameData : GameDataBase
    {
        public SeedBreeding.SeedBreedingConfig Config { get; private set; }
        public List<SeedBreeding.ChessmanModel> ChessmanModels { get; private set; }

        public override void Load()
        {
            Config = LoadAsset<SeedBreeding.SeedBreedingConfig>();
            ChessmanModels = LoadAssets<SeedBreeding.ChessmanModel>();
        }
    }
    public class JumpAndGrowGameData : GameDataBase
    {
        public JumpAndGrow.JumpAndGrowConfig Config { get; private set; }

        public override void Load()
        {
            Config = LoadAsset<JumpAndGrow.JumpAndGrowConfig>();
        }
    }
    public class TransplantingGameData : GameDataBase
    {
        public Transplanting.TransplantingConfig Config { get; private set; }

        public override void Load()
        {
            Config = LoadAsset<Transplanting.TransplantingConfig>();
        }
    }
}