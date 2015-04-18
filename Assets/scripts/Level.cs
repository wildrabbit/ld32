using UnityEngine;
using System.Collections.Generic;

public class Level : MonoBehaviour 
{
    public GameObject m_playerPrefab = null;
    private Vector3 m_playerStart = Vector3.zero;

    private EnemySpawner[] m_enemySpawners = new EnemySpawner[0];
	// Use this for initialization
    public void UnloadLevel()
    {
        GameObject.Destroy(gameObject);
        // TODO: Set state and so on
    }

    void Start () 
    {
        Transform playerStart = transform.FindChild("playerStart");
        if (playerStart != null)
        {
            m_playerStart = playerStart.position;
        }
        GameObject player = Instantiate<GameObject>(m_playerPrefab);
        PlayerCharacterControl playerRef = player.GetComponent<PlayerCharacterControl>();
        playerRef.Initialize(m_playerStart, GameObject.Find("HP").GetComponent<ProgressBar>());
        
        m_enemySpawners = GetComponentsInChildren<EnemySpawner>();
        int numSpawners = m_enemySpawners.Length;
        for (int i = 0; i < numSpawners; ++i)
        {
            m_enemySpawners[i].OnLoadLevel();
        }
	}

	// Update is called once per frame
	void Update () 
    {
	
	}
}
