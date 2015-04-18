using UnityEngine;
using System.Collections.Generic;

public class Level : MonoBehaviour 
{
    public GameObject m_playerPrefab = null;
    public Vector3 m_playerStart = Vector3.zero;

    private EnemySpawner[] m_enemySpawners = new EnemySpawner[0];
	// Use this for initialization
	void Start () 
    {
        GameObject player = Instantiate<GameObject>(m_playerPrefab);
        m_playerPrefab.transform.position = m_playerStart;
        
        m_enemySpawners = GetComponentsInChildren<EnemySpawner>();
        int numSpawners = m_enemySpawners.Length;
        for (int i = 0; i < numSpawners; ++i)
        {
            m_enemySpawners[i].OnLoadLevel();
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
