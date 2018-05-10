namespace Apex.Steering.Components
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Apex.Common;
	using Apex.PathFinding;
	using Apex.Services;
	using Apex.Units;
	using UnityEngine;

	public class PathFinder : MonoBehaviour, INeedPath {
		private readonly object _syncLock = new object();
		private UnitComponent _unit;
		private PathResult _latestResult;
		private Path _currentPath;
		private int _nextNode;

		public float radius {
			get { return _unit.radius; }
		}

		public AttributeMask attributes {
			get { return _unit.attributes; }
		}

		public void ConsumePathResult(PathResult result) {
			_latestResult = result;
		}

		private void ProcessLatestResult() {
			if (_latestResult == null) {
				return;
			}

			// Create lock to keep this atomic
			PathResult result;
			lock (_syncLock) {
				result = _latestResult;
				_latestResult = null;
			}

			// Need to add statements to handle failures and partial completes.
			switch (result.status) {
				case PathingStatus.Complete: {
					break;
				}
				default: {
					// Handle failures
					return;
				}
			}

			_currentPath = result.path;
			for (int i = 0; i < _currentPath.count; i++) {
				var node = _currentPath.PeekFront (i);
				// Do something with each node
			}
		}
		void OnDrawGizmos() {
			if (_currentPath != null) {
				for (int i = 0; i < _currentPath.count; i++) {
					var node = _currentPath.PeekFront (i);

					Gizmos.color = Color.yellow;
					Gizmos.DrawSphere (node.position, 1);
					//GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
					//cube.transform.position = node.position;
					// Do something with each node
				}
			}
		}
		private void Update() {
			ProcessLatestResult ();
		}

		// Housekeeping
		private void Awake() {
			_unit = GetComponent<UnitComponent> ();
		}

		public void RequestPath(Vector3 PathTo) {
			IUnitFacade unit = this.GetUnitFacade();

			BasicPathRequest _pendingPathRequest = new BasicPathRequest
			{
				from = this.transform.position,
				to = PathTo,
				requester = this,
				requesterProperties = unit,
				pathFinderOptions = unit.pathFinderOptions,
				timeStamp = Time.time
			};

			GameServices.pathService.QueueRequest (_pendingPathRequest, unit.pathFinderOptions.pathingPriority);
		}

		// Sets the next node
		public void NextNode() {
			_nextNode++;
		}

		public Vector3 GetNextNode() {
			if (_currentPath != null && _currentPath.count >= _nextNode)
				return _currentPath.PeekBack ((int) _currentPath.count - 2).position;
			else
				return transform.position;
		}
	}
}