using UnityEngine;
using System.Collections;

public class DelayedDeath : MonoBehaviour 
{
    private float m_start = -1.0f;
    private float m_total = -1.0f;

    public delegate void OnTimeoutDelegate(GameObject go);
    private OnTimeoutDelegate m_onTimeout;
    public OnTimeoutDelegate OnTimeout
    {
        get
        {
            return m_onTimeout;
        }
    }
    
    public void Play(float duration, OnTimeoutDelegate timeout = null)
    {
        m_start = Time.time;
        m_total = duration;

        if (timeout != null)
        {
            m_onTimeout -= timeout;
            m_onTimeout += timeout;
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Time.time - m_start >= m_total)
        {
            transform.parent = null;
            if (m_onTimeout != null)
            {
                m_onTimeout(this.gameObject);
            }
            
            GameObject.Destroy(gameObject);
        }
	}
}
