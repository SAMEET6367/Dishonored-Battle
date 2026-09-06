using UnityEngine;
using System.Collections;

public class CameraShakeWhileRunning : MonoBehaviour
{
    public static CameraShakeWhileRunning instance;

    [Header("Running Shake Settings")]
    public float shakeMagnitude = 0.05f;
    public float shakeFrequency = 10f;

    [Header("Heavy Attack Shake")]
    public float heavyShakeMagnitude = 0.3f;
    public float heavyShakeFrequency = 25f;
    public float heavyShakeDuration = 0.35f;

    [Header("Animator Settings")]
    public string runningBoolName = "IsRunning";
    public Transform playerTransform;

    private Animator playerAnimator;
    private Vector3 initialLocalPosition;

    private bool heavyShaking = false;
    private Coroutine heavyShakeRoutine;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        initialLocalPosition = transform.localPosition;

        // Auto find player if not assigned
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform != null)
            playerAnimator = playerTransform.GetComponentInChildren<Animator>();

        if (playerAnimator == null)
            Debug.LogWarning("CameraShakeWhileRunning: No player Animator found.");
    }

    void LateUpdate()
    {
        // Heavy attack shake overrides everything
        if (heavyShaking)
        {
            DoShake(heavyShakeMagnitude, heavyShakeFrequency);
            return;
        }

        if (playerAnimator == null)
            return;

        bool isRunning = playerAnimator.GetBool(runningBoolName);

        if (isRunning)
        {
            DoShake(shakeMagnitude, shakeFrequency);
        }
        else
        {
            transform.localPosition = initialLocalPosition;
        }
    }

    void DoShake(float magnitude, float frequency)
    {
        float offsetX = (Mathf.PerlinNoise(Time.time * frequency, 0f) - 0.5f) * 2f * magnitude;
        float offsetY = (Mathf.PerlinNoise(0f, Time.time * frequency) - 0.5f) * 2f * magnitude;

        transform.localPosition = initialLocalPosition + new Vector3(offsetX, offsetY, 0f);
    }

    // ?? Called by EnemyAI heavy attack
    public void HeavyAttackShake()
    {
        if (heavyShakeRoutine != null)
            StopCoroutine(heavyShakeRoutine);

        heavyShakeRoutine = StartCoroutine(HeavyShakeRoutine());
    }

    IEnumerator HeavyShakeRoutine()
    {
        heavyShaking = true;

        float timer = 0f;

        while (timer < heavyShakeDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        heavyShaking = false;
        transform.localPosition = initialLocalPosition;
    }
}