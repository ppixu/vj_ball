using UnityEngine;
using System.Collections;

public class UVAnimation : MonoBehaviour {
    private float scrollSpeed = .5f;
    private Renderer rend;
    void Start() {
        rend = GetComponent<Renderer>();
    }
    void Update() {
        float offset = 0.01f * Mathf.Sin(Time.time * scrollSpeed);
        rend.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }
}