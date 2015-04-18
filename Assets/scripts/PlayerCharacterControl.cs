using UnityEngine;
using System.Collections;

public class PlayerCharacterControl : MonoBehaviour 
{
    public int baseHp = 100;
    public int baseEloquency = 100;
    public float baseSpeed = 3.0f;
    //------------------
    private int m_hp = 100;
    private int m_eloquency = 100;

    private Rigidbody2D m_bodyRef = null;
    private Collider2D m_colliderRef = null;
    private ProgressBar m_hpBar = null;
    private SpriteRenderer m_spriteRendererRef = null;

    public delegate void OnDead();
    public OnDead m_OnDead;


    public void Initialize (Vector3 position, ProgressBar hpBar)
    {
        transform.position = position;
        m_hp = baseHp;
        m_eloquency = baseEloquency;
        SetHPBar(hpBar);
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
        movement *= baseSpeed;
        GetComponent<Rigidbody2D>().velocity = movement;
	}

    public void UnloadLevel ()
    {
        GameObject.Destroy(gameObject);
    }

    public void SetHPBar (ProgressBar hpBar)
    {
        m_hpBar = hpBar;
        m_hpBar.Build(m_hp / baseHp, 0.0f, "hud");
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

        m_hpBar.SetValue(m_hp / (float)baseHp);
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
        GameObject.Destroy(gameObject);
        yield return null;
    }
}
