using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-2)]
public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    #region Events
    public delegate void StartTouch(Vector2 position, float time);
    public event StartTouch OnStartTouch;
    public delegate void EndTouch(Vector2 position, float time);
    public event EndTouch OnEndTouch;
    #endregion

    //[SerializeField] private float screenToWorldPosZ = 10;

    private PlayerControls playerControls;
    //private Camera mainCamera;

    private void Awake()
    {
        MakeSingleton();

        playerControls = new PlayerControls();
        //mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Start()
    {
        playerControls.Touch.PrimaryContact.started += ctx => StartTouchPrimary(ctx);
        playerControls.Touch.PrimaryContact.canceled += ctx => EndTouchPrimary(ctx);
    }

    private void MakeSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void StartTouchPrimary(InputAction.CallbackContext context)
    {
        if (OnStartTouch != null)
        {
            Vector2 touchPosition = playerControls.Touch.PrimaryPosition.ReadValue<Vector2>();
            //Debug.Log("Touch 2D: " + touchPosition + "TouchScreenToWorld: " + Utils.ScreenToWorld(mainCamera, touchPosition, screenToWorldPosZ));
            if (touchPosition.x != 0 && touchPosition.y != 0)
            {
                //OnStartTouch(Utils.ScreenToWorld(mainCamera, touchPosition, screenToWorldPosZ), (float)context.startTime);
                OnStartTouch(touchPosition, (float)context.startTime);
            }
        }
    }
    
    private void EndTouchPrimary(InputAction.CallbackContext context)
    {
        if (OnEndTouch != null)
        {
            Vector2 touchPosition = playerControls.Touch.PrimaryPosition.ReadValue<Vector2>();
            //Debug.Log("Touch 2D: " + touchPosition + "TouchScreenToWorld: " + Utils.ScreenToWorld(mainCamera, touchPosition, screenToWorldPosZ));
            if (touchPosition.x != 0 && touchPosition.y != 0)
            {
                //OnEndTouch(Utils.ScreenToWorld(mainCamera, touchPosition, screenToWorldPosZ), (float)context.time);
                OnEndTouch(touchPosition, (float)context.time);
            }
        }
    }
}
