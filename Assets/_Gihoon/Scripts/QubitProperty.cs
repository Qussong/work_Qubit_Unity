using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Qubit
{
    public enum EQubitState
    {
        Stable,
        Unstable,
    }

    [RequireComponent(typeof(Image))]
    public class QubitProperty : MonoBehaviour
    {
        EQubitState qubitState = EQubitState.Stable;

        [Header("Properties")]
        [SerializeField] float duration = 0f;
        [SerializeField] Color beforeColor;
        [SerializeField] Color afterColor;

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
        }

        void Start()
        {
            
        }

        void Update()
        {
        
        }

        public void BeUnstable()
        {
            qubitState = EQubitState.Unstable;
            StartCoroutine(ColorChange(duration));
        }

        private IEnumerator ColorChange(float duration)
        {
            if (null == CachedImage)
            {
                yield break;
            }

            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                if(EQubitState.Unstable == qubitState)
                {
                    CachedImage.color = Color.Lerp(beforeColor, afterColor, time / duration);
                }
                else
                {
                    CachedImage.color = Color.Lerp(afterColor, beforeColor, time / duration);
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

