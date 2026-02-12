using UnityEngine;
using UnityEngine.InputSystem;

public class OpeningManager : MonoBehaviour
{
    public static OpeningManager Instance;

    public Player playerScript;
    public GameObject introCanvas;

    public Animator introAnimator;
    public string animationStateName = "TestOpening";

    private bool isAnimationFinished = false;
    private bool hasStarted = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (playerScript != null) playerScript.gameObject.SetActive(false);
        if (introCanvas != null) introCanvas.SetActive(false);
    }

    public void BeginIntro()
    {
        if (introCanvas != null) introCanvas.SetActive(true);
        hasStarted = true;

        if (introAnimator != null)
        {
            introAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            introAnimator.Play(animationStateName, 0, 0f);
        }
    }

    void Update()
    {
        if (!hasStarted || introAnimator == null) return;

        AnimatorStateInfo state = introAnimator.GetCurrentAnimatorStateInfo(0);

        if (state.IsName(animationStateName) && state.normalizedTime >= 0.98f && !introAnimator.IsInTransition(0))
        {
            if (!isAnimationFinished)
            {
                isAnimationFinished = true;
            }
        }

        if (isAnimationFinished)
        {
            bool isKeyboardPressed = Keyboard.current != null && Keyboard.current.jKey.wasPressedThisFrame;
            bool isGamepadPressed = Gamepad.current != null && Gamepad.current.aButton.wasPressedThisFrame;

            if (isKeyboardPressed || isGamepadPressed)
            {
                StartGame();
            }
        }
    }

    void StartGame()
    {
        if (introCanvas != null) introCanvas.SetActive(false);

        if (playerScript != null)
        {
            playerScript.gameObject.SetActive(true);
            playerScript.ResetState();
        }

        Destroy(gameObject);
    }
}