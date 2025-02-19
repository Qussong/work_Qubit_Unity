using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEngine;

namespace Qubit
{
    public enum ViewType
    {
        TitleView,
        PagingView,
        GameGuideView,
        GameView,
        GameEndView,
        MaxCnt,
    }

    public class ViewManager : MonoBehaviour
    {
        private ViewManager instance = null;

        ViewType currentViewType;
        float LoadTitleTimer;
        int currentEndCount;
        [SerializeField] List<GameObject> views = new List<GameObject>();
        [SerializeField] float LoadTitleTime;
        [SerializeField] int endCount;

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

            if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
            {
                LoadTitleTimer = 0;
                return;
            }

            LoadTitleTimer += Time.deltaTime;

            if (LoadTitleTimer > LoadTitleTime)
            {
                LoadTargetView((int)ViewType.TitleView);
            }
        }

        private void LoadTargetView(int ViewIndex) 
        {
            for (int i = 0; i < views.Count; i++)
            {
                if (ViewIndex == i)
                {
                    views[i].gameObject.SetActive(true);
                    currentViewType = (ViewType)i;
                }
                else
                    views[i].gameObject.SetActive(false);
            }
        }

        public void ChangeView(ViewType NewType) 
        {
            if (NewType >= ViewType.MaxCnt)
                return;

            currentViewType = NewType;
            LoadTargetView((int)currentViewType);
        }

        public void LoadNextView() 
        {
            if (currentViewType >= ViewType.MaxCnt)
            {
                currentViewType = ViewType.GameEndView;
                return;
            }

            currentViewType++;
            LoadTargetView((int)currentViewType);
        }

        public void IncreaseEndCount() 
        {
            currentEndCount++;

            if (currentEndCount > endCount) 
            {
                currentEndCount = 0;

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }
}
