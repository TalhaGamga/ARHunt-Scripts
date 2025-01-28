using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARFoundationChecker : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(CheckARSupportCoroutine());
    }

    private IEnumerator CheckARSupportCoroutine()
    {
        Debug.Log("Initial ARSession state: " + ARSession.state);

        Debug.Log("Checking AR availability...");
        yield return ARSession.CheckAvailability();

        Debug.Log("ARSession state after CheckAvailability: " + ARSession.state);

        if (ARSession.state == ARSessionState.Unsupported)
        {
            Debug.LogError("AR is not supported on this device.");
            HandleUnsupportedDevice();
            yield break;
        }

        if (ARSession.state == ARSessionState.NeedsInstall)
        {
            Debug.Log("AR support is available but AR services need to be installed.");
            yield return ARSession.Install();

            Debug.Log("ARSession state after Install: " + ARSession.state);

            if (ARSession.state != ARSessionState.Ready)
            {
                Debug.LogError("Failed to install AR services.");
                HandleUnsupportedDevice();
                yield break;
            }
        }

        if (ARSession.state == ARSessionState.Ready)
        {
            Debug.Log("AR support is ready!");
            StartARSession();
        }
        else
        {
            Debug.LogError("Unexpected ARSession state: " + ARSession.state);
        }
    }

    private void HandleUnsupportedDevice()
    {
        // Handle devices that do not support AR
        Debug.LogError("AR is unsupported on this device.");
    }

    private void StartARSession()
    {
        Debug.Log("Starting AR session...");
        // Start your AR session logic here
    }

    private void HandleSupportedDevice()
    {
        // Logic for AR-supported devices, e.g., start ARSession
        Debug.Log("AR is supported, proceeding...");
    }
}
