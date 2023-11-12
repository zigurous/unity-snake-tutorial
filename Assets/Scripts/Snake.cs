using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    public SnakeSegment segmentPrefab;
    public float speed = 20f;
    public float speedMultiplier = 1f;
    public int initialSize = 4;
    public bool moveThroughWalls = false;

    private readonly List<SnakeSegment> segments = new List<SnakeSegment>();
    private SnakeSegment head;
    private Vector2Int input;
    private float nextUpdate;

    private void Awake()
    {
        if (!TryGetComponent(out head))
        {
            head = gameObject.AddComponent<SnakeSegment>();
            head.hideFlags = HideFlags.HideInInspector;
        }
    }

    private void Start()
    {
        ResetState();
    }

    private void Update()
    {
        // Only allow turning up or down while moving in the x-axis
        if (head.direction.x != 0f)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                input = Vector2Int.up;
            } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                input = Vector2Int.down;
            }
        }
        // Only allow turning left or right while moving in the y-axis
        else if (head.direction.y != 0f)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                input = Vector2Int.right;
            } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                input = Vector2Int.left;
            }
        }
    }

    private void FixedUpdate()
    {
        // Wait until the next update before proceeding
        if (Time.time < nextUpdate) {
            return;
        }

        // Set the new direction based on the input
        if (input != Vector2Int.zero) {
            head.SetDirection(input, Vector2Int.zero);
        }

        // Set each segment's position to be the same as the one it follows. We
        // must do this in reverse order so the position is set to the previous
        // position, otherwise they will all be stacked on top of each other.
        for (int i = segments.Count - 1; i > 0; i--) {
            segments[i].Follow(segments[i - 1], i, segments.Count);
        }

        // Move the snake in the direction it is facing
        // Round the values to ensure it aligns to the grid
        int x = Mathf.RoundToInt(head.transform.position.x) + head.direction.x;
        int y = Mathf.RoundToInt(head.transform.position.y) + head.direction.y;
        head.transform.position = new Vector2(x, y);

        // Set the next update time based on the speed
        nextUpdate = Time.time + (1f / (speed * speedMultiplier));
    }

    public void Grow()
    {
        SnakeSegment segment = Instantiate(segmentPrefab);
        segment.Follow(segments[segments.Count - 1], segments.Count, segments.Count + 1);
        segments.Add(segment);
    }

    public void ResetState()
    {
        head.SetDirection(Vector2Int.right, Vector2Int.zero);
        head.transform.position = Vector3.zero;

        // Start at 1 to skip destroying the head
        for (int i = 1; i < segments.Count; i++) {
            Destroy(segments[i].gameObject);
        }

        // Clear the list but add back this as the head
        segments.Clear();
        segments.Add(head);

        // -1 since the head is already in the list
        for (int i = 0; i < initialSize - 1; i++) {
            Grow();
        }
    }

    public bool Occupies(int x, int y)
    {
        foreach (SnakeSegment segment in segments)
        {
            if (Mathf.RoundToInt(segment.transform.position.x) == x &&
                Mathf.RoundToInt(segment.transform.position.y) == y) {
                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            Grow();
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            ResetState();
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            if (moveThroughWalls) {
                Traverse(other.transform);
            } else {
                ResetState();
            }
        }
    }

    private void Traverse(Transform wall)
    {
        Vector3 position = transform.position;

        if (head.direction.x != 0f) {
            position.x = Mathf.RoundToInt(-wall.position.x + head.direction.x);
        } else if (head.direction.y != 0f) {
            position.y = Mathf.RoundToInt(-wall.position.y + head.direction.y);
        }

        transform.position = position;
    }

}
