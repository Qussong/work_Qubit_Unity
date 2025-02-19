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
        [SerializeField][Tooltip("(���� ���� ����)���� ���Ḧ ���� �ذ��ؾ��� Qubit ����")] public int endCodition = 10;

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

            /*
            // ��ü ���� �� Scene �� ��ġ
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