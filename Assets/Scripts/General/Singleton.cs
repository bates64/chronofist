using UnityEngine;

namespace General
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        #region Properties

        private static T _instance; 
        
        protected static T Instance
        {
            get
            {
                if (_instance is null)
                {
                    FindObjectOfType<T>().initialize();
                }
                return _instance;
            }
        }

        #endregion

        #region Unity Events and Setup Functions

        private void Awake()
        {
            initialize();
        }

        private void initialize()
        {
            if (_instance is null)
            {
                _instance = this as T;
                init();
            }
            else Destroy(this);
        }

        protected abstract void init();

        #endregion
    }

}