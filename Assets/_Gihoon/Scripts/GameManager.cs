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
        [System.NonSerialized] public bool bSet = false;   // ���� ���� �Ϸ� ���� Ȯ��
        private float time = 0.0f;
        private const float GENTERM = 2.0f; // Unstable Qubit �����ð� ����

        [Header("Score")]
        [SerializeField] private int stablePoint = 0;
        [SerializeField][Tooltip("(���� ���� ����)���� ���Ḧ ���� �ذ��ؾ��� Qubit ����")] public int endCodition = 0;
        [SerializeField] int rows = 0;
        [SerializeField] int cols = 0;

        public static GameManager Instance
        {
            get 
            { 
                if (null == instance)
                {
                    // Scene ���� GameManager ã��
                    instance = FindAnyObjectByType<GameManager>();

                    // Scene ���� GameManager�� ���ٸ� ���ο� ������Ʈ ����
                    if(null == instance)
                    {
                        GameObject obj = new GameObject("GameManger");
                        instance = obj.AddComponent<GameManager>();

                        // Scene ��ȯ �� �������� �ʵ��� ����
                        DontDestroyOnLoad(obj);
                    }
                }

                return instance;
            }
        }

        // MonoBehavior �̱⿡ �����ڴ� ������ �ʴ´�.
        private GameManager() { }
        private void Awake()
        {
            // �ߺ� ����
            if(instance == null)
            {
                instance = this;
                // Scene ��ȯ �� �������� �ʵ��� ����
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // Scene �̵��� ������ ������ Manager �� �����ϴ� ���
                // �ش� Scene �� Manager ��ü�� �����Ѵ�.
                Destroy(gameObject);
                return;
            }

            // Qubit Prefab 
            prefab = Resources.Load<GameObject>("Prefabs/Qubit");
            if(null == prefab)
            {
                Debug.LogError("Qubit Prefab �� �ε��� �� �����ϴ�.");
                return;
            }

            // Check Qubit Parent Obj
            if (contentObj == null)
            {
                Debug.LogError("������ Qubit ��ü�� Content ��ü�� �������� �ʾҽ��ϴ�.");
                return;
            }
        }

        public void SetGame()
        {
            if (null == prefab || null == contentObj)
            {
                return;
            }

            // Qubit ��ü ���� �� ��ġ
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

                    // ���� ����
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