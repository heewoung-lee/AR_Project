using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("FishMove")]
[TaskIcon("Assets/Behavior Designer Tutorials/Tasks/Editor/{SkinColor}SeekIcon.png")]
public class FishMoveBehavior : Action
{
    public float speed = 5.0f;
    public float changeDirectionTime = 15.0f;
    public float rotationSpeed = 2.0f;

    private Rigidbody rb;
    private float timer;
    private Vector3 newDirection;

    public override void OnAwake()
    {
        rb = GetComponent<Rigidbody>();
        timer = 5.0f;
        ChooseNewDirection();
    }

    public override TaskStatus OnUpdate()
    {
        // 그냥 시간으로 ? 
        // 조건 넣을만한게 있나 ? 
        // 같은 Tag의 물고기라면 군집형성?

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            ChooseNewDirection();
            timer = 5.0f;
            return TaskStatus.Success;
        }

        Move();
        RotateTowardsDirection();

        return TaskStatus.Running; // 이러면 러닝 상태 그냥 반환이고 
    }

    void ChooseNewDirection()
    {
        float x = Random.Range(-5.0f, 5.0f);
        float y = Random.Range(-5.0f, 5.0f);
        float z = Random.Range(-5.0f, 5.0f);
        newDirection = new Vector3(x, y, z);
    }

    void Move()
    {
        rb.velocity = newDirection * speed * 2f;
    }

    void RotateTowardsDirection()
    {
        Quaternion targetRotation = Quaternion.LookRotation(newDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    

}


