using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System;
using UnityEngine.XR;

/*
 * You Get 50 Points Base per Landing 
 */

[Serializable]
public class LandingZonePositions
{
    public GameObject landingZone;
    public GameObject environmentGround;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Start")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] GameObject startUI;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI gameOverText;

    [Header("Landing Zones")]
    [SerializeField] private List<LandingZonePositions> landingZoneList = new List<LandingZonePositions>();

    [Header("Death")]
    [SerializeField] private GameObject DeathUI;
    [SerializeField] private float respawnTime;
    [SerializeField] private TextMeshProUGUI deathText;

    [Header("Camera")]
    [SerializeField] private Camera mainCam;
    private Vector3 startPos;
    [SerializeField] private float farSize;
    [SerializeField] private float nearSize;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private float currentScore = 0f;

    [Header("Fuel")]
    [SerializeField] private TextMeshProUGUI fuelText;
    public float currentFuel { get; private set;}
    [SerializeField] private float startFuel;
    [SerializeField] private TextMeshProUGUI lowFuelText;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private float currentTime = 0f;
    [SerializeField] private bool isTimerRunning = false;

    [Header("Velocity")]
    [SerializeField] private TextMeshProUGUI horizontalSpeedText;
    [SerializeField] private TextMeshProUGUI verticalSpeedText;

    [Header("Landing")]
    [SerializeField] private GameObject landingPanel;
    [SerializeField] private TextMeshProUGUI landingText;
    [SerializeField] private float landingResetTime = 3f;

    [Header("Altitude")]
    [SerializeField] private TextMeshProUGUI altitudeText;

    bool bGameStarted = false;
    bool bHasLanded = false;
    bool bCameraClose = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        currentFuel = startFuel;

        startPos = mainCam.transform.position;
        mainCam.orthographicSize = farSize;

        UpdateFuel(currentFuel);
    }

    public bool GetCameraClose()
    {
        return bCameraClose;
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            currentTime += Time.deltaTime;
            UpdateTimerText();

            if (currentFuel <= startFuel / 2.5)
            {
                lowFuelText.gameObject.SetActive(true);
                AudioManager.Instance.PlayLowFuelSound();
            }
            else
            {
                lowFuelText.gameObject.SetActive(false);
                AudioManager.Instance.StopLowFuelSound();
            }
        }
    }

    private void ResetGame()
    {
        ResetCamera();

        AudioManager.Instance.StopLowFuelSound();

        lowFuelText.gameObject.SetActive(false);

        currentScore = 0;
        currentFuel = 0;
        currentTime = 0;

        UpdateTimerText();
        UpdateFuel(0);
        SetAltitudeText(0);
        SetSpeedText(0, 0);

        bGameStarted = false;
        isTimerRunning = false;
    }

    private void PlayerDeath(float fuelRemaining)
    {
        currentFuel = fuelRemaining;

        float fuelLoss = UnityEngine.Random.Range(200, 275);
        Mathf.RoundToInt(fuelLoss);

        currentFuel -= fuelLoss;

        if (currentFuel <= 0)
        {
            StartCoroutine(GameOverCorourine());

            return;
        }

        deathText.text = string.Format("AUXILARY FUEL\nTANKS DESTROYED\n{0} FUEL UNITS LOST", fuelLoss);

        StartCoroutine(RespawnCoroutine());

    }

    private IEnumerator GameOverCorourine()
    {
        gameOverUI.SetActive(true);
        lowFuelText.gameObject.SetActive(false);

        for (int i = 0; i < landingZoneList.Count - 1; i++)
        {
            landingZoneList[i].landingZone.GetComponent<LandingZone>().GetBoxCollider().enabled = false;
            landingZoneList[i].landingZone.GetComponent<LandingZone>().GetInstancedTextObj().text = "";
            landingZoneList[i].environmentGround.SetActive(true);
        }

        AudioManager.Instance.StopLowFuelSound();

        yield return new WaitForSeconds(3f);

        AudioManager.Instance.StopLowFuelSound();

        gameOverUI.SetActive(false);
        startUI.SetActive(true);

        ResetGame();
    }

    private IEnumerator RespawnCoroutine()
    {
        DeathUI.SetActive(true);
        lowFuelText.gameObject.SetActive(false);

        AudioManager.Instance.StopLowFuelSound();

        isTimerRunning = false;

        yield return new WaitForSeconds(respawnTime);

        isTimerRunning = true;

        UpdateFuel(currentFuel);

        ResetCamera();

        Respawn();

        DeathUI.SetActive(false);
    }

    public bool GetHasLanded()
    {
        return bHasLanded;
    }

    public IEnumerator LandingCoroutine(GameObject owningObject, int multiplier, float fuelRemaining)
    {
        landingPanel.SetActive(true);
        lowFuelText.gameObject.SetActive(false);

        AudioManager.Instance.StopLowFuelSound();

        float scoreGain = 50f * multiplier;

        landingText.text = string.Format("CONGRATULATIONS\nA PERFECT LANDING\n{0} POINTS", scoreGain);

        currentScore += scoreGain;

        isTimerRunning = false;
        bHasLanded = true;

        yield return new WaitForSeconds(landingResetTime);

        isTimerRunning = true;
        bHasLanded = false;

        currentFuel = fuelRemaining;
        UpdateFuel(currentFuel);

        ResetCamera();

        UpdateScore();

        Destroy(owningObject);
        Respawn();

        landingPanel.SetActive(false);
    }

    public void StartGame()
    {
        if (bGameStarted) return;

        startUI.SetActive(false);

        currentFuel = startFuel;
        UpdateFuel(currentFuel);

        ResetCamera();

        Respawn();

        isTimerRunning = true;
        bGameStarted = true;
    }

    private void UpdateScore()
    {
        scoreText.text = string.Format("SCORE {0}", currentScore.ToString("0000"));
    }

    private void Respawn()
    {
        for (int i = 0; i < landingZoneList.Count - 1; i++)
        {
            landingZoneList[i].landingZone.GetComponent<LandingZone>().GetBoxCollider().enabled = false;
            landingZoneList[i].landingZone.GetComponent<LandingZone>().GetInstancedTextObj().text = "";
            landingZoneList[i].environmentGround.SetActive(true);
        }

        Shuttle playerShuttle = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity).GetComponent<Shuttle>();
        if (playerShuttle)
        {
            playerShuttle.onDeath += PlayerDeath;
        }

        for (int i = 3; i > 0; i--)
        {
            int rand = UnityEngine.Random.Range(0, landingZoneList.Count);

            landingZoneList[rand].landingZone.GetComponent<LandingZone>().GetBoxCollider().enabled = true;
            landingZoneList[rand].landingZone.GetComponent<LandingZone>().GetInstancedTextObj().text = "" + landingZoneList[rand].landingZone.GetComponent<LandingZone>().GetScoreMultiplier() + "X";
            landingZoneList[rand].environmentGround.SetActive(false);
        }
    }

    public void SetCameraClose(Vector3 closePosition)
    {
        bCameraClose = true;

        mainCam.transform.position = closePosition;
        mainCam.orthographicSize = nearSize;
    }

    public void SetCameraFar()
    {
        bCameraClose = false;

        mainCam.transform.position = new Vector3(mainCam.transform.position.x, startPos.y, -10);
        mainCam.orthographicSize = farSize;
    }

    public void ResetCamera()
    {
        bCameraClose = false;

        mainCam.orthographicSize = farSize;

        mainCam.transform.position = startPos;
    }

    public void SetSpeedText(float horizontalSpeed, float verticalSpeed)
    {
        horizontalSpeedText.text = string.Format("HORIZONTAL SPEED   {0}", horizontalSpeed.ToString("00"));
        verticalSpeedText.text = string.Format("VERTICAL SPEED     {0}", verticalSpeed.ToString("00"));
    }

    public void SetAltitudeText(float altitude)
    {
        altitudeText.text = string.Format("ALTITUDE {0}", altitude.ToString("0000"));
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timeText.text = string.Format("TIME  {0}:{1:00}", minutes, seconds);
    }

    public void UpdateFuel(float fuelAmount)
    {
        currentFuel = fuelAmount;
        fuelText.text = string.Format("FUEL   {0}", fuelAmount.ToString("0000"));
    }


}

