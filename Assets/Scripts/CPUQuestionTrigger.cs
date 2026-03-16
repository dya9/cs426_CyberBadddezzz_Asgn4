using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.Collections;

public class CPUQuestionTrigger : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private TMP_Text questionText;

    [Header("Question Settings")]
    [TextArea]
    public string question = "How many bytes are in an int?";

    public float typingSpeed = 0.05f;

    private bool questionShown = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (questionShown) return;
        if (!other.CompareTag("Player")) return;

        questionShown = true;

        ulong clientId = other.GetComponent<NetworkObject>().OwnerClientId;
        ShowQuestionClientRpc(clientId);
    }

    [ClientRpc]
    private void ShowQuestionClientRpc(ulong targetClientId)
    {
        if (NetworkManager.Singleton.LocalClientId != targetClientId)
            return;

        if (questionPanel != null)
            questionPanel.SetActive(true);

        if (questionText != null)
            StartCoroutine(TypeWriterEffect());
    }

    private IEnumerator TypeWriterEffect()
    {
        questionText.text = "";

        foreach (char letter in question.ToCharArray())
        {
            questionText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
