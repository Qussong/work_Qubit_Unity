using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
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

        private float time = 0.0f;
        private const float GENTERM = 2.0f;

        [Header("Score")]
        [SerializeField] private int stablePoint = 0;
        [SerializeField][Tooltip("(게임 종료 조건)게임 종료를 위해 해결해야할 Qubit 개수")] public int endCodition = 10;

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

            /*
            // 객체 생성 및 Scene 에 배치
            Rect contentSpace = contentObj.GetComponent<RectTransform>().rect;
            int columns = 5;
            int rows = 4;
            float spacingX = contentSpace.width / columns;
            float spacingY = contentSpace.height / rows;
            float zigzag = 100.0f;

            for (int col = 0; col < columns; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    GameObject newObj = Instantiate(prefab, Vector3.zero, Quaternion.identity, contentObj.GetComponent<RectTransform>());
                    if (col % 2 == 1)
                    {
                        newObj.GetComponent<RectTransform>().localPosition = new Vector3(col * spacingX + spacingX/2, -row * spacingY - spacingY / 2, 0);
                    }
                    else
                    {
                        newObj.GetComponent<RectTransform>().localPosition = new Vector3(col * spacingX + spacingX / 2, -row * spacingY - spacingY / 2, 0);
                    }

                    newObj.GetComponent<RectTransform>().localPosition += new Vector3(-contentSpace.width / 2 + spacingX / 2, contentSpace.height / 2 - spacingY / 2, 0);

                    qubits.Add(newObj);
                }
            }
            */

            // Test Code
            for(int i = 0; i < 10; ++i)
            {
                GameObject newObj = Instantiate(prefab, Vector3.zero, Quaternion.identity, contentObj.GetComponent<RectTransform>());
                newObj.name = string.Format("Qubit_{0:D2}", i);
                newObj.GetComponent<RectTransform>().localPosition = Vector3.zero;
                qubits.Add(newObj);
            }

            bSet = true;
        }

        public void Progress()
        {
            EndGame();
            ChangeStateRndQubit();
        }

        private void ChangeStateRndQubit()
        {
            time += Time.deltaTime;

            if (time > GENTERM)
            {
                int rndIdx = Random.Range(0, qubits.Count);
                //Debug.Log(rndIdx);

                QubitProperty property = qubits[rndIdx].GetComponent<QubitProperty>();
                if (null != property && EQubitState.Stable == property.QubitState)
                {
                    property.BeUnstable();
                }

                time = 0.0f;
            }
        }

        public void EndGame()
        {
            if(endCodition <= stablePoint)
            {
                bSet = false;
                ViewManager.Instance.LoadNextView();
            }
        }

        public int UpSore()
        {
            return ++stablePoint;
        }

    }   // class end
}