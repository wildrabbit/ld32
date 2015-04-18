using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour 
{
    public Enemy[] m_enemyPrefabs;
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

    public Enemy OnLoadLevel (List<string> deadEnemies)
    {
        if (deadEnemies != null && deadEnemies.Contains(this.name))
        {
            m_enabled = false;
            gameObject.SetActive(false);
            return null;
        }

        Enemy e = null;
        if (m_enabled)
        {
            Transform[] patrolTransforms = GetComponentsInChildren<Transform>();
            int numPoints = patrolTransforms.Length;
            int finalLength = numPoints - 1;
            if (finalLength <= 0)
            {
                finalLength = 0;
            }
            
            Vector3[] patrolPoints = new Vector3[finalLength];
            int patrolIdx = 0;
            for (int i = 0; i < numPoints; ++i)
            {
                if (patrolTransforms[i] != transform)
                {
                    patrolPoints[patrolIdx++] = patrolTransforms[i].position;
                }
            }

            int idx = Random.Range(0, m_enemyPrefabs.Length);
            if (m_enemyPrefabs[idx] != null)
            {
                e = Instantiate<Enemy>(m_enemyPrefabs[idx]);
                e.name = this.name;
                e.LoadLevel(transform.position, patrolPoints);
            }
        }
        gameObject.SetActive(false);
        return e;
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
