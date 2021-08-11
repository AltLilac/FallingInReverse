using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class FollowCameraToPlayer : MonoBehaviour
{
    private void Start()
    {
        var player = transform.parent.gameObject.GetComponentInChildren<FindPlayer>();
        var offset = transform.position - player.transform.position;

        this.UpdateAsObservable()
            .Where(_ => player != null)
            .Subscribe(_ =>
            {
                transform.position = player.transform.position + offset;
            });
    }
}
