using System.Collections;
using UnityEngine;

namespace UI.WorldMap
{
    public abstract class FixedTimeAnimation<T> : MonoBehaviour
    {
        [SerializeField] protected T[] resources;
        [SerializeField] private float frameSpeed;

        protected virtual void Start() { }
        public virtual void Animate()
        {
            StopAllCoroutines();
            StartCoroutine(RunAnimation());
        }
        protected virtual IEnumerator RunAnimation()
        {
            for (int i = 0; i < resources.Length; i++)
            {
                Action(i);
                yield return new WaitForSecondsRealtime(frameSpeed);
            }
        }

        protected abstract void Action(int i);
    }
}

