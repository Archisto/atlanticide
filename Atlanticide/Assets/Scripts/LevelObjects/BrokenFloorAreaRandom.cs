using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atlanticide
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Activatable))]
    public class BrokenFloorAreaRandom : LevelObject
    {
        [Header("Editor")]

        [SerializeField]
        private bool _Create;

        [SerializeField]
        private bool _Clear;

        [SerializeField]
        private bool _Break;

        [Header("Floor parameters")]

        [SerializeField]
        private Vector2 _Size = new Vector2(1, 1);

        [SerializeField]
        private Vector2 YPosBetween = new Vector2(0, 0);

        [SerializeField]
        private Utils.IntVector2 _Density = new Utils.IntVector2(2, 2);

        [SerializeField]
        private Utils.IntVector2 _MinPlatformSize = new Utils.IntVector2(1, 1);

        [SerializeField]
        private Utils.IntVector2 _MaxPlatformSize = new Utils.IntVector2(2, 2);

        [SerializeField]
        private float _Bedrock = -5;

        [Header("Platform parameters")]

        [SerializeField]
        private GameObject _PlatformPrefab;

        [SerializeField]
        private Vector2 HeightBetween = new Vector2(0.25f, 0.25f);

        private BrokenFloor[] _Platforms;
        private Activatable _activatable;
        private bool _alreadyBroken;

        // Use this for initialization
        void Start()
        {
            CheckVariables();
            Fill();
            _activatable = GetComponent<Activatable>();
        }

        protected override void UpdateObject()
        {
            if (!_alreadyBroken && (_activatable.Activated || _Break))
            {
                _Break = false;
                _alreadyBroken = true;
                foreach(BrokenFloor floor in _Platforms)
                {
                    floor.Break = true;

                    //SFXPlayer.Instance.Play(Sound.Ground_Collapse);
                }
            }

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
            } else
            {
                _Platforms = new BrokenFloor[transform.childCount];
                for (int i = 0; i < _Platforms.Length; i++)
                {
                    BrokenFloor floor = transform.GetChild(i).GetComponent<BrokenFloor>();
                    if(floor == null)
                    {
                        Debug.LogError("Child object number " + i + " has no BrokenFloor component");
                    }
                    AddToList(floor, i, floor.transform.localScale);
                }
            }

            if (!_Create)
            {
                return;
            }

            FillArea();
        }

        private void FillArea()
        {
            FillArray2D.ArrayObject[] objects = new FillArray2D().RandomRectangles2(_Density, _MinPlatformSize, _MaxPlatformSize);
            _Platforms = new BrokenFloor[objects.Length];
            int index = 0;
            float factorX = _Size.x / _Density.x;
            float factorY = _Size.y / _Density.y;
            Vector3 PlatformPosition;
            Vector3 PlatformSize;
            foreach (FillArray2D.ArrayObject ao in objects)
            {
                float width = (ao.endX - ao.startX + 1) * factorX;
                float height = (ao.endY - ao.startY + 1) * factorY;
                PlatformSize = new Vector3(width, Random.Range(HeightBetween.x, HeightBetween.y), height);
                float posX = transform.position.x - _Size.x / 2 + (ao.startX * factorX) + width / 2;
                float posZ = transform.position.z - _Size.y/2 + (ao.startY * factorY) + height/2;
                PlatformPosition = new Vector3(posX, transform.position.y + Random.Range(YPosBetween.x, YPosBetween.y), posZ);
                BrokenFloor floor = Instantiate(_PlatformPrefab, PlatformPosition, _PlatformPrefab.transform.rotation, transform).GetComponent<BrokenFloor>();
                AddToList(floor, index, PlatformSize);
                index++;
            }
        }

        /// <summary>
        /// Adds BrokenFloor to _Platforms list, sets name, scale and bedrock
        /// </summary>
        /// <param name="floor">BrokenFloor</param>
        /// <param name="index">Index of _Platforms[] & Index of name</param>
        /// <param name="localScale">localScale of the floor</param>
        private void AddToList(BrokenFloor floor, int index, Vector3 localScale)
        {
            floor.transform.localScale = localScale;
            floor.Bedrock = _Bedrock;
            floor.name = "BrokenFloor" + index;
            _Platforms[index] = floor;
        }

        private void CheckVariables()
        {
            if (_Density.x <= 0)
            {
                _Density.x = 1;
                Debug.LogWarning("Density.x should be above 0. Set to 1");
            }

            if (_Density.y <= 0)
            {
                _Density.y = 1;
                Debug.LogWarning("Density.y should be above 0. Set to 1");
            }


            if (_MinPlatformSize.x <= 0)
            {
                _MinPlatformSize.x = 1;
                Debug.LogWarning("MinPlatformSize.x should be above 0. Set to 1");
            }
            else if (_MinPlatformSize.x > _Density.x)
            {
                _MinPlatformSize.x = _Density.x;
                Debug.LogWarning("MinPlatformSize.x should be below or equal to Density.x. Set to " + _MinPlatformSize.x);
            }

            if (_MinPlatformSize.y <= 0)
            {
                _MinPlatformSize.y = 1;
                Debug.LogWarning("MinPlatformSize.y should be above 0. Set to 1");
            }
            else if (_MinPlatformSize.y > _Density.y)
            {
                _MinPlatformSize.y = _Density.y;
                Debug.LogWarning("MinPlatformSize.y should be below or equal to Density.y. Set to " + _MinPlatformSize.y);
            }

            if (_MaxPlatformSize.x < _MinPlatformSize.x)
            {
                _MaxPlatformSize.x = _MinPlatformSize.x;
                Debug.LogWarning("MaxPlatformSize.x should be above or equal to MinPlatformSize.x. Set to " + _MaxPlatformSize.x);
            }
            else if (_MaxPlatformSize.x > _Density.x)
            {
                _MaxPlatformSize.x = _Density.x;
                Debug.LogWarning("MaxPlatformSize.x should be below or equal to Density.x. Set to " + _MaxPlatformSize.x);
            }

            if (_MaxPlatformSize.y < _MinPlatformSize.y)
            {
                _MaxPlatformSize.y = _MinPlatformSize.y;
                Debug.LogWarning("MaxPlatformSize.y should be above or equal to MinPlatformSize.y. Set to " + _MaxPlatformSize.y);
            }
            else if (_MaxPlatformSize.y > _Density.y)
            {
                _MaxPlatformSize.y = _Density.y;
                Debug.LogWarning("MaxPlatformSize.y should be below or equal to Density.y. Set to " + _MaxPlatformSize.y);
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