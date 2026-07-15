using UnityEngine;
using UnityEngine.VFX;

public class FlameThrower : MonoBehaviour
{
    public VisualEffect flameThrower;

    [SerializeField]
    private float moveSpeed = 10f;

    [SerializeField]
    private float rotateSpeed = 10f;

    private InputSystem_Actions input;

    private void Awake()
    {
        input = new InputSystem_Actions();
    }

    void Start()
    {
        if (flameThrower != null)
        {
            flameThrower.SetFloat("VelocityZ", 30f);

            Gradient flameGrad = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[4];
            colorKeys[0] = new GradientColorKey(new Color(8f, 8f, 2f), 0.0f);
            colorKeys[1] = new GradientColorKey(new Color(8f, 2f, 0f), 0.2f);
            colorKeys[2] = new GradientColorKey(new Color(2f, 0.2f, 0f), 0.5f);
            colorKeys[3] = new GradientColorKey(new Color(0f, 0f, 0f), 1.0f);

            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[4];
            alphaKeys[0] = new GradientAlphaKey(0.0f, 0.0f);
            alphaKeys[1] = new GradientAlphaKey(1.0f, 0.1f);
            alphaKeys[2] = new GradientAlphaKey(0.8f, 0.8f);
            alphaKeys[3] = new GradientAlphaKey(0.0f, 1.0f);

            flameGrad.SetKeys(colorKeys, alphaKeys);
            flameThrower.SetGradient("FlameThrowerGradient", flameGrad);
        }
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        if (input.Player.Attack.WasPressedThisFrame())
        {
            flameThrower.Play();
        }
        if (input.Player.Attack.WasReleasedThisFrame())
        {
            flameThrower.Stop();
        }
    }
}
