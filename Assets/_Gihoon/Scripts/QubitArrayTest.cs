using System.Collections;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using UnityEngine;

public class QubitArrayTest : MonoBehaviour
{
    GameObject prefab = null;

    [SerializeField] int rows = 0;
    [SerializeField] int cols = 0;
    

    private void Awake()
    {
        prefab = Resources.Load<GameObject>("Prefabs/Qubit");
    }

    void Start()
    {
        if(null != prefab)
        {
            GenQubit(rows, cols);
        }
    }

    void GenQubit(int rowCnt, int colCnt, float diameter = 100.0f)
    {
        Rect contentSpace = gameObject.GetComponent<RectTransform>().rect;


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

                GameObject newQubit = Instantiate(prefab, Vector3.zero, Quaternion.identity, gameObject.GetComponent<RectTransform>());
                newQubit.GetComponent<RectTransform>().localPosition = new Vector3(posX, posY, 0.0f);
                newQubit.GetComponent<RectTransform>().localPosition += new Vector3(contenOffsetX, contenOffsetY, 0.0f);
            }
        }
    }
}
