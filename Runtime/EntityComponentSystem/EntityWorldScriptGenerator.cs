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
            using (WriteFile($"{script.typePrefix}EntityDeltaJobs.{fileExtension}"))
            using (FileHeader())
            {
                var deltableTypes = script.ActiveArchetypes
                    .SelectMany(x => x.componentDefinitions)
                    .Where(x => x.deltable)
                    .Select(x => x.TypeName)
                    .Distinct();

                foreach (var deltableType in deltableTypes)
                {
                    Line($"[Unity.Burst.BurstCompile]");
                    using (Section($"internal struct JobCalculate{deltableType}ComponentDelta : IJobParallelFor"))
                    {
                        Line($"[Unity.Collections.ReadOnly] public NativeArray<{deltableType}> previous;");
                        Line($"[Unity.Collections.ReadOnly] public NativeArray<{deltableType}> current;");
                        Line($"[Unity.Collections.WriteOnly] public NativeArray<{deltableType}> delta;");
                        Line("");

                        using (Section($"public JobCalculate{deltableType}ComponentDelta(NativeArray<{deltableType}> previous, NativeArray<{deltableType}> current, NativeArray<{deltableType}> delta)"))
                        {
                            Line($"this.previous = previous;");
                            Line($"this.current = current;");
                            Line($"this.delta = delta;");
                        }

                        using (Section($"public void Execute(int index)"))
                        {
                            Line($"delta[index] = current[index] - previous[index];");
                        }
                    }
                }
            }

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
                            Line($"return ({string.Join($" && ", variableParameters.Select(x => $"{x}.Equals(other{component.name}.{x})"))});");
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
                        using (Section($"public static {component.typeName} operator +({component.typeName} left, {component.typeName} right)"))
                        {
                            Line($"var sum = default({component.typeName});");

                            foreach (var variable in component.variables)
                            {
                                if (variable.deltable)
                                {
                                    Line($"sum.{variable.name} = left.{variable.name} + right.{variable.name};");
                                }
                            }

                            Line($"return sum;");
                        }

                        InlineMethod();
                        using (Section($"public static {component.typeName} operator -({component.typeName} left, {component.typeName} right)"))
                        {
                            Line($"var delta = default({component.typeName});");

                            foreach (var variable in component.variables)
                            {
                                if (variable.deltable)
                                {
                                    Line($"delta.{variable.name} = left.{variable.name} - right.{variable.name};");
                                }
                            }

                            Line($"return delta;");
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
                using (Section($"public sealed partial class {script.typePrefix}EntityManager : {script.managerSettings.baseManagerClass}"))
                {
                    using (Section($"public enum EntityManagerUpdateState"))
                    {
                        Line("Idle, BeforeTick, Tick, AfterTick, BeforeDraw, Draw, AfterDraw");
                    }

                    Space();

                    using (Region("Static Variables"))
                    {
                        Line($"public static Sfloat TicksPerSecond => new(0x{SfMath.Rcp(script.tickDeltaTime.Raw):X}); // {Sfloat.Rcp(script.tickDeltaTime)}");
                        Line($"public static Sfloat TickDeltaTime => new(0x{script.tickDeltaTime.Raw:X}); // {script.tickDeltaTime.Float.RoundToNearest(0.001f)}");
                        Line($"public static float TickDeltaTimeAsFloat => {script.tickDeltaTime.Float.RoundToNearest(0.001f)}f;");
                    }

                    Line($"public EntityManagerUpdateState UpdateState {{ get; private set; }}");

                    using (CommentBlock("Inspector"))
                    using (Conditional("UNITY_EDITOR"))
                    {
                        Line("[Title(\"Entities\")]");
                        Pragma("pragma warning disable IDE0051 // Remove unused private members");

                        foreach (var archetype in script.ActiveEntities)
                        {
                            Line($"[ShowInInspector, HideInPlayMode, DisplayAsString]");
                            Line($"private int Baked{archetype.typeName}Entities {{ get => GetComponentsInChildren<{script.typePrefix}{archetype.typeName}>(true).Length; set {{ }} }}");
                        }

                        LineIf(script.ActiveDataEntities.Any(), "[Title(\"DataEntities\")]");

                        foreach (var archetype in script.ActiveDataEntities)
                        {
                            var archType = $"{script.typePrefix}{archetype.typeName}";

                            Line($"[ShowInInspector, HideInEditorMode, ListDrawerSettings(IsReadOnly = true), LabelText(\"{archetype.typeName}\"), PropertyOrder(1)]");
                            Line($"private {archType}[] EditorSafeList{archetype.typeName} => array{archetype.typeName};");
                        }

                        Space();

                        Line($"[Button(DirtyOnClick = true, Icon = SdfIconType.ArrowRepeat, Name = \"Refresh\"), HideInPlayMode]");
                        using (Section($"{Choose(script.managerSettings.markOnValidateAsOverride, "protected override ", "private ")}void OnValidate()"))
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

                    Line($"private uint entityIdCounter = 0;");
                    Line($"private readonly Dictionary<uint, {script.typePrefix}Entity> entityMap = new(2048);");
                    Line($"private readonly Dictionary<uint, {script.typePrefix}DataEntity> dataEntityMap = new(4096);");
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

                    Line($"Dictionary<Type, Scene> entitiesScenes = new();");
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

                            if (!component.isStatic) Line($"internal {component.TypeName}[] component{archetype.typeName}{component.name}_prev;");

                            if (!component.isStatic && component.deltable)
                            {
                                Line($"internal {component.TypeName}[] component{archetype.typeName}{component.name}_delta;");
                            }
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

                    using (Section($"public bool TryGetEntity({script.typePrefix}EntityRef entityRef, out {script.typePrefix}Entity entity)"))
                    {
                        Line($"return entityMap.TryGetValue(entityRef.uniqueId, out entity);");
                    }

                    using (Section($"{Choose(script.managerSettings.markAwakeAsOverride, "protected override ", "private ")}void Awake()"))
                    {
                        LineIf(script.managerSettings.markAwakeAsOverride, "base.Awake();");
                        Line($"Initialize();");
                    }

                    using (Section($"public void Initialize()"))
                    {
                        Line($"if (initialized) return;");

                        Line($"initialized = true;");

                        foreach (var archetype in script.ActiveArchetypes)
                        {
                            if (archetype.needsUnityGameObjectAccess)
                            {
                                var archType = $"{script.typePrefix}{archetype.typeName}";
                                Line($"entitiesScenes[typeof({archType})] = SceneManager.CreateScene($\"{script.name} - {archType}s [{{GetInstanceID():X}}]\");");
                            }
                        }

                        Space();

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
                                if (!component.isStatic) Line($"component{archetype.typeName}{component.name}_prev = new {component.TypeName}[{archetype.initialDataCapacity}];");
                                if (!component.isStatic && component.deltable) Line($"component{archetype.typeName}{component.name}_delta = new {component.TypeName}[{archetype.initialDataCapacity}];");
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

                    using (Section($"public void OnTick()"))
                    {
                        Line($"var deltaTime = new Sfloat({script.tickDeltaTime.Raw});");
                        Line($"Tick++;");
                        Line($"Time = Tick * deltaTime;");
                        Line($"UpdatePreviousState();");
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
                        Line($"UpdateDeltaState();");
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

                    using (Section($"private void UpdatePreviousState()"))
                    {
                        using (ProfileSample("EntityManager UpdatePreviousState"))
                        {
                            foreach (var archetype in script.ActiveArchetypes)
                            {
                                foreach (var component in archetype.componentDefinitions)
                                {
                                    if (!component.isStatic) Line($"Array.Copy(component{archetype.typeName}{component.name}, component{archetype.typeName}{component.name}_prev, array{archetype.typeName}.Length);");
                                }
                            }
                        }
                    }

                    using (Section($"private void UpdateDeltaState()"))
                    {
                        using (ProfileSample("EntityManager UpdateDeltaState"))
                        {
                            foreach (var archetype in script.ActiveArchetypes)
                            {
                                foreach (var component in archetype.componentDefinitions.Where(x => x.deltable))
                                {
                                    var dataName = $"component{archetype.typeName}{component.name}";

                                    Line($"var na_{dataName} = new NativeArray<{component.TypeName}>({dataName}, Allocator.TempJob);");
                                    Line($"var na_{dataName}_prev = new NativeArray<{component.TypeName}>({dataName}_prev, Allocator.TempJob);");
                                    Line($"var na_{dataName}_delta = new NativeArray<{component.TypeName}>({dataName}.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);");
                                    Line($"");
                                    Line($"var job_{dataName} = new JobCalculate{component.TypeName}ComponentDelta(na_{dataName}_prev, na_{dataName}, na_{dataName}_delta);");
                                    Line($"this.QueueJobParallelFor(ref job_{dataName}, {archetype.typeName}Count, 32);");
                                    Line($"");
                                }
                            }

                            Line($"CompleteJobs();");
                            Line($"");

                            foreach (var archetype in script.ActiveArchetypes)
                            {
                                foreach (var component in archetype.componentDefinitions.Where(x => x.deltable))
                                {
                                    var dataName = $"component{archetype.typeName}{component.name}";
                                    Line($"NativeArray<{component.TypeName}>.Copy(na_{dataName}_delta, {dataName}_delta);");
                                }
                            }

                            Line($"");

                            foreach (var archetype in script.ActiveArchetypes)
                            {
                                foreach (var component in archetype.componentDefinitions.Where(x => x.deltable))
                                {
                                    var dataName = $"component{archetype.typeName}{component.name}";
                                    Line($"this.QueueDispose(na_{dataName});");
                                    Line($"this.QueueDispose(na_{dataName}_prev);");
                                    Line($"this.QueueDispose(na_{dataName}_delta);");
                                    Line($"");
                                }
                            }

                            Line($"CompleteJobs();");
                            Line($"");
                        }
                    }


                    using (Section("public void QueueDispose<T>(NativeArray<T> disposable) where T : struct"))
                    {
                        Line($"DisposeJobHandle = JobHandle.CombineDependencies(DisposeJobHandle, disposable.Dispose(JobHandle));");
                    }

                    using (Section("public void QueueJobsParallel(NativeArray<JobHandle> jobs)"))
                    {
                        Line($"JobHandle = JobHandle.CombineDependencies(JobHandle, JobHandle.CombineDependencies(jobs));");
                    }

                    using (Section("public void QueueJob<T>(ref T job) where T : struct, IJob"))
                    {
                        Line($"JobHandle = job.ScheduleByRef(JobHandle);");
                    }

                    using (Section("public void QueueJobFor<T>(ref T job, int arrayLength) where T : struct, IJobFor"))
                    {
                        Line($"JobHandle = job.ScheduleByRef(arrayLength, JobHandle);");
                    }

                    using (Section("public void QueueJobFor<T>(ref T job, int arrayLength, int innerLoopBatchCount) where T : struct, IJobFor"))
                    {
                        Line($"JobHandle = job.ScheduleParallelByRef(arrayLength, innerLoopBatchCount, JobHandle);");
                    }

                    using (Section("public void QueueJobParallelFor<T>(ref T job, int arrayLength, int innerLoopBatchCount) where T : struct, IJobParallelFor"))
                    {
                        Line($"JobHandle = job.ScheduleByRef(arrayLength, innerLoopBatchCount, JobHandle);");
                    }

                    using (Section("public void QueueJobParallelForTransform<T>(ref T job, TransformAccessArray transforms) where T : struct, IJobParallelForTransform"))
                    {
                        Line($"JobHandle = job.ScheduleByRef(transforms, JobHandle);");
                    }



                    Line($"public T GetSystem<T>() where T : {script.typePrefix}EntitySystem => (systemsByType.TryGetValue(typeof(T), out var system)) ? system as T : default;");

                    using (Section("public void ScheduleJobs()"))
                    {
                        Line($"JobHandle.ScheduleBatchedJobs();");
                    }

                    Line($"[Conditional(\"UNITY_EDITOR\")]");
                    using (Section("public void AssertState(EntityManagerUpdateState state)"))
                    {
                        Line($"if (UpdateState != state) throw new Exception($\"Expecting Entity Manager to be in {{state}} state, but its not.\");");
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

                        Line($"SceneManager.MoveGameObjectToScene(entity.gameObject, entitiesScenes[typeof(T)]);");
                        Line($"return entity;");
                    }

                    foreach(var archetype in script.ActiveEntities)
                    {
                        var archType = $"{script.typePrefix}{archetype.typeName}";
                        var archTypeVarName = archetype.typeName.ToLower();
                        var archTypeIndex = script.ActiveEntities.IndexOf(archetype) + 1;

                        using (Section($"public bool TryGet{archetype.typeName}({script.typePrefix}EntityRef entityRef, out {archType} {archTypeVarName})"))
                        {
                            using (Section($"if (entityMap.TryGetValue(entityRef.uniqueId, out var entity) && entity is {archType} {archTypeVarName}Entity)"))
                            {
                                Line($"{archTypeVarName} = {archTypeVarName}Entity;");
                                Line($"return true;");
                            }
                            using (Section($"else"))
                            {
                                Line($"{archTypeVarName} = null;");
                                Line($"return false;");
                            }
                        }
                    }

                    foreach (var archetype in script.ActiveArchetypes)
                    {
                        var archType = $"{script.typePrefix}{archetype.typeName}";

                        using (Section($"public ReadOnlySpan<{archType}> Get{archetype.typeName}s()"))
                        {
                            Line($"return new ReadOnlySpan<{archType}>(array{archetype.typeName}, 0, count{archetype.typeName});");
                        }

                        Space();

                        if (!archetype.needsUnityGameObjectAccess)
                        {
                            Line($"private readonly List<{archType}> instancePool{archetype.typeName} = new({archetype.initialDataCapacity});");
                            Space();

                            using (Section($"public {archType} GetNew{archetype.typeName}FromPool()"))
                            {
                                Line($"{archType} dataEntity;");
                                Space();

                                Line($"var poolIndex = instancePool{archetype.typeName}.Count - 1;");
                                Space();

                                using (Section($"if (poolIndex >= 0)"))
                                {
                                    Line($"dataEntity = instancePool{archetype.typeName}[poolIndex];");
                                    Line($"instancePool{archetype.typeName}.RemoveAt(poolIndex);");
                                }
                                using (Section($"else"))
                                {
                                    Line($"dataEntity = new {archType}();");
                                }

                                Line($"dataEntity.entityManager = this;");
                                Line($"return dataEntity;");
                            }

                            using (Section($"public void Release{archetype.typeName}ToPool({archType} entity)"))
                            {
                                Line($"if (entity == null) return;");
                                Line($"if (entity.IsCreated) throw new Exception(\"{archetype.typeName} is still alive. Unregister it first before releasing to pool.\");");
                                Line($"instancePool{archetype.typeName}.Add(entity);");
                            }
                        }

                        using (Section($"internal void Register{archetype.typeName}({archType} entity)"))
                        {
                            Line($"if (structuralChangesLocked) throw new Exception(\"Not allowed to add entities during systems update.\");");

                            Line($"if (entity.entityDataIndex != -1) return;");
                            Space();
                            Line($"entity.uniqueId = ++entityIdCounter;");

                            if (archetype.needsUnityGameObjectAccess)
                            {
                                Line($"entityMap[entity.uniqueId] = entity;");
                            }
                            else
                            {
                                Line($"dataEntityMap[entity.uniqueId] = entity;");
                            }

                            Space();

                            if (archetype.deparentTransformOnRegister)
                            {
                                Line($"entity.transform.SetParent(null, false);");
                                Line($"SceneManager.MoveGameObjectToScene(entity.gameObject, entitiesScenes[typeof({archType})]);");
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

                                    if (!component.isStatic) Line($"Array.Resize(ref component{archetype.typeName}{component.name}_prev, nextCapacity);");
                                    if (!component.isStatic && component.deltable) Line($"Array.Resize(ref component{archetype.typeName}{component.name}_delta, nextCapacity);");
                                }

                                foreach (var buffer in archetype.componentBuffers)
                                {
                                    Line($"Array.Resize(ref buffer{archetype.typeName}{buffer.name}, nextCapacity * {script.typePrefix}{archetype.typeName}.Max{buffer.name}Count);");
                                    Line($"Array.Resize(ref bufferCounts{archetype.typeName}{buffer.name}, nextCapacity);");
                                }
                            }

                            Line($"entity.entityDataIndex = index;");
                            Space();

                            Line($"array{archetype.typeName}[index] = entity;");
                            Space();

                            if (archetype.needsTransformAccess)
                            {
                                Line($"transformAccessArray{archetype.typeName}.Add(entity.transform);");
                            }

                            if (archetype.needsUnityGameObjectAccess)
                            {
                                foreach (var component in archetype.componentDefinitions)
                                {
                                    Line($"component{archetype.typeName}{component.name}[index] = entity.initial{component.name};");
                                    if (!component.isStatic) Line($"component{archetype.typeName}{component.name}_prev[index] = entity.initial{component.name};");
                                    if (!component.isStatic && component.deltable) Line($"component{archetype.typeName}{component.name}_delta[index] = default;");
                                }
                            }
                            else
                            {
                                foreach (var component in archetype.componentDefinitions)
                                {
                                    Line($"component{archetype.typeName}{component.name}[index] = default;");
                                    if (!component.isStatic) Line($"component{archetype.typeName}{component.name}_prev[index] = default;");
                                    if (!component.isStatic && component.deltable) Line($"component{archetype.typeName}{component.name}_delta[index] = default;");
                                }
                            }

                            foreach (var buffer in archetype.componentBuffers)
                            {
                                Line($"{script.typePrefix}{archetype.typeName}.Get{buffer.name}Slice(buffer{archetype.typeName}{buffer.name}, bufferCounts{archetype.typeName}{buffer.name}, index).SetData(entity.initial{buffer.name}Buffer);");
                            }

                            Space();

                            Line($"if (entity is I{archetype.typeName}CreatedHandler createdHandler) createdHandler.On{archetype.typeName}Created();");
                            Space();
                            Line($"On{archetype.typeName}Added?.Invoke(entity);");
                        }

                        using (Section($"internal void Unregister{archetype.typeName}({script.typePrefix}{archetype.typeName} entity)"))
                        {
                            Line($"if (structuralChangesLocked) throw new Exception(\"Not allowed to remove entities during systems update.\");");
                            Line($"if (entity.entityDataIndex == -1) return;");
                            Space();
                            Line($"On{archetype.typeName}BeforeRemove?.Invoke(entity);");
                            Space();
                            Line($"if (entity is I{archetype.typeName}DestroyedHandler destroyedHandler) destroyedHandler.On{archetype.typeName}Destroyed();");
                            Space();
                            Line($"count{archetype.typeName}--;");
                            Space();
                            Line($"var index = entity.entityDataIndex;");
                            Line($"var lastEntity = array{archetype.typeName}[count{archetype.typeName}];");
                            Space();
                            Line($"array{archetype.typeName}[index] = lastEntity;");

                            foreach (var component in archetype.componentDefinitions)
                            {
                                Line($"component{archetype.typeName}{component.name}[index] = component{archetype.typeName}{component.name}[count{archetype.typeName}];");
                                if (!component.isStatic) Line($"component{archetype.typeName}{component.name}_prev[index] = component{archetype.typeName}{component.name}_prev[count{archetype.typeName}];");
                                if (!component.isStatic && component.deltable) Line($"component{archetype.typeName}{component.name}_delta[index] = component{archetype.typeName}{component.name}_delta[count{archetype.typeName}];");
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

                            Line($"lastEntity.entityDataIndex = index;");

                            if (archetype.needsUnityGameObjectAccess)
                            {
                                Line($"entityMap.Remove(entity.uniqueId);");
                            }
                            else
                            {
                                Line($"dataEntityMap.Remove(entity.uniqueId);");
                            }

                            Line($"entity.entityDataIndex = -1;");
                            Line($"entity.uniqueId = 0;");
                        }

                        foreach (var component in archetype.componentDefinitions)
                        {
                            using (Section($"public NativeArray<{component.TypeName}> Get{archetype.typeName}{component.name}Components(Allocator allocator = Allocator.TempJob)"))
                            {
                                Line($"var data = new NativeArray<{component.TypeName}>(count{archetype.typeName}, allocator, NativeArrayOptions.UninitializedMemory);");
                                Line($"NativeArray<{component.TypeName}>.Copy(component{archetype.typeName}{component.name}, 0, data, 0, count{archetype.typeName});");
                                Line($"return data;");
                            }

                            if (!component.isStatic)
                            {
                                using (Section($"public NativeArray<{component.TypeName}> Get{archetype.typeName}{component.name}ComponentsPrevious(Allocator allocator = Allocator.TempJob)"))
                                {
                                    Line($"var data = new NativeArray<{component.TypeName}>(count{archetype.typeName}, allocator, NativeArrayOptions.UninitializedMemory);");
                                    Line($"NativeArray<{component.TypeName}>.Copy(component{archetype.typeName}{component.name}_prev, 0, data, 0, count{archetype.typeName});");
                                    Line($"return data;");
                                }

                                if (component.deltable)
                                {
                                    using (Section($"public NativeArray<{component.TypeName}> Get{archetype.typeName}{component.name}ComponentsDeltas(Allocator allocator = Allocator.TempJob)"))
                                    {
                                        Line($"var data = new NativeArray<{component.TypeName}>(count{archetype.typeName}, allocator, NativeArrayOptions.UninitializedMemory);");
                                        Line($"NativeArray<{component.TypeName}>.Copy(component{archetype.typeName}{component.name}_delta, 0, data, 0, count{archetype.typeName});");
                                        Line($"return data;");
                                    }
                                }
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

                    using (Section($"{Choose(script.managerSettings.markOnDisableAsOverride, "protected override ", "private ")}void OnDestroy()"))
                    {
                        LineIf(script.managerSettings.markOnDisableAsOverride, "base.OnDestroy();");

                        using (Section("foreach(var scene in entitiesScenes.Values)"))
                        {
                            Line($"if (scene.isLoaded) SceneManager.UnloadSceneAsync(scene);");
                        }

                        Space();

                        foreach (var archetype in script.ActiveEntities)
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
                    Line($"public virtual void OnClear() {{ }}");
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
                    Line($"internal int entityDataIndex = -1;");
                    Line($"[ShowInInspector, HideInEditorMode, PropertyOrder(-199)]");
                    Line($"public {script.typePrefix}EntityRef {script.typePrefix}EntityRef => new {script.typePrefix}EntityRef(this);");
                    Line($"internal uint uniqueId;");
                    Space();
                    Line($"internal virtual ushort EntityType => 0;");
                    Line($"public bool IsCreated => entityDataIndex != -1;");

                    using (Conditional("UNITY_EDITOR"))
                    using (Section($"protected {Choose(script.entitySettings.markOnResetAsOverride, "override ")}void Reset()"))
                    {
                        LineIf(script.entitySettings.markOnResetAsOverride, "base.Reset();");

                        Line($"entityManager = GetComponentInParent<{script.typePrefix}EntityManager>();");
                    }
                }

                Space();

                Line($"[Serializable]");
                using (Section($"public abstract partial class {script.typePrefix}DataEntity : {script.entitySettings.baseDataEntityClass}"))
                {
                    Line($"[SerializeField, PropertyOrder(-200)]");
                    Line($"internal {script.typePrefix}EntityManager entityManager;");
                    Line($"internal int entityDataIndex = -1;");
                    Line($"[ShowInInspector, HideInEditorMode, PropertyOrder(-199), DisplayAsString]");
                    Line($"public uint uniqueId = 0;");
                    Space();
                    Line($"public bool IsCreated => entityDataIndex != -1;");
                    Line($"public int DataIndex => entityDataIndex;");
                }
            }

            foreach (var dataEntity in script.ActiveDataEntities)
            {
                var archType = $"{script.typePrefix}{dataEntity.typeName}";

                using (WriteFile($"{archType}.{fileExtension}"))
                using (FileHeader())
                {
                    LineIf(!string.IsNullOrWhiteSpace(dataEntity.classAttributes), dataEntity.classAttributes);
                    using (Section($"public {Choose(dataEntity.classSealed, "sealed")} partial class {archType} : {script.typePrefix}DataEntity"))
                    {
                        Line($"internal {archType}() {{ }}");

                        Space();

                        foreach (var component in dataEntity.componentDefinitions)
                        {
                            Line($"public ref {component.TypeName} {component.name} => ref entityManager.component{dataEntity.typeName}{component.name}[entityDataIndex];");
                            if (!component.isStatic) Line($"public ref {component.TypeName} {component.name}Previous => ref entityManager.component{dataEntity.typeName}{component.name}_prev[entityDataIndex];");
                            if (!component.isStatic && component.deltable) Line($"public ref {component.TypeName} {component.name}Delta => ref entityManager.component{dataEntity.typeName}{component.name}_delta[entityDataIndex];");
                        }

                        Space();

                        //foreach (var buffer in dataEntity.componentBuffers)
                        //{
                        //    Line($"public ref {buffer.TypeName} ");
                        //    Line($"internal {buffer.TypeName}[] initial{buffer.name}Buffer = new {buffer.TypeName}[0];");
                        //}

                        //Space();

                        using (Conditional("UNITY_EDITOR"))
                        {
                            Pragma("pragma warning disable IDE0051 // Remove unused private members");
                            foreach (var component in dataEntity.componentDefinitions)
                            {
                                Line($"[ShowInInspector, ShowIf(nameof(IsCreated)), LabelText(\"{component.name}\")]");
                                Line($"{component.TypeName} {component.name}EditorSafe => IsCreated ? {component.name} : default;");
                            }
                            Pragma("pragma warning restore IDE0051 // Remove unused private members");
                        }

                        Space();

                        using (Section($"public void DestroyAndReturnToPool()"))
                        {
                            Line($"if (IsCreated) entityManager.Unregister{dataEntity.typeName}(this);");
                            Line($"entityManager.Release{dataEntity.typeName}ToPool(this);");
                        }
                    }

                    using (Section($"public interface I{archType}CreatedHandler"))
                    {
                        Line($"void On{archType}Created();");
                    }

                    using (Section($"public interface I{archType}DestroyedHandler"))
                    {
                        Line($"void On{archType}Destroyed();");
                    }
                }
            }

            foreach (var archetype in script.ActiveEntities)
            {
                using (WriteFile($"{script.typePrefix}{archetype.typeName}.{fileExtension}"))
                using (FileHeader())
                {
                    LineIf(!string.IsNullOrWhiteSpace(archetype.classAttributes), archetype.classAttributes);
                    Line($"[AddComponentMenu(\"{script.name}/{archetype.typeName}\"), SelectionBase]");
                    using (Section($"public partial class {script.typePrefix}{archetype.typeName} : {script.typePrefix}Entity"))
                    {
                        Line($"internal override ushort EntityType => {script.ActiveEntities.IndexOf(archetype) + 1};");
                        Space();

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
                                Line($"public ref {component.TypeName} {component.name} => ref entityManager.component{archetype.typeName}{component.name}[entityDataIndex];");
                                if (!component.isStatic) Line($"public ref {component.TypeName} {component.name}Previous => ref entityManager.component{archetype.typeName}{component.name}_prev[entityDataIndex];");
                                if (!component.isStatic && component.deltable) Line($"public ref {component.TypeName} {component.name}Delta => ref entityManager.component{archetype.typeName}{component.name}_delta[entityDataIndex];");
                            }

                            foreach (var buffer in archetype.componentBuffers)
                            {
                                Line($"public const int Max{buffer.name}Count = {buffer.maxCapacity};");
                                Summary(buffer.description);
                                Line($"public ArrayBufferSlice<{buffer.TypeName}> {buffer.name} => Get{buffer.name}Slice(entityManager.buffer{archetype.typeName}{buffer.name}, entityManager.bufferCounts{archetype.typeName}{buffer.name}, entityDataIndex);");
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
                                Line($"internal ref {component.TypeName} {component.name}EditorSafe => ref entityDataIndex == -1 ? ref initial{component.name} : ref {component.name};");
                                Line($"private string Inspector{component.name}DataTitle => Inspector{component.name}DataEnabled ? \"{component.name}\" : $\"{component.name} (Controlled by {{GetComponent<I{script.typePrefix}{archetype.typeName}{component.name}Baker>().GetType().GetNameNonAlloc()}})\";");
                                Line($"private bool Inspector{component.name}DataEnabled => entityDataIndex != -1 || !TryGetComponent<I{script.typePrefix}{archetype.typeName}{component.name}Baker>(out var _);");

                                if (!component.isStatic)
                                {
                                    Line($"[FoldoutGroup(\"Previous\"), Title(\"{component.name}\"), HideLabel, ShowInInspector, PropertyOrder(100), HideInEditorMode]");
                                    Line($"private {component.TypeName} Inspector{component.name}PreviousDrawer => entityDataIndex == -1 ? default : entityManager.component{archetype.typeName}{component.name}_prev[entityDataIndex];");

                                    if (component.deltable)
                                    {
                                        Line($"[FoldoutGroup(\"Deltas\"), Title(\"{component.name}\"), HideLabel, ShowInInspector, PropertyOrder(100), HideInEditorMode]");
                                        Line($"private {component.TypeName} Inspector{component.name}DeltaDrawer => entityDataIndex == -1 ? default : entityManager.component{archetype.typeName}{component.name}_delta[entityDataIndex];");
                                    }
                                }
                            }
                            LineIf(archetype.componentDefinitions.Where(x => !x.hideInInspector && !x.hideLabel).Any(), $"[Title(\"Other\")]");
                            foreach (var component in archetype.componentDefinitions.Where(x => !x.hideInInspector && !x.hideLabel))
                            {
                                LineIf(!string.IsNullOrWhiteSpace(component.inspectorDrawerAttributes), component.inspectorDrawerAttributes);
                                Line($"[LabelText(\"{component.name}\"), ShowInInspector, EnableIf(\"Inspector{component.name}DataEnabled\"), Tooltip(\"{component.description}\"), PropertyOrder(-100)]");
                                Line($"private {component.TypeName} Inspector{component.name}Drawer {{ get => {component.name}EditorSafe; set => {component.name}EditorSafe = value; }}");
                                Line($"internal ref {component.TypeName} {component.name}EditorSafe => ref entityDataIndex == -1 ? ref initial{component.name} : ref {component.name};");
                                Line($"private string Inspector{component.name}DataTitle => Inspector{component.name}DataEnabled ? \"{component.name}\" : $\"{component.name} (Controlled by {{GetComponent<I{script.typePrefix}{archetype.typeName}{component.name}Baker>().GetType().GetNameNonAlloc()}})\";");
                                Line($"private bool Inspector{component.name}DataEnabled => entityDataIndex != -1 || !TryGetComponent<I{script.typePrefix}{archetype.typeName}{component.name}Baker>(out var _);");
                            }
                            foreach (var buffer in archetype.componentBuffers.Where(x => !x.hideInInspector))
                            {
                                Line($"private string Inspector{buffer.name}BufferTitle => Inspector{buffer.name}BufferEnabled ? \"{buffer.name}\" : $\"{buffer.name} (Controlled by {{GetComponent<I{script.typePrefix}{archetype.typeName}{buffer.name}Baker>().GetType().GetNameNonAlloc()}})\";");
                                Line($"private bool Inspector{buffer.name}BufferEnabled => entityDataIndex != -1 || !TryGetComponent<I{script.typePrefix}{archetype.typeName}{buffer.name}Baker>(out var _);");
                                LineIf(!string.IsNullOrWhiteSpace(buffer.inspectorDrawerAttributes), buffer.inspectorDrawerAttributes);
                                Line($"[Title(\"@Inspector{buffer.name}BufferTitle\"), ShowInInspector, LabelText(\" \"), EnableIf(\"@Inspector{buffer.name}BufferEnabled\"), Tooltip(\"{buffer.description}\"), PropertyOrder(-100)]");
                                Line($"[RequiredListLength(MaxLength = {buffer.maxCapacity})]");
                                using (Section($"internal {buffer.TypeName}[] Inspector{buffer.name}EditorSafe"))
                                {
                                    Line($"get => entityDataIndex == -1 ? initial{buffer.name}Buffer : {buffer.name}.ToArray();");
                                    Line($"set {{ if (entityDataIndex == -1) initial{buffer.name}Buffer = value; else {buffer.name}.SetData(value); }}");
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
                        using (Section($"{Choose(archetype.classSealed, "private", "protected")} {Choose(script.entitySettings.markUpdateAsOverride, "override ")}void Update()"))
                        {
                            Line($"if (entityDataIndex != -1) return;");

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

                        using (Section($"{Choose(archetype.classSealed, "private", "protected")} {Choose(script.entitySettings.markOnEnableAsOverride, "override ")}void OnEnable()"))
                        {
                            LineIf(script.entitySettings.markOnEnableAsOverride, "base.OnEnable();");

                            using (Conditional("UNITY_EDITOR"))
                            {
                                Line($"if (!Application.isPlaying) return;");
                            }

                            Line($"if (entityManager) entityManager.Register{archetype.typeName}(this);");
                        }

                        using (Section($"{Choose(archetype.classSealed, "private", "protected")} {Choose(script.entitySettings.markOnDisableAsOverride, "override ")}void OnDisable()"))
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

                    using (Section($"public interface I{archetype.typeName}CreatedHandler"))
                    {
                        Line($"void On{archetype.typeName}Created();");
                    }

                    using (Section($"public interface I{archetype.typeName}DestroyedHandler"))
                    {
                        Line($"void On{archetype.typeName}Destroyed();");
                    }
                }
            }

            using (WriteFile($"{script.typePrefix}EntityRef.{fileExtension}"))
            using (FileHeader())
            {
                using (Section($"public readonly struct {script.typePrefix}EntityRef : IEquatable<{script.typePrefix}EntityRef>"))
                {
                    Line($"public static readonly {script.typePrefix}EntityRef None = default;");
                    Space();

                    Line($"public readonly uint uniqueId;");
                    Space();

                    using (Section($"public {script.typePrefix}EntityRef({script.typePrefix}Entity entity)"))
                    {
                        using (Section("if (entity == null)"))
                        {
                            Line($"uniqueId = default;");
                        }
                        using (Section("else"))
                        {
                            Line($"uniqueId = entity.uniqueId;");
                        }
                    }

                    Line($"public override bool Equals(object obj) => obj is {script.typePrefix}EntityRef @ref && Equals(@ref);");
                    Line($"public bool Equals({script.typePrefix}EntityRef other) => uniqueId == other.uniqueId;");
                    Line($"public override int GetHashCode() => uniqueId.GetHashCode();");
                    Line($"public static bool operator ==({script.typePrefix}EntityRef left, {script.typePrefix}EntityRef right) => left.uniqueId == right.uniqueId;");
                    Line($"public static bool operator !=({script.typePrefix}EntityRef left, {script.typePrefix}EntityRef right) => left.uniqueId != right.uniqueId;");
                    Space();

                    Line($"public static implicit operator {script.typePrefix}EntityRef({script.typePrefix}Entity entity) => new(entity);");
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