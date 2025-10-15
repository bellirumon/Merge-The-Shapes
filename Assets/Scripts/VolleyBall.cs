namespace Zanta
{
    using UnityEngine;

    public class VolleyBall : Ball
    {
        [SerializeField] private EBallType _ballType;

        [SerializeField] private GameObject _soccerBallPrefab;

        public override EBallType BallType => _ballType;


        protected override void OnCollisionEnter(Collision collision)
        {
            //if collision not with a volleyball, do nothing
            if (!collision.gameObject.TryGetComponent<Ball>(out Ball ball))
                return;

            if (ball.GetType() != this.GetType())
                return;

            //call merge from one of the two colliding pairs
            if (ball.GetInstanceID() < this.GetInstanceID())
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(false);
            Instantiate(_soccerBallPrefab, collision.GetContact(0).point, Quaternion.identity);

        }

        public override bool TrySetDynamic()
        {
            if (!TryGetComponent<Rigidbody>(out Rigidbody rb))
                return false;

            rb.isKinematic = false;
            return true;
        }


        public override bool TrySetKinematic()
        {
            if (!TryGetComponent<Rigidbody>(out Rigidbody rb))
                return false;

            rb.isKinematic = true;
            return true;
        }
    }
}