using System.Collections;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public enum Side { Middle, Left, Right };
    public Side CurrentPlayerSide = Side.Middle;

    [Header("Player Settings")]
    [SerializeField] private GameObject playerBody;
    // Speed
    [SerializeField] private float speed = 12f;
    // Jump
    [SerializeField] private float jumpForce = 12f;
    // Sliding
    [SerializeField] private float slideDuration = 0.7f;
    // Swipe movement
    [SerializeField] private float sideJumpPos = 5f;
    [SerializeField] private float sideJumpDuration = 0.1f;
    // Detect variables
    [SerializeField] private float playerRotateSpeed = 0.2f;
    [SerializeField] private float bodyPivotPointMinus = 0.9f;
    [SerializeField] private float maxGroundDist = 1.2f;

    [HideInInspector] public bool isReverseMovement = false;

    private bool isOnGround = true;
    private bool isSliding = false;
    private bool isGameStart = false;
    private float _targetXPos = 0;
    private float _targetYPos = 0;
    private Quaternion _playerRotate;
    AnimatorClipInfo[] m_CurrentClipInfo;
    string m_ClipName;

    private Rigidbody _rb;
    private Animator m_Animator;
    private CapsuleCollider m_Collider;
    private SwipeDetection swipeDetection;


    private void Awake()
    {
        swipeDetection = SwipeDetection.instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        m_Animator = playerBody.GetComponent<Animator>();
        m_Collider = playerBody.GetComponent<CapsuleCollider>();
    }

    private void OnEnable()
    {
        swipeDetection.OnSwipeUp += JumpOver;
        swipeDetection.OnSwipeDown += LeanDown;
        swipeDetection.OnSwipeLeft += Left;
        swipeDetection.OnSwipeRight += Right;
    }

    private void OnDisable()
    {
        swipeDetection.OnSwipeUp -= JumpOver;
        swipeDetection.OnSwipeDown -= LeanDown;
        swipeDetection.OnSwipeLeft -= Left;
        swipeDetection.OnSwipeRight -= Right;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isGameStart)
        {
            CheckPlayerVariables();
            _rb.MovePosition(transform.position + (transform.forward * speed * Time.deltaTime));
            playerBody.transform.localPosition = Vector3.Lerp(playerBody.transform.localPosition, new Vector3(_targetXPos, _targetYPos, 0), (Time.deltaTime / sideJumpDuration));
            transform.rotation = Quaternion.Lerp(transform.rotation, _playerRotate, (Time.deltaTime / playerRotateSpeed));
        }
    }  
    
    public void StartPlayer()
    {
        isGameStart = true;
        m_Animator.SetTrigger("Run");
    }

    public void RotatePlayerToRoad(Quaternion rotate)
    {
        //Debug.Log("Rotate Player -> " + rotate);
        _playerRotate = rotate;
    }

    private void JumpOver()
    {
        if (IsAvailableNewAnimation())
        {
            if (isOnGround)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, jumpForce, _rb.velocity.z);
                m_Animator.SetTrigger("Jump");
            }
            if (isSliding)
            {
                StartCoroutine(CancelSliding(0));
                _rb.velocity = new Vector3(_rb.velocity.x, jumpForce, _rb.velocity.z);
                m_Animator.SetTrigger("Jump");
            }
        }
    }

    private void LeanDown()
    {
        if (IsAvailableNewAnimation())
        {
            if (isOnGround)
            {
                // Slide or Roll animation and controls
                StartSliding();
                StartCoroutine(CancelSliding(slideDuration));
            }
            else
            {
                _rb.AddForce(-transform.up * jumpForce * 1.5f, ForceMode.Impulse);
                m_Animator.SetTrigger("JumpToRoll");
                m_Collider.height = 0.6f;
                m_Collider.center = new Vector3(0, 0.3f, 0);
                StartCoroutine(RollingCollideCancel(0.6f));
            }
        }
    }

    private void Left()
    {
        SwipeMovementControl(false);
    }

    private void Right()
    {
        SwipeMovementControl(true);
    }

    private void SwipeMovementControl(bool isRight)
    {
        isRight = isReverseMovement ? !isRight : isRight;

        if (isRight)
        {
            if (IsAvailableNewAnimation())
            {
                if (CurrentPlayerSide == Side.Left)
                {
                    _targetXPos = 0;
                    CurrentPlayerSide = Side.Middle;
                    m_Animator.SetTrigger("RightSideJump");
                }
                else if (CurrentPlayerSide == Side.Middle)
                {
                    _targetXPos = sideJumpPos;
                    CurrentPlayerSide = Side.Right;
                    m_Animator.SetTrigger("RightSideJump");

                }
            }
        }
        else
        {
            if (IsAvailableNewAnimation())
            {
                if (CurrentPlayerSide == Side.Right)
                {
                    _targetXPos = 0;
                    CurrentPlayerSide = Side.Middle;
                    m_Animator.SetTrigger("LeftSideJump");
                }
                else if (CurrentPlayerSide == Side.Middle)
                {
                    _targetXPos = -sideJumpPos;
                    CurrentPlayerSide = Side.Left;
                    m_Animator.SetTrigger("LeftSideJump");
                }
            }
        }
    }

    private void CheckPlayerVariables()
    {
        Vector3 playerBodyPosition = new Vector3(playerBody.transform.position.x, playerBody.transform.position.y + bodyPivotPointMinus, playerBody.transform.position.z);
        Ray rayGrounded = new Ray(playerBodyPosition, -playerBody.transform.up);
        isOnGround = Physics.Raycast(rayGrounded, out RaycastHit hitInfoGround, maxGroundDist);

        Debug.DrawRay(playerBodyPosition, -playerBody.transform.up * maxGroundDist, Color.red);
    }

    private void StartSliding()
    {
        if (!isSliding)
        {
            m_Animator.SetTrigger("Slide");
            isSliding = true;
            m_Collider.height = 0.6f;
            m_Collider.center = new Vector3(0, 0.3f, 0);
        }
    }

    IEnumerator CancelSliding(float time)
    {
        yield return new WaitForSeconds(time);
        isSliding = false;
        m_Collider.height = 1.4f;
        m_Collider.center = new Vector3(0, 0.7f, 0);
    }

    IEnumerator RollingCollideCancel(float time)
    {
        yield return new WaitForSeconds(time);
        m_Collider.height = 1.4f;
        m_Collider.center = new Vector3(0, 0.7f, 0);
    }

    private bool IsAvailableNewAnimation()
    {
        bool isAvailable = true;

        m_CurrentClipInfo = this.m_Animator.GetCurrentAnimatorClipInfo(0);
        m_ClipName = m_CurrentClipInfo[0].clip.name;
        if (m_ClipName == "Roll")
        {
            isAvailable = false;
        }
        //Debug.Log(m_ClipName);

        return isAvailable;
    }
}
