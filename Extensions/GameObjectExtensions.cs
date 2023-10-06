using System;
using System.Collections.Generic;
using Harmony;
using UnityEngine;

namespace PlanetbaseFramework.Extensions
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Adds collision geometry(to enable things like clicking/selection) to the game object.
        /// Collision will be in the shape of the object's mesh, but not any of its children's meshes.
        /// </summary>
        /// <param name="object">The object that collision geometry should be added to</param>
        public static void AddCollisionGeometry(this GameObject @object) {
            var objectMeshFilter = @object.GetComponent<MeshFilter>();
            if (objectMeshFilter != null)
                @object.AddComponent<MeshCollider>().sharedMesh = objectMeshFilter.sharedMesh;
        }

        /// <summary>
        /// Recursively evaluates a callback function for the provided game object and each of its children.
        /// Useful for performing multiple actions on each child without traversing the tree for each one.
        /// </summary>
        /// <param name="rootObject">The root object to evaluate the callback on</param>
        /// <param name="callback">The function to evaluate on the root object and each child</param>
        /// <param name="maxDepth">The max search depth for child nodes within the root object</param>
        public static void ForEachChild(this GameObject rootObject, Action<GameObject> callback, int maxDepth = 5)
        {
            callback(rootObject);

            if (maxDepth == 0)
                return;

            foreach (Transform childTransform in rootObject.transform)
                childTransform.gameObject.ForEachChild(callback, maxDepth - 1);
        }

        /// <summary>
        /// Log object info to the debug log.
        /// </summary>
        public static void Log(this GameObject @object)
        {
            Debug.Log(string.Join("\n", @object.GetInfo().ToArray()));
        }

        /// <summary>
        /// Gets useful information about the GameObject. The contents and formatting of this function's log messages
        /// is subject to change.
        /// </summary>
        public static List<string> GetInfo(this GameObject @object, string prefix = "", int maxDepth = 5)
        {
            var logLines = new List<string>
            {
                $"{prefix}************ Object ************",
                $"{prefix}Name: {@object.name}",
                $"{prefix}Tag: {@object.tag}",
                $"{prefix}Is visible: {@object.GetComponent<Renderer>()?.enabled ?? false}",
                $"{prefix}Is active: {@object.activeSelf}",
                $"{prefix}Components: {@object.GetComponents<Component>().Join(converter: c => $"{c?.name ?? "NULL"} ({c?.GetType().Name ?? "NULL"})", delimiter:" ")}",
            };

            if (maxDepth == 0)
                return new List<string>();

            if (@object.transform.childCount > 0)
                logLines.Add($"{prefix}Children:");

            foreach (Transform childTransform in @object.transform)
                logLines.AddRange(childTransform.gameObject.GetInfo(prefix + "##", maxDepth - 1));

            return logLines;
        }
    }
}