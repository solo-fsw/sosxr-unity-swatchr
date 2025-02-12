using System;
using SmartWeakEvent;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;


//https://www.codeproject.com/Articles/29922/Weak-Events-in-C#heading0002
//https://gist.github.com/dgrunwald/6445360
namespace swatchr
{
    public class Swatch : ScriptableObject
    {
        public Color[] colors;

        [FormerlySerializedAs("URL")] public string SourceURL;

        [NonSerialized]
        private readonly FastSmartWeakEvent<EventHandler> _event = new();

        [NonSerialized]
        private Texture2D texture = null;
        public int numColors => colors?.Length ?? 0;

        public Texture2D cachedTexture
        {
            get
            {
                if (texture == null)
                {
                    texture = CreateTexture();
                }

                return texture;
            }
        }


        public void RegenerateTexture()
        {
            texture = CreateTexture();
        }


        private Texture2D CreateTexture()
        {
            #if SWATCHR_VERBOSE
			Debug.LogWarning("[Swatch] Creating Texture");
            #endif

            var swatch = this;

            if (swatch.colors != null && swatch.colors.Length > 0)
            {
                var colorTexture = new Texture2D(swatch.colors.Length, 1);
                colorTexture.filterMode = FilterMode.Point;
                colorTexture.SetPixels(swatch.colors);
                colorTexture.Apply();

                return colorTexture;
            }

            return null;
        }


        public event EventHandler OnSwatchChanged
        {
            add => _event.Add(value);
            remove => _event.Remove(value);
        }


        public Color GetColor(int colorIndex)
        {
            if (colors == null || colors.Length <= colorIndex || colorIndex < 0)
            {
                return Color.white;
            }

            return colors[colorIndex];
        }


        public static Swatch FromSwatchASEFile(SwatchASEFile file)
        {
            var swatchScriptableObject = CreateInstance<Swatch>();
            swatchScriptableObject.colors = new Color[file.colors.Count];

            for (var i = 0; i < swatchScriptableObject.colors.Length; i++)
            {
                swatchScriptableObject.colors[i] = new Color(file.colors[i].r, file.colors[i].g, file.colors[i].b);
            }

            return swatchScriptableObject;
        }


        public void AddColorsFromASEFile(SwatchASEFile file)
        {
            var initialLength = colors?.Length ?? 0;
            var fileLength = file.colors?.Count ?? 0;
            Array.Resize(ref colors, initialLength + fileLength);
            var i = initialLength;
            var iterator = file.colors.GetEnumerator();

            while (iterator.MoveNext())
            {
                var fileColor = iterator.Current;
                colors[i++] = new Color(fileColor.r, fileColor.g, fileColor.b);
            }

            SignalChange();
        }


        public void AddColorsFromOtherSwatch(Swatch otherSwatch)
        {
            var initialLength = colors?.Length ?? 0;
            var otherSwatchLength = otherSwatch.colors?.Length ?? 0;
            Array.Resize(ref colors, initialLength + otherSwatchLength);
            var i = initialLength;

            for (var j = 0; j < otherSwatchLength; j++)
            {
                colors[i++] = otherSwatch.colors[j];
            }

            SignalChange();
        }


        public void ReplaceSelfWithOtherSwatch(Swatch otherSwatch)
        {
            if (otherSwatch.colors != null)
            {
                Array.Resize(ref colors, otherSwatch.colors.Length);
                Array.Copy(otherSwatch.colors, colors, otherSwatch.colors.Length);
            }
            else
            {
                Array.Resize(ref colors, 0);
            }

            SignalChange();
        }


        public void SignalChange()
        {
            RegenerateTexture();
            _event.Raise(this, EventArgs.Empty);
            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            #endif
        }
    }
}