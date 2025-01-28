using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class UIManager : MonoBehaviour
{
    [Header("UI Parents")]
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject XRUI;
    [SerializeField] private GameObject menuUIOnOffButton;

    [Header("XR References")]
    [SerializeField] private GameObject XR;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private ARSession arSession;
    [SerializeField] private ARTrackedImageManager imageManager;

    private BasketSensor basketSensor;


    private bool isBound = false;

    public void BindIncrementScoreToBasketSensor()
    {
        if (isBound)
        {
            return;
        }

        basketSensor = XR.GetComponentInChildren<BasketSensor>();
        basketSensor.OnBasket = IncrementScore;
        isBound = true;
    }

    private void OnDestroy()
    {
        if (basketSensor == null || (basketSensor != null && basketSensor.OnBasket == null))
        {
            return;
        }

        basketSensor.OnBasket -= IncrementScore;
    }

    void IncrementScore()
    {
        int currentScore;

        if (int.TryParse(scoreText.text, out currentScore))
        {
            currentScore++;
            scoreText.text = currentScore.ToString();
        }

        else
        {
            currentScore = 1;
            scoreText.text = currentScore.ToString();
        }
    }

    public void Play()
    {
        XR.SetActive(true);
        XRUI.SetActive(true);

        menuUI.SetActive(false);

        if (!menuUIOnOffButton.activeInHierarchy)
        {
            menuUIOnOffButton.SetActive(true);
        }
    }

    public void Restart()
    {
        if (arSession != null)
        {
            arSession.Reset();

            Debug.Log("AR Session has been reset.");
        }
        else
        {
            Debug.LogError("ARSession is not assigned in the Inspector.");
        }


        foreach (var trackedImage in imageManager.trackables)
        {
            Destroy(trackedImage.gameObject);
        }

        Debug.Log("Tracked images cleared.");
    }

    public void Exit()
    {
        XR.SetActive(false);
        XRUI.SetActive(false);

        menuUI.SetActive(true);

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        // If running in a build, quit the application
        Application.Quit();
#endif
    }

    public void ManageMenuOnOff()
    {
        if (menuUI.activeInHierarchy)
        {
            menuUI.SetActive(false);
            XR.SetActive(true);
            XRUI.SetActive(true);
            menuUIOnOffButton.SetActive(true);

            return;
        }

        XR.SetActive(false);
        XRUI.SetActive(false);
        menuUI.SetActive(true);
        menuUIOnOffButton.SetActive(false);
    }
}
