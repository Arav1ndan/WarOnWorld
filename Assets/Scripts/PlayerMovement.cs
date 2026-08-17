using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float groundMoveSpeed = 5.0f;
    [SerializeField] private float airMoveSpeed = 6f;
    [SerializeField] private float airlerpSpeed = 0.18f;

    [Header("Jetpack")]
    [SerializeField] private float jetPackForce = 12f;
    [SerializeField] private float maxFuel = 100f;
    [SerializeField] private float fuelDrainRate = 20f;
    [SerializeField] private float fuelRechargeRate = 15f;
    [SerializeField] private float fuelRechargeDelay = 1f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private float currentFuel;
    private bool isGrounded;
    private bool isUsingJetpack;
    private float lastFuelUseTime;

    private Vector2 moveInput;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentFuel = 0;
    }
    void Update()
    {
        ReachargeFuel();
    }
    private void FixedUpdate()
    {
        HandleMovement();
        HandleJetPack();
    }
    public void MovePlayer(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();   
    }
    void HandleMovement()
    {
        if (isGrounded)
        {
            float targetX = moveInput.x * groundMoveSpeed;
            rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, targetX, 0.2f), rb.linearVelocity.y);
        }
        else
        {
            float targetX = moveInput.x * airMoveSpeed;
            float targetY = moveInput.y < 0 ? rb.linearVelocity.y + (moveInput.y *  airMoveSpeed * 1.5f * Time.fixedDeltaTime) : rb.linearVelocity.y;

            rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, targetX, airlerpSpeed), 
                                Mathf.Lerp(rb.linearVelocity.y, targetY, airlerpSpeed));
        }
    }

    void HandleJetPack()
    {
        isUsingJetpack = !isGrounded && moveInput.y > 0.1f && currentFuel > 0;

        if(isUsingJetpack)
        {
            float targetY = jetPackForce * moveInput.y;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Lerp(rb.linearVelocity.y, targetY, airlerpSpeed));

            currentFuel -= fuelDrainRate * Time.deltaTime;
            currentFuel = Mathf.Max(currentFuel, 0);
            lastFuelUseTime = Time.time;    
        }
    }
    void ReachargeFuel()
    {
        bool delayPassed =  Time.time - lastFuelUseTime > fuelRechargeDelay;
        bool canRecharge = isGrounded || (!isGrounded && delayPassed);

        if (canRecharge && currentFuel < maxFuel)
        {
            currentFuel += fuelRechargeRate * Time.deltaTime;
            currentFuel = Mathf.Min(currentFuel, maxFuel);
        }
    }
    public float GetFuelPercent() => currentFuel / maxFuel;
    public bool IsUsingJetPack() => isUsingJetpack;


}
