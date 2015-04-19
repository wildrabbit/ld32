using UnityEngine;
using System.Collections.Generic;


[RequireComponent(typeof(TextMesh))]
public class WOTParticle : MonoBehaviour
{
    TextMesh m_textComponent;

    private float m_start = -1.0f;
    private float m_duration = -1.0f;

    private Vector3 m_defaultScale = Vector3.zero;
    private float m_startScale = 1.0f;
    private float m_endScale = 1.0f;

    private float m_startAlpha = 0.0f;
    private float m_endAlpha = 1.0f;

    private Vector3 m_direction = Vector3.zero;
    private float m_speed = 0.0f;

    public delegate void OnTimeoutDelegate(GameObject go);
    private OnTimeoutDelegate m_onTimeout;
    public OnTimeoutDelegate OnTimeout
    {
        get
        {
            return m_onTimeout;
        }
    }

    public void Play(string text, float duration, float startScale, float endScale, Color color, float startAlpha, float endAlpha, Vector3 localPos, Vector3 direction, float speed, OnTimeoutDelegate lifetimeDelegate = null)
    {
        m_start = Time.time;
        m_duration = duration;
        transform.localPosition = localPos;

        m_direction = direction;
        m_direction.Normalize();
        m_speed = speed;

        m_defaultScale = transform.localScale;
        m_startScale = startScale;
        m_endScale = endScale;
        Vector3 newScale = m_defaultScale;
        m_defaultScale.x *= m_startScale;
        m_defaultScale.y *= m_startScale;
        transform.localScale = newScale;

        m_startAlpha = startAlpha;
        m_endAlpha = endAlpha;
        m_textComponent.color = color;
        Color textColor = m_textComponent.color;
        textColor.a = m_startScale;
        m_textComponent.color = textColor;

        m_textComponent.text = text;

        if (lifetimeDelegate != null)
        {
            m_onTimeout -= lifetimeDelegate;
            m_onTimeout += lifetimeDelegate;
        }
    }

    void Awake()
    {
        m_textComponent = GetComponent<TextMesh>();
    }
    void Update()
    {
        if (m_start >= 0)
        {
            float delta = Time.time - m_start;
            if (delta >= m_duration)
            {
                m_start = -1.0f;
                if (m_onTimeout != null)
                {
                    m_onTimeout(this.gameObject);
                }
            }
            else
            {
                float elapsedRatio = delta / m_duration;
                float lerpValue = Mathf.Lerp(0.0f, 1.0f, elapsedRatio);

                Vector3 pos = transform.localPosition;
                pos += m_direction * m_speed * Time.deltaTime;
                transform.localPosition = pos;

                Color col = m_textComponent.color;
                col.a = m_startAlpha + lerpValue * (m_endAlpha - m_startAlpha);
                m_textComponent.color = col;

                Vector3 newScale = m_defaultScale;
                newScale.x = m_startScale + lerpValue * (m_endScale - m_startScale);
                newScale.y = m_startScale + lerpValue * (m_endScale - m_startScale);
                transform.localScale = newScale;
            }
        }
    }
}