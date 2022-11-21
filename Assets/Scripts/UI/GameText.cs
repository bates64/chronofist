using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using General;
using JetBrains.Annotations;
using UnityEngine;

namespace Ui {
    public class GameText : MonoBehaviour {
        public TextAsset fontXml;
        public Texture2D fontImage;
        public Font font;

        public string text = "Hello, world!";

        private XmlDocument fontXmlDoc;

        private void Start() {
            fontXmlDoc = new XmlDocument();
            fontXmlDoc.LoadXml(fontXml.text);
            CreateSpriteChildren();
        }

        private void CreateSpriteChildren() {
            foreach (Transform child in transform) {
                DestroyImmediate(child.gameObject);
            }

            // For each character in `text`, create a new sprite child
            var x = 0f;
            var y = 0f;

            foreach (char c in text) {
                if (c == ' ') {
                    x += 4f * Util.PIXEL;
                    continue;
                }

                CharacterInfo characterInfo = GetCharacterInfoForChar(c);

                // Create a new game object
                GameObject go = new GameObject();
                go.transform.parent = transform;
                go.transform.localPosition = new Vector3(x + characterInfo.vert.x * Util.PIXEL, y + characterInfo.vert.y * Util.PIXEL , 0f);
                go.name = c.ToString();

                // Create a new sprite renderer
                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = CreateSprite(characterInfo);

                // Update the position
                x += characterInfo.advance * Util.PIXEL;

                // TODO: y
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
