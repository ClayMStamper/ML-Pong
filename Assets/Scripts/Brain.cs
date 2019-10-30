using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct GameState {
	
	public Vector2 ballPos;
	public Vector2 ballVel;
	public Vector2 paddlePos;
	public Vector2 nextBallHit;
	
	public GameState(Vector2 ballPos, Vector2 ballVel, Vector2 paddlePos, Vector2 nextBallHit) {
		this.ballPos = ballPos;
		this.ballVel = ballVel;
		this.paddlePos = paddlePos;
		this.nextBallHit = nextBallHit;
	}
	
	public List<double> GetInputs() {
		return new List<double> {
			ballPos.x,
			ballPos.y,
			ballVel.x,
			ballVel.y,
			paddlePos.x,
			paddlePos.y,
			nextBallHit.x,
			nextBallHit.y
		};
	}
	
}

public class Brain : MonoBehaviour {

	public Transform paddle;
	public Transform ball;
	public float paddleSpeed = 5;
	public List<double> desiredOutputs = new List<double>{0};
	
	[HideInInspector]
	public float numSaved = 0, numMissed = 0;
	
	private Rigidbody2D ballRb;
	private float paddleMinY = 8.8f;
	private float paddleMaxY = 17.4f;
	private ANN ann;

	private float yvel;
	

	// Use this for initialization
	void Start () {
		ann = new ANN(8, 1, 1, 4, 0.11);
		ballRb = ball.GetComponent<Rigidbody2D>();		
	}

	void Update () {
		MovePaddle();
		Learn();
	}

	private void MovePaddle() {
		Vector3 velocity = Vector3.up * yvel;
		Vector3 newPos = paddle.position + velocity;
		//newPos.x = 5;
		newPos.y = Mathf.Clamp(newPos.y, paddleMinY, paddleMaxY);
		paddle.position = newPos;
	}

	private void Learn() {
		int layerMask = 1 << 8;
		
		RaycastHit2D hit = Physics2D.Raycast(ball.transform.position, ballRb.velocity, 1000);
		GameState gameState = new GameState(ball.position,ballRb.velocity, paddle.position, hit.point);

		//desired movement is current pos to pos that ball will be
		float desiredDelta = hit.point.y - paddle.position.y;
		
		List<double> inputs = gameState.GetInputs();
		List<double> output = ann.Train(inputs, new List<double>{desiredDelta});

		yvel = (float) output[0] * Time.deltaTime * paddleSpeed;
		print("Y vel = " + yvel);
	}
	
	// Update is called once per frame

}
