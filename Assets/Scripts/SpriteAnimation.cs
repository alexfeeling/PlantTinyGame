using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SpriteAnimation : MonoBehaviour
{

    public Sprite[] sprites;

    public bool IsPlaying = true;

    public int FPS = 60;

    public bool IsLoop = true;
    public bool ResetOnEnable = true;

    private Image _img;

    private void Awake()
    {
        _img = GetComponent<Image>();
    }

    private void OnEnable()
    {
        if (ResetOnEnable)
            _index = 0;
    }

    private int _index = 0;
    private float _time;

    // Update is called once per frame
    void Update()
    {
        if (IsPlaying && sprites != null && sprites.Length > 0)
        {
            var frameTime = 1f / FPS;
            _time += Time.deltaTime;
            if (_time >= frameTime)
            {
                _time -= frameTime;
                if (_index >= sprites.Length)
                {
                    if (IsLoop)
                        _index = 0;
                    else
                    {
                        _index = sprites.Length - 1;
                        return;
                    }
                }

                _img.sprite = sprites[_index];
                _index++;
            }
        }
    }
}
