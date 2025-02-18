using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 
    /// Page를 관리하고 View를 드래그하여 컨트롤할 수 있는 클래스
    /// Unity에서 제공하는 Scroll Component 를 필수 컴포넌트로 가지고 있어야한다.
    /// Unity Event System 에서 제공하는 IBeginDragHandler, IEndDragHandler 인터페이스를 상속받는다.
    /// 
    /// </summary>

    // Rect Transform 과 ScrollRect 가 필요하다는 의미
    [RequireComponent(typeof(RectTransform))]   // UI에 필수
    [RequireComponent(typeof(ScrollRect))]      // 스크롤 기능 강제
    public class UIPagingViewController : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField]
        protected GameObject contentRoot = null;

        //[SerializeField]
        //protected UIPageControl pageControl = null;

        [SerializeField]
        private float animationDuration = 0.3f;

        private float key1InTangent = 0f;
        private float key1OutTangent = 1f;
        private float key2InTangent = 1f;
        private float key2OutTangent = 0f;

        private bool isAnimating = false;       // 애니메이션 재생 중임을 나타내는 플래그
        private Vector2 destPosition;           // 최종적인 스크롤 위치
        private Vector2 initialPosition;        // 자동 스크롤을 시작할 때의 스크롤 위치
        private AnimationCurve animationCurve;  // 자동 스크롤에 관련된 애니메이션 커브
        private int prevPageIdx = -1;            // 이전 페이지의 인덱스
        private Rect currentViewRect;           // 스크롤 뷰의 사각형 크기

        public RectTransform CachedRectTransform
        {
            get { return gameObject.GetComponent<RectTransform>(); }
        }

        public ScrollRect CachedScrollRect
        {
            get { return gameObject.GetComponent<ScrollRect>(); }
        }

        // IBeginDragHandler Interface
        // 드래그 시작시 호출
        public void OnBeginDrag(PointerEventData eventData)
        {
            // 애니메이션 도중 플래그 리셋
            isAnimating = false;
        }   // end OnBeginDrag

        // IEndDragHandler Interface
        // 드래그 끝날시 호출
        public void OnEndDrag(PointerEventData eventData)
        {
            GridLayoutGroup grid = CachedScrollRect.content.GetComponent<GridLayoutGroup>();

            // 현재 동작중인 스크롤 뷰를 멈춘다.
            CachedScrollRect.StopMovement();

            // Grid Layout Group의 cell size와 spacing을 이용하여 한 페이지의 폭을 계산한다.
            float pageWidth = (grid.cellSize.x + grid.spacing.x);

            // 스크롤의 현재 위치로부터 맞출 페이지의 인덱스를 계산한다.
            int pageIdx = Mathf.RoundToInt((CachedScrollRect.content.anchoredPosition.x) / pageWidth);  // 현재 위치 / 셀 하나의 폭 = 맞춰야하는 인덱스 위치

            if(pageIdx == prevPageIdx && Mathf.Abs(eventData.delta.x) >= 4)
            {
                // 일정 속도 이상으로 드래그할 경우 해당 방향으로 한 페이지 진행시킨다.
                CachedScrollRect.content.anchoredPosition += new Vector2(eventData.delta.x, 0.0f);
                pageIdx += (int)Mathf.Sign(-eventData.delta.x);
            }

            // 첫 페이지 또는 끝 페이지일 경우 그 이상 스크롤하지 않도록 한다.
            if(pageIdx < 0)
            {
                pageIdx = -1;
            }
            else if ( pageIdx > grid.transform.childCount - 1)
            {
                pageIdx = grid.transform.childCount - 1;
            }

            prevPageIdx = pageIdx;  // 현재 페이지의 인덱스를 유지한다.

            // 최종적인 스크롤 위치를 계산한다.
            float destX = pageIdx * pageWidth;
            destPosition = new Vector2(destX, CachedScrollRect.content.anchoredPosition.y);

            // 시작할 때의 스크롤 위치를 저장해둔다.
            initialPosition = CachedScrollRect.content.anchoredPosition;

            // 애니메이션 커브를 작성한다.
            Keyframe keyFrame1 = new Keyframe(Time.time, 0.0f, key1InTangent, key1OutTangent);
            Keyframe keyFrame2 = new Keyframe(Time.time + animationDuration, 1.0f, key2InTangent, key2OutTangent);
            animationCurve = new AnimationCurve(keyFrame1, keyFrame2);

            // 애니메이션 재생중임을 나타내는 플래그 설정
            isAnimating = true;

            /*
            // 페이지 컨트롤 표시를 갱신한다.
            if(pageControl != null)
            {
                pageControl.SetCurrentPage(pageIdx);
            }
            */
        }   // end OnEndDrag

        void Start()
        {
            UpdateView();


        }   // end Start

        void Update()
        {
            
        }   // end Update

        void LateUpdate()
        {
            if(isAnimating)
            {
                if(Time.time >= animationCurve.keys[animationCurve.length - 1].time)
                {
                    // 애니메이션 커브의 마지막 프레임을 지나가면 애니메이션을 끝낸다.
                    CachedScrollRect.content.anchoredPosition = destPosition;
                    isAnimating = false;
                    return;
                }

                // 애니메이션 커브를 사용하여 현재 스크롤 위치를 계산해서 스크롤 뷰를 이동시킨다.
                Vector2 newPosition = initialPosition + (destPosition - initialPosition) * animationCurve.Evaluate(Time.time);
                CachedScrollRect.content.anchoredPosition = newPosition;
            }
        }   // end LateUpdate

        // Scroll Content Padding 을 갱신하는 메서드
        private void UpdateView()
        {
            // 스크롤 뷰의 사각형 크기를 보존해둔다.
            currentViewRect = CachedRectTransform.rect;

            // GridLayoutGroup 의 cell size를 사용하여 Scroll Content의 Padding을 계산하여 설정한다.
            GridLayoutGroup grid = CachedScrollRect.content.GetComponent<GridLayoutGroup>();
            int paddingHeight = Mathf.RoundToInt((currentViewRect.width - grid.cellSize.x) / 2.0f);
            int paddingWidth = Mathf.RoundToInt((currentViewRect.height - grid.cellSize.y) / 2.0f);
            //grid.padding = new RectOffset(paddingHeight, paddingHeight, paddingWidth, paddingHeight);
            grid.padding = new RectOffset(paddingHeight, paddingHeight, 0, 0);
        }   // end UpdateView

    }   // end class
}