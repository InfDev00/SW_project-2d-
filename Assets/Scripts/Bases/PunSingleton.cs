using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

namespace Bases
{
    public class PSingleton<T> : MonoBehaviourPunCallbacks where T : PSingleton<T>
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

        protected void CreateSingleton(T instance)
        {
            if (_instance==null)
            {
                _instance = instance;
                DontDestroyOnLoad(instance.gameObject);
            }
            else Destroy(gameObject);
        }
    }
}