using System;
using General;
using UnityEngine;

namespace Ui {
    [ExecuteAlways]
    [SelectionBase]
    public class GameText : MonoBehaviour {
        public Texture2D fontImage;
        public Font font;

        [SerializeField] [TextArea] string _text = "GameText";
        public string Text {
            get => _text;
            set {
                if (value == _text) {
                    return;
                }

                _text = value;
                CreateSpriteChildren();
            }
        }

        private void Start() {
            CreateSpriteChildren();
        }

        [ContextMenu("CreateSpriteChildren")]
        private void CreateSpriteChildren() {
            // PERF: could make this more efficient by updating existing children if they exist
            DestroySpriteChildren();

            if (font == null || fontImage == null) {
                return;
            }

            // For each character in `Text`, create a new sprite child
            var x = 0f;
            var y = 0f;

            foreach (var c in Text) {
                switch (c) {
                    case ' ':
                        x += 4f * Util.PIXEL;
                        continue;
                    case '\n':
                        y += 16f * Util.PIXEL;
                        x = 0f;
                        continue;
                }

                CharacterInfo characterInfo;
                try {
                    characterInfo = GetCharacterInfoForChar(c);
                } catch (NoGlyphException e) {
                    Debug.LogError(e);
                    characterInfo = GetCharacterInfoForChar('?');
                }

                // Create a new game object
                GameObject go = new GameObject();
                go.transform.parent = transform;
                go.transform.localPosition = new Vector3(x + characterInfo.minX * Util.PIXEL, y + characterInfo.maxY * Util.PIXEL , 0f);
                go.name = c.ToString();

                // Create a new sprite renderer
                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = CreateSprite(characterInfo);

                // Update the position
                x += characterInfo.advance * Util.PIXEL;
            }
        }

        [ContextMenu("DestroySpriteChildren")]
        private void DestroySpriteChildren() {
            foreach (Transform child in transform) {
                child.gameObject.SetActive(false);
                DestroyImmediate(child.gameObject);
            }
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
