using UnityEngine;

namespace RPG.UI {   
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText damageTextPrefab = null;

        public void Spawn(float damage) {
            DamageText instance = Instantiate(damageTextPrefab, transform);
            instance.SetValue(damage);
            Destroy(instance, 0.5f);
        }
    }
}