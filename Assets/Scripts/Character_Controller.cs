using UnityEngine;

public class Character_Controller : MonoBehaviour {

    [SerializeField] float speed = 8;
    [SerializeField] float jumpForce = 10;

    // private
    Rigidbody2D rb;

    Vector3 initialPos;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();

        initialPos = transform.position;
    }

    private void Update() {
        input();
    }

    private void input() {
        if (Input.GetKey(KeyCode.A)) {
            transform.position = transform.position + new Vector3(-1, 0) * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.position = transform.position + new Vector3(1, 0) * speed * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            rb.AddForce(new Vector2(0, jumpForce));
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            transform.position = initialPos;
        }
    }

}
