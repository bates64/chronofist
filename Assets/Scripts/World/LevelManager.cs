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

        private LDtkComponentProject _project;
        private List<LoadedLevel> _loadedLevels = new List<LoadedLevel>();

        protected override void init() {
            _project = world.GetComponent<LDtkComponentProject>();
            if (_project == null) {
                Debug.LogError("World must be an LDtk project!");
                return;
            }
            LDtkIidBank.CacheIidData(_project.FromJson()); // Needed for LoadLevelNeighbours

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

                _loadedLevels.Add(new LoadedLevel(child, level, bounds));
            }
        }

        private LoadedLevel getLoadedLevel(GameObject gameObject) {
            foreach (var level in _loadedLevels) {
                if (level.gameObject == gameObject) {
                    return level;
                }
            }

            return null;
        }

        private LoadedLevel getLoadedLevel(LDtkIid id) {
            foreach (var level in _loadedLevels) {
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

            Instance.LoadLevelNeighbours(enteredLevel.level.Identifier);

            // PERF: queue for later
            Instance.GarbageCollectLevels(enteredLevel);
        }

        /// <summary>
        /// Destroys all loaded levels that are not the given level or its neighbours.
        /// </summary>
        private void GarbageCollectLevels(LoadedLevel enteredLevel) {
            // We are going to perform mark-and-sweep garbage collection.

            // 1. Mark all levels for unload.
            foreach (var level in Instance._loadedLevels) {
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
    
                var neighbour = getLoadedLevel(id);
                if (neighbour != null) {
                    neighbour.MarkForUnload = false;
                } else {
                    // What the fuck?
                    Debug.LogError($"Level '{enteredLevel.level.Identifier}' has a neighbour '{id.Iid}' that is in the scene but the LevelManager doesn't know about it.");
                }
            }

            // 4. Unload all levels that are still marked.
            foreach (var level in _loadedLevels) {
                if (level.MarkForUnload) {
                    Debug.Log($"Unloading level: {level.level.Identifier}");
                    Destroy(level.gameObject);
                    // TODO: Resources.UnloadAsset
                }
            }

            // 5. Remove all levels we unloaded from the list of loaded levels.
            _loadedLevels.RemoveAll(level => level.MarkForUnload);
        }

        private void LoadLevelNeighbours(string levelIdentifier) {
            var levels = _project.FromJson().Levels;

            foreach (var level in levels) {
                if (level.Identifier == levelIdentifier) {
                    foreach (var neighbour in level.Neighbours) {
                        LoadLevel(neighbour.Level);
                    }
                    break;
                }
            }
        }

        private void LoadLevel(Level level) {
            // Check if the level is already loaded.
            foreach (var loadedLevel in _loadedLevels) {
                if (loadedLevel.level.Identifier == level.Identifier) {
                    return;
                }
            }

            // Load the level.
            // PERF: consider LoadAsync
            Debug.Log($"Loading level: {level.Identifier}");
            var path = level.ExternalRelPath.Replace(".ldtkl", "");
            var prefab = Resources.Load<GameObject>(path);

            if (prefab == null) {
                Debug.LogError($"Failed to load resource: {path}");
                return;
            }
    
            var levelObject = Instantiate(prefab, world.transform); // FIXME: doesnt appear to be adding to the scene
            var levelComponent = levelObject.GetComponent<LDtkComponentLevel>();
            var bounds = levelObject.GetComponent<PolygonCollider2D>();

            _loadedLevels.Add(new LoadedLevel(levelObject, levelComponent, bounds));
        }
    }
}
