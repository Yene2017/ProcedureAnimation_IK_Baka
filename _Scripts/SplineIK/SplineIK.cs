using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SplineIK
{
    public enum BoneAxis
    {
        X, Y, Z, IX, IY, IZ,
    }

    [System.Serializable]
    public class Bone
    {
        public Transform target;
        public BoneAxis axis;
        public Vector3 animPos;
        public Vector3 vector;
        public Vector3 tangent;
        public float sqrLength;

        internal void DrawScene()
        {
#pragma warning disable 0618
            Arrow(animPos, animPos + vector, Color.yellow);
            Arrow(animPos + vector, animPos + vector + tangent, Color.white);
            Arrow(animPos, animPos + tangent.normalized * Mathf.Sqrt(sqrLength), Color.green);
#pragma warning restore 0618
        }

        internal static void Arrow(Vector3 p1, Vector3 p2, Color c)
        {
            var dis = Vector3.Distance(p1, p2);
            if (dis < 0.01f)
                return;
            Handles.color = c;
            Handles.ArrowHandleCap(0, p1,
                Quaternion.LookRotation(p2 - p1, Vector3.up),
                dis, EventType.Repaint);
            Handles.color = Color.white;
        }
    }

    public class SplineIK : MonoBehaviour
    {
        [SerializeField] public float _blendHead;
        [SerializeField] public float _blendTail;

        [SerializeField] public Transform _startBone;
        [SerializeField] public Transform _endBone;
        [SerializeField] public BoneAxis _axis;
        [SerializeField] public Bone[] _splineNodes;

        private int _length;
        private bool _cached;
        private bool _postAnim;
        private Vector3 headVector;

        private void OnEnable()
        {
            _length = _splineNodes.Length;
            UnityEditor.SceneView.duringSceneGui += OnScene;
        }

        private void OnDisable()
        {
            UnityEditor.SceneView.duringSceneGui -= OnScene;
        }

        public void Cache(Vector3 vector)
        {
            headVector = vector.normalized;
            _cached = true;
        }

        private void LateUpdate()
        {
            _postAnim = true;
            var pPos = _startBone.position;
            if (_cached)
            {
                _cached = false;
                for (var i = _length - 1; i > -1; i--)
                {
                    var weight = Mathf.Lerp(_blendTail, _blendHead, (float)i / _length) * Time.deltaTime;
                    var node = _splineNodes[i];
                    if (i == 0)
                    {
                        node.vector = Vector3.Lerp(node.vector, headVector, weight);
                    }
                    else
                    {
                        var pNode = _splineNodes[i - 1];
                        pPos = pNode.target.position;
                        node.vector = Vector3.Lerp(node.vector, pNode.vector, weight);
                    }
                    node.animPos = node.target.position;
                }
            }
            for (var i = _length - 1; i > -1; i--)
            {
                var node = _splineNodes[i];
                node.sqrLength = node.target.localPosition.sqrMagnitude;
                if (i == 0)
                {
                    node.tangent = headVector - node.vector;
                }
                else
                {
                    var pNode = _splineNodes[i - 1];
                    node.tangent = pNode.vector - node.vector;
                }
            }
            for (var i = _length - 1; i > -1; i--)
            {
                var node = _splineNodes[i];
                if (Vector3.SqrMagnitude(node.tangent) > 0.01f)
                {
                    node.target.position = node.animPos + node.tangent.normalized * Mathf.Sqrt(node.sqrLength);
                    node.target.rotation = Quaternion.FromToRotation(Axis(node.target, node.axis), node.tangent.normalized) * node.target.rotation;
                }
            }
        }

        private Vector3 Axis(Transform t, BoneAxis axis)
        {
            switch(axis)
            {
                case BoneAxis.X:
                    return t.right;
                case BoneAxis.Y:
                    return t.up;
                case BoneAxis.Z:
                    return t.forward;
                case BoneAxis.IX:
                    return -t.right;
                case BoneAxis.IY:
                    return -t.up;
                case BoneAxis.IZ:
                    return -t.forward;
            }
            return t.forward;
        }

        [ContextMenu("Generate Spline Nodes")]
        void GenerateSplineNodes()
        {
            var node = _startBone;
            var list = new List<Bone>();
            do
            {
                node = node.parent;
                var bone = new Bone()
                {
                    target = node,
                    axis = _axis,
                };
                list.Add(bone);
                if (node == _endBone)
                {
                    break;
                }
            }
            while (node);
            _splineNodes = list.ToArray();
        }

        private void OnScene(SceneView sceneView)
        {
            foreach (var node in _splineNodes)
            {
                node.DrawScene();
            }
            Bone.Arrow(_startBone.position, _startBone.position + headVector, Color.red);
            //{
            //    var node = _splineNodes[0];
            //    Bone.Arrow(node.animPos, node.animPos + node.vector, Color.yellow);
            //    Bone.Arrow(node.animPos + node.vector, node.animPos + node.vector + node.tangent, Color.white);
            //    Bone.Arrow(node.animPos, node.animPos + node.tangent.normalized * Mathf.Sqrt(node.sqrLength), Color.green);
            //}

        }
    }


}
