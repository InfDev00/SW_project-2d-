using Photon.Pun;
using UnityEngine;

namespace Bases
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (!_instance) Debug.Log($"{typeof(T)} is Empty");
                return _instance;
            }
        }

        protected void CreateSingleton(T instance, bool dontDestroyOnLoad=false)
        {
            _instance = instance;
            if(dontDestroyOnLoad) DontDestroyOnLoad(this);
        }
    }   
}
