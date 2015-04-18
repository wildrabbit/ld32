using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
    public const float kUnitsPerPixel = 1 / 64.0f;

    private static GameManager m_instance = null;

    private Level m_level = null;
    private Player m_player = null;
    private List<Enemy> m_enemies = null;

    public GameObject m_startingLevel = null;

    public static GameManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject go = GameObject.Find("GameManager");
                if (go == null)
                {
                    go = new GameObject();
                    go.name = "GameManager";
                }

                m_instance  = go.GetComponent<GameManager>();
                if (m_instance == null)
                {
                    m_instance = go.AddComponent<GameManager>();
                }
                
            }
            return m_instance;
        }
    }

    public void Start ()
    {
        LoadLevel(m_startingLevel.GetComponent<Level>());
    }

    public void LoadLevel (Level levelRef)
    {
        m_enemies = new List<Enemy>();

        m_level = Instantiate<Level>(levelRef);
        m_level.LoadLevel(ref m_player, ref m_enemies);
        if (m_player != null)
        {
            m_player.m_OnDead += OnPlayerDied;
        }

        int numEnemies = m_enemies.Count;
        for (int i = 0; i < numEnemies; ++i)
        {
            m_enemies[i].m_OnDead += OnEnemyDied;
        }
    }

    public void UnloadLevel ()
    {
        if (m_level != null)
        {
            m_level.UnloadLevel();
        }
        m_level = null;
        
        m_player.UnloadLevel();
        m_player = null;

        int numEnemies = m_enemies.Count;
        for (int i = 0; i < numEnemies; ++i)
        {
            m_enemies[i].UnloadLevel();
        }
        m_enemies.Clear();
        m_enemies = null;        
    }

    public void OnPlayerDied ()
    {
        m_player = null;
    }

    public void OnEnemyDied (Enemy e)
    {
        m_enemies.Remove(e);
    }

    public void ChangeLevel(Level levelRef)
    {
        Debug.LogFormat("CHANGE LEVEL:: Now loading {0}", levelRef.name);
        UnloadLevel();
        LoadLevel(levelRef);
    }


    public List<Enemy> GetEnemies ()
    {
        return m_enemies;
    }

    public Player GetPlayer ()
    {
        return m_player;
    }

    public Level GetLevel ()
    {
        return m_level;
    }

    public Enemy GetClosestEnemy (Vector3 position)
    {
        float minDistance = float.PositiveInfinity;
        Enemy candidate = null;
        
        int numEnemies = m_enemies.Count;
        for (int i = 0; i < numEnemies; ++i)
        {
            Enemy e = m_enemies[i];
            float d = Vector3.Distance(e.transform.position, position);
            if (d < minDistance)
            {
                minDistance = d;
                candidate = e;
            }
        }
        return candidate;
    }
}
