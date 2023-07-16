using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement {
    [RequireComponent(typeof(SavingSystem))]
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] float fadeInTime = 1f;
        const string defaultSaveFile = "save";

        private void Awake() {
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene() {
            TransitionFader fader = FindObjectOfType<TransitionFader>();
            fader.FadeOutImmediate();
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            yield return fader.FadeIn(fadeInTime);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L)) {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.S)) {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.Delete)) {

            }
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }

        public void Delete() {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }
    }
}
