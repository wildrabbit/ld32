using UnityEngine;
using System.Collections;

public class DelayedDeath : MonoBehaviour 
{
    private float m_start = -1.0f;
    private float m_total = -1.0f;
    
    public void Play(float duration)
    {
        m_start = Time.time;
        m_total = duration;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Time.time - m_start >= m_total)
        {
            transform.parent = null;
            GameObject.Destroy(gameObject);
        }
	}
}
