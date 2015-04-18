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
            PlayerCharacterControl player = other.GetComponent<PlayerCharacterControl>();
            if (player != null && m_levelRef != null)
            {
                // TODO: Defer this to a proper level controller class
                Debug.LogFormat("Player in!! Now loading {0}", m_levelRef.name);
                Level l = GameObject.FindObjectOfType<Level>();
                if (l != null)
                {
                    l.UnloadLevel();
                }

                player.UnloadLevel();

                Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
                for (int i = 0; i < enemies.Length; ++i)
                {
                    enemies[i].UnloadLevel();
                }

                GameObject newLevel = Instantiate<GameObject>(m_levelRef);
            }
            m_inside = true;
        }
    }    

    void OnTriggerExit2D (Collider2D other)
    {
        if (m_inside &&  other.GetComponent<PlayerCharacterControl>() != null)
        {
            m_inside = false;
        }
    }
}
