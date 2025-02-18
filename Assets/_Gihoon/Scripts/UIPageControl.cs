using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 
    /// 해당 클래스는 필수로 필요하진 않지만,
    /// 현재 View 가 몇번째 View 인지 표시를 위해서 같이 사용하면 유용하다.
    /// Unity 의 UI 의 Toggle 을 활용한다.
    /// 
    /// </summary>

    public class UIPageControl : MonoBehaviour
    {
        [SerializeField]
        private Toggle toggleBase;

        private List<Toggle> listToggles = new List<Toggle>();

        private void Awake()
        {
            // 복사 원본 페이지 인디케이터는 비활성화시켜 둔다.
            toggleBase.gameObject.SetActive(false);
        }

        public void SetNumberOfPage(int number)
        {
            if(listToggles.Count < number)
            {
                // 페이지 인디케이터 수가 지정된 페이지 수보다 적으면
                // 복사 원본 페이지 인디케이터로부터 새로운 페이지 인디케이터를 작성한다.
                for (int i = listToggles.Count; i < number; ++i)
                {
                    Toggle indicator = Instantiate(toggleBase) as Toggle;
                    indicator.gameObject.SetActive(true);
                    indicator.transform.SetParent(toggleBase.transform.parent);
                    indicator.transform.localScale = toggleBase.transform.localScale;
                    listToggles.Add(indicator);
                }
            }
            else if(listToggles.Count > number)
            {
                // 페이지 인디케이터 수가 지정된 페이지 수보다 많으면 삭제한다.
                for(int i = listToggles.Count - 1; i >= number; --i)
                {
                    Destroy(listToggles[i].gameObject);
                    listToggles.RemoveAt(i);
                }
            }
        }

        public void SetCurrentPage(int idx)
        {
            if(idx >= 0 && idx <= listToggles.Count - 1)
            {
                // 지정된 페이지에 대응하되 페이지 인디케이터를 ON으로 지정한다.
                // 토글 그룹을 설정해두었기에 다른 인디케이터는 자동으로 OFF가 된다.
                listToggles[idx].isOn = true;
            }
        }

    }   // end class
}
