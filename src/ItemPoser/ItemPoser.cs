using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;
using Photon.Voice.Unity.Demos;
using pworld.Scripts.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Core.CLI;

namespace ItemPoser;

[BepInAutoPlugin]
[BepInDependency("com.github.MiiMii1205.CanadianCuisine")]

public partial class ItemPoser : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;

    private Renderer m_greenScreenRenderer1 = null!;
    private Renderer m_greenScreenRenderer2 = null!;
    private Renderer m_greenScreenRenderer3 = null!;
    
    private ConfigEntry<int> m_byteR = null!;
    private ConfigEntry<int> m_byteG = null!;
    private ConfigEntry<int> m_byteB = null!;

    private ConfigEntry<float> m_setupAngle = null!;

    private static GameObject _itemHolder = null!;
    private static GameObject _cameraHolder = null!;
    private static GameObject _itemPoserSetup = null!;

    private static readonly Vector3 ItemPosePosition = new Vector3(54.25F, -5, 37);

    private static Color _greenScreenColor = Color.green;

    private static readonly int EmissionColorPropertyId = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        Log = Logger;


        m_byteR = Config.Bind("Backdrop", "Red Value", 0, new ConfigDescription(
            "The red value of the item backdrop", new AcceptableValueRange<
                int>(0, 255)));
        m_byteG = Config.Bind("Backdrop", "Green Value", 255, new ConfigDescription(
            "The green value of the item backdrop", new AcceptableValueRange<
                int>(0, 255)));
        m_byteB = Config.Bind("Backdrop", "Blue Value", 0, new ConfigDescription(
            "The blue value of the item backdrop", new AcceptableValueRange<
                int>(0, 255)));

        m_setupAngle = Config.Bind("Setup", "Setup Rotation", -100f, new ConfigDescription(
            "Rotation of the whole item poser setup. Use this to weak lights", new AcceptableValueRange<
                float>(-360f, 360f)));

        _greenScreenColor = new Color(m_byteR.Value / 255f, m_byteG.Value / 255f, m_byteB.Value / 255f);

        m_byteR.SettingChanged += OnConfigSettingChanged;
        m_byteG.SettingChanged += OnConfigSettingChanged;
        m_byteB.SettingChanged += OnConfigSettingChanged;

        m_setupAngle.SettingChanged += OnSetupAngleChanged;

        Log.LogInfo($"Plugin {Name} is loaded!");
    }

    private void OnSetupAngleChanged(object sender, EventArgs args)
    {
        if (_itemPoserSetup != null)
        {
            _itemPoserSetup.transform.rotation = Quaternion.AngleAxis(m_setupAngle.Value, Vector3.up);
        }
    }

    private void OnConfigSettingChanged(object sender, EventArgs e)
    {
        _greenScreenColor = new Color(m_byteR.Value / 255f, m_byteG.Value / 255f, m_byteB.Value / 255f);

        if (m_greenScreenRenderer1)
        {
            UpdateObjects();
        }
    }

    private static Renderer CreateCube(Vector3 pos, Vector3 scale, Quaternion rotation, Material material, string objectName)
    {
        var cubePrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var renderer = cubePrimitive.GetComponent<Renderer>();
        renderer.material = material;
        cubePrimitive.transform.position = pos;
        cubePrimitive.transform.localScale = scale;
        cubePrimitive.transform.rotation = rotation;
        cubePrimitive.name = objectName;
        return renderer;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "Airport")
        {
            var material = new Material(Shader.Find("Unlit/Color"));
            
            _itemPoserSetup = new GameObject("ItemPoserSetup")
            {
                transform =
                {
                    position = ItemPosePosition
                }
            };

            m_greenScreenRenderer1 = CreateCube(new Vector3(54.25f, -6, 37),
                new Vector3(22, 1, 22), Quaternion.identity, material, "SetupBackdrop_001");
            m_greenScreenRenderer2 = CreateCube(new Vector3(43.25f, 5, 37),
                new Vector3(1, 22, 22), Quaternion.identity, material, "SetupBackdrop_002");
            m_greenScreenRenderer3 = CreateCube(new Vector3(54.25f, 5, 48),
                new Vector3(22, 22, 1), Quaternion.identity, material, "SetupBackdrop_003");
            
            _itemHolder = new GameObject("ItemPoserItemHolder");
            _cameraHolder = new GameObject("ItemPoserCameraHolder");

            m_greenScreenRenderer1.transform.SetParent(_itemPoserSetup.transform, true);
            m_greenScreenRenderer2.transform.SetParent(_itemPoserSetup.transform, true);
            m_greenScreenRenderer3.transform.SetParent(_itemPoserSetup.transform, true);
            _itemHolder.transform.SetParent(_itemPoserSetup.transform, false);
            _cameraHolder.transform.SetParent(_itemPoserSetup.transform, false);

            _itemPoserSetup.transform.rotation = Quaternion.AngleAxis(m_setupAngle.Value, Vector3.up);

            _itemHolder.transform.localPosition = Vector3.zero;
            _itemHolder.transform.localRotation = Quaternion.identity;

            _cameraHolder.transform.localPosition = Vector3.back * 2;
            _cameraHolder.transform.localRotation = Quaternion.identity;

            UpdateObjects();
        }
    }

    private void UpdateObjects()
    {
        m_greenScreenRenderer1.material.color = _greenScreenColor;
        m_greenScreenRenderer1.material.SetColor(EmissionColorPropertyId, _greenScreenColor * 10f);
        m_greenScreenRenderer2.material.color = _greenScreenColor;
        m_greenScreenRenderer2.material.SetColor(EmissionColorPropertyId, _greenScreenColor * 10f);
        m_greenScreenRenderer3.material.color = _greenScreenColor;
        m_greenScreenRenderer3.material.SetColor(EmissionColorPropertyId, _greenScreenColor * 10f);
    }

    private static Vector3[] extentPoints = new Vector3[8];
    private static Bounds _itemBounds;

    private static Bounds TransformBounds(Transform item, Bounds bounds)
    {
        var cen = bounds.center;
        var ext = bounds.extents;

        extentPoints[0] = item.InverseTransformPoint(new Vector3(cen.x - ext.x, cen.y - ext.y, cen.z - ext.z));
        extentPoints[1] = item.InverseTransformPoint(new Vector3(cen.x + ext.x, cen.y - ext.y, cen.z - ext.z));
        extentPoints[2] = item.InverseTransformPoint(new Vector3(cen.x - ext.x, cen.y - ext.y, cen.z + ext.z));
        extentPoints[3] = item.InverseTransformPoint(new Vector3(cen.x + ext.x, cen.y - ext.y, cen.z + ext.z));
        extentPoints[4] = item.InverseTransformPoint(new Vector3(cen.x - ext.x, cen.y + ext.y, cen.z - ext.z));
        extentPoints[5] = item.InverseTransformPoint(new Vector3(cen.x + ext.x, cen.y + ext.y, cen.z - ext.z));
        extentPoints[6] = item.InverseTransformPoint(new Vector3(cen.x - ext.x, cen.y + ext.y, cen.z + ext.z));
        extentPoints[7] = item.InverseTransformPoint(new Vector3(cen.x + ext.x, cen.y + ext.y, cen.z + ext.z));
        
        var min = extentPoints[0];
        var max = extentPoints[0];

        foreach (var v in extentPoints)
        {
            min = Vector3.Min(min, v);
            max = Vector3.Max(max, v);
        }

        bounds.SetMinMax(min, max);
        return bounds;
    }

    [ConsoleCommand]
    public static void PoseItem(ushort id)
    {
        if (ItemDatabase.TryGetItem(id, out var itemInDatabase))
        {

            Log.LogInfo($"Posing {itemInDatabase.GetName()} ({itemInDatabase.name})...");
            
            var itemGameObject = PhotonNetwork.Instantiate($"0_Items/{itemInDatabase.name}", ItemPosePosition + Vector3.up * 100f,
                Quaternion.identity);

            var cameraHeight = _itemPoserSetup.transform.localPosition.y;
            var itemScale = _itemPoserSetup.transform.localPosition.y;

            
            
            if (itemGameObject.TryGetComponent(out Item item))
            {
                item.rig.isKinematic = true;
                item.blockInteraction = true;

                if (_itemHolder.transform.childCount > 0)
                {
                    Log.LogInfo("Deleting previous item...");
                    _itemHolder.transform.DestroyChildren();

                    _itemHolder.transform.localPosition = Vector3.zero;
                    _itemHolder.transform.localRotation = Quaternion.identity;
                    _itemHolder.transform.localScale = Vector3.one;
                }

                itemGameObject.transform.SetParent(_itemHolder.transform, false);
                itemGameObject.transform.localPosition = Vector3.zero;
                itemGameObject.transform.localRotation = Quaternion.identity;

                var rends = item.addtlRenderers.AddToArray(item.mainRenderer);
                
                var radd = new List<Renderer>();
                
                // Get any other pesky hidden renderers that the game forgot about before putting them in the poser booth.
                foreach (var renderer in rends)
                {
                    var rendererParent = renderer.transform.parent;

                    if (rendererParent.gameObject == itemGameObject.gameObject)
                    {
                        rendererParent = renderer.transform;
                    }

                    radd.AddRange(rendererParent.GetComponentsInChildren<MeshRenderer>(true));
                    radd.AddRange(rendererParent.GetComponentsInChildren<SkinnedMeshRenderer>(true));
                }

                rends = rends.AddRangeToArray(radd.ToArray());
                
                foreach (var r in rends)
                {
                    var rendererParent = r.transform.parent;

                    if (rendererParent.gameObject == itemGameObject.gameObject)
                    {
                        rendererParent = r.transform;
                    }
                    
                    rendererParent.SetParent(_itemHolder.transform, true);
                    
                }
                
                itemGameObject.transform.localPosition = Vector3.up * 100f;
                
                // Apply item's UI data offset.
                _itemHolder.transform.Translate(item.UIData.iconPositionOffset);
                _itemHolder.transform.Rotate(item.UIData.iconRotationOffset);
                _itemHolder.transform.localScale = Vector3.one * item.UIData.iconScaleOffset;
                
                _itemBounds = rends.Aggregate<Renderer, Bounds>(item.mainRenderer.bounds, (acc, renderer) =>
                {
                    acc.Encapsulate(renderer.bounds);
                    return acc;
                });
                
                var bounds = TransformBounds(_cameraHolder.transform, _itemBounds);

                var size = bounds.size;

                if (bounds.size is {x: > float.Epsilon, y: > float.Epsilon})
                { 
                    // not using cam.aspect because it is not always updated immediately
                    var aspect = Screen.width /
                                 (float) Screen
                                     .height;
                    
                    var height = size.x / aspect;
                    
                    if (height < size.y)
                    {
                        height = size.y;
                    }

                    itemScale = height / 2f;
                }
                else
                {
                    itemScale = 0.5f;
                }

                cameraHeight = bounds.center.y;

            }

            var freecam = GameObject.Find("UE_Freecam");

            if (freecam && freecam.TryGetComponent(out Camera camera))
            {
                Log.LogInfo("Found UE freecam. Setting freecam to camera holder's position and location");

                freecam.transform.SetPositionAndRotation(_cameraHolder.transform.position,
                    _cameraHolder.transform.rotation);

                freecam.transform.SetYPos(cameraHeight + _cameraHolder.transform.position.y);
                
                camera.orthographic = true;
                camera.orthographicSize = itemScale * (1f + 0.05f);
            }
        }
    }
}