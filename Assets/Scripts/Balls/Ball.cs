namespace Zanta
{
    using UnityEngine;


    public abstract class Ball : MonoBehaviour
    {
        //every ball should define its radius thats publicly accessible
        public abstract EBallType BallType { get; }

        protected abstract void OnCollisionEnter(Collision collision);

        public abstract bool TrySetKinematic();

        public abstract bool TrySetDynamic();


        protected virtual void Awake()
        {

        }


        protected virtual void OnEnable()
        {

        }


        protected virtual void Update()
        {

        }


        protected virtual void OnDisable()
        {

        }


        protected virtual void OnDestroy()
        {

        }
    }
}
