using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AniYa.UI
{

    public class LoginScreenUI : MonoBehaviour
    {

        public Image BGImg;
        public Text TextTitle;
        public Transform TextLogin;
        public Sprite[] sprites;

        public void Show()
        {
            gameObject.SetActive(true);
            var phase = GameManager.Instance.GamePhase;
            TextTitle.text = $"{(int)phase + 1}.{GameManager.GetPhaseName(phase)}";
            BGImg.sprite = sprites[(int)phase];
        }


    }

}