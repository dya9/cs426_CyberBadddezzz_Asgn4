using UnityEngine;
using Unity.Netcode;

public class MemoryPickup : NetworkBehaviour
{
    [Header("CPU Door Reference")]
    public GameObject cpuDoor;   // Assign in Inspector

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;           // Server handles logic
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;

            OpenCpuDoor();
            DespawnMemory();
        }
    }

    private void OpenCpuDoor()
    {
        if (cpuDoor != null)
        {
            NetworkObject netObj = cpuDoor.GetComponent<NetworkObject>();

            if (netObj != null && netObj.IsSpawned)
            {
                netObj.Despawn(true);   // Removes door for everyone
            }
            else
            {
                Destroy(cpuDoor);
            }

            Debug.Log("Memory collected — CPU Door Opened!");
        }
    }

    private void DespawnMemory()
    {
        NetworkObject netObj = GetComponent<NetworkObject>();

        if (netObj != null && netObj.IsSpawned)
        {
            netObj.Despawn(true);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
