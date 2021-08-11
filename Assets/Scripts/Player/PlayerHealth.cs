using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private ObstacleObserver obstacleObserver;

    [SerializeField] private int maxHealth;
    [SerializeField] private string healthChar;

    private int _currentHealth;

    private Subject<Unit> _notifyDeadSubject = new Subject<Unit>();
    public IObservable<Unit> OnDiedPlayer => _notifyDeadSubject;

    private bool _isAlive = true;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    private void Start()
    {
        var healthUI = GetComponentInChildren<FindPlayerHealthUI>().gameObject;
        var healthUITMP = healthUI.GetComponent<TextMeshProUGUI>();

        SetHealthUI(healthUITMP);

        // ダメージを受けたら
        obstacleObserver
            .OnTakedDamage
            .Subscribe(_ =>
            {
                _currentHealth--;
                TakingDamage(healthUITMP);
            });

        // 死亡
        this.UpdateAsObservable()
            .Where(_ => _currentHealth <= 0 && _isAlive == true)
            .Subscribe(_ =>
            {
                _notifyDeadSubject.OnNext(_);
                _isAlive = false;
            });
    }

    private void SetHealthUI(TextMeshProUGUI UI)
    {
        int incrementNum = 1;

        while (incrementNum <= _currentHealth)
        {
            UI.text += healthChar;
            incrementNum++;
        }
    }

    private void TakingDamage(TextMeshProUGUI UI)
    {
        if (UI.text.Length >= 1)
        {
            UI.text = null;
            SetHealthUI(UI);
        }
    }
}
