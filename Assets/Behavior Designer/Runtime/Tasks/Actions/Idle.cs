

using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Returns a TaskStatus of running. Will only stop when interrupted or a conditional abort is triggered.")]
    [TaskIcon("{SkinColor}IdleIcon.png")]
    public class Idle : Action
    {
        float _idleExitTime = 0;

        public override TaskStatus OnUpdate()
        {
            _idleExitTime += Time.deltaTime;

            if (_idleExitTime > 3 ) 
            {
                _idleExitTime = 0;
                Debug.Log("Idle Success");
                return TaskStatus.Success;
            }

            Debug.Log("Idle Running");
            return TaskStatus.Running;
        }

        public override void OnBehaviorComplete()
        {
        }
    }
}