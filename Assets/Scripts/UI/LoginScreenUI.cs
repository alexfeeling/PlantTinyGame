using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AniYa.UI
{

    public class LoginScreenUI : MonoBehaviour
    {

        public Text TextTitle;
        public Transform TextLogin;


        public void Show()
        {
            gameObject.SetActive(true);
            var phase = GameManager.Instance.GamePhase;
            TextTitle.text = $"{(int)phase + 1}.{GameManager.GetPhaseName(phase)}";
        }


    }

}