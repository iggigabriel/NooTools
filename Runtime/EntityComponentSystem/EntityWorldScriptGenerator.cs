#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Noo.Tools
{
    public class EntityWorldScriptGenerator : ScriptGenerator
    {
        readonly string fileExtension;
        readonly EntityWorldDefinition script;

        public EntityWorldScriptGenerator(EntityWorldDefinition script, string fileExtension) : base(script.GetOutputFolder())
        {
            this.script = script;
            this.fileExtension = fileExtension;
        }

        public void Generate()
        {
            using (WriteFile($"{script.typePrefix}EntityComponents.{fileExtension}"))
            using (FileHeader())
            {
                Line($"public interface I{script.typePrefix}EntityComponent {{ }}");
                Space();

                foreach (var component in script.ActiveComponents)
                {
                    LineIf(!string.IsNullOrWhiteSpace(component.structAttributes), component.structAttributes);
                    Line($"[Serializable, StructLayout(LayoutKind.{component.structLayout})]");
                    using (Section($"public partial struct {component.typeName} : I{script.typePrefix}EntityComponent, IEquatable<{component.typeName}>"))
                    {
                        foreach (var variable in component.variables)
                        {
                            LineIf(!string.IsNullOrWhiteSpace(variable.tooltip), $"[Tooltip(\"{variable.tooltip}\")]");
                            LineIf(!string.IsNullOrWhiteSpace(variable.extraAttributes), variable.extraAttributes);
                            LineIf(component.structLayout == System.Runtime.InteropServices.LayoutKind.Explicit, $"[FieldOffset({variable.fieldOffset})]");
                            Line($"public {variable.type} {variable.name};");
                        }

                        Space();

                        var constructorParameters = component.variables
                            .OrderBy(x => x.defaultValue).ThenBy(x => Array.IndexOf(component.variables, x))
                            .Select(x => string.IsNullOrEmpty(x.defaultValue) ? $"{x.type} {x.name}" : $"{x.type} {x.name} = {x.defaultValue}");

                        var stringParameters = component.variables.Select(x => $"{x.name}: {{{x.name}}}");

                        var variableParameters = component.variables.Select(x => $"{x.name}");

                        using (Section($"public {component.typeName}({string.Join(", ", constructorParameters)})"))
                        {
                            foreach (var variable in component.variables)
                            {
                                Line($"this.{variable.name} = {variable.name};");
                            }
                        }

                        InlineMethod();
                        using (Section($"public readonly override bool Equals(object other)"))
                        {
                            Line($"return other is {component.typeName} other{component.name} && Equals(other{component.name});");
                        }

                        InlineMethod();
                        using (Section($"public readonly bool Equals({component.typeName} other{component.name})"))
                        {
                            Line($"return ({string.Join($" && ", variableParameters.Select(x => $"{x} == other{component.name}.{x}"))});");
                        }

                        InlineMethod();
                        using (Section($"public static bool operator ==({component.typeName} left, {component.typeName} right)"))
                        {
                            Line($"return left.Equals(right);");
                        }

                        InlineMethod();
                        using (Section($"public static bool operator !=({component.typeName} left, {component.typeName} right)"))
                        {
                            Line($"return !left.Equals(right);");
                        }

                        InlineMethod();
                        using (Section($"public readonly override int GetHashCode()"))
                        {
                            if (variableParameters.Count() <= 8)
                            {
                                Line($"return HashCode.Combine({string.Join(", ", variableParameters)});");
                            }
                            else
                            {
                                Line($"HashCode hash = new();");
                                foreach (var var in variableParameters) Line($"hash.Add({var});");
                                Line($"return hash.ToHashCode();");
                            }
                        }

                        using (Section($"public override readonly string ToString()"))
                        {
                            Line($"return $\"{component.typeName}({string.Join(", ", stringParameters)})\";");
                        }
                    }
                }
            }

            using (WriteFile($"{script.typePrefix}EntityManager.{fileExtension}"))
            using (FileHeader())
            {
                LineIf(!string.IsNullOrWhiteSpace(script.managerSettings.classAttributes), script.managerSettings.classAttributes);
                Line($"[HideMonoScript, AddComponentMenu(\"{script.name}/{script.typePrefix}EntityManager\"), DefaultExecutionOrder({script.managerSettings.scriptExecutionOrder})]");
                using (Section($"public partial class {script.typePrefix}EntityManager : {script.managerSettings.baseManagerClass}"))
                {
                    using (Section($"public enum EntityManagerUpdateState"))
                    {
                        Line("Idle, BeforeTick, Tick, AfterTick, BeforeDraw, Draw, AfterDraw");
                    }

                    Line($"public EntityManagerUpdateState UpdateState {{ get; private set; }}");

                    using (CommentBlock("Inspector"))
                    using (Conditional("UNITY_EDITOR"))
                    {
                        Line("[Title(\"Entities\")]");
                        Pragma("pragma warning disable IDE0051 // Remove unused private members");
                        foreach (var archeType in script.ActiveArchetypes)
                        {
                            Line($"[ShowInInspector, HideInPlayMode, DisplayAsString]");
                            Line($"private int Baked{archeType.typeName}Entities {{ get => GetComponentsInChildren<{script.typePrefix}{archeType.typeName}>(true).Length; set {{ }} }}");
                        }
                        Space();

                        Line($"[Button(DirtyOnClick = true, Icon = SdfIconType.ArrowRepeat, Name = \"Refresh\"), HideInPlayMode]");
                        using (Section($"protected {Choose(script.managerSettings.markOnValidateAsOverride, "override ")}void OnValidate()"))
                        {
                            LineIf(script.managerSettings.markOnValidateAsOverride, "base.OnValidate();");
                            using (Section($"foreach (var entity in GetComponentsInChildren<{script.typePrefix}Entity>(true).Where(x => x.entityManager != this))"))
                            {
                                Line($"entity.entityManager = this;");
                                Line($"UnityEditor.EditorUtility.SetDirty(entity);");
                            }
                            using (Section("if (gameObject.scene.IsValid() && gameObject.scene.isLoaded)"))
                            {
                                Line($"var sceneEntities = gameObject.scene.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<{script.typePrefix}Entity>(true));");
                                using (Section($"foreach (var entity in sceneEntities.Where(x => x.entityManager == this && !x.transform.IsChildOf(transform)))"))
                                {
                                    Line($"entity.entityManager = null;");
                                    Line($"UnityEditor.EditorUtility.SetDirty(entity);");
                                }
                            }
                        }

                        Line($"[ListDrawerSettings(ShowFoldout = false, ShowPaging = false), PropertyOrder(-1)]");
                        Line($"[PropertySpace, ShowInInspector, DisplayAsString, LabelText(\"Systems\", Icon = SdfIconType.MotherboardFill)]");
                        Line($"private List<string> InspectorSystemsDisplay {{ get => InspectorSystems.Select(x => $\"{{x.GetType().GetNameNonAlloc()}} [Priority: {{x.SystemExecutionOrder}}] {{(x.isActiveAndEnabled ? \"\" : \"(Disabled)\")}}\").ToList(); }}");
                        Line($"private List<{script.typePrefix}EntitySystem> InspectorSystems {{ get => Application.isPlaying ? systems : GetComponentsInChildren<{script.typePrefix}EntitySystem>(true).OrderBy(x => x.SystemExecutionOrder).ToList(); }}");
                        Pragma("pragma warning restore IDE0051 // Remove unused private members");
                    }

                    Line($"private int entityIdCounter = 0;");
                    Space();

                    Line($"[Title(\"Runtime\"), HideInEditorMode, ShowInInspector, DisplayAsString, LabelText(\"Tick\", Icon = SdfIconType.ClockHistory)]");
                    Line($"public int Tick {{ get; private set; }}");
                    Line($"public Sfloat Time {{ get; private set; }}");
                    Space();

                    Pragma("pragma warning disable IDE0051 // Remove unused private members");
                    Line($"[ShowInInspector, HideInEditorMode, DisplayAsString, LabelText(\"Time\", Icon = SdfIconType.ClockFill)]");
                    Line($"private string TimeDisplay => TimeSpan.FromSeconds(Time.Float).ToString(@\"mm\\:ss\\:ff\");");
                    Pragma("pragma warning restore IDE0051 // Remove unused private members");
                    Space();

                    Line($"Scene entitiesScene;");
                    Line($"bool initialized;");
                    Line($"bool structuralChangesLocked;");
                    Line($"private JobHandle jobHandle;");
                    Line($"private bool jobsQueued;");
                    Line($"internal JobHandle JobHandle {{ get {{ return jobHandle; }} set {{ jobsQueued = true; jobHandle = value; }} }}");
                    Line($"private JobHandle disposeJobHandle;");
                    Line($"private bool disposeJobsQueued;");
                    Line($"internal JobHandle DisposeJobHandle {{ get {{ return disposeJobHandle; }} set {{ disposeJobsQueued = true; disposeJobHandle = value; }} }}");
                    Line($"List<{script.typePrefix}EntitySystem> systems;");
                    Line($"readonly List<{script.typePrefix}EntitySystem> activeSystems = new();");
                    Line($"Dictionary<Type, {script.typePrefix}EntitySystem> systemsByType;");
                    Space();

                    Line("[Title(\"Entities\")]");
                    foreach (var archetype in script.ActiveArchetypes)
                    {
                        Line($"[ShowInInspector, ProgressBar(0, \"@array{archetype.typeName}.Length\"), HideInEditorMode]");
                        Line($"public int {archetype.typeName}Count => count{archetype.typeName};");
                        Line($"public int {archetype.typeName}Capacity => array{archetype.typeName}.Length;");
                        Line($"private {script.typePrefix}{archetype.typeName}[] array{archetype.typeName};");
                        Summary($"Has any {archetype.typeName}s been added or removed last frame.");
                        Line($"public bool {archetype.typeName}ListUpdated {{ get; private set; }}");
                        Line($"private int count{archetype.typeName};");

                        if (archetype.needsTransformAccess)
                        {
                            Line($"private TransformAccessArray transformAccessArray{archetype.typeName};");
                        }

                        foreach (var component in archetype.componentDefinitions)
                        {
                            Line($"internal {component.TypeName}[] component{archetype.typeName}{component.name};");
                        }

                        foreach (var buffer in archetype.componentBuffers)
                        {
                            Line($"internal {buffer.TypeName}[] buffer{archetype.typeName}{buffer.name};");
                            Line($"internal int[] bufferCounts{archetype.typeName}{buffer.name};");
                        }

                        Line($"public event Action<{script.typePrefix}{archetype.typeName}> On{archetype.typeName}Added;");
                        Line($"public event Action<{script.typePrefix}{archetype.typeName}> On{archetype.typeName}BeforeRemove;");

                        Space();
                    }

                    using (Section($"protected {Choose(script.managerSettings.markAwakeAsOverride, "override ")}void Awake()"))
                    {
                        LineIf(script.managerSettings.markAwakeAsOverride, "base.Awake();");
                        Line($"Initialize();");
                    }

                    using (Section($"public void Initialize()"))
                    {
                        Line($"if (initialized) return;");

                        Line($"initialized = true;");

                        Line($"entitiesScene = SceneManager.CreateScene($\"{script.typePrefix}World [{{GetInstanceID():X}}]\");");

                        foreach (var archetype in script.ActiveArchetypes)
                        {
                            Line($"array{archetype.typeName} = new {script.typePrefix}{archetype.typeName}[{archetype.initialDataCapacity}];");

                            if (archetype.needsTransformAccess)
                            {
                                Line($"transformAccessArray{archetype.typeName} = new({archetype.initialDataCapacity});");
                            }

                            foreach (var component in archetype.componentDefinitions)
                            {
                                Line($"component{archetype.typeName}{component.name} = new {component.TypeName}[{archetype.initialDataCapacity}];");
                            }

                            foreach (var buffer in archetype.componentBuffers)
                            {
                                Line($"buffer{archetype.typeName}{buffer.name} = new {buffer.TypeName}[{archetype.initialDataCapacity * buffer.maxCapacity}];");
                                Line($"bufferCounts{archetype.typeName}{buffer.name} = new int[{archetype.initialDataCapacity}];");
                            }
                        }
                        Space();

                        Line($"systems = new();");
                        Line($"GetComponentsInChildren(true, systems);");
                        Line($"systems.Sort((a, b) => a.SystemExecutionOrder.CompareTo(b.SystemExecutionOrder));");
                        Line($"systemsByType = systems.ToDictionary(x => x.GetType());");
                    }

                    using (Section($"public void OnTick(Sfloat deltaTime)"))
                    {
                        Line($"Tick++;");
                        Line($"Time += deltaTime;");

                        Line($"structuralChangesLocked = true;");
                        Line($"activeSystems.Clear();");
                        using (Section("for (int i = 0; i < systems.Count; i++)"))
                        {
                            Line($"var system = systems[i];");
                            Line($"if (system && system.isActiveAndEnabled) activeSystems.Add(system);");
                        }
                        using (ProfileSample("EntityManager OnBeforeTick"))
                        {
                            Line("UpdateState = EntityManagerUpdateState.BeforeTick;");

                            using (Section("for (int i = 0; i < activeSystems.Count; i++)"))
                            {
                                using (ProfileSample("{activeSystems[i].GetType().GetNameNonAlloc()}"))
                                {
                                    Line($"activeSystems[i].OnBeforeTick(deltaTime);");
                                }
                            }
                            Line("CompleteJobs();");
                        }
                        using (ProfileSample("EntityManager OnTick"))
                        {
                            Line("UpdateState = EntityManagerUpdateState.Tick;");

                            using (Section("for (int i = 0; i < activeSystems.Count; i++)"))
                            {
                                using (ProfileSample("{activeSystems[i].GetType().GetNameNonAlloc()}"))
                                {
                                    Line($"activeSystems[i].OnTick(deltaTime);");
                                }
                            }
                            Line("CompleteJobs();");
                        }
                        using (ProfileSample("EntityManager OnAfterTick"))
                        {
                            Line("UpdateState = EntityManagerUpdateState.AfterTick;");

                            using (Section("for (int i = 0; i < activeSystems.Count; i++)"))
                            {
                                using (ProfileSample("{activeSystems[i].GetType().GetNameNonAlloc()}"))
                                {
                                    Line($"activeSystems[i].OnAfterTick(deltaTime);");
                                }
                            }
                            Line("CompleteJobs();");
                        }
                        Line($"structuralChangesLocked = false;");
                    }

                    using (Section($"public void OnDraw(float lerpTime)"))
                    {
                        Line($"structuralChangesLocked = true;");
                        Line($"activeSystems.Clear();");
                        using (Section("for (int i = 0; i < systems.Count; i++)"))
                        {
                            Line($"var system = systems[i];");
                            Line($"if (system && system.isActiveAndEnabled) activeSystems.Add(system);");
                        }
                        using (ProfileSample("EntityManager OnBeforeDraw"))
                        {
                            Line("UpdateState = EntityManagerUpdateState.BeforeDraw;");

                            using (Section("for (int i = 0; i < activeSystems.Count; i++)"))
                            {
                                using (ProfileSample("{activeSystems[i].GetType().GetNameNonAlloc()}"))
                                {
                                    Line($"activeSystems[i].OnBeforeDraw(lerpTime);");
                                }
                            }
                            Line("CompleteJobs();");
                        }
                        using (ProfileSample("EntityManager OnDraw"))
                        {
                            Line("UpdateState = EntityManagerUpdateState.Draw;");

                            using (Section("for (int i = 0; i < activeSystems.Count; i++)"))
                            {
                                using (ProfileSample("{activeSystems[i].GetType().GetNameNonAlloc()}"))
                                {
                                    Line($"activeSystems[i].OnDraw(lerpTime);");
                                }
                            }
                            Line("CompleteJobs();");
                        }
                        using (ProfileSample("EntityManager OnAfterDraw"))
                        {
                            Line("UpdateState = EntityManagerUpdateState.AfterDraw;");

                            using (Section("for (int i = 0; i < activeSystems.Count; i++)"))
                            {
                                using (ProfileSample("{activeSystems[i].GetType().GetNameNonAlloc()}"))
                                {
                                    Line($"activeSystems[i].OnAfterDraw(lerpTime);");
                                }
                            }
                            Line("CompleteJobs();");
                        }
                        Line($"structuralChangesLocked = false;");
                        Line("UpdateState = EntityManagerUpdateState.Idle;");
                    }

                    Line($"public T GetSystem<T>() where T : {script.typePrefix}EntitySystem => (systemsByType.TryGetValue(typeof(T), out var system)) ? system as T : default;");

                    using (Section("public void ScheduleJobs()"))
                    {
                        Line($"JobHandle.ScheduleBatchedJobs();");
                    }

                    Line($"[Conditional(\"UNITY_EDITOR\")]");
                    using (Section("public void AssertState(EntityManagerUpdateState state)"))
                    {
                        Line($"if (UpdateState != state) throw new Exception(\"Expecting Entity Manager to be in {{state}} state, but its not.\");");
                    }

                    using (Section("public void CompleteJobs()"))
                    {
                        using (Section("if (jobsQueued)"))
                        {
                            Line($"jobHandle.Complete();");
                            Line($"jobHandle = default;");
                            Line($"jobsQueued = false;");
                        }
                        using (Section("if (disposeJobsQueued)"))
                        {
                            Line($"disposeJobHandle.Complete();");
                            Line($"disposeJobHandle = default;");
                            Line($"disposeJobsQueued = false;");
                        }
                    }

                    using (Section($"public void InitEntity({script.typePrefix}Entity entity)"))
                    {
                        Line($"if (entity.entityManager != null) throw new Exception(\"Entity already managed.\");");
                        Line($"entity.entityManager = this;");
                    }

                    Line($"readonly List<{script.typePrefix}Entity> subentityHelper = new();");
                    Space();

                    using (Section($"public void InitSubentities({script.typePrefix}Entity entity)"))
                    {
                        Line($"entity.GetComponentsInChildren(true, subentityHelper);");
                        Line($"foreach (var subentity in subentityHelper) subentity.entityManager = this;");
                    }

                    using (Section($"public T CreateEntity<T>(T prefab, bool initSubentities = false) where T : {script.typePrefix}Entity"))
                    {
                        Line($"if (prefab.isActiveAndEnabled) throw new Exception(\"Entity prefabs should not be active objects.\");");

                        Line($"var entity = Instantiate(prefab, null, false);");

                        Line($"if (initSubentities) InitSubentities(entity);");
                        Line($"else entity.entityManager = this;");

                        Line($"SceneManager.MoveGameObjectToScene(entity.gameObject, entitiesScene);");
                        Line($"return entity;");
                    }

                    foreach (var archetype in script.ActiveArchetypes)
                    {
                        using (Section($"public ReadOnlySpan<{script.typePrefix}{archetype.typeName}> Get{archetype.typeName}s()"))
                        {
                            Line($"return new ReadOnlySpan<{script.typePrefix}{archetype.typeName}>(array{archetype.typeName}, 0, count{archetype.typeName});");
                        }

                        using (Section($"internal void Register{archetype.typeName}({script.typePrefix}{archetype.typeName} entity)"))
                        {
                            Line($"if (structuralChangesLocked) throw new Exception(\"Not allowed to add entities during systems update.\");");

                            Line($"if (entity.entityRef != -1) return;");
                            Space();
                            Line($"entity.UniqueId = ++entityIdCounter;");
                            Space();

                            if (archetype.deparentTransformOnRegister)
                            {
                                Line($"entity.transform.SetParent(null, false);");
                                Line($"SceneManager.MoveGameObjectToScene(entity.gameObject, entitiesScene);");
                                Space();
                            }

                            Line($"var index = count{archetype.typeName}++;");
                            Line($"var capacity = array{archetype.typeName}.Length;");

                            using (Section($"if (capacity < count{archetype.typeName})"))
                            {
                                Line($"var nextCapacity = Mathf.NextPowerOfTwo(count{archetype.typeName});");
                                Space();
                                Line($"Array.Resize(ref array{archetype.typeName}, nextCapacity);");

                                foreach (var component in archetype.componentDefinitions)
                                {
                                    Line($"Array.Resize(ref component{archetype.typeName}{component.name}, nextCapacity);");
                                }

                                foreach (var buffer in archetype.componentBuffers)
                                {
                                    Line($"Array.Resize(ref buffer{archetype.typeName}{buffer.name}, nextCapacity * {script.typePrefix}{archetype.typeName}.Max{buffer.name}Count);");
                                    Line($"Array.Resize(ref bufferCounts{archetype.typeName}{buffer.name}, nextCapacity);");
                                }
                            }

                            Line($"entity.entityRef = index;");
                            Space();

                            Line($"array{archetype.typeName}[index] = entity;");
                            Space();

                            if (archetype.needsTransformAccess)
                            {
                                Line($"transformAccessArray{archetype.typeName}.Add(entity.transform);");
                            }

                            foreach (var component in archetype.componentDefinitions)
                            {
                                Line($"component{archetype.typeName}{component.name}[index] = entity.initial{component.name};");
                            }

                            foreach (var buffer in archetype.componentBuffers)
                            {
                                Line($"{script.typePrefix}{archetype.typeName}.Get{buffer.name}Slice(buffer{archetype.typeName}{buffer.name}, bufferCounts{archetype.typeName}{buffer.name}, index).SetData(entity.initial{buffer.name}Buffer);");
                            }

                            Space();
                            Line($"On{archetype.typeName}Added?.Invoke(entity);");
                        }

                        using (Section($"internal void Unregister{archetype.typeName}({script.typePrefix}{archetype.typeName} entity)"))
                        {
                            Line($"if (structuralChangesLocked) throw new Exception(\"Not allowed to remove entities during systems update.\");");
                            Line($"if (entity.entityRef == -1) return;");
                            Space();
                            Line($"On{archetype.typeName}BeforeRemove?.Invoke(entity);");
                            Space();
                            Line($"count{archetype.typeName}--;");
                            Space();
                            Line($"var index = entity.entityRef;");
                            Line($"var lastEntity = array{archetype.typeName}[count{archetype.typeName}];");
                            Space();
                            Line($"array{archetype.typeName}[index] = lastEntity;");

                            foreach (var component in archetype.componentDefinitions)
                            {
                                Line($"component{archetype.typeName}{component.name}[index] = component{archetype.typeName}{component.name}[count{archetype.typeName}];");
                            }

                            foreach (var buffer in archetype.componentBuffers)
                            {
                                Line($"var last{buffer.name}DataCount = bufferCounts{archetype.typeName}{buffer.name}[count{archetype.typeName}];");
                                Line($"for(var i = 0; i < last{buffer.name}DataCount; i++) buffer{archetype.typeName}{buffer.name}[index * {buffer.maxCapacity} + i] = buffer{archetype.typeName}{buffer.name}[count{archetype.typeName} * {buffer.maxCapacity} + i];");
                                Line($"bufferCounts{archetype.typeName}{buffer.name}[index] = last{buffer.name}DataCount;");
                            }

                            Space();

                            if (archetype.needsTransformAccess)
                            {
                                Line($"transformAccessArray{archetype.typeName}.RemoveAtSwapBack(index);");
                                Space();
                            }

                            Line($"lastEntity.entityRef = index;");
                            Line($"entity.entityRef = -1;");
                            Line($"entity.UniqueId = -1;");
                        }

                        foreach (var component in archetype.componentDefinitions)
                        {
                            using (Section($"public NativeArray<{component.TypeName}> Get{archetype.typeName}{component.name}Components(Allocator allocator = Allocator.TempJob)"))
                            {
                                Line($"var data = new NativeArray<{component.TypeName}>(count{archetype.typeName}, allocator, NativeArrayOptions.UninitializedMemory);");
                                Line($"NativeArray<{component.TypeName}>.Copy(component{archetype.typeName}{component.name}, 0, data, 0, count{archetype.typeName});");
                                Line($"return data;");
                            }

                            using (Section($"public void Update{archetype.typeName}{component.name}Components(NativeArray<{component.TypeName}> data)"))
                            {
                                Line($"if (data.Length != count{archetype.typeName}) throw new Exception(\"Source array is invalid. It needs to be the same size as internal data container.\");");
                                Line($"NativeArray<{component.TypeName}>.Copy(data, 0, component{archetype.typeName}{component.name}, 0, count{archetype.typeName});");
                            }
                        }

                        foreach (var buffer in archetype.componentBuffers)
                        {
                            using (Section($"public void Get{archetype.typeName}{buffer.name}Buffer(out NativeArray<{buffer.TypeName}> data, out NativeArray<int> dataCounts, Allocator allocator = Allocator.TempJob)"))
                            {
                                Line($"var dataCount = count{archetype.typeName} * {buffer.maxCapacity};");
                                Line($"data = new NativeArray<{buffer.TypeName}>(dataCount, allocator, NativeArrayOptions.UninitializedMemory);");
                                Line($"NativeArray<{buffer.TypeName}>.Copy(buffer{archetype.typeName}{buffer.name}, 0, data, 0, dataCount);");

                                Line($"dataCounts = new NativeArray<int>(count{archetype.typeName}, allocator, NativeArrayOptions.UninitializedMemory);");
                                Line($"NativeArray<int>.Copy(bufferCounts{archetype.typeName}{buffer.name}, 0, dataCounts, 0, count{archetype.typeName});");
                            }

                            using (Section($"public void Update{archetype.typeName}{buffer.name}Buffer(NativeArray<{buffer.TypeName}> data, NativeArray<int> dataCounts)"))
                            {
                                Line($"var dataCount = count{archetype.typeName} * {buffer.maxCapacity};");
                                Line($"if (data.Length != dataCount || dataCounts.Length != count{archetype.typeName}) throw new Exception(\"Source arrays are invalid. They need to be the same size as internal data containers.\");");
                                Line($"NativeArray<{buffer.TypeName}>.Copy(data, 0, buffer{archetype.typeName}{buffer.name}, 0, dataCount);");
                                Line($"NativeArray<int>.Copy(dataCounts, 0, bufferCounts{archetype.typeName}{buffer.name}, 0, count{archetype.typeName});");
                            }
                        }

                        if (archetype.needsTransformAccess)
                        {
                            Line($"public TransformAccessArray Get{archetype.typeName}TransformAccessArray() => transformAccessArray{archetype.typeName};");
                            Space();
                        }
                    }

                    using (Section($"protected {Choose(script.managerSettings.markOnDisableAsOverride, "override ")}void OnDestroy()"))
                    {
                        LineIf(script.managerSettings.markOnDisableAsOverride, "base.OnDestroy();");
                        Line($"if (entitiesScene.isLoaded) SceneManager.UnloadSceneAsync(entitiesScene);");
                        Space();

                        foreach (var archetype in script.ActiveArchetypes)
                        {
                            if (archetype.needsTransformAccess)
                            {
                                Line($"transformAccessArray{archetype.typeName}.Dispose();");
                            }
                        }
                    }

                    using (Conditional("UNITY_EDITOR"))
                    {
                        Line($"[UnityEditor.MenuItem(\"Tools/NooEntities/{script.name}\")]");
                        using (Section($"private static void SelectEntityWorldMainAsset()"))
                        {
                            Line($"var mainAsset = AssetDatabaseUtility.FindAssetWithName<EntityWorldDefinition>(\"{script.name}\");");
                            Line($"UnityEditor.Selection.activeObject = mainAsset;");
                        }
                    }
                }
            }

            using (WriteFile($"{script.typePrefix}EntitySystem.{fileExtension}"))
            using (FileHeader())
            {
                LineIf(!string.IsNullOrWhiteSpace(script.entitySystemSettings.classAttributes), script.entitySystemSettings.classAttributes);
                Line($"[HideMonoScript, DefaultExecutionOrder({script.entitySystemSettings.scriptExecutionOrder})]");
                using (Section($"public abstract partial class {script.typePrefix}EntitySystem : {script.entitySystemSettings.baseSystemClass}"))
                {
                    Line($"[SerializeField]");
                    Line($"private {script.typePrefix}EntityManager entityManager;");
                    Line($"protected {script.typePrefix}EntityManager Entities => entityManager;");
                    Space();

                    Space();
                    Line($"public abstract int SystemExecutionOrder {{ get; }}");
                    Space();
                    Line($"public virtual void OnTick(Sfloat deltaTime) {{ }}");
                    Line($"public virtual void OnDraw(float lerpTime) {{ }}");
                    Line($"public virtual void OnBeforeDraw(float lerpTime) {{ }}");
                    Line($"public virtual void OnAfterDraw(float lerpTime) {{ }}");
                    Line($"public virtual void OnBeforeTick(Sfloat deltaTime) {{ }}");
                    Line($"public virtual void OnAfterTick(Sfloat deltaTime) {{ }}");
                    Space();

                    using (Conditional("UNITY_EDITOR"))
                    using (Section($"protected {Choose(script.entitySystemSettings.markOnResetAsOverride, "override ")}void Reset()"))
                    {
                        LineIf(script.entitySystemSettings.markOnResetAsOverride, "base.Reset();");

                        Line($"entityManager = GetComponentInParent<{script.typePrefix}EntityManager>();");
                    }
                }
            }

            using (WriteFile($"{script.typePrefix}Entity.{fileExtension}"))
            using (FileHeader())
            {
                LineIf(!string.IsNullOrWhiteSpace(script.entitySettings.classAttributes), script.entitySettings.classAttributes);
                Line($"[ExecuteAlways, HideMonoScript]");
                using (Section($"public abstract partial class {script.typePrefix}Entity : {script.entitySettings.baseEntityClass}"))
                {
                    Line($"[SerializeField, PropertyOrder(-200)]");
                    Line($"internal {script.typePrefix}EntityManager entityManager;");
                    Line($"internal int entityRef = -1;");
                    Line($"[ShowInInspector, HideInEditorMode, PropertyOrder(-199), DisplayAsString]");
                    Line($"public int UniqueId {{ get; internal set; }} = -1;");
                    Space();
                    Line($"public bool IsCreated => entityRef != -1;");

                    using (Conditional("UNITY_EDITOR"))
                    using (Section($"protected {Choose(script.entitySettings.markOnResetAsOverride, "override ")}void Reset()"))
                    {
                        LineIf(script.entitySettings.markOnResetAsOverride, "base.Reset();");

                        Line($"entityManager = GetComponentInParent<{script.typePrefix}EntityManager>();");
                    }
                }
            }

            foreach (var archetype in script.ActiveArchetypes)
            {
                using (WriteFile($"{script.typePrefix}{archetype.typeName}.{fileExtension}"))
                using (FileHeader())
                {
                    LineIf(!string.IsNullOrWhiteSpace(archetype.classAttributes), archetype.classAttributes);
                    Line($"[AddComponentMenu(\"{script.name}/{archetype.typeName}\"), SelectionBase]");
                    using (Section($"public partial class {script.typePrefix}{archetype.typeName} : {script.typePrefix}Entity"))
                    {
                        using (CommentBlock("Serialization"))
                        {
                            foreach (var component in archetype.componentDefinitions)
                            {
                                LineIf(!string.IsNullOrWhiteSpace(component.propertyAttributes), component.propertyAttributes);
                                Line($"[SerializeField, HideInInspector]");
                                Line($"internal {component.TypeName} initial{component.name};");
                                Summary(component.description);
                                Line($"public ref {component.TypeName} Initial{component.name} => ref initial{component.name};");
                            }

                            foreach (var buffer in archetype.componentBuffers)
                            {
                                LineIf(!string.IsNullOrWhiteSpace(buffer.propertyAttributes), buffer.propertyAttributes);
                                Line($"[SerializeField, HideInInspector]");
                                Line($"internal {buffer.TypeName}[] initial{buffer.name}Buffer = new {buffer.TypeName}[0];");
                            }
                        }

                        using (CommentBlock("Components"))
                        {
                            foreach (var component in archetype.componentDefinitions)
                            {
                                Summary(component.description);
                                Line($"public ref {component.TypeName} {component.name} => ref entityManager.component{archetype.typeName}{component.name}[entityRef];");
                            }

                            foreach (var buffer in archetype.componentBuffers)
                            {
                                Line($"public const int Max{buffer.name}Count = {buffer.maxCapacity};");
                                Summary(buffer.description);
                                Line($"public ArrayBufferSlice<{buffer.TypeName}> {buffer.name} => Get{buffer.name}Slice(entityManager.buffer{archetype.typeName}{buffer.name}, entityManager.bufferCounts{archetype.typeName}{buffer.name}, entityRef);");
                            }
                        }

                        using (CommentBlock("Inspector"))
                        using (Conditional("UNITY_EDITOR"))
                        {
                            Pragma("pragma warning disable IDE0051 // Remove unused private members");
                            foreach (var component in archetype.componentDefinitions.Where(x => !x.hideInInspector && x.hideLabel))
                            {
                                LineIf(!string.IsNullOrWhiteSpace(component.inspectorDrawerAttributes), component.inspectorDrawerAttributes);
                                Line($"[Title(\"@Inspector{component.name}DataTitle\"), HideLabel, ShowInInspector, EnableIf(\"Inspector{component.name}DataEnabled\"), Tooltip(\"{component.description}\"), PropertyOrder(-100)]");
                                Line($"private {component.TypeName} Inspector{component.name}Drawer {{ get => {component.name}EditorSafe; set => {component.name}EditorSafe = value; }}");
                                Line($"internal ref {component.TypeName} {component.name}EditorSafe => ref entityRef == -1 ? ref initial{component.name} : ref {component.name};");
                                Line($"private string Inspector{component.name}DataTitle => Inspector{component.name}DataEnabled ? \"{component.name}\" : $\"{component.name} (Controlled by {{GetComponent<I{script.typePrefix}{archetype.typeName}{component.name}Baker>().GetType().GetNameNonAlloc()}})\";");
                                Line($"private bool Inspector{component.name}DataEnabled => entityRef != -1 || !TryGetComponent<I{script.typePrefix}{archetype.typeName}{component.name}Baker>(out var _);");
                            }
                            LineIf(archetype.componentDefinitions.Where(x => !x.hideInInspector && !x.hideLabel).Any(), $"[Title(\"Other\")]");
                            foreach (var component in archetype.componentDefinitions.Where(x => !x.hideInInspector && !x.hideLabel))
                            {
                                LineIf(!string.IsNullOrWhiteSpace(component.inspectorDrawerAttributes), component.inspectorDrawerAttributes);
                                Line($"[LabelText(\"{component.name}\"), ShowInInspector, EnableIf(\"Inspector{component.name}DataEnabled\"), Tooltip(\"{component.description}\"), PropertyOrder(-100)]");
                                Line($"private {component.TypeName} Inspector{component.name}Drawer {{ get => {component.name}EditorSafe; set => {component.name}EditorSafe = value; }}");
                                Line($"internal ref {component.TypeName} {component.name}EditorSafe => ref entityRef == -1 ? ref initial{component.name} : ref {component.name};");
                                Line($"private string Inspector{component.name}DataTitle => Inspector{component.name}DataEnabled ? \"{component.name}\" : $\"{component.name} (Controlled by {{GetComponent<I{script.typePrefix}{archetype.typeName}{component.name}Baker>().GetType().GetNameNonAlloc()}})\";");
                                Line($"private bool Inspector{component.name}DataEnabled => entityRef != -1 || !TryGetComponent<I{script.typePrefix}{archetype.typeName}{component.name}Baker>(out var _);");
                            }
                            foreach (var buffer in archetype.componentBuffers.Where(x => !x.hideInInspector))
                            {
                                Line($"private string Inspector{buffer.name}BufferTitle => Inspector{buffer.name}BufferEnabled ? \"{buffer.name}\" : $\"{buffer.name} (Controlled by {{GetComponent<I{script.typePrefix}{archetype.typeName}{buffer.name}Baker>().GetType().GetNameNonAlloc()}})\";");
                                Line($"private bool Inspector{buffer.name}BufferEnabled => entityRef != -1 || !TryGetComponent<I{script.typePrefix}{archetype.typeName}{buffer.name}Baker>(out var _);");
                                LineIf(!string.IsNullOrWhiteSpace(buffer.inspectorDrawerAttributes), buffer.inspectorDrawerAttributes);
                                Line($"[Title(\"@Inspector{buffer.name}BufferTitle\"), ShowInInspector, LabelText(\" \"), EnableIf(\"@Inspector{buffer.name}BufferEnabled\"), Tooltip(\"{buffer.description}\"), PropertyOrder(-100)]");
                                Line($"[RequiredListLength(MaxLength = {buffer.maxCapacity})]");
                                using (Section($"internal {buffer.TypeName}[] Inspector{buffer.name}EditorSafe"))
                                {
                                    Line($"get => entityRef == -1 ? initial{buffer.name}Buffer : {buffer.name}.ToArray();");
                                    Line($"set {{ if (entityRef == -1) initial{buffer.name}Buffer = value; else {buffer.name}.SetData(value); }}");
                                }
                            }
                            Pragma("pragma warning restore IDE0051 // Remove unused private members");
                        }

                        if (archetype.componentBuffers.Any())
                        {
                            using (CommentBlock("Static Utility"))
                            {
                                foreach (var buffer in archetype.componentBuffers)
                                {
                                    InlineMethod();
                                    using (Section($"public static ArrayBufferSlice<{buffer.TypeName}> Get{buffer.name}Slice({buffer.TypeName}[] data, int[] dataCounts, int index)"))
                                    {
                                        Line($"return new ArrayBufferSlice<{buffer.TypeName}>(data, dataCounts, Max{buffer.name}Count, index);");
                                    }

                                    InlineMethod();
                                    using (Section($"public static NativeArrayBufferSlice<{buffer.TypeName}> GetNative{buffer.name}Slice(NativeArray<{buffer.TypeName}>.ReadOnly data, NativeArray<int>.ReadOnly dataCounts, int index, Allocator allocator = Allocator.TempJob)"))
                                    {
                                        Line($"return new NativeArrayBufferSlice<{buffer.TypeName}>(data, dataCounts, Max{buffer.name}Count, index, allocator);");
                                    }

                                    InlineMethod();
                                    using (Section($"public static NativeArrayBufferSlice<{buffer.TypeName}> GetNative{buffer.name}Slice(NativeArray<{buffer.TypeName}> data, NativeArray<int> dataCounts, int index, Allocator allocator = Allocator.TempJob)"))
                                    {
                                        Line($"return new NativeArrayBufferSlice<{buffer.TypeName}>(data.AsReadOnly(), dataCounts.AsReadOnly(), Max{buffer.name}Count, index, allocator);");
                                    }

                                    Summary("Returns only SubArray for reading data. There is no allocations and need to dispose this array.");
                                    InlineMethod();
                                    using (Section($"public static NativeArray<{buffer.TypeName}>.ReadOnly GetNative{buffer.name}SliceReadOnly(NativeArray<{buffer.TypeName}> data, NativeArray<int> dataCounts, int index)"))
                                    {
                                        Line($"return data.GetSubArray(Max{buffer.name}Count * index, dataCounts[index]).AsReadOnly();");
                                    }
                                }
                            }
                        }


                        using (Conditional("UNITY_EDITOR"))
                        using (Section($"protected {Choose(script.entitySettings.markUpdateAsOverride, "override ")}void Update()"))
                        {
                            Line($"if (entityRef != -1) return;");

                            LineIf(script.entitySettings.markOnEnableAsOverride, "if (Application.isPlaying) base.Update();");

                            foreach (var component in archetype.componentDefinitions)
                            {
                                using (Section($"if (TryGetComponent<I{script.typePrefix}{archetype.typeName}{component.name}Baker>(out var bake{component.name}))"))
                                {
                                    Line($"var data = bake{component.name}.Get{archetype.typeName}{component.name}();");
                                    using (Section($"if (!initial{component.name}.Equals(data))"))
                                    {
                                        Line($"initial{component.name} = data;");
                                        Line($"UnityEditor.EditorUtility.SetDirty(this);");
                                    }
                                }
                            }

                            foreach (var buffer in archetype.componentBuffers)
                            {
                                using (Section($"if (TryGetComponent<I{script.typePrefix}{archetype.typeName}{buffer.name}Baker>(out var bake{buffer.name}))"))
                                {
                                    Line($"var data = bake{buffer.name}.Get{archetype.typeName}{buffer.name}();");
                                    using (Section($"if (!initial{buffer.name}Buffer.AsSpan().SequenceEqual(data))"))
                                    {
                                        Line($"initial{buffer.name}Buffer = data.ToArray();");
                                        Line($"UnityEditor.EditorUtility.SetDirty(this);");
                                    }
                                }
                            }
                        }

                        using (Section($"protected {Choose(script.entitySettings.markOnEnableAsOverride, "override ")}void OnEnable()"))
                        {
                            LineIf(script.entitySettings.markOnEnableAsOverride, "base.OnEnable();");

                            using (Conditional("UNITY_EDITOR"))
                            {
                                Line($"if (!Application.isPlaying) return;");
                            }

                            Line($"if (entityManager) entityManager.Register{archetype.typeName}(this);");
                        }

                        using (Section($"protected {Choose(script.entitySettings.markOnDisableAsOverride, "override ")}void OnDisable()"))
                        {
                            LineIf(script.entitySettings.markOnDisableAsOverride, "base.OnDisable();");

                            using (Conditional("UNITY_EDITOR"))
                            {
                                Line($"if (!Application.isPlaying) return;");
                            }

                            Line($"if (entityManager) entityManager.Unregister{archetype.typeName}(this);");
                        }
                    }

                    foreach (var component in archetype.componentDefinitions)
                    {
                        using (Section($"public interface I{script.typePrefix}{archetype.typeName}{component.name}Baker"))
                        {
                            Line($"{component.TypeName} Get{archetype.typeName}{component.name}();");
                        }
                    }

                    foreach (var buffer in archetype.componentBuffers)
                    {
                        using (Section($"public interface I{script.typePrefix}{archetype.typeName}{buffer.name}Baker"))
                        {
                            Line($"ReadOnlySpan<{buffer.TypeName}> Get{archetype.typeName}{buffer.name}();");
                        }
                    }
                }
            }
        }

        void InlineMethod() => Line("[MethodImpl(MethodImplOptions.AggressiveInlining)]");

        public readonly struct ProfilerSampleScope : IDisposable
        {
            readonly ScriptGenerator generator;

            public ProfilerSampleScope(ScriptGenerator generator, string sample)
            {
                this.generator = generator;

                using (generator.Conditional("UNITY_EDITOR || DEVELOPMENT_BUILD"))
                {
                    generator.Line($"Profiler.BeginSample($\"{sample}\");");
                }
            }

            public void Dispose()
            {
                using (generator.Conditional("UNITY_EDITOR || DEVELOPMENT_BUILD"))
                {
                    generator.Line($"Profiler.EndSample();");
                }
            }
        }

        ProfilerSampleScope ProfileSample(string sample)
        {
            return new ProfilerSampleScope(this, sample);
        }

        BracesLevel FileHeader()
        {
            Comment($"Auto generated script, do not modify!");
            Comment($"");
            Comment($"Asset: \"/{AssetDatabase.GetAssetPath(script)}\"");
            Comment($"Unity: {Application.unityVersion}");
            Comment($".NET: {Environment.Version}");
            Space();

            foreach (var @using in script.ActiveUsings) Line($"using {@using};");
            Space();

            return Section($"namespace {script.@namespace}");
        }
    }
}
#endif