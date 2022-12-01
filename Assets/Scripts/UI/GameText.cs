using System;
using System.Collections.Generic;
using General;
using UnityEngine;

namespace Ui {
    [ExecuteAlways]
    [SelectionBase]
    public class GameText : MonoBehaviour {
        public Texture2D fontImage;
        public Font font;
        public int orderInLayer;

        [SerializeField] [TextArea] string text = "GameText";
        public string Text {
            get => text;
            set {
                if (value == text) {
                    return;
                }

                text = value;
                UpdateChildren();
            }
        }

        [SerializeField] [Range(-1, 60)] private int numberOfCharsToRender = -1;
        public int NumberOfCharsToRender {
            get => numberOfCharsToRender;
            set {
                if (value == numberOfCharsToRender) {
                    return;
                }

                numberOfCharsToRender = value;
                UpdateChildren();
            }
        }

        private readonly List<GameObject> childObjects = new List<GameObject>();

        private void Start() {
            foreach (Transform child in transform) {
                childObjects.Add(child.gameObject);
            }

            UpdateChildren();
        }

        [ContextMenu("UpdateChildren")]
        private void UpdateChildren() {
            DisableChildren();

            if (font == null || fontImage == null) {
                return;
            }

            // For each character in `Text`, create a new sprite child
            var x = 0f;
            var y = 0f;

            var index = 0;
            var prevChar = ' ';

            foreach (var c in Text) {
                if (index >= numberOfCharsToRender && numberOfCharsToRender != -1) {
                    break;
                }

                switch (c) {
                    case ' ':
                        x += 4f * Util.PIXEL;
                        index += 1;
                        continue;
                    case '\n':
                        y -= 16f * Util.PIXEL;
                        x = 0f;
                        index += 1;
                        continue;
                    case 'o':
                    case 'a':
                    case 'e':
                        if (prevChar == 'T')
                            x -= 2f * Util.PIXEL;
                        break;
                }

                CharacterInfo characterInfo;
                try {
                    characterInfo = GetCharacterInfoForChar(c);
                } catch (NoGlyphException e) {
                    Debug.LogError(e);
                    characterInfo = GetCharacterInfoForChar('?');
                }

                // Create a new game object
                var go = GetOrCreateChildObject(index);
                go.transform.parent = transform;
                go.transform.localPosition = new Vector3(x + characterInfo.minX * Util.PIXEL, y + characterInfo.maxY * Util.PIXEL , 0f);
                go.name = c.ToString();

                // Create a new sprite renderer
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                if (sr == null) {
                    sr = go.AddComponent<SpriteRenderer>();
                }
                sr.sprite = CreateSprite(characterInfo);
                sr.sortingLayerName = "UI";
                sr.sortingOrder = orderInLayer;

                // Update the position
                x += characterInfo.advance * Util.PIXEL;

                index += 1;
                prevChar = c;
            }
        }

        private void DisableChildren() {
            foreach (var child in childObjects) {
                if (child != null) {
                    child.SetActive(false);

                    if (!Application.isPlaying)
                        DestroyImmediate(child);
                }
            }
        }

        private GameObject GetOrCreateChildObject(int index) {
            // source of visual bugs
            /*if (index < childObjects.Count) {
                var child = childObjects[index];
                child.SetActive(true);
                return child;
            }*/

            var go = new GameObject();
            childObjects.Add(go);
            return go;
        }

        private class NoGlyphException : Exception {
            public NoGlyphException(string message) : base(message) { }
        }

        private CharacterInfo GetCharacterInfoForChar(char c) {
            // Search for character ASCII code as index in font.characterInfo array
            int index = c;

            for (var i = 0; i < font.characterInfo.Length; i++) {
                if (font.characterInfo[i].index == index) {
                    return font.characterInfo[i];
                }
            }

            throw new NoGlyphException("Character info not found for char '" + c + "', is it in the font?");
        }

        private Sprite CreateSprite(CharacterInfo characterInfo) {
            // Make a Sprite from the character info using the font image
            // TODO(perf): cache me
            return Sprite.Create(
                fontImage,
                new Rect(
                    characterInfo.uvTopLeft.x * fontImage.width,
                    characterInfo.uvTopLeft.y * fontImage.height,
                    (characterInfo.uvBottomRight.x * fontImage.width) - (characterInfo.uvTopLeft.x * fontImage.width),
                    (characterInfo.uvBottomRight.y * fontImage.height) - (characterInfo.uvTopLeft.y * fontImage.height)
                ),
                new Vector2(0f, 0f),
                8f
            );
        }
    }
}
