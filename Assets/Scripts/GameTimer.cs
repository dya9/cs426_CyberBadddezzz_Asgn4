using UnityEngine;
using TMPro;
using Unity.Netcode;

public class GameTimer : NetworkBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float gameDuration = 45f;

    private NetworkVariable<float> timeRemaining = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private bool timerStarted = false;

    private void Update()
    {
        if (timerText != null)
        {
            timerText.text = $"TIME: {Mathf.Ceil(timeRemaining.Value)}";
        }

        // Only server checks for player count
        if (IsServer && !timerStarted)
        {
            if (NetworkManager.Singleton.ConnectedClientsList.Count >= 2)
            {
                StartTimer();
            }
        }
    }

    private void StartTimer()
    {
        timerStarted = true;
        timeRemaining.Value = gameDuration;
        InvokeRepeating(nameof(CountDown), 1f, 1f);
    }

    private void CountDown()
    {
        if (!IsServer) return;

        if (timeRemaining.Value > 0)
        {
            timeRemaining.Value -= 1f;
        }
        else
        {
            CancelInvoke(nameof(CountDown));
            EndGame();
        }
    }

    private void EndGame()
    {
        Debug.Log("Game Over!");
        // Add win/lose logic here
    }
}
