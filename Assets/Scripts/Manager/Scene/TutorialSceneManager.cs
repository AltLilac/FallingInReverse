using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class TutorialSceneManager : MonoBehaviour
{
    PlayerInputActions _playerInputActions;
    SceneFadeManager _sceneFadeManager;

    private bool _isEnableKey = true;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
    }

    private void Start()
    {
        _sceneFadeManager = GetComponent<SceneFadeManager>();

        this.UpdateAsObservable()
            .Where(_ => _isEnableKey == true)
            .Subscribe(_ =>
            {
                // アクションタイプが Button なので、float でも 0 と 1 のみの取得ができる
                if (_playerInputActions.Player.PressKeyToNext.ReadValue<float>() == 1)
                {
                    _sceneFadeManager.StartFadeOut(StringManager.Scene_Main);
                    _isEnableKey = false;
                }
            });
    }
}
