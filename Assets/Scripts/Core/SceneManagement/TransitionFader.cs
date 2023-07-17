using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.SceneManagement {
    public class TransitionFader : MonoBehaviour
    {
        CanvasGroup canvas;

        void Awake() {
            canvas = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate() {
            canvas.alpha = 1;
        }

        public IEnumerator FadeOut(float fadeTime) {
            while (canvas.alpha < 1) {
                canvas.alpha += Time.deltaTime / fadeTime;
                yield return new WaitForEndOfFrame();
            }
        }

        public IEnumerator FadeIn(float fadeTime) {
            while (canvas.alpha > 0) {
                canvas.alpha -= Time.deltaTime / fadeTime;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}