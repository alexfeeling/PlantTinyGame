using System.Collections;
using UnityEngine;

namespace NByte.JumpAndGrow
{
    public class ScrScnJumpAndGrow : ScrSceneBase
    {
        [SerializeField] private ScrPlayer Player;

        protected override void Start()
        {
            base.Start();
            StartCoroutine(Steps());

            IEnumerator Steps()
            {
                yield return AppService.HideCurtain();
            }
        }
    }
}