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
        private static ViewManager instance = null;

        ViewType currentViewType;
        float LoadTitleTimer;
        int currentEndCount;
        [SerializeField] List<GameObject> views = new List<GameObject>();
        [SerializeField][Tooltip("(입력 대기 시간) 해당 시간을 넘어서면 Title 화면으로 복귀한다.")] float LoadTitleTimeNoInput;
        [SerializeField][Tooltip("컨텐츠 종료시 해당 시간을 넘어서면 Title 화면으로 복귀한다.")] float LoadTitleTimeOnExit;
        [SerializeField][Tooltip("강제 종료를 위한 버튼 클릭 횟수")] int endCount;

        public static ViewManager Instance
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
            // Return to Title View Condition
            if (ViewType.GameEndView == currentViewType)
            {
                // Content End
                ReturnToTitleOnExit();
            }
            else
            {
                // Input waiting time exceeded
                ReturnToTitleOnNoInput();
            }

            // Game Setting
            if (views[(int)ViewType.GameView].activeSelf 
                && null != GameManager.Instance
                && false == GameManager.Instance.bSet)
            {
                GameManager.Instance.SetGame();
            }

            // Game Progress
            if (null != GameManager.Instance
                && true == GameManager.Instance.bSet)
            {
                GameManager.Instance.Progress();
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
                {
                    views[i].gameObject.SetActive(false);
                }
            }
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
        
        // Call from ExitButton
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

        private void ReturnToTitleOnNoInput()
        {
            // If there is an input value, reset the LoadTitleTimer.
            if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
            {
                LoadTitleTimer = 0.0f;
                return;
            }

            LoadTitleTimer += Time.deltaTime;

            // Return to the title screen if there is no input for a certain period of time.
            if (LoadTitleTimer > LoadTitleTimeNoInput)
            {
                LoadTitleTimer = 0.0f;
                LoadTargetView((int)ViewType.TitleView);
                GameManager.Instance.EndGame();
            }
        }

        private void ReturnToTitleOnExit()
        {
            LoadTitleTimer += Time.deltaTime;

            if (ViewType.GameEndView == currentViewType)
            {
                if (LoadTitleTimer > LoadTitleTimeOnExit)
                {
                    LoadTitleTimer = 0.0f;
                    LoadTargetView((int)ViewType.TitleView);
                }
            }
        }

    }
}
