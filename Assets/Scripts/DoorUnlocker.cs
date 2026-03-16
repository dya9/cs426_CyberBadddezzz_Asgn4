// using UnityEngine;
// using Unity.Netcode;

// public class DoorUnlocker : NetworkBehaviour
// {
//     [Header("Input Devices (Left - Hacker Objective)")]
//     public TargetDevice[] inputDevices;

//     [Header("Output Devices (Right - Defender Objective)")]
//     public TargetDevice[] outputDevices;

//     private bool gameDecided = false;

//     public void CheckWinCondition()
//     {
//         if (!IsServer) return;
//         if (gameDecided) return;

//         bool allInputsDestroyed = true;
//         bool allOutputsDestroyed = true;

//         foreach (var device in inputDevices)
//         {
//             if (device != null && !device.IsDestroyed)
//             {
//                 allInputsDestroyed = false;
//                 break;
//             }
//         }

//         foreach (var device in outputDevices)
//         {
//             if (device != null && !device.IsDestroyed)
//             {
//                 allOutputsDestroyed = false;
//                 break;
//             }
//         }

//         if (allInputsDestroyed)
//         {
//             gameDecided = true;
//             UnlockDoor();
//             Debug.Log("ATTACKER WINS — Door Unlocked");
//         }
//         else if (allOutputsDestroyed)
//         {
//             gameDecided = true;
//             Debug.Log("DEFENDER WINS — Door Locked");
//             // Do nothing — door stays
//         }
//     }

//     private void UnlockDoor()
//     {
//         NetworkObject netObj = GetComponent<NetworkObject>();
//         if (netObj != null)
//         {
//             netObj.Despawn(true);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }
// }
using UnityEngine;
using Unity.Netcode;

public class DoorUnlocker : NetworkBehaviour
{
    [Header("Input Devices (Left - Hacker Objective)")]
    public TargetDevice[] inputDevices;

    [Header("Output Devices (Right - Defender Objective)")]
    public TargetDevice[] outputDevices;

    private bool gameDecided = false;

    // Call this from TargetDevice to trigger a re-check
    public void CheckWinCondition()
    {
        if (!IsServer || gameDecided) return; //

        if (AllDevicesDestroyed(inputDevices))
        {
            gameDecided = true;
            Debug.Log("ATTACKER WINS — Door Unlocked");
            UnlockDoor(); //
        }
        else if (AllDevicesDestroyed(outputDevices))
        {
            gameDecided = true;
            Debug.Log("DEFENDER WINS — Door Locked");
            // Logic for defender win (e.g., end game screen)
        }
    }

    private bool AllDevicesDestroyed(TargetDevice[] devices)
    {
        foreach (var device in devices)
        {
            // Check if the device still exists or if it has been marked destroyed
            if (device != null && !device.IsDestroyed)
            {
                return false;
            }
        }
        return true;
    }

    private void UnlockDoor()
    {
        NetworkObject netObj = GetComponent<NetworkObject>();
        if (netObj != null && netObj.IsSpawned)
        {
            netObj.Despawn(true); // Removes the door for everyone
        }
    }
}
