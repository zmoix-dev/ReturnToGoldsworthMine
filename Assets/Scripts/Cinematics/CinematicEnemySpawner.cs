using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CinematicEnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject cinematicEnemies;
    [SerializeField] GameObject postCinematicEnemies;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<PlayableDirector>().stopped += EnableEnemies;
    }

    // Update is called once per frame
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
