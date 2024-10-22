using UnityEngine;
using GameMechanics.Utilities;
using GameMechanics.Data;

public abstract class UI_ScenicView<T> : SingletonBehaviour<T> where T : MonoBehaviour
{
    protected virtual void Start()
    {
        PersistentGameData.updateGold += SetGoldText;
    }

    protected virtual void Destroy()
    {
        PersistentGameData.updateGold -= SetGoldText;
    }

    protected abstract void SetGoldText(int val);
}

public abstract class UI_ScenicDialogView<T> : UI_ScenicView<T> where T : MonoBehaviour
{

}

namespace GameMechanics.Utilities
{
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T instance;
        private void Awake()
        {
            instance = GetComponent<T>();
        }

        public static T GetInstance()
        {
            return instance;
        }
    }

}
