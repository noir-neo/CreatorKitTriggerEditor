﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClusterVR.CreatorKit.Editor.Builder;
using ClusterVR.CreatorKit.Gimmick;
using ClusterVR.CreatorKit.Gimmick.Implements;
using ClusterVR.CreatorKit.Operation;
using ClusterVR.CreatorKit.Trigger;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using ValueType = ClusterVR.CreatorKit.Operation.ValueType;

namespace CreatorKitTriggerEditor.Editor
{
    public class StateKeyListWindow : EditorWindow
    {
        [MenuItem("CreatorKitTriggerEditor/Open Window")]
        public static void ShowWindow()
        {
            var window = GetWindow<StateKeyListWindow>();
            window.titleContent = new GUIContent("StateKeyListWindow");
        }

        public void OnEnable()
        {
            var root = rootVisualElement;
            root.Add(CreateView());
        }

        static VisualElement CreateView()
        {
            var container = new VisualElement();
            container.style.flexGrow = 1.0f;

            var triggers = new  List<StateKeyComponentsSet>();
            var listView = CreateListView(triggers);

            var button = new Button(() =>
            {
                triggers.Clear();
                triggers.AddRange(Gather(SceneGameObjects())
                    .OrderBy(x => x.StateKey.Target)
                    .ThenBy(x => x.StateKey.Key));
                listView.Refresh();
            })
            {
                text = "Refresh List"
            };
            container.Add(button);
            container.Add(listView);

            return container;
        }

        static ListView CreateListView(List<StateKeyComponentsSet> triggers)
        {
            void BindItem(VisualElement element, int i) => BindListItem(element, triggers[i].StateKey);
            const int itemHeight = 16;
            var listView = new ListView(triggers, itemHeight, MakeListItem, BindItem)
            {
                selectionType = SelectionType.Multiple
            };
            listView.onItemChosen += chosen => Selection.objects = ((StateKeyComponentsSet)chosen).Components.Select(c => c.gameObject).ToArray();
            listView.onSelectionChanged += selection => Selection.objects = selection.SelectMany(s => ((StateKeyComponentsSet)s).Components).Select(c => c.gameObject).ToArray();
            listView.style.flexGrow = 1.0f;
            return listView;
        }

        static VisualElement MakeListItem()
        {
            return new Label();
        }

        static void BindListItem(VisualElement element, StateKey stateKey)
        {
            (element as Label).text = $"{stateKey.Target}: {stateKey.Key}";
        }

        static IEnumerable<GameObject> SceneGameObjects()
        {
            var scene = SceneManager.GetActiveScene();
            var sceneRootObjects = scene.GetRootGameObjects();
            var itemTemplates = ItemTemplateGatherer.GatherItemTemplates(scene);
            return sceneRootObjects.Concat(itemTemplates.Select(t => t.gameObject)).ToArray();
        }

        static IEnumerable<StateKeyComponentsSet> Gather(IEnumerable<GameObject> rootGameObjects)
        {
            var triggers = GatherTriggerParams(rootGameObjects)
                .SelectMany(t => t.Item2.SelectMany(x =>
                    TargetConverter.Convert(x.Target).Select(target => (new StateKey(target, x.Key), t.Item1))));
            var gimmicks = GatherGimmickKey(rootGameObjects)
                .Select(t => (new StateKey(TargetConverter.Convert(t.Item2.Target), t.Item2.Key), t.Item1))
                .Concat(GatherPlayerGimmickKey(rootGameObjects).Select(t =>
                    (new StateKey(TargetConverter.Convert(t.Item2.Key.Target), t.Item2.Key.Key), t.Item1)))
                .Concat(GatherGlobalGimmickKey(rootGameObjects).Select(t =>
                    (new StateKey(TargetConverter.Convert(t.Item2.Key.Target), t.Item2.Key.Key), t.Item1)));
            var logic = Gather(GatherLogic(rootGameObjects));
            return triggers.Concat(gimmicks).Concat(logic).GroupBy(t => t.Item1)
                .Select(g => new StateKeyComponentsSet(g.Key, g.Select(x => x.Item2).ToArray()));
        }

        static IEnumerable<(StateKey, Component)> Gather(IEnumerable<(Component, Logic)> logic)
        {
            return logic.SelectMany(t => t.Item2.Statements
                    .SelectMany(s => Gather(s.SingleStatement.Expression)
                        .Concat(new [] { new StateKey(TargetConverter.Convert(s.SingleStatement.TargetState.Target), s.SingleStatement.TargetState.Key)}))
                    .Select(trigger => (trigger, t.Item1)));
        }

        static IEnumerable<StateKey> Gather(Expression expression)
        {
            switch (expression.Type)
            {
                case ExpressionType.Value:
                    return Gather(expression.Value);
                case ExpressionType.OperatorExpression:
                    return expression.OperatorExpression.Operands
                        .Take(expression.OperatorExpression.Operator.GetRequiredLength())
                        .SelectMany(Gather);
                default:
                    throw new NotImplementedException();
            }
        }

        static IEnumerable<StateKey> Gather(Value value)
        {
            if (value.Type == ValueType.RoomState)
            {
                return new []{ new StateKey(TargetConverter.Convert(value.SourceState.Target), value.SourceState.Key) };
            }

            return Enumerable.Empty<StateKey>();
        }

        static IEnumerable<(Component, ClusterVR.CreatorKit.Trigger.Implements.TriggerParam[])> GatherTriggerParams(IEnumerable<GameObject> rootGameObjects)
        {
            return rootGameObjects.Gather<IItemTrigger>().OfType<Component>()
                    .Concat(rootGameObjects.Gather<IPlayerTrigger>().OfType<Component>())
                    .Concat(rootGameObjects.Gather<IGlobalGimmick>().OfType<Component>())
                    .SelectMany(x => GetValues<ClusterVR.CreatorKit.Trigger.Implements.TriggerParam[]>(x, 2)
                        .Select(t => (x, t)));
        }

        static IEnumerable<(Component, GimmickKey)> GatherGimmickKey(IEnumerable<GameObject> rootGameObjects)
        {
            return rootGameObjects.Gather<IItemGimmick>().OfType<Component>()
                .SelectMany(x => GetValues<GimmickKey>(x)
                    .Select(l => (x, l)));
        }

        static IEnumerable<(Component, PlayerGimmickKey)> GatherPlayerGimmickKey(IEnumerable<GameObject> rootGameObjects)
        {
            return rootGameObjects.Gather<IPlayerGimmick>().OfType<Component>()
                .SelectMany(x => GetValues<PlayerGimmickKey>(x)
                    .Select(l => (x, l)));
        }

        static IEnumerable<(Component, GlobalGimmickKey)> GatherGlobalGimmickKey(IEnumerable<GameObject> rootGameObjects)
        {
            return rootGameObjects.Gather<IGlobalGimmick>().OfType<Component>()
                .SelectMany(x => GetValues<GlobalGimmickKey>(x)
                    .Select(l => (x, l)));
        }

        static IEnumerable<(Component, Logic)> GatherLogic(IEnumerable<GameObject> rootGameObjects)
        {
            return rootGameObjects.Gather<IItemLogic>().OfType<Component>()
                    .Concat(rootGameObjects.Gather<IPlayerLogic>().OfType<Component>())
                    .Concat(rootGameObjects.Gather<IGlobalLogic>().OfType<Component>())
                    .SelectMany(x => GetValues<Logic>(x)
                        .Select(l => (x, l)));
        }

        static IEnumerable<T> GetValues<T>(object obj, int depth = 1)
        {
            if (obj == null || depth < 0) return Enumerable.Empty<T>();
            var fieldInfos = obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            return fieldInfos.SelectMany(f =>
            {
                var value = f.GetValue(obj);
                if (f.FieldType == typeof(T))
                {
                    return new[] {(T) value};
                }

                if (depth < 2 || value == null) return Enumerable.Empty<T>();

                if (!f.FieldType.IsArray) return GetValues<T>(value, depth - 1);

                var result = new List<T>();
                foreach (var e in value as Array)
                {
                    result.AddRange(GetValues<T>(e, depth - 1));
                }
                return result;
            });
        }
    }

    public static class Extensions
    {
        public static IEnumerable<T> Gather<T>(this IEnumerable<GameObject> rootGameObjects)
        {
            return rootGameObjects.SelectMany(g => g.GetComponentsInChildren<T>(true));
        }
    }
}
