using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float rotationSpeed = 120f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;

    private float floorY = 1.12f;
    private float verticalVelocity = 0f;

    [Header("Player Customization")]
    public List<Color> colors = new List<Color>();

    [Header("References")]
    [SerializeField] private GameObject spawnedPrefab;
    private NetworkObject spawnedNetworkObject;

    public GameObject cannon;
    public GameObject bullet;

    [SerializeField] private AudioListener audioListener;
    [SerializeField] private Camera playerCamera;

    private Vector3 spawnPos = new Vector3(0f, 1.12f, -13.75f); 

    // Static NetworkVariable so every player instance can check who the active player is
    // Only the server can change this value, but everyone can read it
    public static NetworkVariable<ulong> CurrentTurnClientId = new NetworkVariable<ulong>(0);

    public override void OnNetworkSpawn()
    {
        // Server setting initial spawn position
        if (IsServer)
        {
            transform.position = spawnPos;
            
            // On the very first spawn, ensure the turn is set to the first player
            if (NetworkManager.Singleton.ConnectedClientsIds.Count > 0)
            {
                CurrentTurnClientId.Value = NetworkManager.Singleton.ConnectedClientsIds[0];
            }
        }

        // Apply player color based on Owner ID
        var renderer = GetComponentInChildren<MeshRenderer>();
        if (renderer != null && colors.Count > 0)
        {
            renderer.material.color = colors[(int)OwnerClientId % colors.Count];
        }

        if (!IsOwner) return;

        // Enable camera and audio only for the local player
        if (audioListener != null) audioListener.enabled = true;
        if (playerCamera != null) playerCamera.enabled = true;
    }

    void Update()
    {
        if (!IsOwner) return;

        HandleMovement();
        HandleSpawning();
        HandleFiring();
    }

    private void HandleMovement()
    {
        // Standard movement logic
        float rotationInput = 0f;
        if (Input.GetKey(KeyCode.A)) rotationInput = -1f;
        if (Input.GetKey(KeyCode.D)) rotationInput = 1f;
        transform.Rotate(Vector3.up * rotationInput * rotationSpeed * Time.deltaTime);

        float moveInput = 0f;
        if (Input.GetKey(KeyCode.W)) moveInput = 1f;
        if (Input.GetKey(KeyCode.S)) moveInput = -1f;
        Vector3 move = transform.forward * moveInput * moveSpeed;

        if (transform.position.y <= floorY + 0.01f)
        {
            verticalVelocity = 0f;
            if (Input.GetKeyDown(KeyCode.Space))
                verticalVelocity = jumpForce;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        move.y = verticalVelocity;
        transform.position += move * Time.deltaTime;
    }

    private void HandleSpawning()
    {
        if (Input.GetKeyDown(KeyCode.I))
            SpawnObjectServerRpc();

        if (Input.GetKeyDown(KeyCode.J))
            DespawnObjectServerRpc();
    }

    private void HandleFiring()
    {
        // TURN CHECK: Only allow firing if it is this player's turn
        if (Input.GetButtonDown("Fire1"))
        {
            if (NetworkManager.Singleton.LocalClientId == CurrentTurnClientId.Value)
            {
                if (cannon != null)
                    BulletSpawningServerRpc(cannon.transform.position, cannon.transform.rotation);
            }
            else
            {
                Debug.Log("Wait for your turn!");
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void BulletSpawningServerRpc(Vector3 position, Quaternion rotation)
    {
        // Standard networked spawning
        GameObject newBullet = Instantiate(bullet, position, rotation);
        NetworkObject netObj = newBullet.GetComponent<NetworkObject>();
        
        if (netObj != null)
        {
            netObj.Spawn(true);

            Rigidbody rb = newBullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.AddForce(newBullet.transform.forward * 1500f);
            }

            StartCoroutine(DespawnBulletAfterSeconds(netObj, 3f));
        }

        // PASS TURN LOGIC: Find the next player in the list of connected clients
        var clients = NetworkManager.Singleton.ConnectedClientsIds;
        for (int i = 0; i < clients.Count; i++)
        {
            if (clients[i] == CurrentTurnClientId.Value)
            {
                // Move to next client index, or wrap back to 0
                int nextIndex = (i + 1) % clients.Count;
                CurrentTurnClientId.Value = clients[nextIndex];
                break;
            }
        }
    }

    private IEnumerator DespawnBulletAfterSeconds(NetworkObject netObj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        // FIX for your error: Ensure the object wasn't already despawned by a target hit
        if (netObj != null && netObj.IsSpawned)
            netObj.Despawn(true);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnObjectServerRpc()
    {
        if (spawnedNetworkObject != null) return;
        GameObject obj = Instantiate(spawnedPrefab);
        NetworkObject netObj = obj.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn(true);
            spawnedNetworkObject = netObj;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnObjectServerRpc()
    {
        if (spawnedNetworkObject != null && spawnedNetworkObject.IsSpawned)
        {
            spawnedNetworkObject.Despawn(true);
            spawnedNetworkObject = null;
        }
    }
}