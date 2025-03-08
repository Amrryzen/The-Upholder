using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    [Header("Loading Settings")]
    public Slider loadingSlider;           // Slider UI (min = 0, max = 1)

    [Header("Teks dan Input")]
    public GameObject preparedText;        // Teks yang sudah disiapkan (nonaktif pada awalnya)

    [Header("Transition Animation")]
    public Image fadeImage;                // UI Image untuk fade (harus berwarna hitam)
    public float zoomDuration = 1f;        // Durasi animasi zoom in kamera
    public float fadeDuration = 1f;        // Durasi animasi fade ke hitam

    [Header("Scene Management")]
    public string sceneToLoad = "Lobby";   // Nama scene tujuan

    void Start()
    {
        // Pastikan fadeImage berwarna hitam dan full opacity (alpha = 1)
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0f, 0f, 0f, 1f); // Hitam dengan alpha 1
        }
        if (preparedText != null)
            preparedText.SetActive(false);

        // Mulai proses: pertama fade out dari hitam, lalu mulai loading
        StartCoroutine(InitialFadeOut());
    }

    IEnumerator InitialFadeOut()
    {
        float duration = 0.5f;  // Durasi fade out awal
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = Mathf.Lerp(1f, 0f, progress);
                fadeImage.color = c;
            }
            yield return null;
        }
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
        // Setelah fade out awal, mulai proses loading
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        yield return StartCoroutine(FillSlider(0f, 0.3f, 0.5f));
        yield return StartCoroutine(FillSlider(0.3f, 0.4f, 3f));
        yield return StartCoroutine(FillSlider(0.4f, 0.65f, 0f));
        yield return StartCoroutine(FillSlider(0.65f, 0.7f, 0.5f));
        yield return StartCoroutine(FillSlider(0.7f, 0.85f, 1f));
        yield return StartCoroutine(FillSlider(0.85f, 0.99f, 0f));
        yield return StartCoroutine(FillSlider(0.95f, 1f, 4f));

        // Setelah slider penuh, sembunyikan slider
        if (loadingSlider != null)
            loadingSlider.gameObject.SetActive(false);

        // Aktifkan teks yang sudah disiapkan
        if (preparedText != null)
            preparedText.SetActive(true);

        // Tunggu input pemain (tap) setelah teks muncul
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.touchCount > 0);

        // Jalankan animasi transisi: zoom in dan fade ke hitam
        yield return StartCoroutine(PlayTransitionAnimation());

        // Pindah ke scene tujuan
        SceneManager.LoadScene(sceneToLoad);
    }

    IEnumerator FillSlider(float startValue, float endValue, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (loadingSlider != null)
                loadingSlider.value = Mathf.Lerp(startValue, endValue, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (loadingSlider != null)
            loadingSlider.value = endValue;
    }

    IEnumerator PlayTransitionAnimation()
    {
        // Animasi Zoom In: Ubah Field of View kamera utama
        Camera cam = Camera.main;
        if (cam != null)
        {
            float startFOV = cam.fieldOfView;
            float targetFOV = startFOV * 0.5f;  // Sesuaikan faktor zoom sesuai kebutuhan
            float elapsed = 0f;
            while (elapsed < zoomDuration)
            {
                cam.fieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsed / zoomDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            cam.fieldOfView = targetFOV;
        }

        // Animasi Fade ke Hitam: Ubah alpha dari fadeImage
        if (fadeImage != null)
        {
            float elapsed = 0f;
            Color color = fadeImage.color;
            while (elapsed < fadeDuration)
            {
                color.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                fadeImage.color = color;
                elapsed += Time.deltaTime;
                yield return null;
            }
            color.a = 1f;
            fadeImage.color = color;
        }
        yield return null;
    }
}
