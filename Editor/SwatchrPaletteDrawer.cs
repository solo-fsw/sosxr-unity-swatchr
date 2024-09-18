using System;
using UnityEditor;
using UnityEngine;


namespace swatchr
{
    public class SwatchrPaletteDrawer
    {
        public const int itemsPerRow = 8;

        private static GUIStyle tempDrawTextureStyle;
        private static Texture2D blackTexture;
        private static Texture2D whiteTexture;
        private static Texture2D paletteTexture;
        private static int paletteTextureCachedHashCode;


        public static bool DrawColorPalette(Swatch swatch, ref int colorKey, bool drawNewColorButton)
        {
            if (swatch == null)
            {
                return false;
            }

            var lastRect = GUILayoutUtility.GetLastRect();

            if (swatch.colors != null && swatch.colors.Length > 0)
            {
                var swatchHash = swatch.cachedTexture.GetHashCode();

                if (paletteTexture == null || paletteTextureCachedHashCode != swatchHash)
                {
                    if (paletteTexture == null)
                    {
                        #if SWATCHR_VERBOSE
						Debug.LogWarning("[SwatchrPalleteDrawer] creating pallete texture because there is none");
                        #endif
                    }
                    else
                    {
                        #if SWATCHR_VERBOSE
						Debug.LogWarningFormat("[SwatchrPalleteDrawer] creating pallete texture because cache miss. {0} != {1}", palleteTextureCachedHashCode, swatchHash);
                        #endif
                    }

                    paletteTexture = textureWithColors(swatch.colors);
                    paletteTextureCachedHashCode = swatchHash;
                }
            }
            else
            {
                paletteTexture = null;
            }

            if (blackTexture == null)
            {
                #if SWATCHR_VERBOSE
				Debug.LogWarning("[SwatchrPalleteDrawer] creating black texture");
                #endif
                blackTexture = textureWithColor(Color.black);
            }

            if (whiteTexture == null)
            {
                #if SWATCHR_VERBOSE
				Debug.LogWarning("[SwatchrPalleteDrawer] creating white texture");
                #endif
                whiteTexture = textureWithColor(Color.white);
            }

            var numColors = swatch.colors != null ? swatch.colors.Length : 0;
            var numPerRow = itemsPerRow;
            var numInBottomRow = numColors % numPerRow;

            float heightOfPallete = 0;
            var textureRect = new Rect(lastRect.x, lastRect.y + lastRect.height, 0.0f, 0.0f);

            if (paletteTexture != null)
            {
                textureRect = new Rect(lastRect.x, lastRect.y + lastRect.height, paletteTexture.width * EditorGUIUtility.singleLineHeight, paletteTexture.height * EditorGUIUtility.singleLineHeight);
                heightOfPallete = textureRect.height;
            }

            if (numInBottomRow == 0)
            {
                heightOfPallete += EditorGUIUtility.singleLineHeight;
            }

            var clickRect = textureRect;

            if (swatch.colors == null || swatch.colors.Length == 0)
            {
                clickRect.width = EditorGUIUtility.singleLineHeight;
            }

            clickRect.height = heightOfPallete;

            GUILayoutUtility.GetRect(clickRect.width, clickRect.height);

            if (paletteTexture != null)
            {
                DrawTexture(paletteTexture, textureRect);
                DrawBlackGrid(textureRect.x, textureRect.y, swatch.colors.Length, paletteTexture.width, paletteTexture.height, (int) EditorGUIUtility.singleLineHeight, blackTexture);
            }

            if (drawNewColorButton)
            {
                DrawNewColorButton(numColors, textureRect);
            }

            var somethingHasChanged = false;

            if (IsClick())
            {
                if (IsClickInRect(clickRect))
                {
                    var e = Event.current;
                    var rectClickPosition = e.mousePosition - textureRect.position;
                    var cellXIndex = (int) (rectClickPosition.x / EditorGUIUtility.singleLineHeight);
                    var cellYIndex = (int) (rectClickPosition.y / EditorGUIUtility.singleLineHeight);
                    var textureWidth = paletteTexture != null ? paletteTexture.width : 0;
                    var clickedOnKey = cellYIndex * textureWidth + cellXIndex;

                    if (numColors > 0 && clickedOnKey < numColors)
                    {
                        colorKey = clickedOnKey;
                        somethingHasChanged = true;
                    }
                    else if (clickedOnKey == numColors)
                    {
                        colorKey = clickedOnKey;
                        Array.Resize(ref swatch.colors, numColors + 1);
                        swatch.colors[colorKey] = Color.white;
                        swatch.SignalChange();
                        somethingHasChanged = true;
                    }
                }
            }

            if (swatch.colors != null && swatch.colors.Length > 0)
            {
                DrawOnSelectedCell(colorKey, textureRect);
                var selectedColorRow = colorKey / itemsPerRow;
                var selectedColorY = selectedColorRow * EditorGUIUtility.singleLineHeight + EditorGUIUtility.singleLineHeight;
                var colorKeyRect = new Rect(lastRect.x + itemsPerRow * EditorGUIUtility.singleLineHeight, lastRect.y + selectedColorY, 64, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(colorKeyRect, colorKey.ToString());
            }

            return somethingHasChanged;
        }


        public static bool DrawDeleteButton(int x, int y)
        {
            var slh = (int) EditorGUIUtility.singleLineHeight;
            var slhHalf = (int) (slh * 0.5f);

            if (blackTexture == null)
            {
                blackTexture = textureWithColor(Color.black);
            }

            DrawBlackGrid(x, y, 1, 1, 1, slh, blackTexture);
            DrawBlackGrid(x + 1, y + 1, 1, 1, 1, slh - 2, whiteTexture);

            var minusLength = 7;
            var halfMinusLength = 3;

            var minusRect = new Rect(x + slhHalf - halfMinusLength, y + slhHalf, minusLength, 1);
            DrawTexture(blackTexture, minusRect);
            var clickRect = new Rect(x, y, slh, slh);

            if (IsClick())
            {
                if (IsClickInRect(clickRect))
                {
                    return true;
                }
            }

            return false;
        }


        public static Rect GetNewColorButtonRect(Swatch swatch)
        {
            var numColors = swatch.colors.Length;
            var totalRows = Mathf.CeilToInt(numColors / (float) itemsPerRow);
            var numInBottomRow = numColors % itemsPerRow;
            var r = new Rect();
            r.x = (int) (numInBottomRow * EditorGUIUtility.singleLineHeight);
            r.y = (int) (totalRows * EditorGUIUtility.singleLineHeight);
            r.width = EditorGUIUtility.singleLineHeight;
            r.height = EditorGUIUtility.singleLineHeight;

            return r;
        }


        private static void DrawBlackGrid(float startingPointX, float startingPointY, int numColors, int cellsX, int cellsY, int cellSize, Texture2D colorTexture)
        {
            if (cellsX == 0 && cellsY == 0)
            {
                return;
            }

            // draw vertical lines
            var currentRect = new Rect(startingPointX, startingPointY, 1, cellSize * cellsY);
            var fullHeight = cellSize * cellsY + 1; // +1 to get the corners
            var oneLessHeight = cellSize * (cellsY - 1) + 1;

            // oneLessHeight will be 1 if theres only one row
            if (cellsY == 1)
            {
                oneLessHeight = 0;
            }

            var numInBottomRow = numColors % cellsX;

            for (var i = 0; i <= cellsX; i++)
            {
                // height will be 1 unit shorter if bottom cell does not exist
                currentRect.x = startingPointX + cellSize * i;
                var bottomCellExists = numInBottomRow == 0 || i <= numInBottomRow;
                currentRect.height = bottomCellExists ? fullHeight : oneLessHeight;
                DrawTexture(colorTexture, currentRect);
            }

            // draw horizontal lines
            currentRect.x = startingPointX;
            currentRect.height = 1;
            currentRect.width = cellSize * cellsX;

            for (var i = 0; i <= cellsY; i++)
            {
                currentRect.y = startingPointY + cellSize * i;

                if ((i == cellsY || cellsY == 1) && numInBottomRow > 0)
                {
                    currentRect.width = numInBottomRow * cellSize;
                }

                DrawTexture(colorTexture, currentRect);
            }
        }


        private static void DrawOnSelectedCell(int selectedCell, Rect textureRect)
        {
            var selectedCellY = selectedCell / itemsPerRow;
            var selectedCellX = selectedCell - itemsPerRow * selectedCellY;
            var smallBlackRect = new Rect(textureRect.x + selectedCellX * EditorGUIUtility.singleLineHeight, textureRect.y + selectedCellY * EditorGUIUtility.singleLineHeight, 10f, 10f);
            DrawBlackGrid(smallBlackRect.x - 1, smallBlackRect.y - 1, 1, 1, 1, (int) EditorGUIUtility.singleLineHeight + 2, blackTexture);
            DrawBlackGrid(smallBlackRect.x, smallBlackRect.y, 1, 1, 1, (int) EditorGUIUtility.singleLineHeight, whiteTexture);
        }


        private static void DrawNewColorButton(int selectedCell, Rect textureRect)
        {
            var selectedCellY = selectedCell / itemsPerRow;
            var selectedCellX = selectedCell - itemsPerRow * selectedCellY;
            var smallBlackRect = new Rect(textureRect.x + selectedCellX * EditorGUIUtility.singleLineHeight, textureRect.y + selectedCellY * EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
            DrawBlackGrid(smallBlackRect.x, smallBlackRect.y, 1, 1, 1, (int) EditorGUIUtility.singleLineHeight, blackTexture);
            DrawBlackGrid(smallBlackRect.x + 1, smallBlackRect.y + 1, 1, 1, 1, (int) (EditorGUIUtility.singleLineHeight - 2), whiteTexture);

            var plusLength = 7;
            var halfPlusLength = 3.0f;

            var centerX = smallBlackRect.x + smallBlackRect.width * 0.5f;
            var centerY = smallBlackRect.y + smallBlackRect.height * 0.5f;
            var plusVerticalRect = new Rect(centerX, centerY - halfPlusLength, 1, plusLength);
            var plusHorizontalRect = new Rect(centerX - halfPlusLength, centerY, plusLength, 1);
            DrawTexture(blackTexture, plusVerticalRect);
            DrawTexture(blackTexture, plusHorizontalRect);
        }


        private static void DrawTexture(Texture2D texture, Rect rect)
        {
            if (tempDrawTextureStyle == null)
            {
                tempDrawTextureStyle = new GUIStyle();
            }

            tempDrawTextureStyle.normal.background = texture;
            EditorGUI.LabelField(rect, "", tempDrawTextureStyle);
        }


        private static bool IsClick()
        {
            var e = Event.current;

            return e != null && e.type == EventType.MouseDown && e.button == 0;
        }


        private static bool IsClickInRect(Rect rect)
        {
            var e = Event.current;

            return e != null && e.type == EventType.MouseDown && e.button == 0 && rect.Contains(e.mousePosition);
        }


        private static Texture2D textureWithColor(Color color)
        {
            var tex = new Texture2D(1, 1, TextureFormat.RGB24, false, true);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.hideFlags = HideFlags.HideAndDontSave;
            tex.SetPixel(0, 0, color);
            tex.Apply();

            return tex;
        }


        private static Texture2D textureWithColors(Color[] colors)
        {
            if (colors == null || colors.Length == 0)
            {
                return new Texture2D(0, 0, TextureFormat.RGBA32, false, true);
            }

            // figure out our texture size based on the itemsPerRow and color count
            var totalRows = Mathf.CeilToInt(colors.Length / (float) itemsPerRow);
            var tex = new Texture2D(itemsPerRow, totalRows, TextureFormat.RGBA32, false, true);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.hideFlags = HideFlags.HideAndDontSave;
            var x = 0;
            var y = 0;

            for (var i = 0; i < colors.Length; i++)
            {
                x = i % itemsPerRow;
                y = totalRows - 1 - Mathf.CeilToInt(i / itemsPerRow);
                tex.SetPixel(x, y, colors[i]);
            }

            for (x++; x < tex.width; x++)
            {
                tex.SetPixel(x, y, Color.clear);
            }

            tex.Apply();

            return tex;
        }
    }
}