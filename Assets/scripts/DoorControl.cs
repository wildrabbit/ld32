using UnityEngine;
using System.Collections;

public class DoorControl : MonoBehaviour 
{
    public GameObject m_levelRef;
    public bool m_enabled = true;

    private Collider2D m_triggerRef;

	// Use this for initialization
	void Start () 
    {
        m_triggerRef = GetComponent<Collider2D>();
        m_triggerRef.isTrigger = m_enabled;        
	}

    void SetDoorEnabled (bool value)
    {
        m_enabled = value;
        m_triggerRef.isTrigger = m_enabled;
    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}


    void OnTriggerEnter2D (Collider2D other)
    {
        PlayerCharacterControl player = other.GetComponent<PlayerCharacterControl>();
        if (player != null && m_levelRef != null)
        {
            Debug.LogFormat("Player in!! Now loading {0}", m_levelRef.name);
        }
    }    
}
