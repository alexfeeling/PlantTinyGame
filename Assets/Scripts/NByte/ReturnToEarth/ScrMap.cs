using System.Collections;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace NByte.ReturnToEarth
{
    public class ScrMap : MonoBehaviour
    {
        private static ReturnToEarthConfig Config => AppService.Instance.ReturnToEarth.Config;

        [SerializeField] private Transform ObstaclesRoot;

        public ScrScnReturnToEarth ScnReturnToEarth { get; private set; }
        public MapModel MapModel { get; private set; }

        public int Stage { get; private set; }
        public int Step { get; private set; }

        public void Init(ScrScnReturnToEarth scnReturnToEarth, MapModel mapModel, int stage, int step)
        {
            ScnReturnToEarth = scnReturnToEarth;
            MapModel = mapModel;
            Stage = stage;
            Step = step;
            Load();
        }
        private void Load()
        {
            gameObject.name = $"Stage: {Stage} & Step: {Step}";
            StartCoroutine(CreateObstacles());
        }
        private IEnumerator CreateObstacles()
        {
            float interval = Config.MapWidth / (MapModel.NumberOfObstacles * 2);
            float offset = -Config.MapWidth / 2 + interval;
            for (int i = 0; i < MapModel.NumberOfObstacles; i++)
            {
                AsyncOperationHandle<GameObject> handle = MapModel.ObstacleAsset.InstantiateAsync(ObstaclesRoot);
                yield return handle;
                ScrObstacle obstacle = handle.Result.GetComponent<ScrObstacle>();
                obstacle.Init(new Vector3(offset, Random.Range(-Config.ObstacleOffset, Config.ObstacleOffset), 0), Config.ObstacleOffset);
                offset += interval * 2;
            }
        }

        private void Update()
        {
            MoveMap();
        }
        private void MoveMap()
        {
            if (ScnReturnToEarth != null)
            {
                if (ScnReturnToEarth.State)
                {
                    transform.position -= new Vector3(Config.MapSpeed * Time.deltaTime, 0, 0);
                }
            }
        }
    }
}