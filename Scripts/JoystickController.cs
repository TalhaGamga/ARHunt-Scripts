using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private RectTransform joystickBackground; 
    [SerializeField] private RectTransform joystickHandle;     

    private Vector2 inputDirection = Vector2.zero; 
    private bool isJoystickActive = false;

    public float maxDistance = 50f; 

    [SerializeField] Projection projection;
    [SerializeField] private Ball ballPrefab;
    [SerializeField] private float force;

    [SerializeField] private Transform ballSpawn;
    [SerializeField] private ParticleSystem launchParticles;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;

    private float distance;
    Vector2 direction;

    [SerializeField] float timeScale;

    void Start()
    {
        joystickBackground.gameObject.SetActive(false);
        Time.timeScale = timeScale;
    }

    private void Update()
    {
        projection.SimulateTrajectory(ballPrefab, ballSpawn.position, ballSpawn.forward.normalized * force);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        joystickBackground.position = eventData.position;
        joystickBackground.gameObject.SetActive(true);
        isJoystickActive = true;
        projection._line.enabled = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isJoystickActive) return;
        Vector2 currentPosition = eventData.position;
        Vector2 center = (Vector2)joystickBackground.position;

        Vector2 aimVector = (center - currentPosition);

        aimVector = Vector2.ClampMagnitude(aimVector, maxDistance);
        direction = aimVector.normalized;

        float distance = Vector2.Distance(joystickHandle.anchoredPosition, Vector2.zero);

        force = distance * 2f;

        joystickHandle.anchoredPosition = aimVector;

        inputDirection = aimVector / maxDistance;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickHandle.anchoredPosition = Vector2.zero;
        inputDirection = Vector2.zero;
        joystickBackground.gameObject.SetActive(false);
        isJoystickActive = false;

        Fire();

        projection._line.enabled = false;
        force = 0;
    }

    public Vector2 GetInputDirection()
    {
        return inputDirection; 
    }

    public void Fire()
    {
        var spawned = Instantiate(ballPrefab, ballSpawn.position, ballSpawn.rotation);

        spawned.Init(ballSpawn.forward * force, false);
        launchParticles.Play();
        source.PlayOneShot(clip);
    }
}
