namespace Zanta
{
    using Unity.VisualScripting;
    using UnityEngine;

    public class Player : MonoBehaviour
    {
        private Ball _currentlySelectedBall;

        [Range(0.1f, 5f)]
        [SerializeField] private float _sensMultiplier;
        [SerializeField] private float _cooldownTime;
        private float _cooldownTimer;
        private bool _cooldownActive = false;
        [SerializeField] private Transform _pickedBallYPos;

        bool ballPicked = false;


        private void Update()
        {
            if (_cooldownActive)
            {
                _cooldownTimer -= Time.deltaTime;

                if (_cooldownTimer > 0f)
                    return;

                _cooldownActive = false;
            }

            if (Input.touchCount <= 0)
                return;

            Touch touch = Input.GetTouch(0);

            float cameraZDist = Mathf.Abs(Camera.main.transform.position.z);
            Vector3 touchPos_Vector3 = new Vector3(touch.position.x, touch.position.y, cameraZDist);
            Vector3 touchPos_WorldSpace = Camera.main.ScreenToWorldPoint(touchPos_Vector3);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                PickBall(touchPos_WorldSpace.x);
                ballPicked = true;
                break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                if (ballPicked)
                {
                    MoveBall(touchPos_WorldSpace.x);
                }
                else
                {
                    PickBall(touchPos_WorldSpace.x);
                    ballPicked = true;
                    MoveBall(touchPos_WorldSpace.x);
                }
                break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                DropBall();
                ballPicked = false;

                //after dropping a ball, activate a cooldown during which the player cannot pick another ball
                _cooldownActive = true;
                _cooldownTimer = _cooldownTime;
                break;
            }

        }


        private void PickBall(float touchPosX_WorldSpace)
        {
            GameObject ballGo = BallSpawnManager.Instance.GetCurrentBall();

            if (ballGo.TryGetComponent<Ball>(out Ball ball))
            {
                _currentlySelectedBall = ball;
            }

            //set the initial position of the picked ball
            _currentlySelectedBall.transform.position = new Vector3(touchPosX_WorldSpace, _pickedBallYPos.position.y, 0f);

        }


        private void MoveBall(float touchPosX_WorldSpace)
        {
            _currentlySelectedBall.transform.position = new Vector3(touchPosX_WorldSpace * _sensMultiplier, _pickedBallYPos.position.y, 0f);
        }


        private void DropBall()
        {
            _currentlySelectedBall.TrySetDynamic();
        }

    }
}
