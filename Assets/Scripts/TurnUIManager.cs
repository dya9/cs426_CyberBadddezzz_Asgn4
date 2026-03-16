using UnityEngine;
using TMPro;
using Unity.Netcode;

public class TurnUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text turnStatusText;

    private void OnEnable()
    {
        // Subscribe to value changes so we only update the UI when the turn actually switches
        PlayerMovement.CurrentTurnClientId.OnValueChanged += UpdateTurnUI;
    }

    private void OnDisable()
    {
        // Clean up subscription to avoid memory leaks
        PlayerMovement.CurrentTurnClientId.OnValueChanged -= UpdateTurnUI;
    }

    private void Start()
    {
        // Set the initial text
        if (NetworkManager.Singleton != null)
        {
            UpdateTurnUI(0, PlayerMovement.CurrentTurnClientId.Value);
        }
    }

    private void UpdateTurnUI(ulong previousId, ulong newId)
    {
        if (turnStatusText == null) return;

        // Check if the new turn ID matches this local player's ID
        if (newId == NetworkManager.Singleton.LocalClientId)
        {
            turnStatusText.text = "YOUR TURN";
            turnStatusText.color = Color.green;
        }
        else
        {
            // You can show the ID of the player currently shooting
            turnStatusText.text = $"PLAYER {newId}'S TURN";
            turnStatusText.color = Color.red;
        }
    }
}