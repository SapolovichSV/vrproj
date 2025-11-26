using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject player; // —сылка на персонажа

    void Start()
    {
        Instantiate(player, transform.position, transform.rotation); // —павним персонажа
    }
}
