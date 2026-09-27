using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Strings/Warning Catalog")]
public class WarningsCatalog : ScriptableObject
{
    [SerializeField]
    private List<Entry> entries;

    [System.Serializable]
    public class Entry
    {
        public string key;
        public string warn, details;
    }

    public string GetWarn(string key)
    {
        return entries.Find(x => x.key == key)?.warn;
    }

    public string GetText(string key)
    {
        return entries.Find(x => x.key == key)?.details;
    }
}
