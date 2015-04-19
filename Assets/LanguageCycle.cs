using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LanguageConfig
{
    public string m_code;
    public Sprite m_icon;
}
public class LanguageCycle : MonoBehaviour 
{
    public List<LanguageConfig> m_languageIcons = new List<LanguageConfig>();
    private SpriteRenderer m_renderer = null;

    void Awake ()
    {
        TextManager.Instance.OnLanguageChange += OnLanguageChanged;
        m_renderer = GetComponentInChildren<SpriteRenderer>();
        m_renderer.enabled = false;
    }
    void Start ()
    {
    }

	public void OnLanguageChanged (string code)
    {
        LanguageConfig cfg = m_languageIcons.Find(x => x.m_code == code);
        if (cfg != null)
        {
            m_renderer.enabled= true;
            m_renderer.sprite = cfg.m_icon;
        }
        else
        {
            m_renderer.enabled = false;                 
        }
    }
}
