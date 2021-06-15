using UnityEngine;

[DefaultExecutionOrder(-1)]
public class SwipeDetection : MonoBehaviour
{
    public static SwipeDetection instance;

    private InputManager inputManager;

    #region Events
    public delegate void SwipeUp();
    public event SwipeUp OnSwipeUp;
    public delegate void SwipeDown();
    public event SwipeDown OnSwipeDown;
    public delegate void SwipeLeft();
    public event SwipeLeft OnSwipeLeft;
    public delegate void SwipeRight();
    public event SwipeRight OnSwipeRight;
    #endregion

    [SerializeField] private float minimumSwipeDistance = .17f;
    //[SerializeField] private float minimumDistance = .2f;
    [SerializeField] private float maximumTime = 1f;
    //[SerializeField, Range(0f, 1f)] private float directionThreshold = .9f;

    private Vector2 startPosition;
    private float startTime;
    private Vector2 endPosition;
    private float endTime;

    private void Awake()
    {
        MakeSingleton();

        inputManager = InputManager.instance;
    }

    private void OnEnable()
    {
        inputManager.OnStartTouch += SwipeStart;
        inputManager.OnEndTouch += SwipeEnd;
    }

    private void OnDisable()
    {
        inputManager.OnStartTouch -= SwipeStart;
        inputManager.OnEndTouch -= SwipeEnd;
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

    private void SwipeStart(Vector2 position, float time)
    {
        startPosition = position;
        startTime = time;
    }

    private void SwipeEnd(Vector2 position, float time)
    {
        endPosition = position;
        endTime = time;
        DetectSwipe();
    }

    #region DOT Version
    /*
    private void DetectSwipe()
    {
        if (Vector2.Distance(startPosition, endPosition) >= minimumDistance && 
            (endTime - startTime) <= maximumTime)
        {
            //Debug.DrawLine(startPosition, endPosition, Color.red, 5f);
            Vector3 direction = endPosition - startPosition;
            Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
            SwipeDirection(direction);
        }
    }

    private void SwipeDirection(Vector2 direction)
    {
        if (Vector2.Dot(Vector2.up, direction) > directionThreshold)
        {
            if (OnSwipeUp != null)
            {
                OnSwipeUp();
            }
        }
        else if (Vector2.Dot(Vector2.down, direction) > directionThreshold)
        {
            if (OnSwipeDown != null)
            {
                OnSwipeDown();
            }
        }
        else if (Vector2.Dot(Vector2.left, direction) > directionThreshold)
        {
            if (OnSwipeLeft != null)
            {
                OnSwipeLeft();
            }
        }
        else if (Vector2.Dot(Vector2.right, direction) > directionThreshold)
        {
            if (OnSwipeRight != null)
            {
                OnSwipeRight();
            }
        }
    }
    */
    #endregion

    #region Magnitute Version
    private void DetectSwipe()
    {
        Vector2 startPosRatio = new Vector2(startPosition.x / (float)Screen.width, startPosition.y / (float)Screen.width);
        Vector2 endPosRatio = new Vector2(endPosition.x / (float)Screen.width, endPosition.y / (float)Screen.width);
        Vector2 swipe = new Vector2(endPosRatio.x - startPosRatio.x, endPosRatio.y - startPosRatio.y);

        if (swipe.magnitude >= minimumSwipeDistance &&
            (endTime - startTime) <= maximumTime)
        {
            SwipeDirection(swipe);
        }
    }

    private void SwipeDirection(Vector2 swipe)
    {
        //Debug.Log("SwipeDirection");
        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
        { // Horizontal swipe
            if (swipe.x > 0)
            {
                if (OnSwipeRight != null)
                {
                    OnSwipeRight();
                }
            }
            else
            {
                if (OnSwipeLeft != null)
                {
                    OnSwipeLeft();
                }
            }
        }
        else
        { // Vertical swipe
            if (swipe.y > 0)
            {
                if (OnSwipeUp != null)
                {
                    OnSwipeUp();
                }
            }
            else
            {
                if (OnSwipeDown != null)
                {
                    OnSwipeDown();
                }
            }
        }
    }
    #endregion

}
