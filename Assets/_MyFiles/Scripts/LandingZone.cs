using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LandingZone : MonoBehaviour
{
    [SerializeField] private int scoreMultiplier = 1;

    [Header("UI")]
    [SerializeField] private GameObject uiPrefab;
    [SerializeField] private Transform uiAttachPoint;

    private TextMeshProUGUI instancedTextComp;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        GameObject instancedTextObj = Instantiate(uiPrefab, FindAnyObjectByType<Canvas>().transform);
        instancedTextComp = instancedTextObj.GetComponent<TextMeshProUGUI>();
        CameraAttachComponent cameraAttachComponent = instancedTextObj.GetComponent<CameraAttachComponent>();
        if (cameraAttachComponent)
        {
            cameraAttachComponent.SetupAttachPoint(uiAttachPoint);
        }

        instancedTextComp.text = " ";

        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;
    }

    public TextMeshProUGUI GetInstancedTextObj()
    { 
        return instancedTextComp;
    }

    public BoxCollider2D GetBoxCollider()
    {
        return boxCollider;
    }

    public int GetScoreMultiplier()
    {
        if (scoreMultiplier <= 0)
        {
            return 1;
        }

        return scoreMultiplier;
    }

}
