using UnityEngine;
using System.Collections;

public class ScaleWithAlpha : MonoBehaviour 
{
    private float m_start = -1.0f;
    private float m_total = -1.0f;
    private SpriteRenderer m_renderer = null;

    public void Play(float duration)
    {
        transform.localScale = Vector3.zero;
        Color col = m_renderer.color;
        col.a = 0.0f;
        m_renderer.color = col;

        m_total = duration;
        m_start = Time.time;
    }

	// Use this for initialization
	void Awake () 
    {
        m_renderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        float delta = Time.time - m_start;
        float elapsedRatio = delta/m_total;
        float value = Mathf.Lerp(0.0f, 1.0f, elapsedRatio) * 0.8f;

        Color col = m_renderer.color;
        col.a = value;
        m_renderer.color = col;

        transform.localScale = new Vector3(value, value, 1.0f);

        if (delta >= m_total)
        {
            transform.parent = null;
            GameObject.Destroy(gameObject);
        }
	}
}
