using UnityEngine;
//수현
// 플레이어와 요리솥 상호작용을 위한 테스트용 코드
public class PlayerController_SH : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpForce = 5f;

    private CharacterController controller;
    private Vector3 moveDirection;
    private float yVelocity;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
        }
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(h, 0, v);
        if (inputDir.magnitude > 1f)
            inputDir = inputDir.normalized;

        // 월드 기준 이동 (카메라 기준은 FollowCamera에서 처리됨)
        Vector3 move = transform.forward * inputDir.z + transform.right * inputDir.x;

        // 중력 처리
        if (controller.isGrounded)
        {
            yVelocity = -1f;
            if (Input.GetButtonDown("Jump"))
            {
                yVelocity = jumpForce;
            }
        }
        else
        {
            yVelocity += gravity * Time.deltaTime;
        }

        moveDirection = move * moveSpeed;
        moveDirection.y = yVelocity;

        controller.Move(moveDirection * Time.deltaTime);
    }
}
