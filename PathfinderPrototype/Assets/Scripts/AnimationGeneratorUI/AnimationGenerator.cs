﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using swellanimations;

[AddComponentMenu ("Animation/Animation Generator")]
[System.Serializable]
public class AnimationGenerator : MonoBehaviour
{
		public int widthLines = 100;
		public int heightLines = 100;
		public float cellWidth = 32.0f;
		public float cellHeight = 32.0f;
		public bool drawing = false;

		[SerializeField]
		public Transform model;

		[SerializeField]
		public Dictionary<string, Transform> modelMap = new Dictionary<string, Transform> () { };

		[SerializeField]
		private List<Vector3> points = new List<Vector3> ();

		public Vector3 planeOrigin = new Vector3 ();
		public Vector3 planeVector1 = new Vector3 ();
		public Vector3 planeVector2 = new Vector3 ();

		[SerializeField]
		public Transform planePoint1;
		[SerializeField]
		public Transform planePoint2;
		[SerializeField]
		public Transform planePoint3;
		public Plane editorPlane;

		private Node[] frames;

		private int currentFrame = 0;

		[SerializeField]
		public float timeBetweenFrames = 0.1f;
		private float timeSinceLastFrame = 0.0f;

		private bool animationPlaying = false;
		private float m_LastEditorUpdateTime;

		private Vector3 beginPostion;
		private Quaternion beginRotation;
		private bool drawPlane = true;

		[SerializeField]
		private string serializedAnimation;

		void OnDrawGizmos ()
		{
				DrawGrid ();
				Gizmos.color = Color.blue;
				if (points.Count > 1) {
						for (int x = 1; x < points.Count; x++) {
								Gizmos.DrawLine (points [x - 1], points [x]);
						}
				}
		}


		void DrawGrid ()
		{
				if (drawPlane) {
						calculatePlaneVectors ();
						Vector3 lineStartBase = (planeVector1 * heightLines / 2);
						Gizmos.color = Color.grey;
						for (int x = -widthLines / 2; x < widthLines / 2; x++) {
								Vector3 lineIncrementBase = ((planeVector2 * x) + planeOrigin);
								Vector3 lineStart = lineIncrementBase + lineStartBase;
								Vector3 LineEnd = lineIncrementBase - lineStartBase;
								Gizmos.DrawLine (lineStart, LineEnd);
						}
						lineStartBase = (planeVector2 * widthLines / 2);
						for (int y = -heightLines / 2; y < heightLines / 2; y++) {
								Vector3 lineIncrementBase = ((planeVector1 * y) + planeOrigin);
								Vector3 lineStart = lineIncrementBase + lineStartBase;
								Vector3 LineEnd = lineIncrementBase - lineStartBase;
								Gizmos.DrawLine (lineStart, LineEnd);
						}
				}
		}

		public void addPoint (Vector3 point)
		{
				points.Add (point);
		}

		public void clearPoints ()
		{
				points.Clear ();
		}

		public void calculatePlaneVectors ()
		{
				Vector3 vectorA = planePoint2.position - planePoint1.position;
				Vector3 vectorB = planePoint3.position - planePoint1.position;
				Vector3 normal = Vector3.Cross (vectorA, vectorB);
				Vector3 perpVectorA = Vector3.Cross (vectorA, normal);
				planeOrigin = planePoint1.position;
				planeVector1 = vectorA.normalized;
				planeVector2 = perpVectorA.normalized;
				editorPlane = new Plane (planePoint1.position, planePoint2.position, planePoint3.position);
		}

		public void ToggleAnimation ()
		{
				if (!animationPlaying) {
						currentFrame = 0;
						beginPostion = model.position;
						beginRotation = model.rotation;
						drawPlane = false;
				} else {
						model.position = beginPostion;
						model.rotation = beginRotation;
						drawPlane = true;
				}
				animationPlaying = !animationPlaying;
		}

		public void UpdateAnimation (float deltaTime)
		{
				if (animationPlaying) {
						if (timeSinceLastFrame >= timeBetweenFrames) {
								if (currentFrame < points.Count) {
										AnimateFrame (currentFrame);
										currentFrame++;
								} else {
										ToggleAnimation ();
								}
								timeSinceLastFrame = 0;
						} else {
								timeSinceLastFrame += deltaTime;
						}
				}
		}

		public void FillModelMap (Transform loc)
		{
				modelMap.Add (loc.gameObject.name, loc);
				foreach (Transform t in loc) {
						FillModelMap (t);
				}
		}

		public void GenerateAnimation ()
		{
				if (points != null && points.Count > 0) {
						frames = BackendAdapter.GenerateFromBackend (AnimationData.CreateModelData (model, points));
						serializedAnimation = BackendAdapter.serializeNodeArray (frames);
						Debug.Log ("Just serialized: " + serializedAnimation);
						modelMap.Clear ();
						FillModelMap (model);
				}
		}

		public void SetModelChildren (Node n)
		{
				if (modelMap.ContainsKey (n.name)) {
						Transform t = modelMap [n.name];
						t.localPosition = new Vector3 (
								n.position.x,
								n.position.y,
								n.position.z);
				} else {
						Debug.Log ("oh shit! map doesn't contain " + n.name);
						return;
				}
		}

		public void SetModel (Node n)
		{
				if (modelMap.ContainsKey (n.name)) {
						Transform t = modelMap [n.name];
						t.position = new Vector3 (
								n.position.x,
								n.position.y,
								n.position.z);

						foreach (Node child in n.children) {
								SetModelChildren (child);
						}
				} else {
						Debug.Log ("oh shit! map doesn't contain " + n.name);
						return;
				}
		}

		public void RestoreAnimation ()
		{
				if (frames == null && serializedAnimation != null && points.Count > 0) {
						FillModelMap (model);
						Debug.Log ("Restored using: " + serializedAnimation);
						frames = BackendAdapter.deserializeNodeArray (serializedAnimation);
						Debug.Log ("Restored: " + frames);
				}
		}

		public void AnimateFrame (int frame)
		{
				if (frame >= frames.Length) {
						Debug.Log ("oops you called me too many times. this is bad!");
						return;
				}
				Node node = frames [frame];
				SetModel (node);
		}


}
