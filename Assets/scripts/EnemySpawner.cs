using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour 
{
    public GameObject[] m_enemyPrefabs;

    private bool m_enabled = true;
    public bool Enabled
    {
        get
        {
            return m_enabled;
        }
        set
        {
            m_enabled = value;
        }
    }

    public void OnLoadLevel ()
    {
        if (m_enabled)
        {
            int idx = Random.Range(0, m_enemyPrefabs.Length);
            if (m_enemyPrefabs[idx] != null)
            {
                GameObject enemy = Instantiate<GameObject>(m_enemyPrefabs[idx]);
                enemy.transform.position = transform.position;
            }
        }
        gameObject.SetActive(false);
    }

	// Use this for initialization
	void Start () 
    {
	    
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
