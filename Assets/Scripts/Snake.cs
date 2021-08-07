using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    private List<SnakeSegment> _segments = new List<SnakeSegment>();
    private SnakeSegment _head;
    public SnakeSegment segmentPrefab;
    public int initialSize = 4;

    private void Awake()
    {
        _head = GetComponent<SnakeSegment>();

        if (_head == null)
        {
            _head = this.gameObject.AddComponent<SnakeSegment>();
            _head.hideFlags = HideFlags.HideInInspector;
        }
    }

    private void Start()
    {
        ResetState();
    }

    private void Update()
    {
        // Only allow turning up or down while moving in the x-axis
        if (_head.direction.x != 0f)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                _head.SetDirection(Vector2.up, Vector2.zero);
            } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                _head.SetDirection(Vector2.down, Vector2.zero);
            }
        }
        // Only allow turning left or right while moving in the y-axis
        else if (_head.direction.y != 0f)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                _head.SetDirection(Vector2.right, Vector2.zero);
            } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                _head.SetDirection(Vector2.left, Vector2.zero);
            }
        }
    }

    private void FixedUpdate()
    {
        // Have each segment follow the one in front of it. We must do this in
        // reverse order so the position is set to the previous position,
        // otherwise they will all be stacked on top of each other.
        for (int i = _segments.Count - 1; i > 0; i--) {
            _segments[i].Follow(_segments[i - 1], i, _segments.Count);
        }

        // Move the snake in the direction it is facing
        // Round the values to ensure it aligns to the grid
        _head.transform.position = new Vector3(
            Mathf.Round(_head.transform.position.x) + _head.direction.x,
            Mathf.Round(_head.transform.position.y) + _head.direction.y);
    }

    public void Grow()
    {
        // Create a new segment and have it follow the last segment
        SnakeSegment segment = Instantiate(this.segmentPrefab);
        segment.Follow(_segments[_segments.Count - 1], _segments.Count, _segments.Count + 1);
        _segments.Add(segment);
    }

    public void ResetState()
    {
        _head.SetDirection(Vector2.right, Vector2.zero);
        _head.transform.position = Vector3.zero;

        // Start at 1 to skip destroying the head
        for (int i = 1; i < _segments.Count; i++) {
            Destroy(_segments[i].gameObject);
        }

        // Clear the list then add the head as the first segment
        _segments.Clear();
        _segments.Add(_head);

        // -1 since the head is already in the list
        for (int i = 0; i < this.initialSize - 1; i++) {
            Grow();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food") {
            Grow();
        } else if (other.tag == "Obstacle") {
            ResetState();
        }
    }

}
