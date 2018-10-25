using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    [ExecuteInEditMode]
    public class BrokenFloorAreaStandard : LevelObject
    {
        [Header("Editor")]

        [SerializeField]
        private bool _Create;

        [SerializeField]
        private bool _Clear;

        [Header("Floor parameters")]

        [SerializeField]
        private Vector2 _Size;

        [SerializeField]
        private int _Columns = 1;

        [SerializeField]
        private int _Rows = 1;

        [SerializeField]
        private float _Bedrock = -5;      

        [Header("Platform parameters")]

        [SerializeField]
        private GameObject _PlatformPrefab;

        [SerializeField]
        private Vector2 HeightBetween = new Vector2(0.25f, 0.25f);

        [SerializeField]
        private Vector2 YPosBetween = new Vector2(0, 0);

        private BrokenFloor[] _Platforms;

        // Use this for initialization
        void Start()
        {
            Fill();
        }

        protected override void UpdateObject()
        {
            base.UpdateObject();
            
        }

        public override void ResetObject()
        {
            base.ResetObject();
        }

        private void Fill()
        {
            if (_Clear)
            {
                int childs = transform.childCount;
                for (int i = childs - 1; i > -1; i--)
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }
                _Platforms = new BrokenFloor[0];
            }

            if (!_Create)
            {
                return;
            }

            FillArea();
        }

        private void FillArea()
        {
            Vector3 PlatformSize = new Vector3(_Size.x / _Rows, 0, _Size.y / _Columns);
            Vector3 PlatformPosition;
            _Platforms = new BrokenFloor[_Columns * _Rows];
            int index = 0;
            for (int i = 0; i < _Columns; i++)
            {
                for (int j = 0; j < _Rows; j++)
                {
                    PlatformSize.Set(PlatformSize.x, Random.Range(HeightBetween.x, HeightBetween.y), PlatformSize.z);
                    float posX = transform.position.x - _Size.x / 2 + PlatformSize.x / 2 + PlatformSize.x * j;
                    float posZ = transform.position.z - _Size.y / 2 + PlatformSize.z / 2 + PlatformSize.z * i;
                    PlatformPosition = new Vector3(posX, transform.position.y + Random.Range(YPosBetween.x, YPosBetween.y), posZ);
                    _Platforms[index] = Instantiate(_PlatformPrefab, PlatformPosition, _PlatformPrefab.transform.rotation, transform).GetComponent<BrokenFloor>();
                    _Platforms[index].transform.localScale = PlatformSize * 0.99f;
                    _Platforms[index].Bedrock = _Bedrock;
                    _Platforms[index].name = "BrokenFloor" + index;
                    index++;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, new Vector3(_Size.x, 0, _Size.y));
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position + transform.TransformDirection(Vector3.up) * _Bedrock, new Vector3(_Size.x, 0, _Size.y));
        }

    }
}