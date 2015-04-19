using UnityEngine;
using System.Collections;

public class ScaleWithAlpha : MonoBehaviour 
{
    private float m_start = -1.0f;
    private float m_total = -1.0f;
    private SpriteRenderer m_renderer = null;

    private float m_startScale = 0.0f;
    private float m_endScale = 1.0f;

    private float m_startAlpha = 0.0f;
    private float m_endAlpha = 1.0f;
    
    public delegate void OnTimeoutDelegate(GameObject go);
    private OnTimeoutDelegate m_onTimeout;
    public OnTimeoutDelegate OnTimeout
    {
        get
        {
            return m_onTimeout;
        }
    }

    public void Play(float duration, float startScale, float endScale, float startAlpha, float endAlpha, OnTimeoutDelegate timeout = null)
    {
        m_startScale = startScale;
        m_endScale = endScale;

        m_startAlpha = startAlpha;
        m_endAlpha = endAlpha;
        
        Vector3 scale = Vector3.zero;
        scale.x = scale.y = m_startScale;
        scale.z = 1.0f;
        transform.localScale = scale;
        
        Color col = m_renderer.color;
        col.a = m_startAlpha;
        m_renderer.color = col;

        m_total = duration;
        m_start = Time.time;

        if (timeout != null)
        {
            m_onTimeout -= timeout;
            m_onTimeout += timeout;
        }
        
    }

	// Use this for initialization
	void Awake () 
    {
        m_renderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (m_start >= 0)
        {
            float delta = Time.time - m_start;
            if (delta <= m_total)
            {
                float elapsedRatio = delta / m_total;
                float value = Mathf.Lerp(0.0f, 1.0f, elapsedRatio);

                Color col = m_renderer.color;
                col.a = m_startAlpha + value * (m_endAlpha - m_startAlpha);
                m_renderer.color = col;

                float scaleValue = m_startScale + value * (m_endScale - m_startScale); // YAY FOR A LERP ON A LERP DERP... FIX LATER (Although we can handle negatives this way)
                transform.localScale = new Vector3(scaleValue, scaleValue, 1.0f);
            }
            else
            {
                if (m_onTimeout != null)
                {
                    m_onTimeout(this.gameObject);
                }
                m_start = -1.0f;
            }
        }
	}
}
