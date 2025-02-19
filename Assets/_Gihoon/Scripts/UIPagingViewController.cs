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
    /// Page�� �����ϰ� View�� �巡���Ͽ� ��Ʈ���� �� �ִ� Ŭ����
    /// Unity���� �����ϴ� Scroll Component �� �ʼ� ������Ʈ�� ������ �־���Ѵ�.
    /// Unity Event System ���� �����ϴ� IBeginDragHandler, IEndDragHandler �������̽��� ��ӹ޴´�.
    /// 
    /// </summary>

    // Rect Transform �� ScrollRect �� �ʿ��ϴٴ� �ǹ�
    [RequireComponent(typeof(RectTransform))]   // UI�� �ʼ�
    [RequireComponent(typeof(ScrollRect))]      // ��ũ�� ��� ����
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

        private bool isAnimating = false;       // �ִϸ��̼� ��� ������ ��Ÿ���� �÷���
        private Vector2 destPosition;           // �������� ��ũ�� ��ġ
        private Vector2 initialPosition;        // �ڵ� ��ũ���� ������ ���� ��ũ�� ��ġ
        private AnimationCurve animationCurve;  // �ڵ� ��ũ�ѿ� ���õ� �ִϸ��̼� Ŀ��
        private int prevPageIdx = -1;           // ���� �������� �ε���
        private Rect currentViewRect;           // ��ũ�� ���� �簢�� ũ��

        public RectTransform CachedRectTransform
        {
            get { return gameObject.GetComponent<RectTransform>(); }
        }

        public ScrollRect CachedScrollRect
        {
            get { return gameObject.GetComponent<ScrollRect>(); }
        }

        void Start()
        {
            UpdateView();
        }

        void LateUpdate()
        {
            if (isAnimating)
            {
                if (Time.time >= animationCurve.keys[animationCurve.length - 1].time)
                {
                    // �ִϸ��̼� Ŀ���� ������ �������� �������� �ִϸ��̼��� ������.
                    CachedScrollRect.content.anchoredPosition = destPosition;
                    isAnimating = false;
                    return;
                }

                // �ִϸ��̼� Ŀ�긦 ����Ͽ� ���� ��ũ�� ��ġ�� ����ؼ� ��ũ�� �並 �̵���Ų��.
                Vector2 newPosition = initialPosition + (destPosition - initialPosition) * animationCurve.Evaluate(Time.time);
                CachedScrollRect.content.anchoredPosition = newPosition;
            }
        }   // end LateUpdate

        // IBeginDragHandler Interface
        // �巡�� ���۽� ȣ��
        public void OnBeginDrag(PointerEventData eventData)
        {
            // �ִϸ��̼� ���� �÷��� ����
            isAnimating = false;
        }   // end OnBeginDrag

        // IEndDragHandler Interface
        // �巡�� ������ ȣ��
        public void OnEndDrag(PointerEventData eventData)
        {
            GridLayoutGroup grid = CachedScrollRect.content.GetComponent<GridLayoutGroup>();

            // ���� �������� ��ũ�� �並 �����.
            CachedScrollRect.StopMovement();

            // Grid Layout Group�� cell size�� spacing�� �̿��Ͽ� �� �������� ���� ����Ѵ�.
            float pageWidth = (grid.cellSize.x + grid.spacing.x);

            // ��ũ���� ���� ��ġ�κ��� ���� �������� �ε����� ����Ѵ�.
            int pageIdx = Mathf.RoundToInt((CachedScrollRect.content.anchoredPosition.x) / pageWidth);  // ���� ��ġ / �� �ϳ��� �� = ������ϴ� �ε��� ��ġ

            if(pageIdx == prevPageIdx && Mathf.Abs(eventData.delta.x) >= 4)
            {
                // ���� �ӵ� �̻����� �巡���� ��� �ش� �������� �� ������ �����Ų��.
                CachedScrollRect.content.anchoredPosition += new Vector2(eventData.delta.x, 0.0f);
                pageIdx += (int)Mathf.Sign(-eventData.delta.x);
            }

            // ù ������ �Ǵ� �� �������� ��� �� �̻� ��ũ������ �ʵ��� �Ѵ�.
            if(pageIdx < 0)
            {
                pageIdx = -1;
            }
            else if ( pageIdx > grid.transform.childCount - 1)
            {
                pageIdx = grid.transform.childCount - 1;
            }

            prevPageIdx = pageIdx;  // ���� �������� �ε����� �����Ѵ�.

            // �������� ��ũ�� ��ġ�� ����Ѵ�.
            float destX = pageIdx * pageWidth;
            destPosition = new Vector2(destX, CachedScrollRect.content.anchoredPosition.y);

            // ������ ���� ��ũ�� ��ġ�� �����صд�.
            initialPosition = CachedScrollRect.content.anchoredPosition;

            // �ִϸ��̼� Ŀ�긦 �ۼ��Ѵ�.
            Keyframe keyFrame1 = new Keyframe(Time.time, 0.0f, key1InTangent, key1OutTangent);
            Keyframe keyFrame2 = new Keyframe(Time.time + animationDuration, 1.0f, key2InTangent, key2OutTangent);
            animationCurve = new AnimationCurve(keyFrame1, keyFrame2);

            // �ִϸ��̼� ��������� ��Ÿ���� �÷��� ����
            isAnimating = true;

            /*
            // ������ ��Ʈ�� ǥ�ø� �����Ѵ�.
            if(pageControl != null)
            {
                pageControl.SetCurrentPage(pageIdx);
            }
            */

        }   // end OnEndDrag

        // Scroll Content Padding �� �����ϴ� �޼���
        private void UpdateView()
        {
            // ��ũ�� ���� �簢�� ũ�⸦ �����صд�.
            currentViewRect = CachedRectTransform.rect;

            // GridLayoutGroup �� cell size�� ����Ͽ� Scroll Content�� Padding�� ����Ͽ� �����Ѵ�.
            GridLayoutGroup grid = CachedScrollRect.content.GetComponent<GridLayoutGroup>();
            int paddingHeight = Mathf.RoundToInt((currentViewRect.width - grid.cellSize.x) / 2.0f);
            int paddingWidth = Mathf.RoundToInt((currentViewRect.height - grid.cellSize.y) / 2.0f);
            //grid.padding = new RectOffset(paddingHeight, paddingHeight, paddingWidth, paddingHeight);
            grid.padding = new RectOffset(paddingHeight, paddingHeight, 0, 0);
        }   // end UpdateView

    }   // end class
}