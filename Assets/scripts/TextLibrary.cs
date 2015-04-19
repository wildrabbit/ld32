using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

public class TextEntry
{
    public string value = "";
    public bool profanityWarning = false;

    public TextEntry(string value, bool profanity = false)
    {
        this.value = value;
        this.profanityWarning = profanity;
    }
}
public class TextLibrary
{
    private string m_languageCode;
    private List<TextEntry> m_insults = new List<TextEntry>();
    private List<TextEntry> m_complaints = new List<TextEntry>();
    private List<TextEntry> m_epitaphs = new List<TextEntry>();
    private Dictionary<string, List<TextEntry>> m_wallsOfText = new Dictionary<string, List<TextEntry>>();
    private Dictionary<string, string> m_mainTexts = new Dictionary<string, string>();

    public void LoadFromXML(XmlNode node, string code)
    {
        m_languageCode = code;
        foreach (XmlNode child in node)
        {
            switch (child.Name)
            {
                case "insults":
                    {
                        LoadInsults(child);
                        break;
                    }
                case "complaints":
                    {
                        LoadComplaints(child);
                        break;
                    }
                case "epitaphs":
                    {
                        LoadEpitaphs(child);
                        break;
                    }
                case "walls_of_text":
                    {
                        LoadWallsOfText(child);
                        break;
                    }
                case "main":
                    {
                        LoadMain(child);
                        break;
                    }
                default: break;
            }
        }
    }

    void LoadInsults(XmlNode node)
    {
        foreach (XmlNode child in node.ChildNodes)
        {
            if (child.Name != "insult")
            {
                continue;
            }

            m_insults.Add(new TextEntry(child.Attributes["value"].Value, child.Attributes["profanity"] != null ? child.Attributes["profanity"].Value == "1" : false));
        }
    }

    void LoadComplaints(XmlNode node)
    {
        foreach (XmlNode child in node.ChildNodes)
        {
            if (child.Name != "complaint")
            {
                continue;
            }

            m_complaints.Add(new TextEntry(child.Attributes["value"].Value, child.Attributes["profanity"] != null ? child.Attributes["profanity"].Value == "1" : false));
        }
    }
    void LoadEpitaphs(XmlNode node)
    {
        foreach (XmlNode child in node.ChildNodes)
        {
            if (child.Name != "epitaph")
            {
                continue;
            }

            m_epitaphs.Add(new TextEntry(child.Attributes["value"].Value, child.Attributes["profanity"] != null ? child.Attributes["profanity"].Value == "1" : false));
        }
    }
    void LoadWallsOfText(XmlNode node)
    {
        foreach (XmlNode child in node.ChildNodes)
        {
            if (child.Name != "wot")
            {
                continue;
            }

            string key = child.Attributes["key"].Value;
            m_wallsOfText[key] = new List<TextEntry>();
            foreach (XmlNode wot in child.ChildNodes)
            {
                if (wot.Name != "sentence")
                {
                    continue;
                }
                m_wallsOfText[key].Add(new TextEntry(wot.Attributes["value"].Value, child.Attributes["profanity"] != null ? child.Attributes["profanity"].Value == "1" : false));
            }
        }
    }
    void LoadMain(XmlNode node)
    {
        foreach (XmlNode child in node.ChildNodes)
        {
            if (child.Name != "main_entry")
            {
                continue;
            }
            m_mainTexts[child.Attributes["key"].Value] = child.Attributes["value"].Value;
        }
    }

    public List<string> GetInsults(bool excludeProfanity = false)
    {
        List<string> result = new List<string>();
        int numInsults = m_insults.Count;
        for (int i =0; i < numInsults; ++i)
        {
            if (!excludeProfanity  || !m_insults[i].profanityWarning)
            {
                result.Add(m_insults[i].value);
            }
        }
        return result;
    }
    public string GetRandomInsult(bool excludeProfanity = false)
    {
        List<string> filtered = GetInsults(excludeProfanity);

        int numFiltered = filtered.Count;
        if (filtered.Count > 0)
        {
            return ReplaceLineBreaks(filtered[Random.Range(0, filtered.Count)]);
        }
        return "";
    }
    public List<string> GetComplaints(bool excludeProfanity = false)
    {
        List<string> result = new List<string>();
        int numComplaints = m_complaints.Count;
        for (int i = 0; i < numComplaints; ++i)
        {
            if (!excludeProfanity || !m_complaints[i].profanityWarning)
            {
                result.Add(m_complaints[i].value);
            }
        }
        return result;
    }
    public string GetRandomComplaint(bool excludeProfanity = false)
    {
        List<string> filtered = GetComplaints(excludeProfanity);

        int numFiltered = filtered.Count;
        if (filtered.Count > 0)
        {
            return ReplaceLineBreaks(filtered[Random.Range(0, filtered.Count)]);
        }
        return "";
    }

    public List<string> GetEpitaphs(bool excludeProfanity = false)
    {
        List<string> result = new List<string>();
        int numEpitaphs = m_epitaphs.Count;
        for (int i = 0; i < numEpitaphs; ++i)
        {
            if (!excludeProfanity || !m_epitaphs[i].profanityWarning)
            {
                result.Add(m_epitaphs[i].value);
            }
        }
        return result;
    }
    public string GetRandomEpitaph(bool excludeProfanity = false)
    {
        List<string> filtered = GetEpitaphs(excludeProfanity);

        int numFiltered = filtered.Count;
        if (filtered.Count > 0)
        {
            return ReplaceLineBreaks(filtered[Random.Range(0, filtered.Count)]);
        }
        return "";
    }

    public List<string> GetWallOfText(string key, bool excludeProfanity = false)
    {
        List<TextEntry> wot = m_wallsOfText[key];
        List<string> filtered = new List<string>();
        for (int i =0; i < wot.Count; ++i)
        {
            if (!excludeProfanity || !wot[i].profanityWarning)
            {
                filtered.Add(wot[i].value);
            }
        }
        return filtered;
    }
    public List<string> GetRandomWallOfText(bool excludeProfanity = false)
    {
        string[] keys = m_wallsOfText.Keys.ToArray<string>();
        if (keys == null || keys.Length == 0)
        {
            return new List<string>();
        }
        return GetWallOfText(keys[Random.Range(0,keys.Length)],excludeProfanity);
    }
    public string GetText (string key, params string[] replacements)
    {
        if (m_mainTexts.ContainsKey(key))
        {
            return ReplaceLineBreaks(string.Format(m_mainTexts[key], replacements));
        }
        else return key;
    }

    public string ReplaceLineBreaks (string inputStr)
    {
        return inputStr.Replace("\\n", "\n");
    }
}