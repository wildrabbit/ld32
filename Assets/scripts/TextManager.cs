using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;


public class TextManager : MonoBehaviour 
{
    private static TextManager m_instance = null;
    public static TextManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject go = GameObject.Find("TextManager");
                if (go == null)
                {
                    go = new GameObject();
                    go.name = "TextManager";
                }

                m_instance = go.GetComponent<TextManager>();
                if (m_instance == null)
                {
                    m_instance = go.AddComponent<TextManager>();
                }
            }
            return m_instance;
        }
    }

    public bool m_excludeProfanity = false;
    public List<TextAsset> m_languageFiles = new List<TextAsset>();
    public List<string> m_languageCodes = new List<string>();
    private Dictionary<string, TextLibrary> m_fullLibrary = new Dictionary<string, TextLibrary>();
    private string m_currentLanguage = "";

	// Use this for initializations
	void Start () 
    {
        int numFiles = m_languageFiles.Count;
	    for (int i = 0; i < numFiles; ++i)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(m_languageFiles[i].text);

            XmlNode mapNode = xmlDoc.GetElementsByTagName("text_library")[0];
            if (mapNode != null)
            {
                string code = mapNode.Attributes["code"].Value;
                m_languageCodes.Add(code);
                m_fullLibrary[code] = new TextLibrary();
                m_fullLibrary[code].LoadFromXML(mapNode, code);
            }
        }

        m_currentLanguage = m_languageCodes.Count > 0 ? m_languageCodes[0] : "";
	}

    public List<string> GetInsultList()
    {
        return m_fullLibrary[m_currentLanguage].GetInsults(m_excludeProfanity);
    }
    public string GetRandomInsult()
    {
        return m_fullLibrary[m_currentLanguage].GetRandomInsult(m_excludeProfanity);
    }
    public List<string> GetEpitaphList()
    {
        return m_fullLibrary[m_currentLanguage].GetEpitaphs(m_excludeProfanity);
    }
    public string GetRandomEpitaph()
    {
        return m_fullLibrary[m_currentLanguage].GetRandomEpitaph(m_excludeProfanity);
    }
    public List<string> GetComplaintList()
    {
        return m_fullLibrary[m_currentLanguage].GetComplaints(m_excludeProfanity);
    }
    public string GetRandomComplaint()
    {
        return m_fullLibrary[m_currentLanguage].GetRandomComplaint(m_excludeProfanity);
    }
    public List<string> GetWallOfText(string key)
    {
        return m_fullLibrary[m_currentLanguage].GetWallOfText(key, m_excludeProfanity);
    }
    public List<string> GetRandomWallOfText()
    {
        return m_fullLibrary[m_currentLanguage].GetRandomWallOfText(m_excludeProfanity);
    }

    public void ChangeLanguage (string newCode)
    {
        if (m_languageCodes.Contains(newCode))
        {
            m_currentLanguage = newCode;
            //NOTIFY
        }
    }

    public string GetText (string key)
    {
        TextLibrary lib = m_fullLibrary[m_currentLanguage];
        if (lib == null)
        {
            return "";
        }

        return lib.GetText(key);
    }

	
	// Update is called once per frame
	void Update () {
	
	}
}
