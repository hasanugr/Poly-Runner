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
    [Header("Base Controls")]
    public SIDE m_Side = SIDE.Middle;
    public HitX hitX = HitX.None;
    public HitY hitY = HitY.None;
    public HitZ hitZ = HitZ.None;
    [SerializeField] private Animator m_Animator;
    [SerializeField] private GameObject characterBody;
    [SerializeField] private float characterSwipeRotateValue;
    [SerializeField] private float XValue;
    [SerializeField] private float SpeedDodge;
    [SerializeField] private float JumpPower = 7f;
    [SerializeField] private float FrwSpeed = 10f;

    private bool isGameActive;
    private bool SwipeLeft, SwipeRight, SwipeUp, SwipeDown;
    private float x;
    private float y;
    private float NewXPos = 0f;

    private float rollingLowCollideTime = 0.6f;
    private float slidingLowCollideTime = 1.1f;
    private float ColHeight;
    private float ColCenterY;

    [Header("Trail And Prints")]
    [SerializeField] private GameObject footPrint;
    [SerializeField] private float footPrintMaxDistance = 0.2f;
    [SerializeField] private GameObject[] slideTrailObject;
    [SerializeField] private TrailRenderer[] slideTrailRenderer;

    [Header("Hit And Effects")]
    [SerializeField] private CapsuleCollider hitDetectorCapsuleCollider;
    [SerializeField] private GameObject characterRagdoll;
    [SerializeField] private CharacterEffects characterEffects;

    [Header("Enemy Controls")]
    [SerializeField] private GameObject enemyHolder;
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private Transform enemyTargetBody;
    [SerializeField] private float chasingDistance = 4f;
    [SerializeField] private float deadBodyClosingSpeed = 10f;
    [SerializeField] private float deadBodyStopDistance = 1f;
    private bool enemyClosingToBody = false;

    [Header("Player Status")]
    [SerializeField] private bool InRoll;
    [SerializeField] private bool isFalling;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isOnGround;
    [SerializeField] private bool isStunned;
    [SerializeField] private bool isInStunDelay;

    [HideInInspector] public bool isCinematic;
    [HideInInspector] public bool isReverseMovement = false;
    [HideInInspector] public bool isTrailing = false;

    ObjectPooler _footPrintPool;

    private InGameManager _igm;
    private CharacterController m_char;
    private SwipeDetection swipeDetection;
    private void Awake()
    {
        swipeDetection = SwipeDetection.instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        _igm = FindObjectOfType<InGameManager>();
        m_char = GetComponent<CharacterController>();
        ColHeight = m_char.height;
        ColCenterY = m_char.center.y;

        _footPrintPool = new ObjectPooler(footPrint);
        _footPrintPool.FillThePool(10);
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
        if (isGameActive && !isCinematic)
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
                        //LeanTween.rotateY(characterBody, -characterSwipeRotateValue, characterSwipeRotateDuration).setEasePunch();
                        //m_Animator.SetTrigger("LeftSideJump");
                    }
                    else if (m_Side == SIDE.Right)
                    {
                        NewXPos = 0;
                        m_Side = SIDE.Middle;
                        //LeanTween.rotateY(characterBody, -characterSwipeRotateValue, characterSwipeRotateDuration).setEasePunch();
                        //m_Animator.SetTrigger("LeftSideJump");
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
                        //LeanTween.rotateY(characterBody, characterSwipeRotateValue, characterSwipeRotateDuration).setEasePunch();
                        //m_Animator.SetTrigger("RightSideJump");
                    }
                    else if (m_Side == SIDE.Left)
                    {
                        NewXPos = 0;
                        m_Side = SIDE.Middle;
                        //LeanTween.rotateY(characterBody, characterSwipeRotateValue, characterSwipeRotateDuration).setEasePunch();
                        //m_Animator.SetTrigger("RightSideJump");
                    }
                }
            }
            float rotY = ((NewXPos - x) / 2.5f) * characterSwipeRotateValue;
            characterBody.transform.rotation = Quaternion.Euler(new Vector3(0, rotY, 0));
            if (isStunned)
            {
                enemyHolder.transform.rotation = Quaternion.Euler(new Vector3(0, rotY, 0));
            }

            x = Mathf.Lerp(x, NewXPos, Time.deltaTime * SpeedDodge);
            Vector3 moveVector = new Vector3(x - transform.position.x, y * Time.deltaTime, FrwSpeed * Time.deltaTime);
            m_char.Move(moveVector);
            Jump();
            Roll();

            LeaveTrail();
        }
        else if (isCinematic)
        {
            y = Mathf.Lerp(y, 0f, Time.deltaTime * SpeedDodge);
            Vector3 moveVector = new Vector3(x - transform.position.x, y * Time.deltaTime, FrwSpeed * Time.deltaTime);
            m_char.Move(moveVector);
            LeaveTrail();
        }

        if (enemyClosingToBody)
        {
            EnemyGoingToDeadBody();
        }
    }

    #region Movement Codes
    protected float swipeUpCountdown = 0;
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

            if (SwipeUp)
            {
                if (swipeUpCountdown > 0)
                {
                    swipeUpCountdown -= 4f * Time.deltaTime; // 0.25 sec
                }else
                {
                    SwipeUp = false;
                }
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
    
    public void FinishMoves()
    {
        SwipeDownHandle();
        isCinematic = true;
        InRoll = true;
        float normalSpeed = FrwSpeed;
        m_Animator.SetTrigger("FinishSlide");
        LeanTween.value(gameObject, normalSpeed, 0, 3f).setOnUpdate((float val) =>
        {
            FrwSpeed = val;
        }).setOnComplete(() =>
        {
            InRoll = false;
            m_Animator.applyRootMotion = true;
            m_Animator.SetTrigger("FinishDance");
        });
    }

    private void SwipeLeftHandle() {
        if (isCinematic)
            return;

        if (isReverseMovement)
        {
            SwipeRight = true;
        }else
        {
            SwipeLeft = true;
        }
    }
    private void SwipeRightHandle() {
        if (isCinematic)
            return;

        if (isReverseMovement)
        {
            SwipeLeft = true;
        }
        else
        {
            SwipeRight = true;
        }
    }
    private void SwipeUpHandle() {
        if (isCinematic)
            return;

        SwipeUp = true;
        swipeUpCountdown = 1;
    }
    private void SwipeDownHandle() {
        if (isCinematic)
            return;

        SwipeDown = true;
    }

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
        
        characterBody.SetActive(false);
        FrwSpeed = 0;
        isGameActive = false;
        isCinematic = true;
        _igm.GameOver();

        float enemyDelay = isStunned ? 0 : 1;
        StartCoroutine(ActivateEnemyGoingToDeadBody(enemyDelay));
    }
    IEnumerator GetStunned()
    {
        isStunned = true;
        isInStunDelay = true;
        characterEffects.EffectProcess(CharacterEffectTypes.Stunned, true);
        EnemyCloser();
        yield return new WaitForSeconds(0.5f);
        isInStunDelay = false;

        yield return new WaitForSeconds(5f);

        isStunned = false;
        characterEffects.EffectProcess(CharacterEffectTypes.Stunned, false);
        EnemyGetAway();
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
        }
    }


    public void FootPrint()
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

    protected int lastIndexOfTrail;
    private void LeaveTrail()
    {
        Debug.DrawRay(transform.position, -transform.up, Color.red, footPrintMaxDistance);
        if (InRoll)
        {
            if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, footPrintMaxDistance) && (hit.collider.CompareTag("Snow") || hit.collider.CompareTag("IceGround")))
            {
                int indexOfTrail = hit.collider.CompareTag("Snow") ? 0 : 1;
                if (!isTrailing || indexOfTrail != lastIndexOfTrail)
                {
                    slideTrailObject[indexOfTrail].transform.position = transform.position;
                    slideTrailRenderer[indexOfTrail].Clear();
                }
                isTrailing = true;

                slideTrailObject[indexOfTrail].transform.position = transform.position;

                lastIndexOfTrail = indexOfTrail;
            }
            else if (isTrailing)
            {
                isTrailing = false;
            }
        }else if (isTrailing)
        {
            isTrailing = false;
        }
    }
    #endregion

    #region Enemy Control Codes
    private void EnemyCloser()
    {
        enemyHolder.SetActive(true);
        enemyAnimator.SetBool("isRunning", true);
        LeanTween.moveLocalZ(enemyHolder, -chasingDistance, 0.5f);
    }

    private void EnemyGetAway()
    {
        if (isGameActive)
        {
            LeanTween.moveLocalZ(enemyHolder, -10f, 0.5f).setOnComplete(() => {
                enemyHolder.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                enemyHolder.SetActive(false);
            });
        }
    }

    IEnumerator ActivateEnemyGoingToDeadBody(float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);

        LeanTween.cancel(enemyHolder);
        enemyHolder.SetActive(true);
        enemyAnimator.SetBool("isRunning", true);
        enemyClosingToBody = true;
    }

    protected string enemyAnimateType = "Running";
    private void EnemyGoingToDeadBody()
    {
        float closingSpeed = deadBodyClosingSpeed;
        float maxDistance = Vector3.Distance(enemyHolder.transform.position, enemyTargetBody.position);
        maxDistance -= deadBodyStopDistance;
        Vector3 directionToMove = enemyTargetBody.position - enemyHolder.transform.position;
        if (enemyAnimateType == "Running" && maxDistance < 5)
        {
            closingSpeed = deadBodyClosingSpeed * 0.5f;
            enemyAnimator.SetBool("isRunning", false);
            enemyAnimator.SetBool("isWalking", true);
            enemyAnimateType = "Walking";
        }
        else if (enemyAnimateType == "Walking" && maxDistance < 1)
        {
            enemyAnimator.SetBool("isWalking", false);
            enemyAnimateType = "Idle";
            enemyClosingToBody = false;
        }
        else if (enemyAnimateType == "Idle" && maxDistance > 5)
        {
            closingSpeed = deadBodyClosingSpeed;
            enemyAnimator.SetBool("isRunning", true);
            enemyAnimateType = "Running";
        }
        directionToMove = directionToMove.normalized * Time.deltaTime * closingSpeed;

        var lookPos = directionToMove;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        enemyHolder.transform.rotation = rotation; //Quaternion.Slerp(enemyHolder.transform.rotation, rotation, Time.deltaTime * 5f);

        //Debug.Log("Distance: " + maxDistance + " directionToMove: " + directionToMove + " ClampMagnitude: " + Vector3.ClampMagnitude(directionToMove, maxDistance));
        Physics.Raycast(enemyHolder.transform.position, -enemyHolder.transform.up, out RaycastHit hit, 10f);
        Vector3 targetPosition = enemyHolder.transform.position + Vector3.ClampMagnitude(directionToMove, maxDistance);
        targetPosition.y = hit.point.y;
        enemyHolder.transform.position = targetPosition;
    }

    #endregion
}
