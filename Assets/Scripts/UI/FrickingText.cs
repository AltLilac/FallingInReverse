using System.Collections;
using UnityEngine;
using TMPro;

public class FrickingText : MonoBehaviour
{
    [SerializeField] private float flickInterval;

    private void Start()
    {
        var text = GetComponent<TextMeshPro>();

        if (text == null)
        {
            var textTMPUGUI = GetComponent<TextMeshProUGUI>();

            StartCoroutine(Fricking(textTMPUGUI));
        }

        StartCoroutine(Fricking(text));
    }

    private IEnumerator Fricking(TextMeshPro text)
    {
        while (text != null)
        {
            text.alpha = 1.0f;
            yield return new WaitForSeconds(flickInterval);

            text.alpha = 0.0f;
            yield return new WaitForSeconds(flickInterval);
        }
    }

    private IEnumerator Fricking(TextMeshProUGUI text)
    {
        while (text != null)
        {
            text.alpha = 1.0f;
            yield return new WaitForSeconds(flickInterval);

            text.alpha = 0.0f;
            yield return new WaitForSeconds(flickInterval);
        }
    }
}
