using System.Collections.Generic;
using UnityEngine;
using LDtkUnity;
using Cinemachine;
using General;

namespace World {
    /// <summary>
    /// Loads/unloads levels and controls the camera based on the LevelActivator's position.
    /// </summary>
    public class LevelManager : Singleton<LevelManager>
    {
        public static GameObject go;
        private class LoadedLevel {

            public readonly GameObject gameObject;
            public readonly LDtkComponentLevel level;
            public readonly PolygonCollider2D bounds;
            public readonly GameObject vcamObject;
            public readonly LDtkIid id;

            public bool MarkForUnload = false;

            const int SCREEN_WIDTH_TILES = 48;
            const int SCREEN_HEIGHT_TILES = 27;

            public LoadedLevel(GameObject gameObject, LDtkComponentLevel level, LDtkIid id, PolygonCollider2D bounds) {
                this.gameObject = gameObject;
                this.level = level;
                this.bounds = bounds;
                this.id = id;

                // The bounds needs to be on the LevelBounds layer.
                bounds.gameObject.layer = LayerMask.NameToLayer("LevelBounds");
                bounds.isTrigger = true;
        
                // The children of the level need to be on the Level layer.
                foreach (Transform childTransform in level.gameObject.transform) {
                    childTransform.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Level"));
                }

                // Create a child object to hold the virtual camera.
                vcamObject = new GameObject("VirtualCamera");
                vcamObject.transform.parent = gameObject.transform;

                // Center it on the level.
                vcamObject.transform.position = new Vector3(bounds.bounds.center.x, bounds.bounds.center.y, -10f);
                
                // Deactivate it by default (we will enable it when a LevelActivator enters).
                vcamObject.SetActive(false);

                // Set up the actual vcam component.
                var vcam = vcamObject.AddComponent<CinemachineVirtualCamera>();
                vcam.m_Lens.OrthographicSize = 13.5f;
                if (!useStaticCamera()) {
                    // FIXME
                    //vcam.Follow = LevelActivator.Transform();

                    var confiner = vcamObject.AddComponent<CinemachineConfiner2D>();
                    confiner.m_BoundingShape2D = bounds;
                }
            }

            public void Enter() {
                vcamObject.SetActive(true);
            }

            public void Exit() {
                vcamObject.SetActive(false);
            }

            private bool useStaticCamera() {
                return level.BorderRect.width == SCREEN_WIDTH_TILES && level.BorderRect.height == SCREEN_HEIGHT_TILES;
            }
        }

        public Camera mainCamera;
        public GameObject world;

        private LDtkComponentProject _project;
        private List<LoadedLevel> _loadedLevels = new List<LoadedLevel>();
        private LoadedLevel _currentLevel;
        private LoadedLevel _previousLevel;

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

                var id = child.GetComponent<LDtkIid>();
                _loadedLevels.Add(new LoadedLevel(child, level, id,bounds));
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
                if (level.id.Iid == id.Iid) {
                    return level;
                }
            }

            return null;
        }

        public static void EnterLevel(GameObject enteredLevelObject) {
            Instance.enterLevel(enteredLevelObject);
        }

        public static void ExitLevel(GameObject exitedLevelObject) {
            Instance.exitLevel(exitedLevelObject);
        }

        private void enterLevel(GameObject enteredLevelObject) {
            var enteredLevel = getLoadedLevel(enteredLevelObject);
            if (enteredLevel == null) {
                Debug.LogError($"Cannot enter unloaded level '{enteredLevelObject.name}'.");
                return;
            }

            enterLevel(enteredLevel);
        }

        private void enterLevel(LoadedLevel enteredLevel) {
            if (_currentLevel == enteredLevel || enteredLevel == null) {
                return;
            }

            if (_currentLevel != null) {
                _currentLevel.Exit();
            }

            _previousLevel = _currentLevel;
            _currentLevel = enteredLevel;
            enteredLevel.Enter();

            // PERF: we could do this asynchronously
            loadLevelNeighbours(enteredLevel.level.Identifier);
            garbageCollectLevels(enteredLevel);
        }

        private void exitLevel(GameObject exitedLevelObject) {
            var exitedLevel = getLoadedLevel(exitedLevelObject);
            if (exitedLevel == null) {
                Debug.LogError($"Cannot exit unloaded level '{exitedLevelObject.name}'.");
                return;
            }

            if (_currentLevel == exitedLevel) {
                // Avoids this issue:
                // 1. Player is in level A
                // 2. Enters level B
                // 3. *Does not exit level A* (e.g. by standing on the seam)
                // 4. Exits level B
                // -> what level are we in??
                enterLevel(_previousLevel);
            }
        }

        /// <summary>
        /// Destroys all loaded levels that are not the given level or its neighbours.
        /// </summary>
        private void garbageCollectLevels(LoadedLevel enteredLevel) {
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

        private void loadLevelNeighbours(string levelIdentifier) {
            var levels = _project.FromJson().Levels;

            foreach (var level in levels) {
                if (level.Identifier == levelIdentifier) {
                    foreach (var neighbour in level.Neighbours) {
                        loadLevel(neighbour.Level);
                    }
                    break;
                }
            }
        }

        private void loadLevel(Level level) {
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
            GameObject levelObject = Instantiate(prefab,world.transform); // FIXME: doesnt appear to be adding to the scene
            var levelComponent = levelObject.GetComponent<LDtkComponentLevel>();
            var id = levelObject.GetComponent<LDtkIid>();
            var bounds = levelObject.GetComponent<PolygonCollider2D>();
            if(go is null) go = levelObject;
            _loadedLevels.Add(new LoadedLevel(levelObject, levelComponent,id,bounds));
        }
    }
}
