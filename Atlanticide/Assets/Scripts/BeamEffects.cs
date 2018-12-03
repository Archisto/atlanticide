using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    public class BeamEffects : MonoBehaviour
    {
        public float offsetXSpeed,
            offsetYSpeed;

        private float _offsetX = 0f,
            _offsetY = 0f;

        public float scaleChangeInterval,
            scaleMultiplier;

        private float scaleChangeCountdown,
            randomScale,
            currentScale = 1f;

        private Material _beamMaterial;

        // Use this for initialization
        void Start()
        {
            _beamMaterial = GetComponent<MeshRenderer>().material;
            scaleChangeCountdown = scaleChangeInterval + Random.Range(-scaleChangeInterval / 2, scaleChangeInterval);
            randomScale = Random.Range(1f, scaleMultiplier);
        }

        // Update is called once per frame
        void Update()
        {
            _offsetX += Time.deltaTime * offsetXSpeed;
            _offsetY += Time.deltaTime * offsetYSpeed;
            _beamMaterial.SetTextureOffset("_MainTex", new Vector2(_offsetX, _offsetY));

            scaleChangeCountdown -= Time.deltaTime;
            if (scaleChangeCountdown <= 0f)
            {
                scaleChangeCountdown = scaleChangeInterval + Random.Range(-scaleChangeInterval / 2, scaleChangeInterval);
                randomScale = Random.Range(1f, scaleMultiplier);
            }

            currentScale = Mathf.Lerp(currentScale, randomScale, scaleChangeCountdown);
            transform.localScale = new Vector3(transform.localScale.x, currentScale, currentScale);
        }
    }
}