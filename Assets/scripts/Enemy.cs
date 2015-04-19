using UnityEngine;
using System.Collections;

public enum EnemyState
{
    kNone = -1,
    kIdle,
    kPatrol,
    kChase,
    kAttack,
    kHit,
    kDying,
    kDead,
    kFear,
    kStun,
}

public class Enemy : MonoBehaviour 
{
    public GameObject m_hpBarPrefab = null;

    public float m_maxSpeedThreshold = 0.75f;
    public float m_approachThreshold = 0.005f;
    public float m_detectRadius = 2.0f;
    public float m_attackRadius = 1.0f;
    public float m_patrolSpeed = 3.0f;
    public float m_chaseSpeed = 5.0f;

    public int m_damage = 5;

    public float m_attackCooldown = 1.5f;
    private float m_startAttack = 0.0f;
    private bool m_frozenCooldown = false;

    public Vector3[] m_patrolPoints = new Vector3[0];
    private int m_numPatrolPoints = 0;
    private bool m_reversePatrol = false;

    private int m_currentIdx = 0;
    private Vector3 m_startPosition = Vector3.zero;
    private Vector3 m_targetPosition = Vector3.zero;
    private Vector3 m_velocity = Vector3.zero;
    private float m_currentSpeed = 0.0f;
    private EnemyState m_currentState = EnemyState.kNone;
    private EnemyState m_previousState = EnemyState.kNone;

    public int m_baseHP = 100;
    private int m_hp= 100;

    //private bool m_charmed = false;
    private float m_specialInflictedStart = -1.0f;
    private float m_specialInflictedTime = -1.0f;
    private AttackType m_specialInflictedType = AttackType.kNone;

    private Player m_playerRef = null;

    public delegate void OnDead(Enemy e);
    public OnDead m_OnDead;

    private BoxCollider2D m_colliderRef = null;
    private SpriteRenderer m_spriteRendererRef = null;
    private ProgressBar m_hpBar = null;

    public GameObject m_textPrefab = null;
    public GameObject m_stunPrefab = null;

    private Transform m_textAttachment = null;
    private Transform m_stunPSAttachment = null;


	// Use this for initialization
	void Start () 
    {
        m_hp = m_baseHP;
        //m_charmed = false;
        m_playerRef = GameManager.Instance.GetPlayer();
        m_playerRef.m_OnDead += OnPlayerDied;
        m_startAttack = -1;
        if (m_numPatrolPoints > 1)
        {
            ChangeState(EnemyState.kPatrol);
        }
        else
        {
            ChangeState(EnemyState.kIdle);
        }

	}
	
	// Update is called once per frame
	void Update () 
    {
        UpdateState();
	}

    public void LoadLevel (Vector3 startPos, Vector3[] patrolPoints)
    {
        m_colliderRef = GetComponent<BoxCollider2D>();
        m_spriteRendererRef = GetComponent<SpriteRenderer>();
        m_stunPSAttachment = transform.FindChild("stunPSAttachment");
        m_textAttachment = transform.FindChild("textAttachment");
        
        m_startPosition = startPos;
        m_patrolPoints = patrolPoints;
        m_numPatrolPoints = m_patrolPoints.Length;
        m_currentIdx = 0;
        transform.position = m_startPosition;

        if (m_hpBarPrefab != null)
        {
            GameObject go = Instantiate<GameObject>(m_hpBarPrefab);
            go.transform.parent = this.transform;
            go.transform.localPosition = new Vector3(0.0f, -0.09f, 0.0f);
            m_hpBar = go.GetComponent<ProgressBar>();
            m_hpBar.Build(m_hp / (float)m_baseHP, 0.0f, "units");
            Vector3 adjustPosition = m_hpBar.transform.localPosition;
            adjustPosition.x = -(m_hpBar.width * GameManager.kUnitsPerPixel * 0.5f);
            m_hpBar.transform.localPosition = adjustPosition;
        }
    }

    public void UnloadLevel()
    {
        GameObject.Destroy(gameObject);
    }

    // TODO: Refactor to a better state machine implementation
    void ChangeState (EnemyState next)
    {
        ExitState();
        m_currentState = next;
        EnterState();
    }

    void EnterState ()
    {
        switch (m_currentState)
        {
            case EnemyState.kIdle:
            {
                if (Vector2.Distance(m_targetPosition, m_startPosition) > 1.0f)
                {
                    m_targetPosition = m_startPosition;
                }
                m_currentSpeed = 0.0f;
                break;
            }
            case EnemyState.kPatrol:
            {
                m_targetPosition = m_patrolPoints[m_currentIdx];
                DetermineSpeed(Vector3.Distance(transform.position, m_targetPosition), m_patrolSpeed);
                break;
            }
            case EnemyState.kChase:
            {
                m_targetPosition = m_playerRef.transform.position;
                DetermineSpeed(Vector3.Distance(transform.position, m_targetPosition), m_chaseSpeed);
                break;
            }
            case EnemyState.kAttack:
            {
                m_targetPosition = m_playerRef.transform.position;
                DetermineSpeed(Vector3.Distance(transform.position, m_targetPosition), m_chaseSpeed);
                if (!m_frozenCooldown)
                {
                    m_startAttack = -1.0f;
                }
                else
                {
                    m_frozenCooldown = false;
                }
                break;
            }
            case EnemyState.kStun:
            {
                if (m_stunPSAttachment != null && m_stunPrefab != null)
                {
                    GameObject go = Instantiate<GameObject>(m_stunPrefab);
                    go.GetComponent<DelayedDeath>().Play(m_specialInflictedTime);
                    go.transform.parent = m_stunPSAttachment;
                    go.transform.localPosition = Vector3.zero;
                }
                m_specialInflictedStart = Time.time;
                m_currentSpeed = 0.0f;
                break;
            }
            case EnemyState.kHit:
            {
                if (m_textAttachment != null && m_textPrefab != null)
                {
                    if (m_textAttachment.childCount != 0)
                    {
                        foreach (Transform t in m_textAttachment.GetComponentsInChildren<Transform>())
                        {
                            if (t != m_textAttachment)
                            {
                                GameObject.Destroy(t.gameObject);
                            }
                        }
                    }

                    string text = TextManager.Instance.GetRandomComplaint();
                    if (text != "")
                    {
                        GameObject go = Instantiate<GameObject>(m_textPrefab);
                        go.GetComponent<DelayedDeath>().Play(1.5f);
                        go.GetComponent<TextMesh>().text = text;
                        go.transform.parent = m_textAttachment;
                        go.transform.localPosition = Vector3.zero;
                    }
                }
                break;
            }
            case EnemyState.kDying:
            {
                if (m_textAttachment != null && m_textPrefab != null)
                {
                    if (m_textAttachment.childCount != 0)
                    {
                        foreach (Transform t in m_textAttachment.GetComponentsInChildren<Transform>())
                        {
                            if (t != this.transform)
                            {
                                GameObject.Destroy(t.gameObject);
                            }
                        }
                    }
                    string text = TextManager.Instance.GetRandomEpitaph();
                    if (text != "")
                    {
                        GameObject go = Instantiate<GameObject>(m_textPrefab);
                        go.GetComponent<DelayedDeath>().Play(1.0f);
                        go.GetComponent<TextMesh>().text = text;
                        go.transform.parent = m_textAttachment;
                        go.transform.localPosition = Vector3.zero;
                    }
                }

                StartCoroutine(Fadeout());
                break;
            }
            case EnemyState.kDead:
            {
                if (m_OnDead != null)
                {
                    m_OnDead(this);
                }
                break;
            }
            default: break;
        }
    }

    void ExitState ()
    {
        switch (m_currentState)
        {
            case EnemyState.kDead:
                {
                    m_OnDead = null;
                    GameObject.Destroy(gameObject);
                    break;
                }
            default: break;
        }
    }

    void UpdateState ()
    {
        if (m_currentState == EnemyState.kNone) return;
        if (m_playerRef == null || transform == null)
        {
            Debug.Break();
        }


        EnemyState next = m_currentState;
        Vector3 m_playerVec = m_playerRef.transform.position - transform.position;
        float playerDistance = m_playerVec.magnitude;

        switch (m_currentState)
        {
            case EnemyState.kIdle:
            {
                next = UpdateIdle(playerDistance);
                break;
            }
            case EnemyState.kPatrol:
            {
                next = UpdatePatrol(playerDistance);
                break;
            }
            case EnemyState.kChase:
            {
                next = UpdateChase(m_playerVec, playerDistance);
                break;
            }
            case EnemyState.kAttack:
            {
                next = UpdateAttack(m_playerVec, playerDistance);
                break;
            }
            case EnemyState.kStun:
            {
                if (m_specialInflictedStart >= 0.0f && Time.time - m_specialInflictedStart >= m_specialInflictedTime)
                {
                    if (m_previousState == EnemyState.kNone)
                    {
                        Debug.Log("STUN WTF! NONE ATTEMPTED");
                    }
                    next = m_previousState;
                    m_specialInflictedTime = -1.0f;
                    m_specialInflictedStart = -1.0f;
                    m_specialInflictedType = AttackType.kNone;
                    m_previousState = EnemyState.kNone;
                }
                break;
            }
            case EnemyState.kHit:
            {
                next = m_previousState;
                if (m_previousState == EnemyState.kNone)
                {
                    Debug.Log("HIT WTF! NONE ATTEMPTED");
                }
                m_previousState = EnemyState.kNone;
                break;
            }
            case EnemyState.kDying:
            {
                break;
            }
            case EnemyState.kDead:
            {
                next = EnemyState.kNone;
                break;
            }
            default: break;
        }

        // UPDATE CALCULATIONS
        m_velocity = (m_targetPosition - transform.position);
        m_velocity.Normalize();

        if (m_velocity != Vector3.zero && !Mathf.Approximately(m_currentSpeed, 0.0f))
        {
            transform.Translate(m_velocity * Time.deltaTime * m_currentSpeed);
        }

        if (next != m_currentState)
        {
            ChangeState(next);
        }
    }

    EnemyState UpdateIdle (float playerDistance)
    {
        if (playerDistance < m_detectRadius)
        {
            if (playerDistance < m_attackRadius)
            {
                return EnemyState.kAttack;
            }
            else
            {
                return EnemyState.kChase;
            }
        }
        return EnemyState.kIdle;
    }

    EnemyState UpdatePatrol (float playerDistance)
    {
        if (playerDistance < m_detectRadius)
        {
            if (playerDistance < m_attackRadius)
            {
                return EnemyState.kAttack;
            }
            else
            {
                return EnemyState.kChase;
            }
        }
        // patrol update
        float distanceToTarget = Vector3.Distance(transform.position, m_targetPosition);
        if (distanceToTarget <= m_approachThreshold)
        {
            if (m_reversePatrol)
            {
                m_currentIdx--;
                if (m_currentIdx < 0)
                {
                    m_currentIdx = 1;
                    m_reversePatrol = false;
                }
            }
            else
            {
                m_currentIdx++;
                if (m_currentIdx == m_numPatrolPoints)
                {
                    m_currentIdx = m_numPatrolPoints - 2;
                    m_reversePatrol = true;
                }
            }
            m_targetPosition = m_patrolPoints[m_currentIdx];
        }

        DetermineSpeed(distanceToTarget, m_patrolSpeed);
        
        return EnemyState.kPatrol;
    }

    EnemyState UpdateAttack(Vector3 chaseDirection, float targetDistance)
    {
        if (targetDistance > m_attackRadius)
        {
            if (targetDistance < m_detectRadius)
            {
                DetermineSpeed(targetDistance, m_chaseSpeed);
                return EnemyState.kChase;
            }
            else
            {
                DetermineSpeed(targetDistance, m_patrolSpeed);
                return (m_numPatrolPoints > 1 ? EnemyState.kPatrol : EnemyState.kIdle);
            }
        }

        if (m_startAttack < 0)
        {
            m_playerRef.OnEnemyAttacked(this);
            m_startAttack = Time.time;
        }
        else if (Time.time - m_startAttack >= m_attackCooldown)
        {
            m_startAttack = -1.0f;
        }

        m_targetPosition = m_playerRef.transform.position;
        DetermineSpeed(targetDistance, m_chaseSpeed);
     
        return EnemyState.kAttack;
    }

    EnemyState UpdateChase(Vector3 chaseDirection, float targetDistance)
    {
        if (targetDistance > m_detectRadius)
        {
            DetermineSpeed(targetDistance, m_patrolSpeed);

            if (m_numPatrolPoints > 1)
            { 
                return EnemyState.kPatrol;
            }
            else
            {
                return EnemyState.kIdle;
            }
        }
        else if (targetDistance< m_attackRadius)
        {
            DetermineSpeed(targetDistance, m_chaseSpeed);
            return EnemyState.kAttack;
        }

        m_targetPosition = m_playerRef.transform.position;
        Vector3 noise = new Vector3(-0.15f + Random.Range(0.0f, 0.3f), -0.15f + Random.Range(0.0f, 0.3f), 0.0f);
        m_targetPosition += noise;

        RaycastHit2D obstacleHit = Physics2D.Raycast(transform.position, chaseDirection, targetDistance);
        if (obstacleHit.collider != null && obstacleHit.collider.transform != m_playerRef.transform)
        {
            if (chaseDirection.x > chaseDirection.y)
            {
                bool offsetDir = Random.Range(0.0f, 1.0f) > 0.5;
                float amount = Random.Range(0.0f, 0.5f);
                chaseDirection.x += (offsetDir)?amount : -amount;
            }
            else
            {
                bool offsetDir = Random.Range(0.0f, 1.0f) > 0.5;
                float amount = Random.Range(0.0f, 0.5f);
                chaseDirection.y += (offsetDir) ? amount : -amount;
            }
            chaseDirection.Normalize();
            chaseDirection *= targetDistance;
            m_targetPosition = transform.position + chaseDirection;
        }
        DetermineSpeed(targetDistance, m_chaseSpeed);

        return EnemyState.kChase;
    }

    void DetermineSpeed(float distanceToTarget, float maxSpeed)
    {
        if (distanceToTarget <= m_maxSpeedThreshold)
        {
            float range = m_maxSpeedThreshold - m_approachThreshold;
            float distanceRatio = distanceToTarget / range;

            m_currentSpeed = maxSpeed - Mathf.Lerp(0.0f, 1.0f, distanceRatio);
        }
        else
        {
            m_currentSpeed = maxSpeed;
        }
    }

    public void OnPlayerDied ()
    {
        m_playerRef = null;
        ChangeState(EnemyState.kNone);
    }

    public void OnPlayerUsedDefaultAttack (int damage)
    {
        if (m_playerRef != null)
        {
            m_hp -= damage;
            if (m_hp <= 0)
            {
                m_hp = 0;
                ChangeState(EnemyState.kDying);
            }
            else
            {
                Debug.LogFormat("Saving prev. after hit {0}", m_currentState);
                if (m_previousState == EnemyState.kNone)
                {
                    m_previousState = m_currentState;
                }
                m_frozenCooldown = true;
                ChangeState(EnemyState.kHit);
            }
        }
        m_hpBar.SetValue(m_hp / (float)m_baseHP);
    }

    public void OnPlayerUsedSpecialAttack (AttackType attackType, float effectiveTime)
    {
        if (m_currentState == EnemyState.kDying || m_currentState == EnemyState.kDead || m_currentState == EnemyState.kNone)
        {
            return;
        }

        switch (attackType)
        {
            case AttackType.kStun:
                {
                    if (m_currentState == EnemyState.kStun)
                    {
                        m_specialInflictedStart = Time.time;
                    }
                    else
                    {
                        m_specialInflictedType = attackType;
                        if (m_previousState == EnemyState.kNone)
                        {
                            m_previousState = m_currentState;
                        }
                        m_specialInflictedTime = effectiveTime;
                        ChangeState(EnemyState.kStun);
                    }
                    break;
                }
            default: break;
        }
    }

    public IEnumerator Fadeout()
    {
        while (m_spriteRendererRef.color.a > 0.0f)
        {
            Color c = m_spriteRendererRef.color;
            c.a -= 0.1f;
            m_spriteRendererRef.color = c;
            yield return new WaitForSeconds(0.05f);
        }
        ChangeState(EnemyState.kDead);
        yield return null;
    }
}
