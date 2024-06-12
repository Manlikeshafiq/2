using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace MSA
{
    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager instance;

        [SerializeField] int worldSceneIndex = 1;

        private void Awake()
        {
            //Can only be one
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Start()
        {

        }

        public IEnumerator LoadNewGame()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

            yield return null;
        }

        public int GetWorldSceneIndex()
        {
            return worldSceneIndex;
        }
    }

}
