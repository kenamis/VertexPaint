using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Written by Kenamis
namespace JBooth.VertexPainterPro
{
    [CreateAssetMenu(menuName = "Vertex Painter Brush/Bitmask Brush", fileName = "bitmask_brush")]
    public class BitmaskCustomBrush : VertexPainterCustomBrush
    {
        [System.Serializable]
        public class BrushData
        {
            public Channels targetChannel;
            public bool write_X;
            public int bitmask_X;

            public bool write_Y;
            public int bitmask_Y;

            public bool write_Z;
            public int bitmask_Z;

            public bool write_W;
            public int bitmask_W;
        }
        public BrushData brushData = new BrushData();

        //might not want to use 32 bits in a shader, only 23 bits according to IEEE for significand?
        private string[] displayOptions = new string[23] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
            "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22","23" }; //,"24","25","26","27","28","29","30","31","32" };
        private const int maxIntValue = 8388607;

        public override void DrawGUI()
        {
            brushData.targetChannel = (Channels)EditorGUILayout.EnumPopup("Channel", brushData.targetChannel);

            EditorGUILayout.BeginHorizontal();
            brushData.write_X = EditorGUILayout.Toggle(GUIContent.none, brushData.write_X, GUILayout.MaxWidth(20));
            EditorGUI.BeginDisabledGroup(!brushData.write_X);
            brushData.bitmask_X = EditorGUILayout.MaskField("Bitmask X", brushData.bitmask_X, displayOptions);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            brushData.bitmask_X = DrawRange(brushData.bitmask_X, 0, 12);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            brushData.bitmask_X = DrawRange(brushData.bitmask_X, 12, 23);
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.BeginHorizontal();
            brushData.write_Y = EditorGUILayout.Toggle(GUIContent.none, brushData.write_Y, GUILayout.MaxWidth(20));
            EditorGUI.BeginDisabledGroup(!brushData.write_Y);
            brushData.bitmask_Y = EditorGUILayout.MaskField("Bitmask Y", brushData.bitmask_Y, displayOptions);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            brushData.bitmask_Y = DrawRange(brushData.bitmask_Y, 0, 12);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            brushData.bitmask_Y = DrawRange(brushData.bitmask_Y, 12, 23);
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.BeginHorizontal();
            brushData.write_Z = EditorGUILayout.Toggle(GUIContent.none, brushData.write_Z, GUILayout.MaxWidth(20));
            EditorGUI.BeginDisabledGroup(!brushData.write_Z);
            brushData.bitmask_Z = EditorGUILayout.MaskField("Bitmask Z", brushData.bitmask_Z, displayOptions);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            brushData.bitmask_Z = DrawRange(brushData.bitmask_Z, 0, 12);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            brushData.bitmask_Z = DrawRange(brushData.bitmask_Z, 12, 23);
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.BeginHorizontal();
            brushData.write_W = EditorGUILayout.Toggle(GUIContent.none, brushData.write_W, GUILayout.MaxWidth(20));
            EditorGUI.BeginDisabledGroup(!brushData.write_W);
            brushData.bitmask_W = EditorGUILayout.MaskField("Bitmask W", brushData.bitmask_W, displayOptions);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            brushData.bitmask_W = DrawRange(brushData.bitmask_W, 0, 12);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            brushData.bitmask_W = DrawRange(brushData.bitmask_W, 12, 23);
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }

        private int DrawRange(int bitmask, int startIndex, int endIndex)
        {
            for (int bit = startIndex; bit < endIndex; bit++)
            {
                int thisBitmask = (1 << bit);
                if(GUILayout.Toggle(((bitmask & thisBitmask) != 0), displayOptions[bit], "Button", GUILayout.Width(25)))
                {
                    bitmask = bitmask | thisBitmask;
                }
                else
                {
                    bitmask = bitmask & ~thisBitmask;
                }
            }

            if(bitmask > maxIntValue || bitmask == -1)
            {
                bitmask = maxIntValue;
            }

            return bitmask;
        }

        void LerpFunc(PaintJob j, int idx, ref object val, float r)
        {
            BrushData bd = val as BrushData;

            Vector4 writeBitmask = new Vector4();
            if (bd.write_X) { writeBitmask.x = bd.bitmask_X; }
            if (bd.write_Y) { writeBitmask.y = bd.bitmask_Y; }
            if (bd.write_Z) { writeBitmask.z = bd.bitmask_Z; }
            if (bd.write_W) { writeBitmask.w = bd.bitmask_W; }

            switch (bd.targetChannel)
            {
                case Channels.Colors:
                case Channels.Normals:
                case Channels.Positions:
                    break;
                case Channels.UV0:
                    j.stream.uv0[idx] = writeBitmask;
                    break;
                case Channels.UV1:
                    j.stream.uv1[idx] = writeBitmask;
                    break;
                case Channels.UV2:
                    j.stream.uv2[idx] = writeBitmask;
                    break;
                case Channels.UV3:
                    j.stream.uv3[idx] = writeBitmask;
                    break;
                default:
                    break;                        
            }
        }

        public override Channels GetChannels()
        {
            return brushData.targetChannel;
        }

        public override VertexPainterWindow.Lerper GetLerper()
        {
            return LerpFunc;
        }

        public override object GetBrushObject()
        {
            return brushData;
        }
    }
}
