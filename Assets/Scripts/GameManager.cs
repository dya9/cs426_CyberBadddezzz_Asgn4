// using UnityEngine;

// public class GameManager : MonoBehaviour
// {
//     public static GameManager Instance;

//     public string joinCode;

//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }
// }

using UnityEngine;
using Unity.Netcode; // Required for NetworkBehaviour and NetworkVariables

public class GameManager : NetworkBehaviour // Changed from MonoBehaviour to NetworkBehaviour
{
    public static GameManager Instance;

    public string joinCode;

    // Synchronized variables
    // TimeRemaining tracks the 45-second countdown across all clients
    public NetworkVariable<float> TimeRemaining = new NetworkVariable<float>(45f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    // IsGameActive determines if players are allowed to shoot
    public NetworkVariable<bool> IsGameActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Reset the timer and game state whenever the network starts
            TimeRemaining.Value = 45f;
            IsGameActive.Value = false;

            // Start the game timer only when a second player (client) connects
            NetworkManager.Singleton.OnClientConnectedCallback += CheckPlayerCount;
        }
    }

    private void CheckPlayerCount(ulong clientId)
    {
        // Total players >= 2 (Host + at least one Client)
        if (NetworkManager.Singleton.ConnectedClients.Count >= 2)
        {
            IsGameActive.Value = true;
        }
    }

    private void Update()
    {
        // Only the server calculates the time to ensure it remains identical for everyone
        if (!IsServer || !IsGameActive.Value) return;

        if (TimeRemaining.Value > 0)
        {
            TimeRemaining.Value -= Time.deltaTime;
        }
        else
        {
            TimeRemaining.Value = 0;
            IsGameActive.Value = false;
            Debug.Log("Game Over: 45 seconds have passed!");
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= CheckPlayerCount;
        }
    }
}