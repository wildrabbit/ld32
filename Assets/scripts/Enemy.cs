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
    public float m_maxSpeedThreshold = 0.75f;
    public float m_approachThreshold = 0.005f;
    public float m_detectRadius = 2.0f;
    public float m_attackRadius = 1.0f;
    public float m_patrolSpeed = 3.0f;
    public float m_chaseSpeed = 5.0f;

    public Vector3[] m_patrolPoints = new Vector3[0];
    private int m_numPatrolPoints = 0;
    private bool m_reversePatrol = false;

    private int m_currentIdx = 0;
    private Vector3 m_startPosition = Vector3.zero;
    private Vector3 m_targetPosition = Vector3.zero;
    private Vector3 m_velocity = Vector3.zero;
    private float m_currentSpeed = 0.0f;
    private EnemyState m_currentState = EnemyState.kNone;

    public int m_baseHP = 100;
    private int m_hp= 100;

    private bool m_charmed = false;

    private PlayerCharacterControl m_playerRef = null;

	// Use this for initialization
	void Start () 
    {
        m_hp = m_baseHP;
        m_charmed = false;
        m_playerRef = GameObject.FindObjectOfType<PlayerCharacterControl>();
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
        m_startPosition = startPos;
        m_patrolPoints = patrolPoints;
        m_numPatrolPoints = m_patrolPoints.Length;
        m_currentIdx = 0;
        transform.position = m_startPosition;
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
                break;
            }
            case EnemyState.kPatrol:
            {
                m_targetPosition = m_patrolPoints[m_currentIdx];
                break;
            }
            default: break;
        }
    }

    void ExitState ()
    {
        switch (m_currentState)
        {
            default: break;
        }
    }

    void UpdateState ()
    {
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
            default: break;
        }

        // UPDATE CALCULATIONS
        m_velocity = (m_targetPosition - transform.position);
        m_velocity.Normalize();

        if (m_velocity != Vector3.zero)
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
        
        if (distanceToTarget <= m_maxSpeedThreshold)
        {
            float range = m_maxSpeedThreshold - m_approachThreshold;
            float distanceRatio = distanceToTarget / range;

            m_currentSpeed = m_patrolSpeed - Mathf.Lerp(0.0f, 1.0f, distanceRatio);
        }
        else
        {
            m_currentSpeed = m_patrolSpeed;        
        }
        return EnemyState.kPatrol;
    }
}
