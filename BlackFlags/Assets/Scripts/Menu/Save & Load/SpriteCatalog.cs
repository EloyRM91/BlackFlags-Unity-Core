using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sprites/Sprite Catalog")]
public class SpriteCatalog : ScriptableObject
{
    [SerializeField]
    private List<Entry> entries;

    [System.Serializable]
    public class Entry
    {
        public string key;
        public Sprite sprite;
    }

    public Sprite Get(string key)
    {
        return entries.Find(x => x.key == key)?.sprite;
    }
}
