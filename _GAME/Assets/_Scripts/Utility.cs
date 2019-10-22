using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    // Move Function
    public IEnumerator Move(Vector3 moveDirection, Vector3 start, Rigidbody2D rb)
    {
        Vector3 end = start + moveDirection;
        float percentage = 0f;
        float speed = 0.1f;
        while (percentage < 1f)
        {
            rb.MovePosition(Vector3.Lerp(start, end, percentage));
            percentage += speed;
            yield return null;
        }
        rb.MovePosition(Vector3.Lerp(start, end, 1f));
        yield break;
    }
    
    public Vector3 findMovePosition(bool checkX, bool checkPos, List<Vector3> walls, List<Vector3> moveWalls, int mwInt)
    {
        Vector3 add = new Vector3(0f, 0f, 0f);

        while (true)
        {
            if (checkX)
            {
                if (checkPos)
                {
                    add.x += 1f;
                }
                else
                {
                    add.x -= 1f;
                }
            }
            else
            {
                if (checkPos)
                {
                    add.y += 1f;
                }
                else
                {
                    add.y -= 1f;
                }
            }

            // Is there a wall in this location?
            for (int i = 0; i < walls.Count; i++)
            {
                if ((moveWalls[mwInt] + add) == walls[i])
                {
                    Debug.Log("Wall location(XPOS) : " + walls[i]);
                    return walls[i];
                }
            }
            // Is there a moveWall in this location?
            for (int i = 0; i < moveWalls.Count; i++)
            {
                if ((moveWalls[mwInt] + add) == moveWalls[i])
                {
                    Debug.Log("MoveWall location(XPOS) : " + moveWalls[i]);
                    return moveWalls[i];
                }
            }
        }
    }
};