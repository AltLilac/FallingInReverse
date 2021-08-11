using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;

    PlayerInputActions _playerInputActions;
    PlayerHealth playerHealth;

    private bool _isEnableController = true;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
    }

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();

        var player = GetComponentInChildren<FindPlayer>().gameObject;
        var playerRb = player.GetComponent<Rigidbody>();

        // 回転させない
        playerRb.constraints = RigidbodyConstraints.FreezeRotation;

        playerHealth
            .OnDiedPlayer
            .Subscribe(_ =>
            {
                _isEnableController = false;
                FreezePlayer(playerRb);
            });

        this.UpdateAsObservable()
            .Where(_ => _isEnableController == true)
            .Subscribe(_ =>
            {
                var direction = _playerInputActions.Player.Move.ReadValue<Vector2>();
                var newLocation = new Vector3(direction.x, 0, direction.y);

                transform.Translate(newLocation * speed * Time.deltaTime);

                if (_playerInputActions.Player.ReturnMainMenu.ReadValue<float>() == 1)
                {
                    _isEnableController = false;
                    FreezePlayer(playerRb);
                }
            });
    }

    private void FreezePlayer(Rigidbody rigidbody)
    {
        rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rigidbody.isKinematic = true;
    }
}
