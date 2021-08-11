using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;

public class SceneFadeManager : MonoBehaviour
{
    [SerializeField] private float fadeSpeed = 0.75f;
    [SerializeField] Image fadeImage;

    [ColorUsage(true, true), SerializeField] private Color fadeImageParams;

    private bool _isFadeOut = false;
    private bool _isFadeIn = false;

    private string _afterScene;

    private Canvas _canvas;

    private void Awake()
    {
        fadeImageParams.a = 0.0f;
        SetColor();

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        _canvas = GetComponentInChildren<Canvas>();

        this.UpdateAsObservable()
        .Where(_ => _isFadeOut == true)
        .Subscribe(_ =>
        {
            fadeImageParams.a += fadeSpeed * Time.deltaTime;
            SetColor();

            if (fadeImageParams.a >= 1)
            {
                _isFadeOut = false;

                SceneManager.sceneLoaded += StartFadeIn;
                SceneManager.LoadSceneAsync(_afterScene);
            }
        });

        this.UpdateAsObservable()
            .Where(_ => _isFadeIn == true)
            .Subscribe(_ =>
            {
                // シーン読み込み後の MainCamera を取得する
                _canvas.worldCamera = Camera.main;

                fadeImageParams.a -= fadeSpeed * Time.deltaTime;
                SetColor();

                if (fadeImageParams.a <= 0)
                {
                    _isFadeIn = false;

                    Destroy(gameObject);
                }
            });
    }

    public void StartFadeOut(string nextScene)
    {
        SetColor();

        _isFadeOut = true;
        _afterScene = nextScene;
    }

    private void StartFadeIn(Scene scene, LoadSceneMode mode)
    {
        _isFadeIn = true;
    }

    private void SetColor()
    {
        fadeImage.color = fadeImageParams;
    }
}
