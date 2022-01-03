using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script to spawn pick-up texts and other UI-info at world space
/// </summary>
public class UIpickupText : MonoBehaviour
{
    [SerializeField]
    float speed = 1;

    [SerializeField]
    float appearTime = 0.6f;

    [SerializeField]
    float fadeTime = 0.4f;

    [SerializeField]
    float moveAmt = 1;

    private float timer;
    private Text txt;
    bool started;

    //Moves text up
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.up, moveAmt * (speed * Time.deltaTime));
        timer -= Time.deltaTime;

        if(!started && timer < appearTime / 2)
        {
            started = true;
            StartCoroutine(FadeOutRoutine());
        }
    }

    public void Initialize(string type, Color txtColor, int amount)
    {
        txt = GetComponent<Text>();
        txt.color = txtColor;
        txt.text = "+" + amount.ToString() + " " + type;
        timer = appearTime;
    }

    /// <summary>
    /// Fade out text while it moves up
    /// </summary>
    /// <returns>Null</returns>
    private IEnumerator FadeOutRoutine()
    {
        Color originalColor = txt.color;
        for (float t = 0.01f; t < fadeTime; t += Time.deltaTime)
        {
            txt.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(1, t / fadeTime));
            yield return null;
        }

        DisActivate();
    }

    private void DisActivate()
    {
        timer = appearTime;
        StopAllCoroutines();
        started = false;
        gameObject.SetActive(false);
    }
}
