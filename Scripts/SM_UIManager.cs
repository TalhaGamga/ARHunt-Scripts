using TMPro;
using UnityEngine;

public class SM_UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI killCounter;

    [SerializeField] private int kills = 0; // Tracks the number of kills

    private void OnEnable()
    {
        EventManager.Instance.OnBirdDied += handleOnBirdKilled;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnBirdDied -= handleOnBirdKilled;
    }

    public void handleFireButton()
    {
        InputTriggerContainer.Instance?.FireTrigger?.OnTrigger?.Invoke(null);
    }

    private void handleOnBirdKilled()
    {
        kills++;
        Debug.Log("kiled");

        UpdateKillCounterText();
    }

    private void UpdateKillCounterText()
    {
        if (killCounter != null)
        {
            killCounter.text = "Kill: " + kills.ToString();
        }
    }
}
