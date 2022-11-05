using System.Collections.Generic;
using UnityEngine;
using LDtkUnity;
using General;

namespace World {
    /// <summary>
    /// Loads/unloads levels and controls the camera based on the LevelActivator's position.
    /// </summary>
    public class LevelManager : Singleton<LevelManager> {
        private class LoadedLevel {
            public readonly GameObject gameObject;
            public readonly LDtkComponentLevel level;
            public readonly PolygonCollider2D bounds;

            public bool MarkForUnload = false;

            public LoadedLevel(GameObject gameObject, LDtkComponentLevel level, PolygonCollider2D bounds) {
                this.gameObject = gameObject;
                this.level = level;
                this.bounds = bounds;

                // The bounds needs to be on the LevelBounds layer.
                bounds.gameObject.layer = LayerMask.NameToLayer("LevelBounds");
                bounds.isTrigger = true;

                // The children of the level need to be on the Level layer.
                foreach (Transform childTransform in level.gameObject.transform) {
                    childTransform.gameObject.layer = LayerMask.NameToLayer("Level");
                }
            }
        }

        public Camera mainCamera;
        public GameObject world;

        private List<LoadedLevel> loadedLevels = new List<LoadedLevel>();

        protected override void init() {
            if (world.GetComponent<LDtkComponentProject>() == null) {
                Debug.LogError("World must be an LDtk project!");
                return;
            }

            // For each child of the world, turn it into a LoadedLevel.
            foreach (Transform childTransform in world.transform) {
                var child = childTransform.gameObject;

                var level = child.GetComponent<LDtkComponentLevel>();
                if (level == null) {
                    Debug.LogError($"World child '{child.name}' is not an LDtk level.");
                    continue;
                }

                var bounds = child.GetComponent<PolygonCollider2D>();
                if (bounds == null) {
                    Debug.LogError($"Level '{child.name}' is missing a PolygonCollider2D - ensure 'Use Composite Collider' is enabled on the World asset.");
                }

                loadedLevels.Add(new LoadedLevel(child, level, bounds));
            }
        }

        private LoadedLevel getLoadedLevel(GameObject gameObject) {
            foreach (var level in loadedLevels) {
                if (level.gameObject == gameObject) {
                    return level;
                }
            }

            return null;
        }

        private LoadedLevel getLoadedLevel(LDtkIid id) {
            foreach (var level in loadedLevels) {
                if (level.level.Identifier == id.Iid) {
                    return level;
                }
            }

            return null;
        }

        public static void EnterLevel(GameObject enteredLevelObject) {
            var enteredLevel = Instance.getLoadedLevel(enteredLevelObject);
            if (enteredLevel == null) {
                Debug.LogError($"Cannot enter unloaded level '{enteredLevelObject.name}'.");
                return;
            }

            // Load the level's neighbours.
            // TODO: how?? need to get the resource via json data as level.Neighbours will be null[]

            // PERF: queue for later
            Instance.GarbageCollectLevels(enteredLevel);
        }

        /// <summary>
        /// Destroys all loaded levels that are not the given level or its neighbours.
        /// </summary>
        private void GarbageCollectLevels(LoadedLevel enteredLevel) {
            // We are going to perform mark-and-sweep garbage collection.

            // 1. Mark all levels for unload.
            foreach (var level in Instance.loadedLevels) {
                level.MarkForUnload = true;
            }

            // 2. Unmark `enteredLevel`.
            enteredLevel.MarkForUnload = false;

            // 3. Unmark the neighbours of `enteredLevel`.
            foreach (var id in enteredLevel.level.Neighbours) {
                if (id == null) {
                    Debug.LogError($"Level '{enteredLevel.level.Identifier}' has unloaded neighbour(s). Aborting garbage collection.");
                    return;
                }
    
                var neighbour = Instance.getLoadedLevel(id);
                if (neighbour != null) {
                    neighbour.MarkForUnload = false;
                } else {
                    // What the fuck?
                    Debug.LogError($"Level '{enteredLevel.level.Identifier}' has a neighbour '{id.Iid}' that is in the scene but the LevelActivator doesn't know about it.");
                }
            }

            // 4. Unload all levels that are still marked.
            foreach (var level in Instance.loadedLevels) {
                if (level.MarkForUnload) {
                    Destroy(level.gameObject);
                }
            }

            // 5. Remove all levels we unloaded from the list of loaded levels.
            Instance.loadedLevels.RemoveAll(level => level.MarkForUnload);
        }
    }
}
