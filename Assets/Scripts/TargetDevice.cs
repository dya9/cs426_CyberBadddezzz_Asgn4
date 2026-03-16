// using UnityEngine;
// using Unity.Netcode;

// public class TargetDevice : NetworkBehaviour
// {
//     public enum DeviceType
//     {
//         Input,
//         Output
//     }

//     public DeviceType deviceType;

//     [Header("Linked Device (Input ↔ Output)")]
//     public TargetDevice linkedDevice;

//     private bool _isDestroyed = false;
//     public bool IsDestroyed => _isDestroyed;

//     private void OnTriggerEnter(Collider other)
//     {
//         if (!IsServer) return;
//         if (_isDestroyed) return;

//         if (other.CompareTag("Bullet"))
//         {
//             DestroyDevice();

//             // Destroy bullet
//             var bulletNet = other.GetComponent<NetworkObject>();
//             if (bulletNet != null)
//                 bulletNet.Despawn(true);
//         }
//     }

// //     public void DestroyDevice()
// //     {
// //         if (_isDestroyed) return;

// //         _isDestroyed = true;

// //         // Notify door system
// //         DoorUnlocker door = FindObjectOfType<DoorUnlocker>();
// //         if (door != null)
// //         {
// //             door.CheckWinCondition();
// //         }

// //         // Despawn this device
// //         GetComponent<NetworkObject>().Despawn(true);

// //         if (linkedDevice != null && !linkedDevice.IsDestroyed)
// //         {
// //             linkedDevice._isDestroyed = true;
// //             linkedDevice.GetComponent<NetworkObject>().Despawn(true);
// //         }
// //     }
// // }
// // Update this specific part in TargetDevice.cs
// public void DestroyDevice()
// {
//     if (_isDestroyed) return;
//     _isDestroyed = true;

//     // Fix: Instead of FindObjectOfType, ensure your targets have a direct reference 
//     // or use a more reliable static instance if only one door exists.
//     DoorUnlocker door = Object.FindFirstObjectByType<DoorUnlocker>();
//     if (door != null)
//     {
//         door.CheckWinCondition();
//     }

//     if (IsServer) 
//     {
//         GetComponent<NetworkObject>().Despawn(true); //
//     }
// }
// }
using UnityEngine;
using Unity.Netcode;

public class TargetDevice : NetworkBehaviour
{
    public enum DeviceType
    {
        Input,
        Output
    }

    public DeviceType deviceType;
    public TargetDevice linkedDevice;
    public GameObject debrisPrefab; 

    private bool _isDestroyed = false;
    public bool IsDestroyed => _isDestroyed;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (_isDestroyed) return;

        if (other.CompareTag("Bullet"))
        {
            DestroyDevice();

            var bulletNet = other.GetComponent<NetworkObject>();
            if (bulletNet != null)
                bulletNet.Despawn(true);
        }
    }

    public void DestroyDevice()
    {
        if (_isDestroyed) return;
        _isDestroyed = true;

        PlayExplosionClientRpc(transform.position);

        DoorUnlocker door = Object.FindFirstObjectByType<DoorUnlocker>();
        if (door != null)
        {
            door.CheckWinCondition();
        }

        if (IsServer)
        {
            if (linkedDevice != null && !linkedDevice.IsDestroyed)
            {
                linkedDevice.DestroyDevice();
            }

            GetComponent<NetworkObject>().Despawn(true);
        }
    }

    [ClientRpc]
    private void PlayExplosionClientRpc(Vector3 position)
    {
        if (debrisPrefab != null)
        {
            Instantiate(debrisPrefab, position, Quaternion.identity);
        }
    }
}