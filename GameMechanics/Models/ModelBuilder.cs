using Planetbase;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PlanetbaseFramework.GameMechanics.Models
{
    public abstract class ModelBuilder<T> where T : ModelBuilder<T>
    {
        // TODO add support for anchor points, particle points, etc.

        public T AddObjectToList(GameObject @object, List<GameObject> list)
        {
            // Copying the object allows it to be used multiple times, if needed
            list.Add(Object.Instantiate(@object));
            return (T)this;
        }

        /// <summary>
        /// Processes a given list and adds the to the root object under a new object with the provided tag.
        /// </summary>
        protected void GenerateObjectForList(GameObject parentObject, List<GameObject> childObjects, string tag = null)
        {
            if (childObjects.Count == 0)
                return;

            var listRootObjectName = parentObject.name;
            if (string.IsNullOrEmpty(tag))
                tag = Constants.TagUntagged;
            else
                listRootObjectName += $"_{tag}";

            var listRootObject = new GameObject(listRootObjectName)
            {
                tag = tag
            };
            listRootObject.transform.SetParent(parentObject.transform, false);

            AddCopiesToParent(listRootObject, childObjects);
        }

        /// <summary>
        /// Adds a new copy of each provided child object to the parent.
        /// </summary>
        protected void AddCopiesToParent(GameObject rootObject, IEnumerable<GameObject> childObjects)
        {
            foreach (var childObject in childObjects)
            {
                // A copy is made so that this function can be called multiple times to create multiple new game objects
                // without affecting already created objects
                var childCopy = Object.Instantiate(childObject);
                childCopy.transform.SetParent(rootObject.transform, false);
            }
        }

        /// <summary>
        /// This functions smooths meshes recursively, akin to `calculateSmoothMeshRecursive`. The difference between
        /// the two functions is that this supports meshes that have already been smoothed, which allows for mixing
        /// custom game objects with base game prefabs.
        /// </summary>
        protected void SmoothMeshesRecursively(GameObject @object)
        {
            SmoothMesh(@object);

            foreach (Transform childTransform in @object.transform)
                SmoothMeshesRecursively(childTransform.gameObject);
        }

        /// <summary>
        /// Non-recursive version of SmoothMeshesRecursively. If the object has no mesh filter,
        /// then nothing is done.
        /// </summary>
        protected void SmoothMesh(GameObject @object)
        {
            // Do nothing if there is no mesh to smooth
            var component = @object.GetComponent<MeshFilter>();
            if (component == null || component.sharedMesh == null)
                return;

            // Do nothing if the object already has a smooth mesh component
            if (@object.GetComponent<MeshComponent>() != null)
                return;

            var meshComponent = @object.AddComponent<MeshComponent>();
            meshComponent.setMesh(MeshUtil.smoothVertices(component.sharedMesh), true);
        }

        /// <summary>
        /// Adds collision geometry to the provided GameObject. If the game object contains a
        /// mesh filter, then the collision geometry is based off of this. Otherwise it is
        /// based off the renderer's bounding box.
        ///
        /// This function should be called instead of `AddCollisionGeometryIfSupported` when
        /// it is expected that the collision geometry should always be addable, such as
        /// when loading from a known object file.
        /// </summary>
        public static GameObject AddCollisionGeometry(GameObject @object)
        {
            var wasGeometryAdded = AddCollisionGeometryIfSupported(@object);
            if (!wasGeometryAdded)
                throw new Exception($"Unable to add collision geometry to {@object}");

            return @object;
        }

        /// <summary>
        /// Adds collision geometry to the provided GameObject, and its recursive children,
        /// for each instance that supports it.If the game object contains a mesh filter,
        /// then the collision geometry is based off of this. Otherwise it is based off the
        /// renderer's bounding box.
        /// </summary>
        public static GameObject AddCollisionGeometryRecursively(GameObject @object)
        {
            AddCollisionGeometryIfSupported(@object);
            foreach(Transform childTransform in @object.transform)
                AddCollisionGeometryRecursively(childTransform.gameObject);

            return @object;
        }

        /// <summary>
        /// Adds collision geometry to the provided GameObject. If the game object contains a
        /// mesh filter, then the collision geometry is based off of this. Otherwise it is
        /// based off the renderer's bounding box.
        /// </summary>
        /// <returns>True if collision geometry was added or already existed, false otherwise</returns>
        public static bool AddCollisionGeometryIfSupported(GameObject @object)
        {
            if (@object.GetComponent<Collider>() != null)
                return true;

            var meshFilter = @object.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                var meshCollider = @object.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = meshFilter.sharedMesh;
                return true;
            }

            var renderer = @object.GetComponent<Renderer>();
            if (renderer == null)
                return false;

            var rendererBoundingBox = renderer.bounds;
            var boxCollider = @object.AddComponent<BoxCollider>();
            boxCollider.center = rendererBoundingBox.center;
            boxCollider.size = rendererBoundingBox.size;

            return true;
        }
    }
}