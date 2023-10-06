using System;
using System.Collections.Generic;
using System.Linq;
using Planetbase;
using PlanetbaseFramework.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PlanetbaseFramework.GameMechanics.Buildings
{
    public class ModuleModelBuilder
    {
        // These constants are re-listed here so that doc strings can be attached
        // TODO move this to an `Assembly-CSharp.xml` file at some point

        /// <summary>
        /// Objects with this tag are displayed when the dome tops are on, and hidden
        /// when dome tops are off.
        ///
        /// These objects are considered a part of the dome mesh.
        ///
        /// This is typically used by the metal-looking "frames" on the module domes.
        /// </summary>
        public const string TagDomeOpaque = Constants.TagDomeOpaque;

        /// <summary>
        /// Objects with this tag are not opaque. When a warning appears, the color
        /// of this object will change accordingly. For example, when the module
        /// is low on oxygen, this object will be colored red.
        ///
        /// These objects will display when the dome tops are on, and will be hidden
        /// when dome tops are off.
        ///
        /// These objects are considered a part of the dome mesh.
        ///
        /// These objects will not be displayed when the module is extremely damaged.
        ///
        /// These objects will not be displayed after the module receives an impact
        /// event.
        ///
        /// The "tint" property of these objects will be updated based upon various
        /// environment effects.
        ///
        /// This is typically used by the glass on the module domes.
        /// </summary>
        public const string TagDomeTranslucent = Constants.TagDomeTranslucent;

        /// <summary>
        /// Objects with this tag are not affected by top toggles.
        /// 
        /// These objects are considered a part of the dome mesh.
        ///
        /// If a module contains an object with this tag, then the repair radius is
        /// determined by the apothem in the x axis of the bounding box containing
        /// this object. However, this rule only applies to external modules.
        /// See Planetbase.Module::postInit for details.
        ///
        /// The indoor walls and orange frame arches on the airlock use this tag.
        /// </summary>
        public const string TagDomeStatic = Constants.TagDomeStatic;

        /// <summary>
        /// Objects with this tag follow all the rules of DomeTranslucent, except:
        ///
        /// <ul>
        /// <li>Most of the rules will only apply if the object does not also contain
        ///  a DomeTranslucent object.</li>
        /// <li>The color of this object will not be changed when a warning
        /// appears.</li>
        /// </ul>
        /// 
        /// The outer door on the airlock uses this tag.
        /// </summary>
        public const string TagDomeStaticTranslucent = Constants.TagDomeStaticTranslucent;

        /// <summary>
        /// Objects with this tag are the only ones considered apart of the "base mesh",
        /// and not a part of the dome mesh.
        ///
        /// This is typically used by the short raised "ring" that appears around module
        /// walls after the module is built.
        /// </summary>
        public const string TagDomeBase = Constants.TagDomeBase;

        /// <summary>
        /// These objects are always shown, even before the module is built.
        ///
        /// Objects with this tag will not case a shadow.
        ///
        /// This is used on the "plate" that is shown when the module is placed.
        /// </summary>
        public const string TagDomeFloor = Constants.TagDomeFloor;

        /// <summary>
        /// These objects are purely cosmetic decorations.
        ///
        /// Any object with this tag will be hidden (set inactive) if a connection
        /// collides with them.
        ///
        /// If a module contains any objects with this tag, then the character navigation
        /// radius is slightly increased.
        ///
        /// Objects with this tag will have their collider removed, if any exists.
        ///
        /// This is used for the purely cosmetic objects that appear around the outside of
        /// modules.
        /// </summary>
        public const string TagDomeProp = Constants.TagDomeProp;

        /// <summary>
        /// Objects with this tag have special rotation logic applied to them. Due to how
        /// specific and rigid the base game's logic is for objects with this tag, it is
        /// not recommended that this be used by mods.
        /// 
        /// These objects are considered a part of the dome mesh.
        ///
        /// Objects with this tag will have their collider removed, if any exists.
        /// 
        /// This is used by spinning objects such as the windmill blades.
        /// </summary>
        public const string TagDomeMobile = Constants.TagDomeMobile;

        /// <summary>
        /// This is not currently used by any module in the base game.
        /// </summary>
        public const string TagDomePanel = Constants.TagDomePanel;

        public static readonly string[] BaseGameTags = {
            TagDomeFloor,
            TagDomeBase,
            TagDomeMobile,
            TagDomeOpaque,
            TagDomePanel,
            TagDomeProp,
            TagDomeStatic,
            TagDomeStaticTranslucent,
            TagDomeTranslucent
        };

        public List<GameObject> FloorObjects { get; protected set; } = new List<GameObject>();
        public List<GameObject> TranslucentObjects { get; protected set; } = new List<GameObject>();
        public List<GameObject> StaticTranslucentObjects { get; protected set; } = new List<GameObject>();
        public List<GameObject> OpaqueObjects { get; protected set; } = new List<GameObject>();
        public List<GameObject> StaticObjects { get; protected set; } = new List<GameObject>();
        public List<GameObject> PropObjects { get; protected set; } = new List<GameObject>();
        public List<GameObject> UnmanagedObjects { get; protected set; } = new List<GameObject>();

        public ModuleModelBuilder AddObjectToList(GameObject @object, List<GameObject> list)
        {
            // Copying the object allows it to be used multiple times, if needed
            list.Add(Object.Instantiate(@object));
            return this;
        }

        public ModuleModelBuilder AddFloorObject(GameObject @object)
        {
            return AddObjectToList(@object, FloorObjects);
        }

        public ModuleModelBuilder AddObject(GameObject @object, bool translucent = false, bool togglable = false)
        {
            List<GameObject> listToAddTo;
            if (translucent)
                listToAddTo = togglable ? TranslucentObjects : StaticTranslucentObjects;
            else
                listToAddTo = togglable ? OpaqueObjects : StaticObjects;

            return AddObjectToList(@object, listToAddTo);
        }

        public ModuleModelBuilder AddProp(GameObject @object)
        {
            return AddObjectToList(@object, PropObjects);
        }

        public ModuleModelBuilder AddUnmanagedObject(GameObject @object)
        {
            return AddObjectToList(@object, UnmanagedObjects);
        }

        /// <summary>
        /// Adds objects to the appropriate list when the object's name is a "Dome" tag.
        /// If a given the top level object is not named with a Dome tag, then this will
        /// search the top level object's first generation of children.
        ///
        /// Supported GameObject structures:
        /// * Root GameObject { Name: Dome$SomeDomeTag }
        /// * Root GameObject { Name: $Unset or not Dome$AnyDomeTag }
        ///   * Child 1 GameObject { Name: Dome$SomeDomeTag }
        ///   * Child 2 GameObject { Name: Dome$SomeDomeTag }
        ///   * Child 3 GameObject { Name: Dome$SomeOtherDomeTag }
        /// </summary>
        public ModuleModelBuilder AddPreStructuredObjects(GameObject @object, bool checkChildren = true)
        {
            if (BaseGameTags.Any(validDomeTags => @object.name == validDomeTags))
            {
                List<GameObject> listToAddTo;
                switch (@object.name)
                {
                    case TagDomeFloor:
                        listToAddTo = FloorObjects;
                        break;
                    case TagDomeTranslucent:
                        listToAddTo = TranslucentObjects;
                        break;
                    case TagDomeStaticTranslucent:
                        listToAddTo = StaticTranslucentObjects;
                        break;
                    case TagDomeOpaque:
                        listToAddTo = OpaqueObjects;
                        break;
                    case TagDomeStatic:
                        listToAddTo = StaticObjects;
                        break;
                    case TagDomeProp:
                        listToAddTo = PropObjects;
                        break;
                    default:
                        listToAddTo = UnmanagedObjects;
                        break;
                }

                return AddObjectToList(@object, listToAddTo);
            }

            if (!checkChildren || @object.transform.childCount <= 0)
                return this;

            foreach (Transform childTransform in @object.transform)
                AddPreStructuredObjects(childTransform.gameObject, checkChildren: false);

            return this;
        }

        /// <summary>
        /// Logs info if the built module is configured in such a way that it does not fit
        /// the best practices for a new module model.
        /// </summary>
        /// <returns>True if the model is passes the tests, false otherwise</returns>
        public bool DoesFitBestPractices()
        {
            var doAllChecksPass = true;
            if (FloorObjects.Count == 0)
            {
                doAllChecksPass = false;
                Debug.LogWarning("The builder contains no floor objects");
            }

            if (TranslucentObjects.Count == 0 && StaticTranslucentObjects.Count == 0 &&
                OpaqueObjects.Count == 0 && StaticObjects.Count == 0 &&
                UnmanagedObjects.Count == 0)
            {
                doAllChecksPass = false;
                Debug.LogWarning("The builder contains no meaningful objects");
            }

            return doAllChecksPass;
        }

        /// <summary>
        /// Adds collision geometry to the provided GameObject. If the game object contains a
        /// mesh filter, then the collision geometry is based off of this. Otherwise it is
        /// based off the renderer's bounding box.
        /// </summary>
        public static GameObject AddCollisionGeometry(GameObject @object)
        {
            @object.Log();

            if (@object.GetComponent<Collider>() != null)
                return @object;

            var meshFilter = @object.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                var meshCollider = @object.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = meshFilter.sharedMesh;
                return @object;
            }

            var renderer = @object.GetComponent<Renderer>() ?? throw new ArgumentException("the provided gameobject has no renderer or mesh filter", nameof(@object));
            var rendererBoundingBox = renderer.bounds;
            var boxCollider = @object.AddComponent<BoxCollider>();
            boxCollider.center = rendererBoundingBox.center;
            boxCollider.size = rendererBoundingBox.size;

            return @object;
        }

        /// <summary>
        /// Produced a new GameObject for the builder's configuration.
        /// </summary>
        /// <param name="name">The name of the root object</param>
        /// <param name="shouldSetVisible">True if all child objects should be set to visible, false if visibility is handled elsewhere</param>
        public GameObject GenerateObject(string name = "", bool shouldSetVisible = true)
        {
            var rootObject = new GameObject(name);

            GenerateObjectForList(FloorObjects, TagDomeFloor, rootObject);
            GenerateObjectForList(TranslucentObjects, TagDomeTranslucent, rootObject);
            GenerateObjectForList(StaticTranslucentObjects, TagDomeStaticTranslucent, rootObject);
            GenerateObjectForList(OpaqueObjects, TagDomeOpaque, rootObject);
            GenerateObjectForList(StaticObjects, TagDomeStatic, rootObject);

            foreach (var childObject in UnmanagedObjects)
            {
                var childCopy = Object.Instantiate(childObject);
                childCopy.transform.SetParent(rootObject.transform, false);
            }

            SmoothMeshesRecursively(rootObject);

            if (shouldSetVisible)
                rootObject.setVisibleRecursive(true);

            // The top level object should not be active to prevent it from showing up on the title screen
            rootObject.SetActive(false);

            return rootObject;
        }

        /// <summary>
        /// Processes a given list and adds the to the root object under a new object with the provided tag.
        /// </summary>
        protected void GenerateObjectForList(List<GameObject> childObjects, string tag, GameObject rootObject)
        {
            if (childObjects.Count == 0)
                return;

            var listRootObject = new GameObject($"{rootObject.name}_floor")
            {
                tag = tag
            };
            listRootObject.transform.SetParent(rootObject.transform, false);

            foreach (var childObject in childObjects)
            {
                // A copy is made so that this function can be called multiple times to create multiple new game objects
                // without affecting already created objects
                var childCopy = Object.Instantiate(childObject);
                childCopy.transform.SetParent(listRootObject.transform, false);
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
    }
}