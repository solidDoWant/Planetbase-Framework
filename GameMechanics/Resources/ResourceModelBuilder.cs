using PlanetbaseFramework.GameMechanics.Models;
using System.Collections.Generic;
using Planetbase;
using UnityEngine;

namespace PlanetbaseFramework.GameMechanics.Resources
{
    public class ResourceModelBuilder : ModelBuilder<ResourceModelBuilder>
    {
        public List<GameObject> MeshObjects { get; } = new List<GameObject>();

        public ResourceModelBuilder AddMeshObject(GameObject @object)
        {
            return AddObjectToList(@object, MeshObjects);
        }

        /// <summary>
        /// Produced a new GameObject for the builder's configuration.
        /// </summary>
        /// <param name="name">The name of the root object</param>
        /// <param name="shouldSetVisible">True if all child objects should be set to visible, false if visibility is handled elsewhere</param>
        public GameObject GenerateObject(string name = "", bool shouldSetVisible = true)
        {
            var rootObject = new GameObject(name);

            var body = rootObject.AddComponent<Rigidbody>();
            body.isKinematic = true;

            AddCopiesToParent(rootObject, MeshObjects);
            SmoothMeshesRecursively(rootObject);

            if (shouldSetVisible)
                rootObject.setVisibleRecursive(true);

            // The top level object should not be active to prevent it from showing up on the title screen
            rootObject.SetActive(false);

            return rootObject;
        }
    }
}