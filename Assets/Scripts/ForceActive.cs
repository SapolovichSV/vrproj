using UnityEngine;

public class ForceActive : MonoBehaviour
{
    // LateUpdate вызывается в самом конце каждого кадра,
    // ПОСЛЕ всех обычных Update.
    void LateUpdate()
    {
        // Мы даем другому скрипту выключить наш объект,
        // а потом в самом конце кадра - принудительно включаем его обратно.
        gameObject.SetActive(true);

        // После того как мы один раз его включили,
        // этот скрипт нам больше не нужен. Выключаем его для оптимизации.
        this.enabled = false; 
    }
}