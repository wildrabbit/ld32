using UnityEngine;
using System.Collections.Generic;


public class WallOfText : MonoBehaviour 
{
    public GameObject m_particlePrefab;
    public int m_maxParticles = 20;

    public float m_particleLifetime = 3.0f;
    public float m_particleSpeed = 0.2f;

    public Color m_colour = Color.black;
    private List<string> m_sentencesRef = null;

    private float m_start = -1.0f;
    private float m_duration = -1.0f;

    private WOTParticle[] m_pool = new WOTParticle[0];
    
    void Start ()
    {
        m_pool = new WOTParticle[m_maxParticles];
        for (int i = 0; i < m_maxParticles; ++i)
        {
            m_pool[i] = Instantiate<GameObject>(m_particlePrefab).GetComponent<WOTParticle>();
            m_pool[i].transform.parent = transform;
            m_pool[i].transform.localPosition = Vector3.zero;
            Vector3 startScale = Vector3.zero;
            startScale.z = 1.0f;
            m_pool[i].transform.localScale = startScale;
            m_pool[i].GetComponent<TextMesh>().text = "";
            m_pool[i].gameObject.SetActive(false);
        }
    }

    void Update ()
    {
        transform.position = Vector3.zero; //Forget about the parent!
        if (m_start >= 0)
        {
            if (Time.time - m_start >= m_duration)
            {
                m_start = -1.0f;
            }
            else
            {
                if (Random.Range(0.0f, 1.0f) < 0.05f)
                {
                    SpawnParticle();
                }
            }
        }
    }

    public void Play(float duration, List<string> sentences)
    {
        m_sentencesRef = sentences;

        m_start = Time.time;
        m_duration = duration;
    }

    WOTParticle GetParticle ()
    {
        WOTParticle retParticle = null;
        for (int i = 0; i < m_maxParticles; ++i)
        {
            if (!m_pool[i].gameObject.activeSelf)
            {
                retParticle = m_pool[i];
                break;
            }
        }        
        if (retParticle != null)
        {
            retParticle.gameObject.SetActive(true);
        }
        return retParticle;
    }
    
    void SpawnParticle ()
    {
        string text = (m_sentencesRef != null && m_sentencesRef.Count > 0) 
            ? m_sentencesRef[Random.Range(0,m_sentencesRef.Count)]
            : "";
        if (text == "")    
        {
            return;
        }

        WOTParticle p = GetParticle();
        if (p != null)
        {
            Vector3 dir = Vector3.zero;
            dir.x = -1.0f + Random.Range(0.0f, 2.0f);
            dir.y = -1.0f + Random.Range(0.0f, 2.0f);
            dir.Normalize();

            float speed = m_particleSpeed + Random.Range(-0.5f,0.5f);

            Vector3 local = transform.parent.position;
            local.x += -1.5f + Random.Range(0.0f,3.0f);
            local.y += -0.5f + Random.Range(0.0f,0.5f);

            p.Play(text, m_particleLifetime,0.1f, 0.5f, m_colour, 0.5f, 1.0f, local, dir, speed, OnLifetimeParticleExpired);
        }
    }

    void OnLifetimeParticleExpired (GameObject particleGO)
    {
        WOTParticle p = particleGO.GetComponent<WOTParticle>();
        if (p != null)
        {
            RecycleParticle(p);
        }
    }

    void RecycleParticle (WOTParticle particle)
    {
        particle.transform.localPosition = Vector3.zero;
        particle.gameObject.SetActive(false);
    }
    
}
