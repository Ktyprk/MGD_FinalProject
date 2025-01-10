using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class UIManager : MonoBehaviour
{
    public Slider healthSlider; 
    public PlayerController _player;

    public Transform scoreboard;

    void Update()
    {

        if (_player != null)
        {
            healthSlider.value = _player.Health;
        }

    }

}
