using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Qubit
{
    public class GameManager : MonoBehaviour
    {
        [Header("Essential Settings")]
        [SerializeField] GameObject contentObj = null;

        private static GameManager instance = null;
        private GameObject prefab = null;
        private List<GameObject> qubits = new List<GameObject>();
        [System.NonSerialized] public bool bSet = false;   // 게임 설정 완료 여부 확인
        private float time = 0.0f;
        private const float GENTERM = 2.0f; // Unstable Qubit 생성시간 간격

        [Header("Score")]
        [SerializeField] private int stablePoint = 0;
        [SerializeField][Tooltip("(게임 종료 조건)게임 종료를 위해 해결해야할 Qubit 개수")] public int endCodition = 0;
        [SerializeField] int rows = 0;
        [SerializeField] int cols = 0;

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

            // Qubit 객체 생성 및 배치
            GenQubit(rows, cols);

            bSet = true;
        }

        void GenQubit(int rowCnt, int colCnt)
        {
            Rect contentSpace = contentObj.GetComponent<RectTransform>().rect;
            float diameter = prefab.GetComponent<RectTransform>().rect.width;

            float spaceX = (contentSpace.width - diameter * colCnt) / (colCnt + 1);
            float spaceY = diameter;

            float paddingX = (contentSpace.width - spaceX * (colCnt - 1) - diameter * colCnt) / 2;
            float paddingY = (contentSpace.height - spaceY * (rowCnt - 1) - diameter * rowCnt) / 2;

            float contenOffsetX = -contentSpace.width / 2;
            float contenOffsetY = contentSpace.height / 2;

            float zigzag = diameter;

            for (int col = 0; col < colCnt; ++col)
            {
                for (int row = 0; row < rowCnt; ++row)
                {
                    float posX = paddingX + col * (diameter + spaceX) + diameter / 2;
                    float posY = -paddingY - row * (diameter + spaceY) - diameter / 2;

                    if (col % 2 == 1)
                    {
                        posY += zigzag;
                    }

                    GameObject newQubit = Instantiate(prefab, Vector3.zero, Quaternion.identity, contentObj.GetComponent<RectTransform>());
                    newQubit.GetComponent<RectTransform>().localPosition = new Vector3(posX, posY, 0.0f);
                    newQubit.GetComponent<RectTransform>().localPosition += new Vector3(contenOffsetX, contenOffsetY, 0.0f);

                    // 관리 목적
                    qubits.Add(newQubit);
                }
            }
        }

        public void Progress()
        {
            if (endCodition <= stablePoint)
            {
                EndGame();
                ViewManager.Instance.LoadNextView();
            }

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
            StopAllCoroutines();

            bSet = false;
            stablePoint = 0;

            for (int i = 0; i < qubits.Count; ++i)
            {
                Destroy(qubits[i]);
            }
            qubits.Clear();
        }

        public int UpSore()
        {
            return ++stablePoint;
        }



    }   // class end
}