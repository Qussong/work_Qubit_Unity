using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Qubit
{
    public enum ViewType
    {
        PagingView,
        GameView,
        MaxCnt,
    }

    public class ViewManager : MonoBehaviour
    {
        private ViewManager instance = null;

        [SerializeField] List<GameObject> views = new List<GameObject>();

        public ViewManager Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = FindAnyObjectByType<ViewManager>();

                    if(null == instance)
                    {
                        GameObject obj = new GameObject("ViewManger");
                        instance = obj.AddComponent<ViewManager>();

                        DontDestroyOnLoad(obj);
                    }
                }
                return instance;
            }
        }

        private void Awake()
        {
            if(null == instance)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }
        }

        private void Update()
        {
            if (views[(int)ViewType.GameView].activeSelf 
                && null != GameManager.Instance
                && false == GameManager.Instance.bSet)
            {
                GameManager.Instance.SetGame();
            }
        }
    }
}
