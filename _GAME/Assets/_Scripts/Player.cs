using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject Utility;

    private List<Vector3> walls = new List<Vector3>(); // reference to wall positions
    private List<Vector3> moveWalls = new List<Vector3>(); // reference to movewall positions
    private List<GameObject> moveWallRef = new List<GameObject>();
    private GameObject[] reference; // variable is used as a reference to find gameobjects with specific tags
    private Animator animator;
    private Vector3 change;
    private Vector3 direction; // variable will be used if needed to move blocks
    private Rigidbody2D playerRigidbody;
    private float cooldown = 0f, grabCoolDown = 0f;
    private int refNum = -1;
    private bool checkX, checkPos;
    private bool grab = false;

    private float timer = 0;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        direction = new Vector3(0, -1, 0); // Player starts game facing down
        playerRigidbody = GetComponent<Rigidbody2D>();

        reference = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in reference) // foreach gameobject in the scene with tag "Wall" record its position in vector3 list walls
        {
           walls.Add(wall.transform.position);
           //Debug.Log("Added Wall to list of walls : " + wall.transform.position);
        }
        reference = GameObject.FindGameObjectsWithTag("MoveBlock");
        foreach (GameObject moveWall in reference)
        {
            moveWalls.Add(moveWall.transform.position);
            moveWallRef.Add(moveWall);
            //Debug.Log("Added moveable block to list of moveWalls : " + moveWall.transform.position);
        }
    }

    void Update()
    {
        cooldown += Time.deltaTime;
        timer += Time.deltaTime;
        //grabCoolDown += Time.deltaTime;
        change = Vector3.zero;
        if (change != Vector3.zero)
        {
            cooldown = 0f;
            MoveCharacter();
        }

        // Movement
        if (Input.GetButtonDown("Up")) {MoveUp();}
        else if (Input.GetButtonDown("Down")) {MoveDown();}
        if (Input.GetButtonDown("Right")) {MoveRight();}
        else if (Input.GetButtonDown("Left")) {MoveLeft();}

        if (timer > 0.35f)
        {
            timer = 0;
            //Debug.Log("OFF COOLDOWN");    
            if (direction == new Vector3(1, 0, 0))
                MoveRight();
            else if (direction == new Vector3(-1, 0, 0))
                MoveLeft();
            else if (direction == new Vector3(0, 1, 0))
                MoveUp();
            else if (direction == new Vector3(0, -1, 0))
                MoveDown();
                
        }
    }

    public void MoveCharacter()
    {
        playerRigidbody.MovePosition(transform.position + change);
    }
    public void MoveRight()
    {
        StartCoroutine(moveLerp(new Vector3(1, 0, 0)));
        direction = new Vector3(1, 0, 0);
        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);
    }
    public void MoveLeft()
    {
        StartCoroutine(moveLerp(new Vector3(-1, 0, 0)));
        direction = new Vector3(-1, 0, 0);
        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);
    }
    public void MoveUp()
    {
        StartCoroutine(moveLerp(new Vector3(0, 1, 0)));
        direction = new Vector3(0, 1, 0);
        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);
    }
    public void MoveDown()
    {
        StartCoroutine(moveLerp(new Vector3(0, -1, 0)));
        direction = new Vector3(0, -1, 0);
        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);
    }

    private IEnumerator moveToPos(Vector3 endPos, int i, Vector3 direction)
    {
        while (moveWalls[i] != endPos + (direction * -1))
        {
            //Debug.Log("MOVE TO POSITION");
            moveWalls[i] += direction;
            yield return StartCoroutine( Utility.GetComponent<Utility>().Move(direction, moveWallRef[i].transform.position, moveWallRef[i].GetComponent<Rigidbody2D>()) );
            moveWallRef[i].transform.position = moveWalls[i];
            //Debug.Log(moveWallRef[i].transform.position);
        }
        //Debug.Log("STOP");
        yield break;
    }

    // Player Movement & Collision Check
    private IEnumerator moveLerp(Vector3 moveDirection)
    {
        // Wall collision check
        for (int i = 0; i < walls.Count; i++)
        {
            if (walls[i] == transform.position + moveDirection)
            {
                //Debug.Log("COLLISION");
                yield break; // exit coroutine (no movement)
            }
        }
        // MoveBlock collision check
        for (int i = 0; i < moveWalls.Count; i++)
        {
            // if where the player wants to move is occupied by a move block
            if (moveWalls[i] == transform.position + moveDirection)
            {
                for (int j = 0; j < walls.Count; j++)
                {
                    if (walls[j] == transform.position + (2 * moveDirection))
                    {
                        //Debug.Log("### Wall Blocking MoveBlock ###");
                        yield break; // exit coroutine
                    }
                }
                //Debug.Log("MOVEWALL_COLLISION");
                moveWalls[i] += moveDirection;
                StartCoroutine( Utility.GetComponent<Utility>().Move(moveDirection, moveWallRef[i].transform.position, moveWallRef[i].GetComponent<Rigidbody2D>()) );
            }
        }
        // If not on cooldown
        if (cooldown > 0.3f)
        {
            cooldown = 0f; // Reset
            Vector3 start = transform.position;
            Vector3 end = transform.position + moveDirection;
            // While not on cooldown lerp from start to end position in 0.2 seconds
            while (cooldown <= 0.3f)
            {
                playerRigidbody.MovePosition(Vector3.Lerp(start, end, cooldown / 0.3f)); // lerp from start to end
                yield return null; // continue coroutine
            }
            playerRigidbody.MovePosition(Vector3.Lerp(start, end, 1f)); // make sure last update is in the exact end position
            yield break; // exit coroutine
        }
    }
}
