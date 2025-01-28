using System;
using System.Threading.Tasks;
using UnityEngine;

public class GameInitiator : MonoBehaviour // Responsible from initiating our instances
{
    [Header("Character Related")]
    [SerializeField] private GameObject characterPref;
    [SerializeField] private Vector3 characterInitialPosition;
    [NonSerialized] private Character character;

    [Header("AR Related")]
    [SerializeField] private Camera arCamera;

    [Header("General Game Related")]
    [SerializeField] private LoadingScreen loadingScreen;

    [Header("Bird")]
    [SerializeField] private GameObject birdPref;
    [NonSerialized] private Bird bird;

    [Header("Flock")]
    [SerializeField] private Flock flock;

    private async void Start()
    {
        await BindObjects();

        loadingScreen.Show(50);

        await PrepareScene();

        loadingScreen.Show(100);
    }

    private Task BindObjects()
    {
        character = Instantiate(characterPref).GetComponent<Character>();
        character.gameObject.transform.SetParent(arCamera.transform, false);
        character.gameObject.transform.SetPositionAndRotation(characterInitialPosition, Quaternion.identity);

        flock.SetTaregt(character.transform);

        loadingScreen = new LoadingScreen();

        return Task.CompletedTask; // No background task needed
    }

    private Task PrepareScene()
    {
        // Unity operations: Call methods directly

        character.InitAnimatorManager();
        character.EnableAnimatorManager();

        character.InitFirearm(arCamera);
        character.EnableFirearm();

        //bird.Init(character.transform);
        flock.Init(10);

        return Task.CompletedTask; // No background task needed
    }

}
