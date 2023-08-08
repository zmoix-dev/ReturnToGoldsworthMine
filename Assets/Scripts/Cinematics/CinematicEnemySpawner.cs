using System.Collections;
using System.Collections.Generic;
using RPG.Game.Animation;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class CinematicEnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject cinematicEnemies;
    [SerializeField] GameObject postCinematicEnemies;

    void Start()
    {
        GetComponent<PlayableDirector>().played += CinematicEnemySetup;
        GetComponent<PlayableDirector>().stopped += EnableEnemies;
    }

    void CinematicEnemySetup(PlayableDirector director) {
        MoveCinematicEnemies(cinematicEnemies);
    }

    private void MoveCinematicEnemies(GameObject parent) {
        foreach (Transform transform in parent.transform) {
            Debug.Log("Move!");
            transform.gameObject.SetActive(true);
            Mover mover = transform.gameObject.GetComponent<Mover>();
            mover.MoveTo(transform.position - new Vector3(1f, 0f, 0.5f) * 20);
        }
    }

    void EnableEnemies(PlayableDirector director)
    {
        ToggleAllChildren(cinematicEnemies, false);
        ToggleAllChildren(postCinematicEnemies, true);
    }

    private void ToggleAllChildren(GameObject parent, bool toggle) {

        parent.SetActive(toggle);
        foreach (Transform transform in parent.transform) {
            transform.gameObject.SetActive(toggle);
        }
    }

}
