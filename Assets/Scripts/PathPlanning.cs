using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class PathPlanning : MonoBehaviour
{
    [SerializeField]
    private DogInterface dog_interface;
    private NavMeshPath path;
    private const float ang_vel = .2f; 
    private const float lin_vel = .2f; 
    private const float threshold = 0.1f; 

    private void Start()
    {
        transform.rotation = transform.parent.rotation;
        Application.targetFrameRate = 20;
        path = new NavMeshPath();
        StartCoroutine(FollowPath());
        StartCoroutine(VisualizePath());
    }

    private IEnumerator FollowPath()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("NavTarget");
        foreach (GameObject target in targets)
        {
            NavMesh.CalculatePath(transform.position, target.transform.position, NavMesh.AllAreas, path);
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                yield return StartCoroutine(GoToNextPoint(path.corners[i], path.corners[i+1]));
            }
            yield return new WaitForSeconds(3f); 
        }
    }

    private IEnumerator GoToNextPoint(Vector3 start, Vector3 end)
    {
        yield return StartCoroutine(RotateTowards(start, end));
        yield return StartCoroutine(MoveTowards  (start, end));
    }

    private IEnumerator RotateTowards(Vector3 start, Vector3 end)
    {
        Vector3 directionToTarget = (end - start).normalized;
        float targetAngleDegrees = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, targetAngleDegrees, 0);
        float difference = (transform.parent.rotation * Quaternion.Inverse(targetRotation)).eulerAngles.y;
        Debug.Log(difference);
        while (Quaternion.Angle(transform.parent.rotation, targetRotation) > threshold)
        {
            transform.parent.rotation = Quaternion.RotateTowards(transform.parent.rotation, targetRotation, ang_vel * Mathf.Rad2Deg * Time.deltaTime);
            /* SEND TO DOG */
            if (difference > 180)
                dog_interface.SendAngularVelocity(-ang_vel);
            else
                dog_interface.SendAngularVelocity(ang_vel);
            /* SEND TO DOG */
            yield return null;
        }
        dog_interface.SendAngularVelocity(0);
        transform.parent.rotation = targetRotation;
    }

    private IEnumerator MoveTowards(Vector3 start, Vector3 end)
    {
        float distance = Vector3.Distance(start, end);
        while (Vector3.Distance(transform.position, end) > threshold)
        {
            transform.position = Vector3.MoveTowards(transform.position, end, lin_vel * Time.deltaTime);
            /* SEND TO DOG */
            dog_interface.SendLinearVelocity(lin_vel);
            /* SEND TO DOG */
            yield return null;
        }
        dog_interface.SendLinearVelocity(0);
        transform.position = end;
    }

    private IEnumerator VisualizePath()
    {
        while (true)
        {
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red, 0.1f);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
