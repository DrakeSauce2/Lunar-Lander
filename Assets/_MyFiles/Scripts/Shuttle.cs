using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shuttle : MonoBehaviour
{
    IA_Player playerInput;
    Rigidbody2D rBody;

    [Header("Start")]
    [SerializeField] private float startLaunchForce = 1000f;

    [Header("Stats")]
    [SerializeField] private float thrustForce = 5.0f;
    [SerializeField] private float rollRate = 2.5f;
    [SerializeField] private float fuel = 0;
    [SerializeField] private float fuelConsumptionSpeed = 0.1f;

    [Header("Colliders")]
    [SerializeField] private GameObject sideCollider;
    [SerializeField] private GameObject bottomCollider;

    [Header("Debug")]
    [SerializeField] private float groundCheckDistance = 100f;
    [SerializeField] private LayerMask layermask;

    [Header("Visual")]
    [SerializeField] private Transform flamesTransform;

    private float beforeLandingYVel = 0;

    public delegate void OnDeath(float fuelRemaining);
    public OnDeath onDeath;

    bool bIsDead = false;
    bool bWasSuccessful = false;

    private void Awake()
    {
        rBody = GetComponent<Rigidbody2D>();

        playerInput = new IA_Player();
        playerInput.Enable();

        sideCollider.SetActive(true);
        bottomCollider.SetActive(true);

        bWasSuccessful = false;
        bIsDead = false;

        SimpleCameraFollow.Instance.SetNewPlayer(this);
    }

    private void Start()
    {
        rBody.AddForce(transform.right * startLaunchForce, ForceMode2D.Force);

        fuel = GameManager.Instance.currentFuel;
    }

    private void Update()
    {
        CheckGround();   
    }

    private void FixedUpdate()
    {
        ProcessRoll();

        ApplyThrust();

        GameManager.Instance.SetAltitudeText(transform.position.y);
        GameManager.Instance.SetSpeedText(rBody.velocity.x, -rBody.velocity.y);
    }

    private void ProcessRoll()
    {
        float rollValue = InputManager.Instance.GetRollInput();

        if (rollValue == 0) return;

        float rollAngle = rollRate * rollValue;

        transform.Rotate(Vector3.forward, rollAngle);

        float currentZRotation = transform.localEulerAngles.z;

        if (currentZRotation > 180)
            currentZRotation -= 360;

        float clampedZRotation = Mathf.Clamp(currentZRotation, -90f, 90f);

        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, clampedZRotation);
    }

    private void ApplyThrust()
    {
        float thrustValue = InputManager.Instance.GetThrustInput();

        if (fuel <= 0)
        {
            AudioManager.Instance.StopThrustSound();

            flamesTransform.localScale = Vector3.Lerp(flamesTransform.localScale, new Vector3(0.1f, 0, 0.1f), 5f * Time.fixedDeltaTime);

            return;
        }


        if (thrustValue <= 0)
        {
            AudioManager.Instance.StopThrustSound();

            flamesTransform.localScale = Vector3.Lerp(flamesTransform.localScale, new Vector3(0.1f, 0, 0.1f), 5f * Time.fixedDeltaTime);

            return;
        }
        else
        {
            flamesTransform.localScale = Vector3.Lerp(flamesTransform.localScale, new Vector3(0.1f, 0.275f, 0.1f) * thrustValue, 5f * Time.fixedDeltaTime);

            AudioManager.Instance.PlayThrustSound();

            rBody.AddForce(transform.up * thrustForce * Time.fixedDeltaTime);
            fuel -= fuelConsumptionSpeed * Time.deltaTime;
            GameManager.Instance.UpdateFuel(fuel);
        }

    }

    private void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector3.up, groundCheckDistance, layermask);
        if (hit.collider)
        {
            GameManager.Instance.SetCameraClose(new Vector3(hit.point.x, hit.point.y + (groundCheckDistance/4f), -10));

            transform.localScale = new Vector3(5, 5, 5);

            float distance = Vector3.Distance(transform.position, hit.point);
            if (distance < 50f && distance > 0.1f)
            {
                beforeLandingYVel = -rBody.velocity.y;
            }
        }
        else
        {
            GameManager.Instance.SetCameraFar();

            transform.localScale = new Vector3(15, 15, 15);

        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + -Vector3.up * groundCheckDistance);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bottomCollider.SetActive(false);
        sideCollider.SetActive(false);

        AudioManager.Instance.StopThrustSound();

        if (transform.localEulerAngles.z > 2 || transform.localEulerAngles.z < -2 || beforeLandingYVel > 20f || rBody.velocity.magnitude > 10f)
        {
            if (bWasSuccessful == true || bIsDead == true) return;

            AudioManager.Instance.PlayDeathSound();

            bIsDead = true;
            onDeath.Invoke(fuel);

            Debug.Log("Death Flag");

            Destroy(gameObject);

            return;
        }
        else
        {
            if (bWasSuccessful == true || bIsDead == true) return;

            bWasSuccessful = true;
            
            int multiplier = 1;

            foreach (ContactPoint2D contact in collision.contacts)
            {
                LandingZone landingZone = contact.collider.gameObject.GetComponent<LandingZone>();
                if (landingZone)
                {
                    multiplier = landingZone.GetScoreMultiplier();
                    break;
                }
            }

            // Gain 50 Fuel for each successful landing
            StartCoroutine(GameManager.Instance.LandingCoroutine(gameObject, multiplier, fuel + 50));
        }
    }

}
