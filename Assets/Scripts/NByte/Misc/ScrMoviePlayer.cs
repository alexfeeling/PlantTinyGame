using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace NByte
{
    [RequireComponent(typeof(PlayableDirector))]
    public class ScrMoviePlayer : MonoBehaviour
    {
        [SerializeField] private Transform CameraStartState;

        private CinemachineVirtualCamera MovieCamera { get; set; }
        private CustomAwaiter AwaiterPlay { get; set; } = new();

        public void Init(CinemachineVirtualCamera movieCamera)
        {
            MovieCamera = movieCamera;
        }

        public void PrepareCamera()
        {
            MovieCamera.gameObject.transform.SetPositionAndRotation(CameraStartState.position, CameraStartState.rotation);
            MovieCamera.Priority = 20;
        }
        public IEnumerator Play()
        {
            AwaiterPlay.IsCompleted = false;
            GetComponent<PlayableDirector>().Play();
            MovieCamera.Priority = 0;
            yield return AwaiterPlay;
        }
        public void FinishPlay()
        {
            AwaiterPlay.IsCompleted = true;
        }
    }
}