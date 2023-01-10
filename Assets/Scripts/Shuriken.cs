using TSUtils.Sounds;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class Shuriken : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float Impulse;
    [SerializeField] private Texture IdleTexture;
    [SerializeField] private Texture SpinningTexture;
    [SerializeField] private float VisualRotationSpeed;
    [SerializeField] private Renderer VisualRenderer;
    [SerializeField] private Transform VisualTransform;
    [SerializeField] private TrailRenderer TrailRenderer;
    [SerializeField] private SoundAsset ThrowSound;
    [SerializeField] private SoundAsset CollisionSound;
    [SerializeField] private SoundAsset OutOfBoundsSound;
    [SerializeField] private GameObject CollisionFx;
    [SerializeField] private GameObject ChargeFx;
    
    public ShurikenState State => _state;

    private Rigidbody _rigidBody;
    private ShurikenState _state;
    private Vector3 _aimStartPosition;
    private Camera _camera;
    private Vector3 _originalPosition;
    private Quaternion _originalVisualRotation;
    private Quaternion _originalRotation;
    private GameObject _chargeFxInstance;

    private void Start()
    {
        _camera = Camera.main;
        _rigidBody = GetComponent<Rigidbody>();
        _state = ShurikenState.Idle;
        VisualRenderer.material.mainTexture = IdleTexture;
        
        var t = transform; 
        _originalPosition = t.position;
        _originalRotation = t.rotation;
        _originalVisualRotation = VisualTransform.rotation;
    }

    private void Update()
    {
        if (_state == ShurikenState.Moving)
        {
            VisualTransform.Rotate(Vector3.forward, VisualRotationSpeed * Time.deltaTime);
            
            var distanceFromZero = Vector3.Distance(transform.position, Vector3.zero);
            if (distanceFromZero > 15)
            {
                SoundManager.Instance.Play(OutOfBoundsSound);
                Stop();
                _state = ShurikenState.OutOfBounds;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_state == ShurikenState.Idle)
        {
            if (ChargeFx != null)
            {
                if(_chargeFxInstance != null)
                    Destroy(_chargeFxInstance);

                var t = transform;
                _chargeFxInstance = Instantiate(ChargeFx, t.position + new Vector3(0, 0.5f, 0), Quaternion.identity, t);
            }
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_state == ShurikenState.Idle)
        {
            if(_chargeFxInstance != null)
                Destroy(_chargeFxInstance);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_state == ShurikenState.Idle)
        {
            _aimStartPosition = ScreenToWorld(eventData.position);
            _state = ShurikenState.Aiming;
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_state == ShurikenState.Aiming)
        {
            var aimDirection = ScreenToWorld(eventData.position) - _aimStartPosition;
            Throw(aimDirection);
        }
    }

    private void Throw(Vector3 direction)
    {
        if (direction.magnitude < .5f)
            return;
        
        if(_chargeFxInstance != null)
            Destroy(_chargeFxInstance);

        _state = ShurikenState.Moving;
        VisualRenderer.material.mainTexture = SpinningTexture;
        direction = new Vector3(direction.x, 0, direction.z).normalized;
        _rigidBody.AddForce(direction * Impulse, ForceMode.Impulse);
        SoundManager.Instance.Play(ThrowSound);
    }

    private Vector3 ScreenToWorld(Vector2 screenPosition)
    {
        var ray = _camera.ScreenPointToRay(screenPosition);
        var plane = new Plane(Vector3.up, Vector3.zero);
        return plane.Raycast(ray, out var enter) ? ray.GetPoint(enter) : Vector3.zero;
    }

    public void ResetToStart()
    {
        _state = ShurikenState.Idle;
        VisualRenderer.material.mainTexture = IdleTexture;

        var t = transform;
        t.position = _originalPosition;
        t.rotation = _originalRotation;
        
        Stop();
        VisualTransform.rotation = _originalVisualRotation;
        TrailRenderer.Clear();
    }

    public void OnTriggerEnter(Collider other)
    {
        var bamboo = other.GetComponent<Bamboo>();
        if (bamboo != null)
        {
            bamboo.CutAt(1, _rigidBody.velocity, transform.position);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        SoundManager.Instance.Play(CollisionSound);
        if (CollisionFx != null)
        {
            var contact = collision.GetContact(0);
            Instantiate(CollisionFx, contact.point, Quaternion.LookRotation(contact.normal));
        }
    }

    private void Stop()
    {
        _rigidBody.velocity = Vector3.zero;
        _rigidBody.angularVelocity = Vector3.zero;
    }
}
