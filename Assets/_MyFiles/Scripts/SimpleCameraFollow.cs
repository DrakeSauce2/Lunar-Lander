using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    public static SimpleCameraFollow Instance;

    [SerializeField] private Transform relativeWorldTarget;

    private Shuttle player;

    [SerializeField] private float smoothTime = 1;
    [SerializeField] private float lerpSpeed = 1;
    float refVel = 0;
    Vector3 refVector = Vector3.zero;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public void SetNewPlayer(Shuttle newPlayer)
    {
        player = newPlayer;
    }

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (player == null || GameManager.Instance.GetCameraClose()) return;

        Vector3 targetPlayerMatchPoint = new Vector3(relativeWorldTarget.position.x, player.transform.position.y, 0);
        relativeWorldTarget.position = targetPlayerMatchPoint;

        float distance = Vector3.Distance(relativeWorldTarget.position, player.transform.position);
        if (distance > 350f)
        {
            float xPos = Mathf.SmoothDamp(transform.position.x, player.transform.position.x - 50f, ref refVel, smoothTime);
            Vector3 targetPosition = Vector3.SmoothDamp(transform.position, new Vector3(xPos, transform.position.y, -10), ref refVector, lerpSpeed * Time.deltaTime);
            transform.position = targetPosition;
            relativeWorldTarget.position = new Vector3(xPos, player.transform.position.y, 0);
        }

    }

}
