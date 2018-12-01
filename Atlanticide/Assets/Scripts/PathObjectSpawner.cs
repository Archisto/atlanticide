using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atlanticide.WaypointSystem;

namespace Atlanticide
{
    public class PathObjectSpawner : MonoBehaviour
    {
        //[SerializeField]
        //private Path _path;

        [SerializeField]
        private int _objectCount;

        [SerializeField]
        private GameObject _objectPrefab;

        [SerializeField]
        private Transform _startPoint;

        [SerializeField]
        private Transform _endPoint;

        [SerializeField]
        private Vector3 _rotation1;

        [SerializeField]
        private Vector3 _rotation2;

        [SerializeField]
        private bool _drawGizmos = true;

        private List<GameObject> _gameObjects;
        private List<Vector3> _positions = new List<Vector3>();

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _gameObjects = new List<GameObject>(_objectCount);
            CreateObjectsOnLine();

            //if (_path != null)
            //{
            //    CreateObjectsOnPath();
            //}
            //else
            //{
            //    Debug.LogError(Utils.GetFieldNullString("Path"));
            //}
        }

        private void PopulatePositionList()
        {
            _positions.Clear();

            if (_objectCount <= 0)
            {
                return;
            }
            else if (_objectCount == 1)
            {
                _positions.Add(Vector3.Lerp(_startPoint.position, _endPoint.position, 0.5f));
                return;
            }
            else
            {
                _positions.Add(_startPoint.position);

                if (_objectCount >= 3)
                {
                    int remainderDistDivider = _objectCount - 1;
                    float ratioPart = 1f / remainderDistDivider;
                    for (int i = 1; i < remainderDistDivider; i++)
                    {
                        float ratio = i * ratioPart;
                        Vector3 position = Vector3.Lerp(_startPoint.position, _endPoint.position, ratio);
                        _positions.Add(position);
                    }
                }

                _positions.Add(_endPoint.position);
            }
        }

        private void CreateObjectsOnLine()
        {
            if (_objectCount <= 0)
            {
                return;
            }
            else
            {
                PopulatePositionList();
                foreach (Vector3 position in _positions)
                {
                    SpawnObject(position);
                }

                // Adds LevelObjects to GameManager's LevelObject array
                if (_gameObjects.Count > 0 && _gameObjects[0].GetComponent<LevelObject>() != null)
                {
                    LevelObject[] levelObjects = new LevelObject[_gameObjects.Count];
                    for (int i = 0; i < levelObjects.Length; i++)
                    {
                        levelObjects[i] = _gameObjects[i].GetComponent<LevelObject>();
                    }

                    GameManager.Instance.AddLevelObjectsToArray(levelObjects);
                }
            }
        }

        //private void CreateObjectsOnPath()
        //{
        //    for (int i = 0; i < _objectCount; i++)
        //    {
        //        _path.GetPointAt(float ratio);
        //    }
        //}

        private void SpawnObject(Vector3 position)
        {
            GameObject newObject = Instantiate(_objectPrefab, position,
                GetRandomRotation(), transform);
            _gameObjects.Add(newObject);
        }

        private Quaternion GetRandomRotation()
        {
            Vector3 rotation = new Vector3(
                Random.Range(_rotation1.x, _rotation2.x),
                Random.Range(_rotation1.y, _rotation2.y),
                Random.Range(_rotation1.z, _rotation2.z));

            return Quaternion.Euler(rotation);
        }

        private void OnValidate()
        {
            PopulatePositionList();
        }

        private void OnDrawGizmos()
        {
            if (_drawGizmos)
            {
                Gizmos.color = (_positions.Count > 0 ? Color.green : Color.red);
                Gizmos.DrawLine(_startPoint.position, _endPoint.position);

                if (_positions.Count > 0)
                {
                    float markLength = 0.5f;
                    foreach (Vector3 position in _positions)
                    {
                        Gizmos.DrawLine(position - Vector3.up * markLength / 2,
                                        position + Vector3.up * markLength / 2);
                    }
                }
            }
        }
    }
}
