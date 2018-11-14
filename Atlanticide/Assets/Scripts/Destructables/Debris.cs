using UnityEngine;
using System.Collections;

namespace Atlanticide
{
    [RequireComponent(typeof(Material))]
    public class Debris : MonoBehaviour
    {
        public float destroyDelay;
        private float _destroyCountup;
        private Material _material;
        private Color _materialOriginalColor,
            _materialCurrentColor;
        public float fadeOutStart;

        // Use this for initialization
        void Start()
        {
            _destroyCountup = 0f;
            _material = GetComponent<Renderer>().material;
            _materialOriginalColor = GetComponent<Renderer>().material.color;
            _materialCurrentColor = _materialOriginalColor;
        }

        // Update is called once per frame
        void Update()
        {
            _destroyCountup += Time.deltaTime;
            if (_destroyCountup >= fadeOutStart)
            {
                float t = (_destroyCountup - fadeOutStart) / (destroyDelay - fadeOutStart);
                _materialCurrentColor.a = Mathf.Lerp(1f, 0f, t);
                GetComponent<Renderer>().material.color = _materialCurrentColor;
            }
            if (_destroyCountup >= destroyDelay)
            {
                GetComponent<Renderer>().material.color = _materialOriginalColor; // Reset the material's color before sending the Debris back to it's pool.
                gameObject.SetActive(false);
            }
        }
    }
}