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
        [SerializeField] private Transform _bucketLeftBound;
        [SerializeField] private Transform _bucketRightBound;
        [SerializeField] private LineRenderer _lineRenderer;

        bool ballPicked = false;
        float _pickedBallRadius;


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

            //enable the line renderer
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, _currentlySelectedBall.transform.position);

            _lineRenderer.gameObject.SetActive(true);

            //compute the left and right bounds for the picked ball
            ComputeBoundsForPickedBall(_currentlySelectedBall);
        }


        private void MoveBall(float touchPosX_WorldSpace)
        {
            //compute new move position
            Vector3 newPos = new Vector3(touchPosX_WorldSpace * _sensMultiplier, _pickedBallYPos.position.y, 0f);

            //clamp the new position within the bounds of the bucket and considering the size of the ball
            newPos.x = Mathf.Clamp(newPos.x, _bucketLeftBound.position.x + _pickedBallRadius, _bucketRightBound.position.x - _pickedBallRadius);

            //update ball position
            _currentlySelectedBall.transform.position = newPos;

            //update line renderer
            _lineRenderer.SetPosition(0, _currentlySelectedBall.transform.position);

            //do a raycast from beneath the edge of the the currently picked ball until a hit is detected
            //add the hit point as the second position for the linerenderer
            Ray ray = new Ray(_currentlySelectedBall.transform.position - new Vector3(0f, _pickedBallRadius, 0f), Vector3.down);
            
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 20f))
            {
                Vector3 hitPoint = hitInfo.point;
                _lineRenderer.SetPosition(1, hitPoint);
            }            
        }


        private void DropBall()
        {
            _currentlySelectedBall.TrySetDynamic();

            //disable line renderer
            _lineRenderer.gameObject.SetActive(false);
        }


        private void ComputeBoundsForPickedBall(Ball ball)
        {
            if (!ball.TryGetComponent<SphereCollider>(out SphereCollider sphereCollider))
                return;

            _pickedBallRadius = sphereCollider.radius * ball.transform.lossyScale.x;
        }

    }
}
