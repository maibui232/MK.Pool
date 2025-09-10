namespace MK.Pool.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEngine;

    public sealed class PoolStatisticWindow : OdinEditorWindow
    {
        [MenuItem("MK/Pool Statistics")]
        private static void OpenWindow()
        {
            var window = GetWindow<PoolStatisticWindow>();
            window.titleContent = new GUIContent("Pool Statistics");
            window.Show();
        }

        [Title("Pool Statistics", TitleAlignment = TitleAlignments.Centered)]
        
        [BoxGroup("Controls")]
        [HorizontalGroup("Controls/H")]
        [Button(ButtonSizes.Medium)]
        [GUIColor(0.3f, 0.8f, 0.3f)]
        private void RefreshStatistics()
        {
            RefreshData();
        }

        [HorizontalGroup("Controls/H")]
        [Button(ButtonSizes.Medium)]
        [GUIColor(0.8f, 0.3f, 0.3f)]
        [EnableIf("HasStatistics")]
        private void ClearAllStatistics()
        {
            if (EditorUtility.DisplayDialog("Clear Statistics", 
                "Are you sure you want to clear all pool statistics?", 
                "Clear", "Cancel"))
            {
                PoolStatistics.TypeToPool.Clear();
                PoolStatistics.PrefabToPool.Clear();
                RefreshData();
            }
        }

        [ShowInInspector]
        [HideLabel]
        [InfoBox("$GetSummaryInfo", InfoMessageType.None)]
        private string SummaryInfo => GetSummaryInfo();

        [Title("IPoolable Statistics")]
        [ShowInInspector]
        [TableList(ShowIndexLabels = false, ShowPaging = true, NumberOfItemsPerPage = 10)]
        [LabelText("Type-based Pools")]
        private List<PoolableStatisticEntry> poolableStatistics = new();

        [Title("GameObject Statistics")]
        [ShowInInspector]
        [TableList(ShowIndexLabels = false, ShowPaging = true, NumberOfItemsPerPage = 10)]
        [LabelText("Prefab-based Pools")]
        private List<GameObjectStatisticEntry> gameObjectStatistics = new();

        private bool HasStatistics => poolableStatistics.Count > 0 || gameObjectStatistics.Count > 0;

        protected override void OnEnable()
        {
            base.OnEnable();
            PoolStatistics.OnSpawned += OnPoolChanged;
            PoolStatistics.OnRecycled += OnPoolChanged;
            RefreshData();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PoolStatistics.OnSpawned -= OnPoolChanged;
            PoolStatistics.OnRecycled -= OnPoolChanged;
        }

        private void OnPoolChanged()
        {
            RefreshData();
            Repaint();
        }

        private string GetSummaryInfo()
        {
            if (!Application.isPlaying)
                return "Enter Play Mode to see pool statistics";

            if (!HasStatistics)
                return "No pool statistics available";

            var totalPoolableTypes = poolableStatistics.Count;
            var totalPoolableActive = poolableStatistics.Sum(p => p.SpawnedCount);
            var totalPoolableCached = poolableStatistics.Sum(p => p.CachedCount);

            var totalGameObjectTypes = gameObjectStatistics.Count;
            var totalGameObjectActive = gameObjectStatistics.Sum(p => p.SpawnedCount);
            var totalGameObjectCached = gameObjectStatistics.Sum(p => p.CachedCount);

            return $"IPoolable Types: {totalPoolableTypes} (Active: {totalPoolableActive}, Cached: {totalPoolableCached})\n" +
                   $"GameObject Types: {totalGameObjectTypes} (Active: {totalGameObjectActive}, Cached: {totalGameObjectCached})";
        }

        private void RefreshData()
        {
            // Refresh IPoolable statistics
            poolableStatistics.Clear();
            foreach (var kvp in PoolStatistics.TypeToPool)
            {
                poolableStatistics.Add(new PoolableStatisticEntry
                {
                    TypeName = kvp.Key.Name,
                    FullTypeName = kvp.Key.FullName,
                    CachedCount = kvp.Value.cached?.Count ?? 0,
                    SpawnedCount = kvp.Value.spawned?.Count ?? 0
                });
            }
            poolableStatistics = poolableStatistics.OrderBy(p => p.TypeName).ToList();

            // Refresh GameObject statistics
            gameObjectStatistics.Clear();
            foreach (var kvp in PoolStatistics.PrefabToPool)
            {
                if (kvp.Key != null)
                {
                    gameObjectStatistics.Add(new GameObjectStatisticEntry
                    {
                        Prefab = kvp.Key,
                        PrefabName = kvp.Key.name,
                        CachedCount = kvp.Value.cached?.Count ?? 0,
                        SpawnedCount = kvp.Value.spawned?.Count ?? 0
                    });
                }
            }
            gameObjectStatistics = gameObjectStatistics.OrderBy(p => p.PrefabName).ToList();
        }

        [Serializable]
        public class PoolableStatisticEntry
        {
            [TableColumnWidth(150, Resizable = false)]
            [DisplayAsString]
            public string TypeName { get; set; }

            [TableColumnWidth(80)]
            [DisplayAsString]
            [GUIColor(0.3f, 0.8f, 0.3f)]
            public int SpawnedCount { get; set; }

            [TableColumnWidth(80)]
            [DisplayAsString]
            [GUIColor(0.8f, 0.8f, 0.3f)]
            public int CachedCount { get; set; }

            [TableColumnWidth(80)]
            [DisplayAsString]
            [ShowInInspector]
            public int TotalCount => SpawnedCount + CachedCount;

            [TableColumnWidth(100)]
            [ShowInInspector]
            [ProgressBar(0, 1, ColorGetter = "GetUtilizationColor")]
            [LabelText("Utilization")]
            private float Utilization => TotalCount > 0 ? (float)SpawnedCount / TotalCount : 0;

            [HideInTables]
            public string FullTypeName { get; set; }

            private Color GetUtilizationColor()
            {
                if (Utilization < 0.3f) return Color.green;
                if (Utilization < 0.7f) return Color.yellow;
                return new Color(1f, 0.3f, 0.3f);
            }

            [TableColumnWidth(60)]
            [Button("Details")]
            private void ShowDetails()
            {
                Debug.Log($"Type: {FullTypeName}\n" +
                         $"Spawned: {SpawnedCount}\n" +
                         $"Cached: {CachedCount}\n" +
                         $"Total: {TotalCount}\n" +
                         $"Utilization: {Utilization:P}");
            }
        }

        [Serializable]
        public class GameObjectStatisticEntry
        {
            [TableColumnWidth(150, Resizable = false)]
            [DisplayAsString]
            public string PrefabName { get; set; }

            [TableColumnWidth(80)]
            [DisplayAsString]
            [GUIColor(0.3f, 0.8f, 0.3f)]
            public int SpawnedCount { get; set; }

            [TableColumnWidth(80)]
            [DisplayAsString]
            [GUIColor(0.8f, 0.8f, 0.3f)]
            public int CachedCount { get; set; }

            [TableColumnWidth(80)]
            [DisplayAsString]
            [ShowInInspector]
            public int TotalCount => SpawnedCount + CachedCount;

            [TableColumnWidth(100)]
            [ShowInInspector]
            [ProgressBar(0, 1, ColorGetter = "GetUtilizationColor")]
            [LabelText("Utilization")]
            private float Utilization => TotalCount > 0 ? (float)SpawnedCount / TotalCount : 0;

            [HideInTables]
            public GameObject Prefab { get; set; }

            private Color GetUtilizationColor()
            {
                if (Utilization < 0.3f) return Color.green;
                if (Utilization < 0.7f) return Color.yellow;
                return new Color(1f, 0.3f, 0.3f);
            }

            [TableColumnWidth(60)]
            [Button("Select")]
            private void SelectPrefab()
            {
                if (Prefab != null)
                {
                    Selection.activeObject = Prefab;
                }
            }

            [TableColumnWidth(60)]
            [Button("Details")]
            private void ShowDetails()
            {
                Debug.Log($"Prefab: {PrefabName}\n" +
                         $"Spawned: {SpawnedCount}\n" +
                         $"Cached: {CachedCount}\n" +
                         $"Total: {TotalCount}\n" +
                         $"Utilization: {Utilization:P}");
            }
        }
    }
}