using UnityEngine;
using System.Collections.Generic;

public class SessionLevelState
{
    public List<string> m_deadEnemies = new List<string>();
}
public class GameSession
{
    public int m_playerHP = 0;
    public float m_playerEloquence = 0.0f;
    public int m_selectedSpecialIndex = 0;

    public Dictionary<string, SessionLevelState> m_levelStates = new Dictionary<string, SessionLevelState>();
}

public class GameManager : MonoBehaviour 
{
    public GameObject m_victory = null;
    public GameObject m_defeat = null;
    public const float kUnitsPerPixel = 1 / 64.0f;

    private static GameManager m_instance = null;

    private Level m_level = null;
    private Player m_player = null;
    private List<Enemy> m_enemies = null;
    private List<string> m_deadEnemies = null;

    private bool m_gameStarted = false;

    private bool m_awaitingReset = false;

    public GameObject m_startingLevel = null;
    private GameSession m_session = null;
    public GameSession Session
    {
        get
        {
            return m_session;
        }
    }

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

    void Update ()
    {
        if (!m_gameStarted && Input.anyKeyDown)
        {
            GameObject.Destroy(GameObject.Find("Intro"));

            m_session = null;
            LoadLevel(m_startingLevel.GetComponent<Level>());

            m_gameStarted = true;
        }

        if (m_awaitingReset && Input.anyKeyDown)
        {
            Application.LoadLevel(0);
        }
    }

    public void OnLastLevel ()
    {
        m_player = null;
        m_victory.SetActive(true);
        m_awaitingReset = true;
    }

    public void PersistSessionData()
    {
        if (m_session == null)
        {
            m_session = new GameSession();
        }
        if (m_player != null)
        {
            m_session.m_playerHP = m_player.CurrentHP;
            m_session.m_playerEloquence = m_player.CurrentEloquence;
            m_session.m_selectedSpecialIndex = m_player.m_selectedSpecialIdx;
        }
    }

    public void LoadLevel (Level levelRef)
    {
        m_enemies = new List<Enemy>();
        if (m_session != null && m_session.m_levelStates.ContainsKey(levelRef.name))
        {
            m_deadEnemies = m_session.m_levelStates[levelRef.name].m_deadEnemies;
        }

        m_level = Instantiate<Level>(levelRef);
        m_level.LoadLevel(ref m_player, ref m_enemies, m_session);

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
        m_awaitingReset = true;
        m_defeat.SetActive(true);
    }

    public void OnEnemyDied (Enemy e)
    {
        if (e == null)
        {
            Debug.Log("GM::OnEnemyDied - null e??");
            return;
        }

        if (m_deadEnemies != null && !m_deadEnemies.Contains(e.name))
        {
            m_deadEnemies.Add(e.name);
        }
        m_enemies.Remove(e);
    }

    public void ChangeLevel(Level levelRef)
    {
        PersistSessionData();
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

    public List<Enemy> GetEnemiesInRange (Vector3 position, float range)
    {
        List<Enemy> inRange = new List<Enemy>();
        int numEnemies = m_enemies.Count;
        Enemy e = null;
        for (int i = 0; i < numEnemies; ++i)
        {
            e = m_enemies[i];
            if (Vector3.Distance(e.transform.position, position) <= range)
            {
                inRange.Add(e);
            }
        }
        return inRange;
    }
}
