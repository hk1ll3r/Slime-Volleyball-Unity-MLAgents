using UnityEngine;

public class SlimeColliderGenerator : MonoBehaviour {
    void Start() {
        GenerateCollider();
    }

    public void GenerateCollider(int c = 20) {
        float r = 0.75f;
        float delta = 180 / (c - 1);
        Vector2[] path = new Vector2[c];
        for (int i = 0; i < c - 1; i++) {
            float f = i * Mathf.PI / (c - 1);
            path[i] = new Vector2(r * Mathf.Cos(f), r * Mathf.Sin(f));
        }
        path[c - 1] = new Vector2(r * -1, 0);

        PolygonCollider2D pc = GetComponent<PolygonCollider2D>();
        pc.pathCount = 1;
        pc.SetPath(0, path);
    }
}