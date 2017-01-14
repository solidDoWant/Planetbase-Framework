using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PlanetbaseFramework
{
    public static class ImportOBJ
    {
        public static GameObject createGameObject(Mesh mesh)
        {
            GameObject newObject = new GameObject("testObject");
            newObject.AddComponent<MeshFilter>();
            newObject.AddComponent<MeshRenderer>();
            newObject.GetComponent<MeshFilter>().mesh = mesh;

            newObject.GetComponent<MeshRenderer>().material.mainTexture = Utils.LoadPNGFromFile(@"pathtopngfile.png");

            newObject.GetComponent<MeshRenderer>().enabled = true;

            return newObject;
        }

        public static Mesh import(string FilePath)
        {
            Mesh mesh = new Mesh();

            List<ObjData> directives = new List<ObjData>();
            using (FileStream file = new FileStream(FilePath, FileMode.Open))
            using (StreamReader reader = new StreamReader(file, Encoding.UTF8))
            {
                file.Seek(0, SeekOrigin.Begin);

                while (!reader.EndOfStream)
                {
                    //Turn all the lines into parsed ObjData objects
                    string line = reader.ReadLine();
                    line = line.Trim();
                    if(line.Length > 0)
                    {
                        directives.Add(ObjData.parseLine(line));

                    }
                }
            }

            List<int> triangleVerticies = new List<int>();
            List<int> triangleTextures = new List<int>();
            List<int> triangleNormals = new List<int>();
            List<Vector3> verticies = new List<Vector3>();
            Vector2[] uvVerticies;

            foreach (ObjData directive in directives)
            {
                if (directive.GetType().Compare(typeof(ObjData.ObjFace)))
                {
                    ObjData.ObjFace casted = (ObjData.ObjFace)directive;

                    triangleVerticies.AddRange(computeTriangles(casted.vIndices));

                    if (casted.tIndices != null)
                    {
                        triangleTextures.AddRange(computeTriangles(casted.tIndices));
                    }

                    if (casted.nIndices != null)
                    {
                        triangleNormals.AddRange(computeTriangles(casted.nIndices));
                    }
                } else if (directive.GetType().Compare(typeof(ObjData.ObjVertex)))
                {
                    verticies.Add(((ObjData.ObjVertex)directive).Vector);
                }
            }

            uvVerticies = new Vector2[verticies.Count];
            for(int i = 0; i < uvVerticies.Length; i++)
            {
                uvVerticies[i] = new Vector2(1.0f, 1.0f);
            }

            mesh.vertices = verticies.ToArray();
            mesh.triangles = triangleVerticies.ToArray();

            mesh.uv = uvVerticies;

            mesh.RecalculateBounds();
            mesh.Optimize();

            return mesh;
        }

        private static int[] computeTriangles(int[] vertexArray)
        {
            int[] triangles = new int[3 * (vertexArray.Length - 2)];
            for (int i = 0; i < vertexArray.Length - 2; i++) //There are vIndicies.Length - 2 triangles required for an arbitrary polygon to be drawn
            {
                for(int j = 0; j < 3; j++)
                {
                    triangles[3 * i + j] = vertexArray[0 + j] - 1;
                }
                //triangles[3 * i + 0] = vertexArray[0 + 0];
                //triangles[3 * i + 1] = vertexArray[i + 1];
                //triangles[3 * i + 2] = vertexArray[i + 2];
                //triangles.AddRange(new int[] { vertexArray[0], vertexArray[i + 1], vertexArray[i + 2] });
            }

            return triangles;
        }

        public abstract class ObjData
        {
            public ObjData(ref string line)
            {
                string[] lines = line.Split(' ');
                line = String.Empty;
                for(int i = 1; i < lines.Length; i++)
                {
                    line += lines[i] + ' ';
                }

                line = line.Trim();
            }

            public static ObjData parseLine(string line)
            {
                line = line.Trim();
                if (line.Length > 0)
                {
                    switch (line[0])
                    {
                        case '#':
                            return new ObjComment(line);
                        case 'o':
                            return new ObjObject(line);
                        case 'v':
                            switch (line[1])
                            {
                                case ' ':
                                    return new ObjVertex(line);
                                case 't':
                                    return new ObjVertexTexture(line);
                                case 'n':
                                    return new ObjVertexNormal(line);
                                case 'p':
                                    return new ObjParameterSpaceVertices(line);
                                default:
                                    Debug.Log("Unrecognized vertex directive, line value: " + line);
                                    break;
                            }
                            break;
                        case 'g':
                            return new ObjGroup(line);
                        case 's':
                            return new ObjSmoothShading(line);
                        case 'f':
                            return new ObjFace(line);
                        case 'm':
                            switch (line[1])
                            {
                                case 't':
                                    return new ObjMaterialLibrary(line);
                                default:
                                    Debug.Log("Unrecognized 'm' directive, line value: " + line);
                                    break;
                            }
                            break;
                        case 'u':
                            switch (line[1])
                            {
                                case 's':
                                    return new ObjUseMaterial(line);
                                default:
                                    Debug.Log("Unrecognized 'u' directive, line value: " + line);
                                    break;
                            }
                            break;
                        default:
                            Debug.Log("Unrecongnized directive, line value: " + line);
                            break;
                    }
                }

                return null;
            }

            public class ObjComment : ObjData
            {
                public ObjComment(string RawLine) : base(ref RawLine)
                {
                    Comment = RawLine;
                }

                public string Comment { get; private set; }
            }

            public class ObjVertex : ObjVector
            {
                public ObjVertex(string RawLine) : base(ref RawLine)
                {
                }
            }

            public class ObjParameterSpaceVertices : ObjVector
            {
                public ObjParameterSpaceVertices(string RawLine) : base(ref RawLine)
                {
                }
            }

            public class ObjVector : ObjData
            {
                public ObjVector(ref string RawLine) : base(ref RawLine)
                {
                    string[] splitString = RawLine.Split(' ');

                    Vector = new Vector3(float.Parse(splitString[0]), float.Parse(splitString[1]), float.Parse(splitString[2]));
                }

                public Vector3 Vector { get; private set; }
            }

            public class ObjFace : ObjData
            {
                public int[] vIndices { get; set; }
                public int[] tIndices { get; set; } = null;
                public int[] nIndices { get; set; } = null;

                public ObjFace(string line) : base(ref line)
                {
                    string[] elements = line.Split(' ');    //Split the line up. Each element should be similar to "v1/vt1/vn1"

                    vIndices = new int[elements.Length];  //The face directive always requires vertex indices

                    string[] testElement = elements[0].Split('/');

                    switch (testElement.Length)
                    {
                        case 1: //The elements only contain vertex indicies
                            for (int i = 0; i < elements.Length; i++)
                            {
                                vIndices[i] = int.Parse(elements[i]);
                            }
                            break;
                        case 2: //The elements only contain vertex indicies and texture indicies
                            tIndices = new int[elements.Length];
                            for (int i = 0; i < elements.Length; i++)
                            {
                                string[] splitElement = elements[i].Split('/');
                                vIndices[i] = int.Parse(splitElement[0]);
                                tIndices[i] = int.Parse(splitElement[1]);
                            }
                            break;
                        case 3://The elements contain vertex indicies, normal indicies, and/or texture indicies
                            if (testElement[1] == "")   //The elements don't contain texture indicies
                            {
                                nIndices = new int[elements.Length];
                                for (int i = 0; i < elements.Length; i++)
                                {
                                    string[] splitElement = elements[i].Split('/');
                                    vIndices[i] = int.Parse(splitElement[0]);
                                    nIndices[i] = int.Parse(splitElement[2]);
                                }
                            }
                            else   //The elements contain all three values
                            {
                                tIndices = new int[elements.Length];
                                nIndices = new int[elements.Length];
                                for (int i = 0; i < elements.Length; i++)
                                {
                                    string[] splitElement = elements[i].Split('/');
                                    vIndices[i] = int.Parse(splitElement[0]);
                                    tIndices[i] = int.Parse(splitElement[1]);
                                    nIndices[i] = int.Parse(splitElement[2]);
                                }
                            }
                            break;
                        default:
                            Debug.Log("OBJ line has more than two '/', unable to read");
                            break;
                    }
                }
            }

            public class ObjMaterialLibrary : ObjData
            {
                public ObjMaterialLibrary(string line) : base(ref line)
                {
                    MTL = line;
                }

                public string MTL { get; set; }
            }

            public class ObjUseMaterial : ObjData
            {
                public ObjUseMaterial(string line) : base(ref line)
                {
                    MTL = line;
                }

                public string MTL { get; set; }
            }

            public class ObjObject : ObjData
            {
                public ObjObject(string line) : base(ref line)
                {
                    ObjectName = line;
                }

                public string ObjectName { get; set; }
            }

            public class ObjGroup : ObjData
            {
                public ObjGroup(string line) : base(ref line)
                {
                    GroupName = line;
                }

                public string GroupName { get; set; }
            }

            public class ObjSmoothShading : ObjData
            {
                public ObjSmoothShading(string line) : base(ref line)
                {
                    if (line.Contains("off") || line.Contains("0"))
                    {
                        SmoothShading = false;
                    }
                    else if(line.Contains("on") || line.Contains("1"))
                    {
                        SmoothShading = true;
                    }
                    else
                    {
                        Debug.Log("Smooth shading directive's value unrecognized. Line: " + line);
                    }
                }

                public bool SmoothShading { get; set; }
            }

            public class ObjVertexNormal : ObjVector
            {
                public ObjVertexNormal(string RawLine) : base(ref RawLine)
                {
                }
            }

            public class ObjVertexTexture : ObjVector
            {
                public ObjVertexTexture(string RawLine) : base(ref RawLine)
                {
                }
            }
        }
    }
}
