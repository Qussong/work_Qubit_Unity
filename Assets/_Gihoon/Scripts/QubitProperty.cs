using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Qubit
{
    public enum EQubitState
    {
        Stable,
        Unstable,
    }
    public struct QubitState
    {
        public Color currentColor;
        public float progressTime;
        public Coroutine runningCoroutine;
    }

    [RequireComponent(typeof(Image))]
    public class QubitProperty : MonoBehaviour, IPointerClickHandler
    {
        EQubitState qubitState = EQubitState.Stable;

        [Header("Properties")]
        [SerializeField] float duration = 0f;
        [SerializeField] Color beforeColor;
        [SerializeField] Color afterColor;

        private QubitState changingState;

        private Image CachedImage
        {
            get { return gameObject.GetComponent<Image>(); }
        }

        public EQubitState QubitState
        {
            get { return qubitState; }
        }

        

        private void Awake()
        {
            if(null == CachedImage)
            {
                Debug.LogError("Image 컴포넌트를 찾을 수 없습니다.");
                return;
            }

            CachedImage.color = beforeColor;

            changingState = new QubitState();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Click" + this.name);

            if(EQubitState.Unstable == qubitState)
            {
                BeStable();
                GameManager.Instance.UpSore();
            }
            else
            {
                Debug.Log("This Qubit state is Stable.");
            }
        }

        public void BeUnstable()
        {
            qubitState = EQubitState.Unstable;
            changingState.runningCoroutine = StartCoroutine(ColorChange(duration));
        }

        public void BeStable()
        {
            qubitState = EQubitState.Stable;
            StartCoroutine(ColorChange(duration));
            changingState.runningCoroutine = null;
        }

        private IEnumerator ColorChange(float duration)
        {
            if (null == CachedImage)
            {
                yield break;
            }

            float time = (EQubitState.Unstable == qubitState) ? 0f : changingState.progressTime;

            while (time < duration)
            {
                time += Time.deltaTime;
                if(EQubitState.Unstable == qubitState)
                {
                    CachedImage.color = Color.Lerp(beforeColor, afterColor, time / duration);
                    
                    changingState.progressTime = time;
                    changingState.currentColor = CachedImage.color;
                }
                else
                {
                    CachedImage.color = Color.Lerp(changingState.currentColor, beforeColor, time / duration);
                }

                yield return null;
            }

            if (EQubitState.Unstable == qubitState)
            {
                CachedImage.color = afterColor;
            }
            else 
            { 
                CachedImage.color = beforeColor;
            }
        }

    }
}

