using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
using Unity.Collections; 

public class NetworkManagerUI : NetworkBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button host_btn;
    [SerializeField] private Button client_btn;
    [SerializeField] private TMP_Text joinCodeText;
    [SerializeField] private TMP_InputField joinCodeInputField;

    [Header("Settings")]
    [SerializeField] private string gameplaySceneName = "Assignment02";
    [SerializeField] private int maxPlayers = 4;

    // NetworkVariable syncs the string automatically to all players.
    public NetworkVariable<FixedString32Bytes> NetworkJoinCode = 
        new NetworkVariable<FixedString32Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    //specifiying spwan point(needs to be middle of map for fairness)
    private Vector3 spawnPos = new Vector3(0f, 1.12f, -13.75f);

    private void Awake()
    {
        //preventing destruction when the scene loads
        DontDestroyOnLoad(gameObject);

        host_btn.onClick.AddListener(() =>
        {
            StartHostRelay();
        });

        client_btn.onClick.AddListener(() =>
        {
            if (joinCodeInputField != null)
            {
                StartClientRelay(joinCodeInputField.text);
            }
        });
    }

    private async void Start()
    {
        try
        {
            //sign in
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Services Initialization Error: {e}");
        }
    }

    public async void StartHostRelay()
    {
        Allocation allocation = null;
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            //setting the NetworkVariable value (Server,Host)
            NetworkJoinCode.Value = joinCode;
            GameManager.Instance.joinCode = joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return;
        }

        var serverData = allocation.ToRelayServerData("dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);

        if (NetworkManager.Singleton.StartHost())
        {
            //updating local UI if it exists in this scene
            if (joinCodeText != null) joinCodeText.text = NetworkJoinCode.Value.ToString();

            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
            NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
        }
    }

    public async void StartClientRelay(string codeToJoin)
    {
        JoinAllocation joinAllocation = null;
        try
        {
            joinAllocation = await RelayService.Instance.JoinAllocationAsync(codeToJoin);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return;
        }

        var serverData = joinAllocation.ToRelayServerData("dtls");
        //seting relay data
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);
        //joining as client
        NetworkManager.Singleton.StartClient();
    }

    private void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, 
    System.Collections.Generic.List<ulong> clientsCompleted, 
    System.Collections.Generic.List<ulong> clientsTimedOut)
{
    if (NetworkManager.Singleton.IsHost)
    {
        var playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
        if (playerObject != null)
        {
            playerObject.transform.position = spawnPos;
        }

        // 🔹 Find the join code text in the NEW scene
        TMP_Text newJoinCodeText = FindFirstObjectByType<TMP_Text>();

        if (newJoinCodeText != null)
        {
            newJoinCodeText.text = NetworkJoinCode.Value.ToString();
        }

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnSceneLoaded;
    }
}

}

