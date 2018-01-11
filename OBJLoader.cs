/*
(C) 2015 AARO4130
DO NOT USE PARTS OF, OR THE ENTIRE SCRIPT, AND CLAIM AS YOUR OWN WORK
*/

//Credit for this entire file, less a few minor changes, goes to AARO4130 (https://forum.unity.com/threads/free-runtime-obj-loader.365884/)

using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
namespace PlanetbaseFramework
{
    public class ObjLoader
    {
        public static bool SplitByMaterial = false;
        public static string[] SearchPaths = { "", "%FileName%_Textures" + Path.DirectorySeparatorChar };
        //structures
        struct ObjFace
        {
            public string MaterialName;
            public string MeshName;
            public int[] Indexes;
        }

        public static Vector3 ParseVectorFromCMPS(string[] cmps)
        {
            float x = float.Parse(cmps[1]);
            float y = float.Parse(cmps[2]);
            if (cmps.Length == 4)
            {
                float z = float.Parse(cmps[3]);
                return new Vector3(x, y, z);
            }
            return new Vector2(x, y);
        }
        public static Color ParseColorFromCMPS(string[] cmps, float scalar = 1.0f)
        {
            float Kr = float.Parse(cmps[1]) * scalar;
            float Kg = float.Parse(cmps[2]) * scalar;
            float Kb = float.Parse(cmps[3]) * scalar;
            return new Color(Kr, Kg, Kb);
        }

        public static string OBJGetFilePath(string path, string basePath, string fileName)
        {
            foreach (string sp in SearchPaths)
            {
                string s = sp.Replace("%FileName%", fileName);
                if (File.Exists(basePath + s + path))
                {
                    return basePath + s + path;
                }
                else if (File.Exists(path))
                {
                    return path;
                }
            }

            return null;
        }
        public static Material[] LoadMTLFile(string fn, List<Texture2D> loadedTextures)
        {
            Material currentMaterial = null;
            List<Material> matlList = new List<Material>();
            FileInfo mtlFileInfo = new FileInfo(fn);
            string baseFileName = Path.GetFileNameWithoutExtension(fn);
            string mtlFileDirectory = mtlFileInfo.Directory.FullName + Path.DirectorySeparatorChar;
            foreach (string ln in File.ReadAllLines(fn))
            {
                string l = ln.Trim().Replace("  ", " ");
                string[] cmps = l.Split(' ');
                string data = l.Remove(0, l.IndexOf(' ') + 1);

                if (cmps[0] == "newmtl")
                {
                    if (currentMaterial != null)
                    {
                        matlList.Add(currentMaterial);
                    }
                    currentMaterial = new Material(Shader.Find("Standard"));
                    currentMaterial.name = data;
                }
                else if (cmps[0] == "Kd")
                {
                    currentMaterial.SetColor("_Color", ParseColorFromCMPS(cmps));
                }
                else if (cmps[0] == "map_Kd")
                {
                    //TEXTURE
                    Texture2D texture = loadedTextures.FindObjectByFilepath(data);
                    if (texture != null)
                    {
                        currentMaterial.SetTexture("_MainTex", texture);
                    }
                    else
                    {
                        Debug.Log("Could not load texture \"" + Path.GetFileName(data) + "\" for material \"" + fn);
                    }
                }
                else if (cmps[0] == "map_Bump")
                {
                    Texture2D texture = loadedTextures.FindObjectByFilepath(data);
                    if (texture != null)
                    {
                        currentMaterial.SetTexture("_MainTex", texture);
                        currentMaterial.EnableKeyword("_NORMALMAP");

                    }
                    else
                    {
                        Debug.Log("Could not load texture \"" + Path.GetFileName(data) + "\" for material \"" + fn);
                    }
                }
                else if (cmps[0] == "Ks")
                {
                    currentMaterial.SetColor("_SpecColor", ParseColorFromCMPS(cmps));
                }
                else if (cmps[0] == "Ka")
                {
                    currentMaterial.SetColor("_EmissionColor", ParseColorFromCMPS(cmps, 0.05f));
                    currentMaterial.EnableKeyword("_EMISSION");
                }
                else if (cmps[0] == "d")
                {
                    float visibility = float.Parse(cmps[1]);
                    if (visibility < 1)
                    {
                        Color temp = currentMaterial.color;

                        temp.a = visibility;
                        currentMaterial.SetColor("_Color", temp);

                        //TRANSPARENCY ENABLER
                        currentMaterial.SetFloat("_Mode", 3);
                        currentMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        currentMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        currentMaterial.SetInt("_ZWrite", 0);
                        currentMaterial.DisableKeyword("_ALPHATEST_ON");
                        currentMaterial.EnableKeyword("_ALPHABLEND_ON");
                        currentMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        currentMaterial.renderQueue = 3000;
                    }

                }
                else if (cmps[0] == "Ns")
                {
                    float Ns = float.Parse(cmps[1]);
                    Ns = (Ns / 1000);
                    currentMaterial.SetFloat("_Glossiness", Ns);

                }
            }
            if (currentMaterial != null)
            {
                matlList.Add(currentMaterial);
            }
            return matlList.ToArray();
        }

        public static GameObject LoadOBJFile(string FilePath, List<Texture2D> LoadedTextures)
        {

            string meshName = Path.GetFileNameWithoutExtension(FilePath);

            bool hasNormals = false;
            //OBJ LISTS
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            //UMESH LISTS
            List<Vector3> uvertices = new List<Vector3>();
            List<Vector3> unormals = new List<Vector3>();
            List<Vector2> uuvs = new List<Vector2>();
            //MESH CONSTRUCTION
            List<string> materialNames = new List<string>();
            List<string> objectNames = new List<string>();
            Dictionary<string, int> hashtable = new Dictionary<string, int>();
            List<ObjFace> faceList = new List<ObjFace>();
            string cmaterial = "";
            string cmesh = "default";
            //CACHE
            Material[] materialCache = null;
            //save this info for later
            FileInfo OBJFileInfo = new FileInfo(FilePath);

            foreach (string ln in File.ReadAllLines(FilePath))
            {
                if (ln.Length > 0 && ln[0] != '#')
                {
                    string l = ln.Trim().Replace("  ", " ");
                    string[] cmps = l.Split(' ');
                    string data = l.Remove(0, l.IndexOf(' ') + 1);

                    if (cmps[0] == "mtllib")
                    {
                        //load cache
                        string pth = OBJGetFilePath(data, OBJFileInfo.Directory.FullName + Path.DirectorySeparatorChar, meshName);
                        if (pth != null)
                            materialCache = LoadMTLFile(pth, LoadedTextures);

                    }
                    else if ((cmps[0] == "g" || cmps[0] == "o") && SplitByMaterial == false)
                    {
                        cmesh = data;
                        if (!objectNames.Contains(cmesh))
                        {
                            objectNames.Add(cmesh);
                        }
                    }
                    else if (cmps[0] == "usemtl")
                    {
                        cmaterial = data;
                        if (!materialNames.Contains(cmaterial))
                        {
                            materialNames.Add(cmaterial);
                        }

                        if (SplitByMaterial)
                        {
                            if (!objectNames.Contains(cmaterial))
                            {
                                objectNames.Add(cmaterial);
                            }
                        }
                    }
                    else if (cmps[0] == "v")
                    {
                        //VERTEX
                        vertices.Add(ParseVectorFromCMPS(cmps));
                    }
                    else if (cmps[0] == "vn")
                    {
                        //VERTEX NORMAL
                        normals.Add(ParseVectorFromCMPS(cmps));
                    }
                    else if (cmps[0] == "vt")
                    {
                        //VERTEX UV
                        uvs.Add(ParseVectorFromCMPS(cmps));
                    }
                    else if (cmps[0] == "f")
                    {
                        int[] indexes = new int[cmps.Length - 1];
                        for (int i = 1; i < cmps.Length; i++)
                        {
                            string felement = cmps[i];
                            int vertexIndex = -1;
                            int normalIndex = -1;
                            int uvIndex = -1;
                            if (felement.Contains("//"))
                            {
                                //doubleslash, no UVS.
                                string[] elementComps = felement.Split('/');
                                vertexIndex = int.Parse(elementComps[0]) - 1;
                                normalIndex = int.Parse(elementComps[2]) - 1;
                            }
                            else if (felement.Split('/').Length == 3)
                            {
                                //contains everything
                                string[] elementComps = felement.Split('/');
                                vertexIndex = int.Parse(elementComps[0]) - 1;
                                uvIndex = int.Parse(elementComps[1]) - 1;
                                normalIndex = int.Parse(elementComps[2]) - 1;
                            }
                            else if (!felement.Contains("/"))
                            {
                                //just vertex inedx
                                vertexIndex = int.Parse(felement) - 1;
                            }
                            else
                            {
                                //vertex and uv
                                string[] elementComps = felement.Split('/');
                                vertexIndex = int.Parse(elementComps[0]) - 1;
                                uvIndex = int.Parse(elementComps[1]) - 1;
                            }
                            string hashEntry = vertexIndex + "|" + normalIndex + "|" + uvIndex;
                            if (hashtable.ContainsKey(hashEntry))
                            {
                                indexes[i - 1] = hashtable[hashEntry];
                            }
                            else
                            {
                                //create a new hash entry
                                indexes[i - 1] = hashtable.Count;
                                hashtable[hashEntry] = hashtable.Count;
                                uvertices.Add(vertices[vertexIndex]);
                                if (normalIndex < 0 || (normalIndex > (normals.Count - 1)))
                                {
                                    unormals.Add(Vector3.zero);
                                }
                                else
                                {
                                    hasNormals = true;
                                    unormals.Add(normals[normalIndex]);
                                }
                                if (uvIndex < 0 || (uvIndex > (uvs.Count - 1)))
                                {
                                    uuvs.Add(Vector2.zero);
                                }
                                else
                                {
                                    uuvs.Add(uvs[uvIndex]);
                                }

                            }
                        }
                        if (indexes.Length < 5 && indexes.Length >= 3)
                        {
                            ObjFace f1 = new ObjFace();
                            f1.MaterialName = cmaterial;
                            f1.Indexes = new int[] { indexes[0], indexes[1], indexes[2] };
                            f1.MeshName = (SplitByMaterial) ? cmaterial : cmesh;
                            faceList.Add(f1);
                            if (indexes.Length > 3)
                            {

                                ObjFace f2 = new ObjFace();
                                f2.MaterialName = cmaterial;
                                f2.MeshName = (SplitByMaterial) ? cmaterial : cmesh;
                                f2.Indexes = new int[] { indexes[2], indexes[3], indexes[0] };
                                faceList.Add(f2);
                            }
                        }
                    }
                }
            }

            if (objectNames.Count == 0)
                objectNames.Add("default");

            //build objects
            GameObject parentObject = new GameObject(meshName);


            foreach (string obj in objectNames)
            {
                GameObject subObject = new GameObject(obj);
                subObject.transform.parent = parentObject.transform;
                subObject.transform.localScale = new Vector3(-1, 1, 1); //IIRC I did this to correct for some rotation issues? TODO review this
                //Create mesh
                Mesh m = new Mesh();
                m.name = obj;
                //LISTS FOR REORDERING
                List<Vector3> processedVertices = new List<Vector3>();
                List<Vector3> processedNormals = new List<Vector3>();
                List<Vector2> processedUVs = new List<Vector2>();
                List<int[]> processedIndexes = new List<int[]>();
                Dictionary<int, int> remapTable = new Dictionary<int, int>();
                //POPULATE MESH
                List<string> meshMaterialNames = new List<string>();

                List<ObjFace> ofaces = faceList.FindAll(x => x.MeshName == obj);
                foreach (string mn in materialNames)
                {
                    ObjFace[] faces = ofaces.FindAll(x => x.MaterialName == mn).ToArray();
                    if (faces.Length > 0)
                    {
                        int[] indexes = new int[0];
                        foreach (ObjFace f in faces)
                        {
                            int l = indexes.Length;
                            Array.Resize(ref indexes, l + f.Indexes.Length);
                            Array.Copy(f.Indexes, 0, indexes, l, f.Indexes.Length);
                        }
                        meshMaterialNames.Add(mn);
                        if (m.subMeshCount != meshMaterialNames.Count)
                            m.subMeshCount = meshMaterialNames.Count;

                        for (int i = 0; i < indexes.Length; i++)
                        {
                            int idx = indexes[i];
                            //build remap table
                            if (remapTable.ContainsKey(idx))
                            {
                                //ezpz
                                indexes[i] = remapTable[idx];
                            }
                            else
                            {
                                processedVertices.Add(uvertices[idx]);
                                processedNormals.Add(unormals[idx]);
                                processedUVs.Add(uuvs[idx]);
                                remapTable[idx] = processedVertices.Count - 1;
                                indexes[i] = remapTable[idx];
                            }
                        }

                        processedIndexes.Add(indexes);
                    }
                    else
                    {

                    }
                }

                //apply stuff
                m.vertices = processedVertices.ToArray();
                m.normals = processedNormals.ToArray();
                m.uv = processedUVs.ToArray();

                for (int i = 0; i < processedIndexes.Count; i++)
                {
                    m.SetTriangles(processedIndexes[i], i);
                }

                if (!hasNormals)
                {
                    m.RecalculateNormals();
                }
                m.RecalculateBounds();
                ;

                MeshFilter mf = subObject.AddComponent<MeshFilter>();
                MeshRenderer mr = subObject.AddComponent<MeshRenderer>();

                Material[] processedMaterials = new Material[meshMaterialNames.Count];
                for (int i = 0; i < meshMaterialNames.Count; i++)
                {

                    if (materialCache == null)
                    {
                        processedMaterials[i] = new Material(Shader.Find("Standard (Specular setup)"));
                    }
                    else
                    {
                        Material mfn = Array.Find(materialCache, x => x.name == meshMaterialNames[i]); ;
                        if (mfn == null)
                        {
                            processedMaterials[i] = new Material(Shader.Find("Standard (Specular setup)"));
                        }
                        else
                        {
                            processedMaterials[i] = mfn;
                        }

                    }
                    processedMaterials[i].name = meshMaterialNames[i];
                }

                mr.materials = processedMaterials;
                mr.enabled = true;
                mr.useLightProbes = false;
                mf.mesh = m;
            }

            return parentObject;
        }
    }
}