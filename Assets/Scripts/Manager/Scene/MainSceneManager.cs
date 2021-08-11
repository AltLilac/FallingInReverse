using System;
using System.Collections;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class MainSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject goalObject;
    [SerializeField] private GameObject gameOverObject;

    PlayerInputActions _playerInputActions;
    SceneFadeManager _sceneFadeManager;

    private bool _isEnableKey = true;

    private Subject<Unit> _notifyGameOverSubject = new Subject<Unit>();
    public IObservable<Unit> GameOverNotification => _notifyGameOverSubject;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
    }

    private void Start()
    {
        _sceneFadeManager = GetComponent<SceneFadeManager>();

        var player = playerObject.GetComponentInChildren<FindPlayer>().gameObject;
        var goal = goalObject.GetComponentInChildren<FindGoal>().gameObject;
        var gameOverText = playerObject.GetComponentInChildren<FindGameOverText>().gameObject;
        var winText = playerObject.GetComponentInChildren<FindWinText>().gameObject;

        ChangeVisibleState(value: false, gameOverText, winText);

        // ゴール時
        var goalCollider = goal.GetComponent<Collider>();

        goalCollider.isTrigger = true;

        goalCollider
            .OnTriggerEnterAsObservable()
            .Where(collider => collider.CompareTag(StringManager.Tag_Player) && goalCollider != null)
            .Subscribe(collider =>
            {
                winText.SetActive(true);
                goalObject.SetActive(false);

                Observable
                    .FromCoroutine(WaitText)
                    .Subscribe(_ =>
                    {
                        _sceneFadeManager.StartFadeOut(StringManager.Scene_Title);
                    });
            });

        // ゲームオーバー時
        var gameOverCollider = gameOverObject.GetComponent<Collider>();

        gameOverCollider.isTrigger = true;

        gameOverCollider
            .OnTriggerEnterAsObservable()
            .Where(collider => collider.CompareTag(StringManager.Tag_Player) && gameOverCollider != null)
            .Subscribe(collider =>
            {
                GameOver(gameOverText);
            });

        var playerHealth = playerObject.GetComponent<PlayerHealth>();

        playerHealth
            .OnDiedPlayer
            .Subscribe(_ =>
            {
                GameOver(gameOverText);
            });

        // メインメニューに戻る
        this.UpdateAsObservable()
            .Where(_ => _isEnableKey == true)
            .Subscribe(_ =>
            {
                // アクションタイプが Button なので、float でも 0 と 1 のみの取得ができる
                if (_playerInputActions.Player.ReturnMainMenu.ReadValue<float>() == 1)
                {
                    _sceneFadeManager.StartFadeOut(StringManager.Scene_Title);
                    _isEnableKey = false;
                }
            });
    }

    private void GameOver(GameObject gameOverText)
    {
        gameOverText.SetActive(true);

        Observable
            .FromCoroutine(WaitText)
            .Subscribe(_ =>
            {
                _sceneFadeManager.StartFadeOut(StringManager.Scene_Main);
            });  
    }

    private IEnumerator WaitText()
    {
        yield return new WaitForSeconds(3.0f);
    }

    private void ChangeVisibleState(bool value, params GameObject[] gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(value);
            }
        }
    }
}
