using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum AttackType
{
    kNone = 0,
    kInsult,
    kStun,
    kCharm,
    kTerror,
    kDeathWord
}

[System.Serializable]
public class AttackConfig
{
    public AttackType type = AttackType.kNone;
    public bool available = true;

    public int minCost = 1;
    public int maxCost = 100;
    public int minDamage = 0;
    public int maxDamage = 0;
    public float minRadius = 0.0f;
    public float maxRadius = 0.0f;
    public float castTime = 0.1f;
    public float minEffectTime = 0.0f;
    public float maxEffectTime = 0.0f;
    public float cooldown = 0.0f;
    public Sprite icon = null;

    // Visual effect, cooldown?...
}


public class Player : MonoBehaviour 
{
    public string defaultAttackButtonName = "";
    public string specialAttackButtonName = "";
    public string cycleSpecialNextButtonName = "";
    public string cycleSpecialPrevButtonName = "";

    public float m_maxHoldTime = 0.3f; //Keep button pressed to raise effects to max

    public AttackConfig m_defaultAttack = new AttackConfig();
    public AttackConfig[] m_specialAttacks = new AttackConfig[0];
    public int m_selectedSpecialIdx = 0;

    public int m_baseHp = 100;
    public float m_baseEloquence = 100;
    public float m_baseSpeed = 3.0f;
    public int m_refillRate = 10;   // 4 units per second?

    //------------------
    //private float m_defaultAttackDownTime = -1.0f;
    //private float m_specialAttackDownTime = -1.0f;

    public float m_defaultAttackStart = -1.0f;
    public float m_specialAttackActiveStart = -1.0f;
    public float m_specialAttackCooldownStart = -1.0f;
    //------------------

    public GameObject m_stunAreaVfxPrefab;
    public GameObject m_textPrefab;
    public GameObject m_stunTextPrefab;

    public Transform m_vfxAttachment;
    public Transform m_textAttachment;
    
    private int m_hp = 100;
    public int CurrentHP
    {
        get
        {
            return m_hp;
        }
    }
    private float m_eloquence = 100;
    public int CurrentEloquence
    {
        get
        {
            return (int)m_eloquence;
        }
    }

    private Rigidbody2D m_bodyRef = null;
    private Collider2D m_colliderRef = null;
    private ProgressBar m_hpBar = null;
    private ProgressBar m_eloquenceBar = null;
    private SpriteRenderer m_spriteRendererRef = null;

    public delegate void OnDead();
    public OnDead m_OnDead;

    //-----------------------------

    //-----------------------------
    public void LoadSession (GameSession session)
    {
        if (session == null)
        {
            return;
        }

        m_hp = session.m_playerHP;
        m_eloquence = session.m_playerEloquence;
        m_selectedSpecialIdx = session.m_selectedSpecialIndex;
        
        if (m_hpBar != null)
        {
            m_hpBar.SetValue(m_hp / (float)m_baseHp);
        }
        if (m_eloquenceBar != null)
        {
            m_eloquenceBar.SetValue(m_eloquence / (float)m_baseEloquence);
        }
    }
    public void OnLoadLevel(Vector3 position, ProgressBar hpBar, ProgressBar eloquenceBar)
    {
        transform.position = position;
        m_hp = m_baseHp;
        m_eloquence = m_baseEloquence;
        SetHPBar(hpBar);
        SetEloquenceBar(eloquenceBar);

        //m_defaultAttackDownTime = -1.0f;
        //m_specialAttackDownTime = -1.0f;
    }

	// Use this for initialization
	void Start () 
    {
        m_bodyRef = GetComponent<Rigidbody2D>();
        m_colliderRef = GetComponent<Collider2D>();
        m_spriteRendererRef = GetComponent<SpriteRenderer>();
        m_vfxAttachment = transform.FindChild("textAttachment");
        m_textAttachment = transform.FindChild("vfxAttachment");
	}
	
	// Update is called once per frame
	void Update () 
    {
        float xValue = Input.GetAxis("Horizontal");
        float yValue = Input.GetAxis("Vertical");
        Vector3 movement = Vector3.zero;
        movement.x = xValue;
        movement.y = yValue;
        movement.Normalize();
        movement *= m_baseSpeed;
        m_bodyRef.velocity = movement;

        // Eloquence refill
        if (m_eloquence < m_baseEloquence)
        {
            m_eloquence += m_refillRate * Time.deltaTime;
            if (m_eloquence > m_baseEloquence)
            {
                m_eloquence = m_baseEloquence;
            }
            
            if (m_eloquenceBar != null)
            {
                m_eloquenceBar.SetValue(m_eloquence / m_baseEloquence);
            }
        }

        if (m_defaultAttackStart >= 0 && Time.time - m_defaultAttackStart >= m_defaultAttack.cooldown)
        {
            m_defaultAttackStart = -1.0f;
        }

        AttackConfig specialConfig = m_specialAttacks[m_selectedSpecialIdx];
        if (m_specialAttackActiveStart >= 0.0f)
        {
            if ( Time.time - m_specialAttackActiveStart >= specialConfig.castTime)
            {
                m_specialAttackActiveStart = -1.0f;
                Debug.LogFormat("zzzip....Finished applying {0}. Cooldown: {1}s", specialConfig.type.ToString(), specialConfig.cooldown);
                m_specialAttackCooldownStart = Time.time;
            }
            else
            {
                float range = specialConfig.minRadius;
                List<Enemy> enemiesInRange = GameManager.Instance.GetEnemiesInRange(transform.position, range);
                int rangeNum = enemiesInRange.Count;
                for (int i = 0; i < rangeNum; ++i)
                {
                    enemiesInRange[i].OnPlayerUsedSpecialAttack(specialConfig.type, specialConfig.minEffectTime);
                }
            }
        }
        
        if (m_specialAttackCooldownStart >= 0.0f)
        {
            if (Time.time - m_specialAttackCooldownStart >= specialConfig.cooldown)
            {
                Debug.Log("Finished special attack cooldown");
                m_specialAttackCooldownStart = -1.0f;
            }
            else
            {
                // Update some stuff?
            }
        }

        // Default attack stuff
        if (Input.GetButtonDown(defaultAttackButtonName))
        {
            
        }
        else if (Input.GetButtonUp(defaultAttackButtonName))
        {
            AttemptDefaultAttack();
        }
        else if (Input.GetButton(defaultAttackButtonName))
        {

        }

        if (Input.GetButtonDown(specialAttackButtonName))
        {
            AttemptSpecialAttack();
        }
        else if (Input.GetButtonUp(specialAttackButtonName))
        {

        }
        else if (Input.GetButton(specialAttackButtonName))
        {

        }

        // Cycling specials
        if (Input.GetButtonDown(cycleSpecialNextButtonName))
        {
            NextSpecial();
        }
        else if (Input.GetButtonDown(cycleSpecialPrevButtonName))
        {
            PrevSpecial();
        }        
	}

    void NextSpecial ()
    {
        m_selectedSpecialIdx = (m_selectedSpecialIdx + 1) % (m_specialAttacks.Length);
        
        Debug.LogFormat("NEXT: {0}", m_specialAttacks[m_selectedSpecialIdx].type.ToString());
        // dispatch event to update HUD
    }

    void PrevSpecial ()
    {
        m_selectedSpecialIdx--;
        if (m_selectedSpecialIdx < 0)
        {
            m_selectedSpecialIdx = m_specialAttacks.Length - 1;
        }
        Debug.LogFormat("PREV: {0}", m_specialAttacks[m_selectedSpecialIdx].type.ToString());
        // dispatch event to update HUD

    }

    void AttemptSpecialAttack ()
    {
        // HARDCODED!!
        AttackConfig specialCfg = Array.Find<AttackConfig>(m_specialAttacks, x => x.type == AttackType.kStun);

        if (m_eloquence < specialCfg.minCost)
        {
            Debug.LogFormat("Not enough eloquence for {0}. Needed: {1}, Had: {2}", specialCfg.type.ToString(), specialCfg.minCost, m_eloquence);
            return;
        }
        if (m_specialAttackActiveStart >= 0)
        {
            Debug.LogFormat("Still launching special attack. Don't be greedy!");
            return;
        }
        if (m_specialAttackCooldownStart >= 0)
        {
            float cooldownLeft = specialCfg.cooldown - (Time.time - m_specialAttackCooldownStart);
            Debug.LogFormat("Special attack on cooldown. Wait for {0:F2} more s", cooldownLeft);
            return;
        }
        
        m_specialAttackActiveStart = Time.time;
        switch(specialCfg.type)
        {
            case AttackType.kStun:
            {
                if (m_vfxAttachment != null)
                {
                    if (m_stunAreaVfxPrefab != null)
                    {
                        GameObject obj = Instantiate<GameObject>(m_stunAreaVfxPrefab);
                        obj.GetComponent<ScaleWithAlpha>().Play(specialCfg.castTime);
                        obj.transform.parent = m_vfxAttachment;
                        obj.transform.localPosition = Vector3.zero;
                    }
                }
                break;
            }
            default: break;
        }
        m_eloquence -= specialCfg.minCost;
        if (m_eloquence < 0.0f)
        {
            m_eloquence = 0.0f;
            // Event?
        }

        if (m_eloquenceBar != null)
        {
            m_eloquenceBar.SetValue(m_eloquence / m_baseEloquence);
        }
    }

    void AttemptDefaultAttack ()
    {
        if (m_eloquence < 0)
        {
            Debug.LogFormat("Not enough eloquence for the default attack. Needed: {1}, Had: {2}", m_defaultAttack.minCost, m_eloquence);
            return;
        }
        if (m_defaultAttackStart > 0)
        {
            float cooldownLeft = m_defaultAttack.cooldown - (Time.time - m_defaultAttackStart);
            Debug.LogFormat("Default attack on cooldown. Wait for {0:F2} more s", cooldownLeft);
            return;
        }

        Enemy e = GameManager.Instance.GetClosestEnemy(transform.position);
        if (e != null && Vector3.Distance(transform.position, e.transform.position) <= m_defaultAttack.minRadius)
        {
            Debug.Log("Insulting!!!");
            e.OnPlayerUsedDefaultAttack(m_defaultAttack.minDamage);
            m_defaultAttackStart = Time.time;
            m_eloquence -= m_defaultAttack.minCost;
            if (m_eloquence <= 0.0f)
            {
                m_eloquence = 0.0f;
            }

            if (m_eloquenceBar != null)
            {
                m_eloquenceBar.SetValue(m_eloquence / m_baseEloquence);
            }
            // TODO: Update eloquence feedback
        }
    }

    public void UnloadLevel ()
    {
        GameObject.Destroy(gameObject);
    }

    public void SetHPBar (ProgressBar hpBar)
    {
        m_hpBar = hpBar;
        m_hpBar.Build(m_hp / m_baseHp, 0.0f, "hud");
    }

    public void SetEloquenceBar (ProgressBar eloquenceBar)
    {
        m_eloquenceBar = eloquenceBar;
        m_eloquenceBar.Build(m_eloquence / m_baseEloquence, 0.0f, "hud");
    }

    public void OnEnemyAttacked (Enemy enemyRef)
    {
        //m_hp -= enemyRef.damage;
        m_hp -= 5;
        if (m_hp <= 0)
        {
            m_hp = 0;
            StartCoroutine(Fadeout());
        }

        m_hpBar.SetValue(m_hp / (float)m_baseHp);
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
        m_OnDead();
        m_OnDead = null;
        GameObject.Destroy(gameObject);
        yield return null;
    }
}
