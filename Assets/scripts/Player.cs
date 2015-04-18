using UnityEngine;
using System;
using System.Collections;

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
    public float m_baseEloquency = 100;
    public float m_baseSpeed = 3.0f;
    public int m_refillRate = 10;   // 4 units per second?

    //------------------
    private float m_defaultAttackDownTime = -1.0f;
    private float m_specialAttackDownTime = -1.0f;

    public float m_defaultAttackStart = -1.0f;
    public float m_specialAttackActiveStart = -1.0f;
    public float m_specialAttackCooldownStart = -1.0f;
    //------------------
    
    private int m_hp = 100;
    private float m_eloquency = 100;

    private Rigidbody2D m_bodyRef = null;
    private Collider2D m_colliderRef = null;
    private ProgressBar m_hpBar = null;
    private ProgressBar m_eloquencyBar = null;
    private SpriteRenderer m_spriteRendererRef = null;

    public delegate void OnDead();
    public OnDead m_OnDead;

    //-----------------------------

    //-----------------------------
    public void OnLoadLevel(Vector3 position, ProgressBar hpBar, ProgressBar eloquencyBar)
    {
        transform.position = position;
        m_hp = m_baseHp;
        m_eloquency = m_baseEloquency;
        SetHPBar(hpBar);
        SetEloquencyBar(eloquencyBar);

        m_defaultAttackDownTime = -1.0f;
        m_specialAttackDownTime = -1.0f;
    }

	// Use this for initialization
	void Start () 
    {
        m_bodyRef = GetComponent<Rigidbody2D>();
        m_colliderRef = GetComponent<Collider2D>();
        m_spriteRendererRef = GetComponent<SpriteRenderer>();
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

        if (m_defaultAttackStart >= 0 && Time.time - m_defaultAttackStart >= m_defaultAttack.cooldown)
        {
            m_defaultAttackStart = -1.0f;
        }

        // Default attack stuff
        if (Input.GetButtonDown(defaultAttackButtonName))
        {
            
        }
        else if (Input.GetButtonUp(defaultAttackButtonName))
        {
            if (m_eloquency >= m_defaultAttack.minCost && m_defaultAttackStart < 0)
            {
                Enemy e = GameManager.Instance.GetClosestEnemy(transform.position);
                if (e != null && Vector3.Distance(transform.position, e.transform.position) <= m_defaultAttack.minRadius)
                {
                    Debug.Log("Insulting!!!");
                    e.OnPlayerUsedDefaultAttack(m_defaultAttack.minDamage);
                    m_defaultAttackStart = Time.time;
                    m_eloquency -= m_defaultAttack.minCost;
                    if (m_eloquency <= 0.0f)
                    {
                        m_eloquency = 0.0f;
                    }
                    
                    if (m_eloquencyBar != null)
                    {
                        m_eloquencyBar.SetValue(m_eloquency / m_baseEloquency);
                    }
                    // Update eloquency feedback
                }
            }
        }
        else if (Input.GetButton(defaultAttackButtonName))
        {

        }

        if (Input.GetButtonDown(specialAttackButtonName))
        {
            Debug.Log("Pressed Special!");
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
            Debug.Log("Pressed Next!");
        }
        else if (Input.GetButtonDown(cycleSpecialPrevButtonName))
        {
            Debug.Log("Pressed Prev!");
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

    public void SetEloquencyBar (ProgressBar eloquencyBar)
    {
        m_eloquencyBar = eloquencyBar;
        m_eloquencyBar.Build(m_eloquency / m_baseEloquency, 0.0f, "hud");
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
