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

    public void LoadLevel (ref Player player, ref List<Enemy> enemies)
    {
        Transform playerStart = transform.FindChild("playerStart");
        if (playerStart != null)
        {
            m_playerStart = playerStart.position;
        }
        GameObject playerGO = Instantiate<GameObject>(m_playerPrefab);
        player = playerGO.GetComponent<Player>();
        player.OnLoadLevel(m_playerStart, GameObject.Find("HP").GetComponent<ProgressBar>(), GameObject.Find("Eloquency").GetComponent<ProgressBar>());

        m_enemySpawners = GetComponentsInChildren<EnemySpawner>();
        int numSpawners = m_enemySpawners.Length;
        Enemy e = null;
        for (int i = 0; i < numSpawners; ++i)
        {
            e = m_enemySpawners[i].OnLoadLevel();
            if (e != null)
            {
                enemies.Add(e);
            }
        }
    }

    void Start () 
    {        
	}

	// Update is called once per frame
	void Update () 
    {
	
	}
}
