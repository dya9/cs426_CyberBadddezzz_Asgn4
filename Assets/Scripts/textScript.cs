using UnityEngine;
using TMPro;

public class JoinCodeDisplay : MonoBehaviour
{
    private void Start()
    {
        TMP_Text textComponent = GetComponent<TMP_Text>();

        if (GameManager.Instance != null)
        {
            textComponent.text = GameManager.Instance.joinCode;
        }
        else
        {
            Debug.LogWarning("GameManager not found!");
        }
    }
}
