using UnityEngine;
using UnityEngine.InputSystem;
using VR.Weapons;

public class InputTester : MonoBehaviour
{
    // Перетащи сюда объект с пистолетом (Right Controller)
    public GunShooter gun; 
    
    public InputAction testAction;

    // ЭТОТ МЕТОД МЫ ДОБАВИЛИ ДЛЯ ДИАГНОСТИКИ
    void Start()
    {
        Debug.Log("--- ДИАГНОСТИКА ЗАПУЩЕНА ---");

        if (gun == null)
        {
            Debug.LogError("!!! В методе Start() поле 'gun' уже ПУСТОЕ (null). Unity не смог его присвоить.");
        }
        else
        {
            Debug.Log("+++ В методе Start() поле 'gun' ЗАПОЛНЕНО. Проблема не в присваивании.");
            
            if (!gun.gameObject.activeInHierarchy)
            {
                Debug.LogError("!!! Объект '" + gun.gameObject.name + "' НЕАКТИВЕН в иерархии!");
            }
            else
            {
                Debug.Log("+++ Объект '" + gun.gameObject.name + "' АКТИВЕН в иерархии.");
            }

            if (!gun.enabled)
            {
                Debug.LogError("!!! Компонент GunShooter на объекте '" + gun.gameObject.name + "' ВЫКЛЮЧЕН (disabled)!");
            }
            else
            {
                Debug.Log("+++ Компонент GunShooter на объекте '" + gun.gameObject.name + "' ВКЛЮЧЕН (enabled).");
            }
        }
        Debug.Log("--- ДИАГНОСТИКА ЗАВЕРШЕНА ---");
    }

    void OnEnable()
    {
        testAction.Enable();
    }

    void OnDisable()
    {
        testAction.Disable();
    }

    void Update()
    {
        if (testAction.WasPressedThisFrame())
        {
            Debug.Log("!!! Клавиша F нажата, вызываю выстрел...");
            
            if (gun != null)
            {
                gun.FireOnceExternal();
                Debug.Log("!!! Функция выстрела вызвана!");
            }
            else
            {
                Debug.LogError("!!! Во время Update поле 'gun' оказалось ПУСТЫМ (null)!");
            }
        }
    }
}