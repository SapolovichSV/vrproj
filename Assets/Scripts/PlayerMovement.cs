using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 5f;

    void Update()
    {
        float moveX = Input.GetAxis("XR Device Simulator Horizontal");
        float moveZ = Input.GetAxis("XR Device Simulator Vertical");

        Vector3 movement = new Vector3(moveX, 0, moveZ) * speed * Time.deltaTime;
        controller.Move(movement);
    }
}