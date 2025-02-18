using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Qubit
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance = null;

        private Canvas canvas = null;
        private int targetCnt = 0;
        private List<Target> qubits = new List<Target>();

        public static GameManager Instance
        {
            get 
            { 
                if (null == instance)
                {
                    // Scene 에서 GameManager 찾음
                    instance = FindAnyObjectByType<GameManager>();

                    // Scene 에서 GameManager가 없다면 새로운 오브젝트 생성
                    if(null == instance)
                    {
                        GameObject obj = new GameObject("GameManger");
                        instance = obj.AddComponent<GameManager>();

                        // Scene 전환 시 삭제되지 않도록 설정
                        DontDestroyOnLoad(obj);
                    }
                }

                return instance;
            }
        }

        // MonoBehavior 이기에 생성자는 사용되지 않는다.
        private GameManager() { }

        private void Awake()
        {
            // 중복 방지
            if(instance == null)
            {
                instance = this;
                // Scene 전환 시 삭제되지 않도록 설정
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // Scene 이동시 기존에 생성된 Manager 가 존재하는 경우
                // 해당 Scene 의 Manager 객체는 제거한다.
                Destroy(gameObject);
                return;
            }

            // Scene 에서 Canvas 탐색
            canvas = FindAnyObjectByType<Canvas>();
            if(null == canvas)
            {
                Debug.LogError("Scene 에 Canvas 가 존재하지 않습니다. Target 생성이 불가능합니다.");
                return;
            }

        }

        void SetGame()
        {
            if (null == canvas)
            {
                return;
            }

            // 객체 생성 및 Scene 에 배치


            // List 에 객체 추가

            
        }

    }   // class end
}