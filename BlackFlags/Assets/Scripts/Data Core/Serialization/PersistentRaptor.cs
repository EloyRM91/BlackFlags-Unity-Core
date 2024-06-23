using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentRaptor : Persistent
{
    private static PersistentRaptor sit;
    protected override void Awake()
    {
        base.Awake();

        if (sit != null)
        {
            if (sit != this)
            {
                Destroy(sit.gameObject);
                sit = this;
            }
                
        }
        else sit = this;
    }
}
