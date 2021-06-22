using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public enum SIDE { Left, Middle, Right };
public enum RunMode { Straight, Slope, Bridge };
public enum HitX { Left, Middle, Right, None };
public enum HitY { Up, Middle, Down, None };
public enum HitZ { Forward, Middle, Backward, None };

public class Character : MonoBehaviour
{
    [Header("Base Controls")]
    public SIDE m_Side = SIDE.Middle;
    public RunMode runMode = RunMode.Straight;
    public HitX hitX = HitX.None;
    public HitY hitY = HitY.None;
    public HitZ hitZ = HitZ.None;
    [SerializeField] private GameObject characterBody;
    [SerializeField] private GameObject animatorBody;
    [SerializeField] private Animator m_Animator;
    [SerializeField] private Transform groundDirection;
    [SerializeField] private float characterSwipeRotateValue;
    [SerializeField] private float XValue;
    [SerializeField] private float SpeedDodge;
    [SerializeField] private float JumpPower = 7f;
    [SerializeField] private float Speed = 10f;
    [SerializeField] private float ExtraSlopeSpeed = 5f;
    [SerializeField] private float SlopeSpeedMultiple = 2f;
    [SerializeField] private float slopeGroundDistance = 0.2f;

    private bool isGameActive;
    private bool SwipeLeft, SwipeRight, SwipeUp, SwipeDown;
    private bool isSlopeGrounded = false;
    private float x;
    private float y;
    private float NewXPos = 0f;
    private float slopeSpeed = 0f;
    private float FrwSpeed;

    private float rollingLowCollideTime = 0.6f;
    private float slidingLowCollideTime = 1.1f;
    private float ColHeight;
    private float ColCenterY;

    private float forwardAngle;
    Ray groundRay;
    RaycastHit groundHit;

    [Header("Trail And Prints")]
    [SerializeField] private GameObject footPrint;
    [SerializeField] private float footPrintMaxDistance = 0.2f;
    [SerializeField] private GameObject[] slideTrailObject;
    [SerializeField] private TrailRenderer[] slideTrailRenderer;

    [Header("Hit And Effects")]
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
    public bool isJumping;
    [SerializeField] private bool isOnGround;
    [SerializeField] private bool isStunned;
    [SerializeField] private bool isInStunDelay;

    /*[HideInInspector]*/ public bool isCinematic;
    /*[HideInInspector]*/ public bool isReverseMovement = false;
    /*[HideInInspector]*/ public bool isTrailing = false;

    ObjectPooler _footPrintPool;

    [SerializeField] private InGameManager _igm;
    [SerializeField] private CameraFollow _cameraFollow;
    [SerializeField] private CharacterController m_char;
    private SwipeDetection swipeDetection;

    private void Awake()
    {
        swipeDetection = SwipeDetection.instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        /*_igm = FindObjectOfType<InGameManager>();
        _cameraFollow = FindObjectOfType<CameraFollow>();
        m_char = GetComponent<CharacterController>();*/
        ColHeight = m_char.height;
        ColCenterY = m_char.center.y;
        FrwSpeed = Speed;

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
        animatorBody.transform.localPosition = Vector3.zero;
        animatorBody.transform.rotation = Quaternion.identity;
        m_Animator.SetTrigger("Run");
    }

    public void ResetPlayer()
    {
        m_Animator.applyRootMotion = false;
        m_char.enabled = false;
        m_char.transform.position = new Vector3(0, 0.1f, 5);
        m_char.transform.rotation = Quaternion.identity;
        m_char.enabled = true;
        characterBody.SetActive(true);

        // Character props
        isCinematic = false;
        isGameActive = false;
        isReverseMovement = false;
        isTrailing = false;
        InRoll = false;
        isFalling = false;
        isJumping = false;
        isStunned = false;
        isInStunDelay = false;
        enemyClosingToBody = false;
        isSlopeGrounded = false;
        m_Side = SIDE.Middle;
        runMode = RunMode.Straight;
        hitX = HitX.None;
        hitY = HitY.None;
        hitZ = HitZ.None;
        FrwSpeed = Speed;
        slopeSpeed = 0;
        x = 0;
        NewXPos = 0;
        y = 0;

        // Swipes
        SwipeLeft = false;
        SwipeRight = false;
        SwipeDown = false;
        SwipeUp = false;

        // Hit And Effects
        characterRagdoll.transform.parent = gameObject.transform;
        characterRagdoll.transform.localPosition = Vector3.zero;
        characterRagdoll.SetActive(false);
        characterEffects.EffectProcess(CharacterEffectTypes.Stunned, false);

        // Enemy Controls
        LeanTween.cancel(enemyHolder);
        enemyClosingToBody = false;
        enemyHolder.SetActive(false);
        enemyHolder.transform.position = new Vector3(0, 0, -10f);
        enemyHolder.transform.rotation = Quaternion.identity;
        enemyAnimator.SetBool("isRunning", false);
        enemyAnimator.SetBool("isWalking", false);
    }

    void Update()
    {
        isOnGround = m_char.isGrounded;
        if (isGameActive && !isCinematic)
        {
            if (runMode == RunMode.Straight)
            {
                StraightMove();
            }else if(runMode == RunMode.Slope)
            {
                SlopeMove();
            }else if(runMode == RunMode.Bridge)
            {
                NewXPos = 0;
                m_Side = SIDE.Middle;

                StraightMove();
            }
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
    private void StraightMove()
    {
        Rotate();
        Jump();
        Roll();

        groundDirection.eulerAngles = new Vector3(0, 0, 0);
        if (slopeSpeed > 0)
        {
            isSlopeGrounded = false;
            slopeSpeed -= SlopeSpeedMultiple * Time.deltaTime;
            slopeSpeed = Mathf.Clamp(slopeSpeed, 0, ExtraSlopeSpeed);
        }
        Vector3 moveVector = new Vector3(x - transform.position.x, y * Time.deltaTime, (FrwSpeed + (slopeSpeed * 0.3f)) * Time.deltaTime);
        m_char.Move(moveVector);

        LeaveTrail();
    }
    private void SlopeMove()
    {
        if (InRoll)
            InRoll = false;

        Rotate();
        groundRay.origin = transform.position;
        groundRay.direction = Vector3.down;

        //Debug.DrawRay(groundRay.origin, groundRay.direction * slopeGroundDistance, Color.red, 0.1f);
        if (Physics.Raycast(groundRay, out groundHit, slopeGroundDistance) && m_char.isGrounded)
        {
            isSlopeGrounded = true;
            if (groundHit.collider.name == "Slope Ground")
            {
                m_Animator.SetTrigger("SlopeSliding");
                _cameraFollow.isRampSlidingMode = true;
            }else if (groundHit.collider.name == "Ground")
            {
                runMode = RunMode.Straight;
                m_Animator.SetTrigger("JumpToRoll");
            }
            
        }
        else if (!Physics.Raycast(groundRay, out groundHit, slopeGroundDistance) && !m_char.isGrounded)
        {
            isSlopeGrounded = false;
        }

        if (isSlopeGrounded)
        {
            forwardAngle = Vector3.Angle(groundDirection.forward, groundHit.normal) - 90; //Chekcing the forwardAngle against the slope
            groundDirection.eulerAngles += new Vector3(-forwardAngle, 0, 0); //Rotating groundDirection X
            slopeSpeed += SlopeSpeedMultiple * Time.deltaTime;
            slopeSpeed = Mathf.Clamp(slopeSpeed, 0, ExtraSlopeSpeed);

            Vector3 forwardMove = groundDirection.forward * (FrwSpeed + slopeSpeed) * Time.deltaTime;
            Vector3 moveVector = new Vector3(x - transform.position.x, forwardMove.y, forwardMove.z);
            m_char.Move(moveVector);
            LeaveTrail();
        }
        else
        {
            //slopeSpeed = 0;
            groundDirection.eulerAngles = new Vector3(0, 0, 0);
            Jump();

            Vector3 moveVector = new Vector3(x - transform.position.x, y * Time.deltaTime, FrwSpeed * Time.deltaTime);
            m_char.Move(moveVector);
        }
    }

    private void Rotate()
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
                }
                else if (m_Side == SIDE.Right)
                {
                    NewXPos = 0;
                    m_Side = SIDE.Middle;
                }
            }
        }
        else if (SwipeRight)
        {
            SwipeRight = false;
            if (!InRoll)
            {
                if (m_Side == SIDE.Middle)
                {
                    NewXPos = XValue;
                    m_Side = SIDE.Right;
                }
                else if (m_Side == SIDE.Left)
                {
                    NewXPos = 0;
                    m_Side = SIDE.Middle;
                }
            }
        }
        float rotY = ((NewXPos - x) / 2.5f) * characterSwipeRotateValue;
        Quaternion swipeRotationEuler = isSlopeGrounded ? Quaternion.Euler(new Vector3(0, 0, rotY * 0.33f)) : Quaternion.Euler(new Vector3(0, rotY, 0));
        characterBody.transform.rotation = swipeRotationEuler;
        if (isStunned)
        {
            enemyHolder.transform.rotation = Quaternion.Euler(new Vector3(0, rotY, 0));
        }

        x = Mathf.Lerp(x, NewXPos, Time.deltaTime * SpeedDodge);
    }
    protected float swipeUpCountdown = 0;
    private void Jump()
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
    private void Roll()
    {
        RollCounter -= Time.deltaTime;
        if (RollCounter <= 0f)
        {
            RollCounter = 0f;
            m_char.center = new Vector3(0, ColCenterY, 0);
            m_char.height = ColHeight;
            InRoll = false;
        }

        if (SwipeDown)
        {
            SwipeDown = false;
            if (m_char.isGrounded)
            {
                RollCounter = slidingLowCollideTime;
                m_char.center = new Vector3(0, ColCenterY / 2f, 0);
                m_char.height = ColHeight / 2f;
                m_Animator.SetTrigger("Slide");
                InRoll = true;
            }else
            {
                RollCounter = rollingLowCollideTime;
                y -= 10f;
                m_char.center = new Vector3(0, ColCenterY / 2f, 0);
                m_char.height = ColHeight / 2f;
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
        m_char.center = new Vector3(0, ColCenterY / 2f, 0);
        m_char.height = ColHeight / 2f;
        LeanTween.value(gameObject, normalSpeed, 0, 3f).setOnUpdate((float val) =>
        {
            FrwSpeed = val;
        }).setOnComplete(() =>
        {
            InRoll = false;
            m_Animator.applyRootMotion = true;
            m_Animator.SetTrigger("FinishDance");
            m_char.center = new Vector3(0, ColCenterY, 0);
            m_char.height = ColHeight;
        });
    }
    public void SetAnimate(string animationName)
    {
        m_Animator.SetTrigger(animationName);
    }

    private void SwipeLeftHandle() {
        if (isCinematic || runMode == RunMode.Bridge)
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
        if (isCinematic || runMode == RunMode.Bridge)
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
        if (isCinematic || runMode == RunMode.Slope)
            return;

        SwipeUp = true;
        swipeUpCountdown = 1;
    }
    private void SwipeDownHandle() {
        if (isCinematic || runMode == RunMode.Slope)
            return;

        SwipeDown = true;
    }

    #endregion

    #region Hit Control Codes

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.CompareTag("Obstacle"))
        {
            OnCharacterColliderHit(hit.collider);
        }
    }
    public void OnCharacterColliderHit(Collider col)
    {
        if (isInStunDelay)
            return;
        
        hitX = GetHitX(col);
        hitY = GetHitY(col);
        hitZ = GetHitZ(col);

        //Debug.Log("Tag -> " + col.tag + " Hit(XYZ) -> " + hitX + " - " + hitY + " - " + hitZ);

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
    public void DieProcess(string type = "")
    {
        if (isStunned)
        {
            isStunned = false;
            characterEffects.EffectProcess(CharacterEffectTypes.Stunned, false);
        }


        FrwSpeed = 0;
        isGameActive = false;
        isCinematic = true;

        if (type == "Drown")
        {
            Debug.Log("Drowned");
        }else
        {
            characterRagdoll.transform.parent = null;
            characterRagdoll.SetActive(true);
            characterBody.SetActive(false);
        }
        
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
            hit = HitY.Up;
        else if (average < 0.66f)
            hit = HitY.Middle;
        else
            hit = HitY.Down;

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
        //Debug.DrawRay(transform.position, -transform.up, Color.red, footPrintMaxDistance);
        if (InRoll || isSlopeGrounded)
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
