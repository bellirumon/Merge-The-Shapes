namespace Zanta
{
    using UnityEngine;
    using UnityEngine.Serialization;

    public class BallSpawnManager : MonoBehaviour
    {
        public static BallSpawnManager Instance { get; private set; }
        private GameObject _currentBall;
        private GameObject _nextBall;

        [SerializeField] private GameObject[] _spawnableBallPrefabs;
        [SerializeField] private Transform _currentBallSpawnPos;
        [SerializeField] private Transform _nextBallSpawnPos;


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Another instance of BallSpawnManager already exists! Destroying this one");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }


        private void OnEnable()
        {
            //spawn a current ball and a next ball
            _currentBall = SpawnBall(_currentBallSpawnPos.position);
            _nextBall = SpawnBall(_nextBallSpawnPos.position);
        }


        public GameObject SpawnBall(Vector3 position)
        {
            byte index = (byte)Random.Range(0, _spawnableBallPrefabs.Length);

            GameObject ballGo = Instantiate(_spawnableBallPrefabs[index], position, Quaternion.identity);

            if (ballGo.TryGetComponent<Ball>(out Ball ball))
            {
                ball.TrySetKinematic();
            }

            return ballGo;
        }


        public GameObject GetCurrentBall()
        {
            //getting current ball is akin to using the current ball, so spawn another ball and move next ball to current ball
            
            GameObject currentBall = _currentBall; //return this

            //move next ball to current ball
            _currentBall = _nextBall;
            _nextBall.transform.position = _currentBallSpawnPos.position;

            //spawn a new ball at next ball
            _nextBall = SpawnBall(_nextBallSpawnPos.position);

            return currentBall;
        } 
            

    }
}