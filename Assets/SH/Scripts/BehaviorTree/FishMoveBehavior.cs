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
        // �׳� �ð����� ? 
        // ���� �������Ѱ� �ֳ� ? 
        // ���� Tag�� ������� ��������?

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            ChooseNewDirection();
            timer = 5.0f;
            return TaskStatus.Success;
        }

        Move();
        RotateTowardsDirection();

        return TaskStatus.Running; // �̷��� ���� ���� �׳� ��ȯ�̰� 
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


