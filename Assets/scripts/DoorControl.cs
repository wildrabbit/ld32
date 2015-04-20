using UnityEngine;
using System.Collections;

public class DoorControl : MonoBehaviour 
{
    public GameObject m_levelRef;
    public bool m_enabled = true;

    private Collider2D m_triggerRef;
    private bool m_inside = false;

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
        if (!m_inside)
        {
            Player player = other.GetComponent<Player>();
            if (player != null && m_levelRef != null)
            {
                if (GameManager.Instance.IsLastLevel())
                {
                    GameManager.Instance.OnLastLevel();
                }
                // TODO: Defer this to a proper level controller class
                GameManager.Instance.ChangeLevel(m_levelRef.GetComponent<Level>());
            }
            m_inside = true;
        }
    }    

    void OnTriggerExit2D (Collider2D other)
    {
        if (m_inside &&  other.GetComponent<Player>() != null)
        {
            m_inside = false;
        }
    }
}
