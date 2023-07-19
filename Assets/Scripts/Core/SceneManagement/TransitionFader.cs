using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.SceneManagement {
    public class TransitionFader : MonoBehaviour
    {
        CanvasGroup canvas;
        Coroutine activeFade;

        void Awake() {
            canvas = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate() {
            canvas.alpha = 1;
        }

        public Coroutine FadeOut(float fadeTime) {
            return FadeTo(1, fadeTime);
        }

        public Coroutine FadeIn(float fadeTime) {
            return FadeTo(0, fadeTime);
        }

        private Coroutine FadeTo(float target, float fadeTime) {
            if (activeFade != null) {
                StopCoroutine(activeFade);
            }
            activeFade = StartCoroutine(FadeRoutine(target, fadeTime));
            return activeFade;
        }

        private IEnumerator FadeRoutine(float target, float fadeTime) {
            while (!Mathf.Approximately(canvas.alpha, target)) {
                canvas.alpha = Mathf.MoveTowards(canvas.alpha, target, Time.deltaTime / fadeTime);
                yield return new WaitForEndOfFrame();
                Debug.Log("Ping");
            }
            Debug.Log("Pong");
            yield return null;
        }
    }
}