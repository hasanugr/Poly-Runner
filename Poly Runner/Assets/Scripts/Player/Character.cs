using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public enum SIDE { Left, Middle, Right };
public enum HitX { Left, Middle, Right, None };
public enum HitY { Up, Middle, Down, None };
public enum HitZ { Forward, Middle, Backward, None };

public class Character : MonoBehaviour
{
    public SIDE m_Side = SIDE.Middle;
    public HitX hitX = HitX.None;
    public HitY hitY = HitY.None;
    public HitZ hitZ = HitZ.None;
    [SerializeField] private float XValue;
    [SerializeField] private float SpeedDodge;
    [SerializeField] private float JumpPower = 7f;
    [SerializeField] private float FrwSpeed = 7f;

    [SerializeField] private GameObject footPrint;
    [SerializeField] private float footPrintMaxDistance = 0.2f;

    [SerializeField] private CapsuleCollider hitDetectorCapsuleCollider;
    [SerializeField] private GameObject characterRagdoll;
    [SerializeField] private CharacterEffects characterEffects;

    private bool isGameActive;
    private bool SwipeLeft, SwipeRight, SwipeUp, SwipeDown;
    private float x;
    private float y;
    private float NewXPos = 0f;

    private float rollingLowCollideTime = 0.6f;
    private float slidingLowCollideTime = 1.1f;
    private float ColHeight;
    private float ColCenterY;
    [SerializeField] private bool InRoll;
    [SerializeField] private bool isFalling;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isOnGround;
    [SerializeField] private bool isStunned;
    [SerializeField] private bool isInStunDelay;

    [HideInInspector] public bool isReverseMovement = false;

    ObjectPooler _footPrintPool;
    ObjectPooler _rollingPrintPool;

    private CharacterController m_char;
    private Animator m_Animator;
    private SwipeDetection swipeDetection;
    private void Awake()
    {
        swipeDetection = SwipeDetection.instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_char = GetComponent<CharacterController>();
        ColHeight = m_char.height;
        ColCenterY = m_char.center.y;
        m_Animator = GetComponent<Animator>();

        _footPrintPool = new ObjectPooler(footPrint);
        _footPrintPool.FillThePool(10);

        _rollingPrintPool = new ObjectPooler(footPrint);
        _rollingPrintPool.FillThePool(20);
    }


    private void OnEnable()
    {
        swipeDetection.OnSwipeUp += SwipeUpHandle;
        swipeDetection.OnSwipeDown += SwipeDownHandle;
        swipeDetection.OnSwipeLeft += SwipeLeftHandle;
        swipeDetection.OnSwipeRight += SwipeRightHandle;
    }

    private void OnDisable()
    {
        swipeDetection.OnSwipeUp -= SwipeUpHandle;
        swipeDetection.OnSwipeDown -= SwipeDownHandle;
        swipeDetection.OnSwipeLeft -= SwipeLeftHandle;
        swipeDetection.OnSwipeRight -= SwipeRightHandle;
    }

    public void StartPlayer()
    {
        isGameActive = true;
        m_Animator.SetTrigger("Run");
    }

    void Update()
    {
        isOnGround = m_char.isGrounded;
        if (isGameActive)
        {
            if (SwipeLeft)
            {
                SwipeLeft = false;
                if (!InRoll)
                {
                    if (m_Side == SIDE.Middle)
                    {
                        NewXPos = -XValue;
                        m_Side = SIDE.Left;
                        m_Animator.SetTrigger("LeftSideJump");
                    }
                    else if (m_Side == SIDE.Right)
                    {
                        NewXPos = 0;
                        m_Side = SIDE.Middle;
                        m_Animator.SetTrigger("LeftSideJump");
                    }
                }
            }else if (SwipeRight)
            {
                SwipeRight = false;
                if (!InRoll)
                {
                    if (m_Side == SIDE.Middle)
                    {
                        NewXPos = XValue;
                        m_Side = SIDE.Right;
                        m_Animator.SetTrigger("RightSideJump");
                    }
                    else if (m_Side == SIDE.Left)
                    {
                        NewXPos = 0;
                        m_Side = SIDE.Middle;
                        m_Animator.SetTrigger("RightSideJump");
                    }
                }
            }

            x = Mathf.Lerp(x, NewXPos, Time.deltaTime * SpeedDodge);
            Vector3 moveVector = new Vector3(x - transform.position.x, y * Time.deltaTime, FrwSpeed * Time.deltaTime);
            m_char.Move(moveVector);
            Jump();
            Roll();

            LeaveTrail();
        }
    }

    #region Movement Codes
    public void Jump()
    {
        if (m_char.isGrounded)
        {
            if (isFalling)
            {
                isFalling = false;
                isJumping = false;
                y = 0;
            }
            if (SwipeUp)
            {
                SwipeUp = false;
                isJumping = true;
                y = JumpPower;
                m_Animator.SetTrigger("Jump");
            }
        }
        else
        {
            if (isJumping)
            {
                y -= JumpPower * 2 * Time.deltaTime;
            }else
            {
                y -= JumpPower * 1.5f * Time.deltaTime;
            }

            if (m_char.velocity.y < -0.1f)
            {
                isFalling = true;
            }
        }
    }

    internal float RollCounter;
    public void Roll()
    {
        RollCounter -= Time.deltaTime;
        if (RollCounter <= 0f)
        {
            RollCounter = 0f;
            m_char.center = new Vector3(0, ColCenterY, 0);
            hitDetectorCapsuleCollider.center = new Vector3(0, ColCenterY, 0);
            m_char.height = ColHeight;
            hitDetectorCapsuleCollider.height = ColHeight;
            InRoll = false;
        }

        if (SwipeDown)
        {
            SwipeDown = false;
            if (m_char.isGrounded)
            {
                RollCounter = slidingLowCollideTime;
                //y -= 10f;
                m_char.center = new Vector3(0, ColCenterY / 2f, 0);
                hitDetectorCapsuleCollider.center = new Vector3(0, ColCenterY / 2f, 0);
                m_char.height = ColHeight / 2f;
                hitDetectorCapsuleCollider.height = ColHeight / 2f;
                m_Animator.SetTrigger("Slide");
                InRoll = true;
            }else
            {
                RollCounter = rollingLowCollideTime;
                y -= 10f;
                m_char.center = new Vector3(0, ColCenterY / 2f, 0);
                hitDetectorCapsuleCollider.center = new Vector3(0, ColCenterY / 2f, 0);
                m_char.height = ColHeight / 2f;
                hitDetectorCapsuleCollider.height = ColHeight / 2f;
                m_Animator.SetTrigger("JumpToRoll");
                InRoll = true;
            }
        }
    }

    private void SwipeLeftHandle() {
        if (isReverseMovement)
        {
            SwipeRight = true;
        }else
        {
            SwipeLeft = true;
        }
    }
    private void SwipeRightHandle() {
        if (isReverseMovement)
        {
            SwipeLeft = true;
        }
        else
        {
            SwipeRight = true;
        }
    }
    private void SwipeUpHandle() { SwipeUp = true; }
    private void SwipeDownHandle() { SwipeDown = true; }

    #endregion

    #region Hit Control Codes
    public void OnCharacterColliderHit(Collider col)
    {
        if (isInStunDelay)
            return;
        
        hitX = GetHitX(col);
        hitY = GetHitY(col);
        hitZ = GetHitZ(col);

        if (hitZ == HitZ.Forward && hitX == HitX.Middle)
        {
            if (hitY == HitY.Down)
            {
                // Death Lower
                Debug.Log("Death Lower");
                DieProcess();
            }else if (hitY == HitY.Middle)
            {
                // Death Normal
                Debug.Log("Death Normal");
                DieProcess();
            }
            else if (hitY == HitY.Up)
            {
                // Death Upper
                Debug.Log("Death Upper");
                DieProcess();
            }
        }
        else if (hitZ == HitZ.Middle)
        {
            // Stun on the first hit and kill on the second.
            if (hitX == HitX.Right)
            {
                // Stumble Side Right (Send to back Right)
                Debug.Log("Stumble Side Right");
                if (isStunned)
                {
                    DieProcess();
                }else
                {
                    SwipeRightHandle();
                    StartCoroutine(GetStunned());
                }
            }
            else if (hitX == HitX.Left)
            {
                // Stumble Side Left (Send to back Left)
                Debug.Log("Stumble Side Left");
                if (isStunned)
                {
                    DieProcess();
                }
                else
                {
                    SwipeLeftHandle();
                    StartCoroutine(GetStunned());
                }
            }
        }else
        {
            // Stun on the first hit and kill on the second.
            if (hitX == HitX.Right)
            {
                // Stumble Corner Right (Send to Right)
                Debug.Log("Stumble Corner Right");
                if (isStunned)
                {
                    DieProcess();
                }
                else
                {
                    StartCoroutine(GetStunned());
                }
            }
            else if (hitX == HitX.Left)
            {
                // Stumble Corner Left (Send to Left)
                Debug.Log("Stumble Corner Left");
                if (isStunned)
                {
                    DieProcess();
                }
                else
                {
                    StartCoroutine(GetStunned());
                }
            }
        }
    }
    public void DieProcess()
    {
        if (isStunned)
        {
            isStunned = false;
            characterEffects.EffectProcess(CharacterEffectTypes.Stunned, false);
        }

        characterRagdoll.transform.parent = null;
        characterRagdoll.SetActive(true);
        gameObject.SetActive(false);
    }
    IEnumerator GetStunned()
    {
        isStunned = true;
        isInStunDelay = true;
        characterEffects.EffectProcess(CharacterEffectTypes.Stunned, true);
        yield return new WaitForSeconds(0.5f);
        isInStunDelay = false;

        yield return new WaitForSeconds(5f);

        isStunned = false;
        characterEffects.EffectProcess(CharacterEffectTypes.Stunned, false);
    }
    private HitX GetHitX(Collider col)
    {
        Bounds char_bounds = m_char.bounds;
        Bounds col_bounds = col.bounds;

        float min_x = Mathf.Max(col_bounds.min.x, char_bounds.min.x);
        float max_x = Mathf.Min(col_bounds.max.x, char_bounds.max.x);
        float average = (min_x + max_x) / 2f - col_bounds.min.x;

        HitX hit;
        if (average > col_bounds.size.x - 0.33f)
            hit = HitX.Right;
        else if (average < 0.33f)
            hit = HitX.Left;
        else
            hit = HitX.Middle;

        return hit;
    }
    private HitY GetHitY(Collider col)
    {
        Bounds char_bounds = m_char.bounds;
        Bounds col_bounds = col.bounds;

        float min_y = Mathf.Max(col_bounds.min.y, char_bounds.min.y);
        float max_y = Mathf.Min(col_bounds.max.y, char_bounds.max.y);
        float average = ((min_y + max_y) / 2f - col_bounds.min.y) / char_bounds.size.y;

        HitY hit;
        if (average < 0.33f)
            hit = HitY.Down;
        else if (average < 0.66f)
            hit = HitY.Middle;
        else
            hit = HitY.Up;

        return hit;
    }
    private HitZ GetHitZ(Collider col)
    {
        Bounds char_bounds = m_char.bounds;
        Bounds col_bounds = col.bounds;

        float min_z = Mathf.Max(col_bounds.min.z, char_bounds.min.z);
        float max_z = Mathf.Min(col_bounds.max.z, char_bounds.max.z);
        float average = ((min_z + max_z) / 2f - char_bounds.min.z) / char_bounds.size.z;
        
        HitZ hit;
        if (average < 0.33f)
            hit = HitZ.Backward;
        else if (average < 0.66f)
            hit = HitZ.Middle;
        else
            hit = HitZ.Forward;

        return hit;
    }
    #endregion

    #region Prints And Trails Codes
    private IEnumerator SendToPoolObjectTimout(GameObject poolObject, string objectType, float time)
    {
        // Wait for X second
        yield return new WaitForSeconds(time);

        if (objectType == "FootPrint")
        {
            _footPrintPool.SendObjectToPool(poolObject);
        }else if (objectType == "RollingPrint")
        {
            _rollingPrintPool.SendObjectToPool(poolObject);
        }
    }


    public void FootPrint(string footType)
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, footPrintMaxDistance) && hit.collider.CompareTag("Snow"))
        {
            Vector3 pos = hit.point;
            Quaternion rot = Quaternion.FromToRotation(transform.position, hit.normal);
            AddFootPrint(pos, rot);
        }
    }

    private void AddFootPrint(Vector3 pos, Quaternion rot)
    {
        GameObject footPrintObject = _footPrintPool.GetObjectFromPool();
        footPrintObject.transform.rotation = rot;
        footPrintObject.transform.position = pos;
        footPrintObject.transform.Translate(new Vector3(0, 0, 0.1f), Space.Self);

        StartCoroutine(SendToPoolObjectTimout(footPrintObject, "FootPrint", 2f));
    }

    private void LeaveTrail()
    {
        Debug.DrawRay(transform.position, -transform.up, Color.red, footPrintMaxDistance);

        if (InRoll)
        {
            if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, footPrintMaxDistance) && hit.collider.CompareTag("Snow"))
            {
                Vector3 pos = hit.point;
                Quaternion rot = Quaternion.FromToRotation(transform.position, hit.normal);

                GameObject rollingPrintObject = _rollingPrintPool.GetObjectFromPool();
                rollingPrintObject.transform.localScale = new Vector3(1, 0.25f, 0.25f);
                rollingPrintObject.transform.rotation = rot;
                rollingPrintObject.transform.position = pos;
                rollingPrintObject.transform.Translate(new Vector3(0, 0, 0.1f), Space.Self);

                StartCoroutine(SendToPoolObjectTimout(rollingPrintObject, "RollingPrint", 1f));
            }
        }
    }
    #endregion
}
