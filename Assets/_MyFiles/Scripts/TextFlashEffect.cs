using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextFlashEffect : MonoBehaviour
{
    private TextMeshProUGUI owningText;
    string originalText;

    [SerializeField] private float flashSpeed = 0.75f; // In Seconds

    private void Awake()
    {
        owningText = GetComponent<TextMeshProUGUI>();

        originalText = owningText.text;

        StartCoroutine(FlashTextCoroutine(flashSpeed));
    }

    private IEnumerator FlashTextCoroutine(float waitTime)
    {
        owningText.text = "";

        yield return new WaitForSeconds(waitTime/2);

        owningText.text = originalText;

        yield return new WaitForSeconds(waitTime/2);

        StartCoroutine(FlashTextCoroutine(waitTime));
    }

}
