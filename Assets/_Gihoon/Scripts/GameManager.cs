using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Qubit
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance = null;

        [SerializeField] GameObject contentObj = null;
        private GameObject prefab = null;
        private List<GameObject> qubits = new List<GameObject>();

        public bool bSet = false;

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

            // Qubit Prefab 
            prefab = Resources.Load<GameObject>("Prefabs/Qubit");
            if(null == prefab)
            {
                Debug.LogError("Qubit Prefab 을 로드할 수 없습니다.");
                return;
            }

            // Check Qubit Parent Obj
            if (contentObj == null)
            {
                Debug.LogError("생성될 Qubit 객체의 Content 객체가 설정되지 않았습니다.");
                return;
            }
        }

        public void SetGame()
        {
            if (null == prefab || null == contentObj)
            {
                return;
            }

            // 객체 생성 및 Scene 에 배치
            int columns = 5;
            int rows = 4;
            float spacingX = 200f;
            float spacingY = 200f;

            for (int col = 0; col < columns; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    GameObject newObj = Instantiate(prefab, Vector3.zero, Quaternion.identity, contentObj.GetComponent<RectTransform>());
                    if (col % 2 == 1)
                    {
                        float zigzag = 100.0f;
                        newObj.GetComponent<RectTransform>().localPosition = new Vector3(col * spacingX, -row * spacingY - zigzag, 0);
                    }
                    else
                    {
                        newObj.GetComponent<RectTransform>().localPosition = new Vector3(col * spacingX, -row * spacingY, 0);
                    }
                    qubits.Add(newObj);
                }
            }

            /*GameObject qubit = Instantiate(prefab, contentObj.GetComponent<RectTransform>());
            // List 에 객체 추가
            qubits.Add(qubit);*/

            bSet = true;
        }

    }   // class end
}