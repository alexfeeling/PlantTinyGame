using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace NByte.SeedBreeding
{
    public class ScrChessboard : MonoBehaviour
    {
        private AppService AppService => AppService.Instance;
        private SeedBreedingGameData GameData => AppService.SeedBreeding;
        private SeedBreedingConfig Config => GameData.Config;

        [SerializeField] private Camera MainCamera;
        [SerializeField] private Transform CreateHeight;
        [SerializeField] private Transform Tray;
        [SerializeField] private Transform ChessmenRoot;
        [SerializeField] private Transform ExplosionsRoot;

        public ScrScnSeedBreeding ScnSeedBreeding { get; private set; }
        public Vector3 OriginPosition { get; private set; }
        private Vector3[,] CreatePositions { get; set; }
        private List<ScrChessman> EnabledChessmen { get; set; } = new();

        public void Init(ScrScnSeedBreeding scnSeedBreeding)
        {
            ScnSeedBreeding = scnSeedBreeding;
            Tray.transform.localPosition = new(0, -Config.ChessboardSize.y / 2 - Config.TrayThickness / 2, 0);
            OriginPosition = new(-Config.ChessboardSize.x / 2 + Config.ChessmanSize / 2, -Config.ChessboardSize.y / 2 + Config.ChessmanSize / 2, 0);
            CreatePositions = new Vector3[Config.Rows, Config.Columns];
            for (int i = 0; i < Config.Rows; i++)
            {
                for (int j = 0; j < Config.Columns; j++)
                {
                    CreatePositions[i, j] = new(OriginPosition.x + Config.ChessmanSize * j, CreateHeight.position.y + OriginPosition.y + Config.ChessmanSize * i);
                }
            }
        }

        public IEnumerator Prepare()
        {
            int[,] indexes;
            while (true)
            {
                //todo clear chessmen
                indexes = RandomIndexes();
                if (CheckIndexes(indexes))
                {
                    break;
                }
            }
            yield return CreateChessmen(indexes);
            yield return new WaitForSeconds(Config.ShutGravityDelay);
            EnabledChessmen.ForEach(t => t.GravityState = false);
            EnabledChessmen.ForEach(t => t.InputState = true);
        }
        private int[,] RandomIndexes()
        {
            int[,] indexes = new int[Config.Rows, Config.Columns];
            for (int i = 0; i < Config.Rows; i++)
            {
                for (int j = 0; j < Config.Columns; j++)
                {
                    indexes[i, j] = Random.Range(0, GameData.ChessmanModels.Count);
                }
            }
            return indexes;
        }
        private bool CheckIndexes(int[,] indexes)
        {
            for (int i = 0; i < Config.Rows; i++)
            {
                for (int j = 0; j < Config.Columns; j++)
                {
                    if (!CheckRight(i, j, indexes) || !CheckUp(i, j, indexes))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private bool CheckRight(int i, int j, int[,] indexes)
        {
            int index = indexes[i, j];
            for (int step = 1; step <= Config.ConnectionNumbers - 1; step++)
            {
                if (GetIndex(step) != index)
                {
                    return true;
                }
            }
            return false;

            int? GetIndex(int step)
            {
                if (j + step >= Config.Columns)
                {
                    return null;
                }
                else
                {
                    return indexes[i, j + step];
                }
            }
        }
        private bool CheckUp(int i, int j, int[,] indexes)
        {
            int index = indexes[i, j];
            for (int step = 1; step <= Config.ConnectionNumbers - 1; step++)
            {
                if (GetIndex(step) != index)
                {
                    return true;
                }
            }
            return false;

            int? GetIndex(int step)
            {
                if (i + step >= Config.Rows)
                {
                    return null;
                }
                else
                {
                    return indexes[i + step, j];
                }
            }
        }
        private IEnumerator CreateChessmen(int[,] indexes)
        {
            for (int i = 0; i < Config.Rows; i++)
            {
                for (int j = 0; j < Config.Columns; j++)
                {
                    ChessmanModel chessmanModel = GameData.ChessmanModels[indexes[i, j]];
                    AsyncOperationHandle<GameObject> asyncOperation = chessmanModel.ChessmanAsset.InstantiateAsync(Vector3.down * 10, Quaternion.identity, ChessmenRoot);
                    yield return asyncOperation;
                    ScrChessman chessman = asyncOperation.Result.GetComponent<ScrChessman>();
                    chessman.Init(this, chessmanModel, i + 1, j + 1);
                    asyncOperation.Result.transform.SetLocalPositionAndRotation(CreatePositions[i, j], Quaternion.identity);
                    EnabledChessmen.Add(chessman);
                }
                yield return new WaitForSeconds(Config.CreateChessmanInterval);
            }
        }

        public Vector3 GetChessmanPosition(int row, int column)
        {
            return new Vector3(OriginPosition.x + Config.ChessmanSize * (column - 1), OriginPosition.y + Config.ChessmanSize * (row - 1), 0);
        }
        public Vector3 GetPointerPosition(Vector3 chessmanPosition)
        {
            Vector3 targetPosition = ChessmenRoot.transform.InverseTransformPoint(MainCamera.ScreenToWorldPoint(Input.mousePosition));
            Vector2 direction = Vector2.ClampMagnitude(targetPosition - chessmanPosition, Config.ChessmanSize);
            return chessmanPosition + new Vector3(direction.x, direction.y, -Config.ChessmanSize);
        }

        public bool Swap(ScrChessman chessman, Vector2 direction)
        {
            EnabledChessmen.ForEach(t => t.InputState = false);
            int row = chessman.Row;
            int column = chessman.Column;

            if (GetNeighbor(chessman, direction) is ScrChessman neighbor)
            {
                chessman.Move(neighbor.Row, neighbor.Column);
                neighbor.Move(row, column);
                ScnSeedBreeding.Operations++;
                Eliminate();
                return true;
            }
            else
            {
                EnabledChessmen.ForEach(t => t.InputState = true);
                return false;
            }
        }
        private ScrChessman GetNeighbor(ScrChessman chessman, Vector2 direction)
        {
            if (direction.magnitude >= Config.ChessmanSize / 2)
            {
                float angle = Vector2.SignedAngle(new(1, -1), direction);
                int row = chessman.Row;
                int column = chessman.Column;
                if (angle >= 90)
                {
                    row++;
                }
                else if (angle >= 0)
                {
                    column++;
                }
                else if (angle <= -90)
                {
                    column--;
                }
                else
                {
                    row--;
                }
                return EnabledChessmen.SingleOrDefault(t => t.Row == row && t.Column == column);
            }
            else
            {
                return null;
            }
        }

        public void Eliminate()
        {
            StartCoroutine(Steps());

            IEnumerator Steps()
            {
                yield return null;
                while (CheckChessmen())
                {
                    yield return DestroyChessmen();
                    yield return RefillChessmen();
                }
                EnabledChessmen.ForEach(t => t.InputState = true);
            }
        }
        private bool CheckChessmen()
        {
            for (int i = 1; i <= Config.Rows; i++)
            {
                for (int j = 1; j <= Config.Columns; j++)
                {
                    j += CheckRight(i, j);
                }
            }
            for (int j = 1; j <= Config.Columns; j++)
            {
                for (int i = 1; i <= Config.Rows; i++)
                {
                    i += CheckUp(i, j);
                }
            }
            return EnabledChessmen.Any(t => t.DeleteState);
        }
        private int CheckRight(int i, int j)
        {
            return Check(EnabledChessmen.Single(t => t.Row == i && t.Column == j), 0, 1);
        }
        private int CheckUp(int i, int j)
        {
            return Check(EnabledChessmen.Single(t => t.Row == i && t.Column == j), 1, 0);
        }
        private int Check(ScrChessman chessman, int rowStep, int columnStep)
        {
            List<ScrChessman> chessmen = new() { chessman };
            while (true)
            {
                ScrChessman last = chessmen.Last();
                if (EnabledChessmen.SingleOrDefault(t => t.Row == last.Row + rowStep && t.Column == last.Column + columnStep) is ScrChessman target)
                {
                    if (target.ChessmanModel == last.ChessmanModel)
                    {
                        chessmen.Add(target);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            if (chessmen.Count >= Config.ConnectionNumbers)
            {
                chessmen.ForEach(t => t.DeleteState = true);
            }
            return chessmen.Count - 1;
        }
        private IEnumerator DestroyChessmen()
        {
            for (int i = 2; i <= Config.Rows; i++)
            {
                for (int j = 1; j <= Config.Columns; j++)
                {
                    ScrChessman chessman = EnabledChessmen.Single(t => t.Row == i && t.Column == j);
                    chessman.Init(i - EnabledChessmen.Where(t => t.Row < i && t.Column == j).Count(t => t.DeleteState), j);
                }
            }
            SeedBreedingConfig.EliminateConfig eliminateConfig = Config.Eliminates
                .Where(t => EnabledChessmen.Count(t => t.DeleteState) >= t.Numbers)
                .OrderBy(t => t.Numbers)
                .LastOrDefault();
            foreach (ScrChessman chessman in EnabledChessmen.Where(t => t.DeleteState).OrderBy(t => t.Row).ThenBy(t => t.Column))
            {
                Transform transform = chessman.gameObject.transform;
                yield return eliminateConfig.ExplosionAsset.InstantiateAsync(transform.position, transform.rotation, ExplosionsRoot);
                AppService.PlaySound(eliminateConfig.ExplosionSound);
                yield return chessman.gameObject.transform.DOScale(0, Config.EliminateInterval).WaitForCompletion();
                Destroy(chessman.gameObject);
            }
            yield return eliminateConfig.InfoAsset.InstantiateAsync(gameObject.transform);
            AppService.PlaySound(eliminateConfig.InfoSound);
            ScnSeedBreeding.AddPoints(EnabledChessmen.Where(t => t.DeleteState));
            EnabledChessmen.RemoveAll(t => t.DeleteState);
        }
        private IEnumerator RefillChessmen()
        {
            EnabledChessmen.ForEach(t => t.GravityState = true);
            int[,] indexes;
            while (true)
            {
                indexes = RandomIndexes();
                if (CheckIndexes(indexes))
                {
                    break;
                }
            }

            for (int j = 0; j < Config.Columns; j++)
            {
                for (int i = EnabledChessmen.Count(t => t.Column == j + 1); i < Config.Rows; i++)
                {
                    ChessmanModel chessmanModel = GameData.ChessmanModels[indexes[i, j]];
                    AsyncOperationHandle<GameObject> asyncOperation = chessmanModel.ChessmanAsset.InstantiateAsync(Vector3.down * 10, Quaternion.identity, ChessmenRoot);
                    yield return asyncOperation;
                    ScrChessman chessman = asyncOperation.Result.GetComponent<ScrChessman>();
                    chessman.Init(this, chessmanModel, i + 1, j + 1);
                    asyncOperation.Result.transform.SetLocalPositionAndRotation(CreatePositions[i, j], Quaternion.identity);
                    EnabledChessmen.Add(chessman);
                    yield return new WaitForSeconds(Config.CreateChessmanInterval);
                }
            }
            yield return new WaitForSeconds(Config.ShutGravityDelay);
            EnabledChessmen.ForEach(t => t.GravityState = false);
        }
    }
}