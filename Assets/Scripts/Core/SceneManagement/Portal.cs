using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

namespace RPG.SceneManagement {
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier {
            A, B, C
        }

        [SerializeField] Transform spawnPoint;
        [SerializeField] int sceneToLoad = -1;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeInTime;
        [SerializeField] float fadeOutTime;
        [SerializeField] float fadeWaitTime;

        private void OnTriggerEnter(Collider other) {
            if (other.tag.Equals(UnitType.GetType(UnitType.Type.PLAYER))) {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition() {
            if (sceneToLoad < 0) {
                Debug.LogError("Scene to load is not set.");
                yield break;
            }
            TransitionFader fader = FindObjectOfType<TransitionFader>();
            yield return fader.FadeOut(fadeOutTime);
            DontDestroyOnLoad(gameObject);
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            SpawnPlayer(GetDestinationPortal());
            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime);
            Destroy(gameObject);
        }

        private void SpawnPlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag(UnitType.GetType(UnitType.Type.PLAYER));
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = otherPortal.spawnPoint.transform.position;
            player.transform.rotation = otherPortal.spawnPoint.transform.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        private Portal GetDestinationPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>()) {
                if (portal == this) continue;

                if (portal.destination == destination) {
                    return portal;
                }
            }
            return null;
        }
    }
}