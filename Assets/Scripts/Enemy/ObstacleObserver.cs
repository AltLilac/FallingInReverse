using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class ObstacleObserver : MonoBehaviour
{
    private Subject<Unit> _takingDamageSubject = new Subject<Unit>();
    public IObservable<Unit> OnTakedDamage => _takingDamageSubject;

    void Start()
    {
        var obstacleColliders = GetComponentsInChildren<Collider>();

        foreach (var obstacleCollider in obstacleColliders)
        {
            obstacleCollider.isTrigger = true;

            obstacleCollider
                .OnTriggerEnterAsObservable()
                .Where(collider => collider.CompareTag(StringManager.Tag_Player))
                .Subscribe(collider =>
                {
                    Unit _;

                    _takingDamageSubject.OnNext(_);

                    Destroy(obstacleCollider.gameObject);
                });
        }
    }
}
