using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class UIManager : MonoBehaviour
{
    public Slider healthSlider; 
    public PlayerController _player;

    public Text countDownText;
    public bool startCountdown;
    public float countdownTimer = 4f;

    public Transform scoreboard;

    void Update()
    {

        if (_player != null)
        {
            Debug.Log("Player is not null");
            healthSlider.value = _player.Health;
        }

    }

    void FixedUpdate()
    {
        // if (_player != null)
        // {
        //     if (startCountdown && countdownTimer > 0)
        //     {
        //         countdownTimer -= Time.fixedDeltaTime;
        //         countDownText.text = ((int)countdownTimer).ToString();
        //         countDownText.enabled = true;
        //     }
        //     else if(startCountdown && countdownTimer <= 0 && _player.locked)
        //     {
        //         _player.locked = false;
        //         countDownText.enabled = false;
        //     }
        // }
        //
        // if(displayWinnerTimer > 0)
        // {
        //     displayWinnerTimer -= Time.fixedDeltaTime;
        //     winnerName.gameObject.SetActive(true);
        // }
        // else
        // {
        //     winnerName.gameObject.SetActive(false);
        // }
        
    }

    float displayWinnerTimer = 0f;
    public Text winnerName;

    public void DisplayWinner(Player winner, float duration)
    {
        winnerName.text = winner.NickName;
        displayWinnerTimer = duration;
    }


}
